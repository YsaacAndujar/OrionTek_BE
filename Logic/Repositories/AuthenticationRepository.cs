using AutoMapper;
using Data;
using Data.Models;
using Logic.Dtos.Authentication;
using Logic.Dtos.Users;
using Logic.Interfaces;
using Logic.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly OrionTekDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private HttpContext httpContext;
        private readonly IEmailSender emailSender;

        public AuthenticationRepository(
            OrionTekDbContext _dbContext,
            IConfiguration _configuration,
            IMapper _mapper,
            IHttpContextAccessor _httpContextAccessor,
            IEmailSender _emailSender

            )
        {
            mapper = _mapper;
            httpContext = _httpContextAccessor.HttpContext;
            configuration = _configuration;
            emailSender = _emailSender;
            dbContext = _dbContext;
        }
        public async Task ChangePassword(ChangePasswordDto dto)
        {
            var id = GetUserId();
            var user = await dbContext.Users.FindAsync(id);
            user.Password = HashEncryption.ComputeSHA256Hash(dto.Password);
            var codes = await dbContext.PasswordRecoveryCodes
                .Where(prc => prc.UserId == user.Id && prc.Expires > DateTime.Now).ToListAsync();
            foreach (var code in codes)
            {
                code.Expires = DateTime.Now;
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task<User> GetUser(int? id)
        {
            if (id == null) return null;
            return await dbContext.Users
                .FindAsync(id);
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var passwordHash = HashEncryption.ComputeSHA256Hash(loginRequestDto.Password);
            var user = await dbContext.Users.Where(u =>
                u.Email == loginRequestDto.Email
                && u.Password == passwordHash
                )
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new CustomException(404, "User does not exists or credentials are wrong.");
            }
            return BuildToken(user);
        }
        private LoginResponseDto BuildToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", user.Email),
                new Claim("id", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyJwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expire = DateTime.UtcNow.AddMonths(1);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expire, signingCredentials: creds);
            return new LoginResponseDto()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expires = expire
            };
        }
        public async Task<UserDto> Me()
        {
            var id = GetUserId();
            var user = await dbContext.Users.FindAsync(id);
            return mapper.Map<UserDto>(user);
        }
        private int? GetUserId()
        {
            var token = httpContext.Request.Headers["Authorization"].ToString();
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            int? userId = null;
            
            try
            {
                userId = int.Parse((tokenHandler.ReadToken(token) as JwtSecurityToken)?.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value);
            }
            catch
            {

            }
            return userId;
        }
        public async Task PasswordRecovery(PasswordRecoveryDto passwordRecoveryDto)
        {
            var recoveryCode = await dbContext.PasswordRecoveryCodes.Where(prc =>
                prc.UserId == passwordRecoveryDto.UserId &&
                prc.Code == passwordRecoveryDto.Code
            ).FirstOrDefaultAsync();
            if (recoveryCode == null)
            {
                throw new CustomException(400, "Invalid user or code.");
            }
            if (DateTime.Now > recoveryCode.Expires)
            {
                throw new CustomException(400, "The code has expired.");
            }
            var user = await dbContext.Users.FindAsync(passwordRecoveryDto.UserId);
            user.Password = HashEncryption.ComputeSHA256Hash(passwordRecoveryDto.Password);
            recoveryCode.Expires = DateTime.Now;
            await dbContext.SaveChangesAsync();
        }

        public async Task RequestPasswordRecovery(RequestPasswordRecoveryDto requestPasswordRecoveryDto)
        {
            var user = await dbContext.Users.Where(u => u.Email == requestPasswordRecoveryDto.Email).FirstOrDefaultAsync();
            if (user == null) return;
            var code = RandomCodeGenerator.GenerateRandomCode();
            await dbContext.AddAsync(new PasswordRecoveryCode()
            {
                UserId = user.Id,
                Code = code
            });
            await dbContext.SaveChangesAsync();
            var frontUrl = configuration["FrontEndUrl"];
            var redirectUrl = $"{frontUrl}auth/recover-password?userid={user.Id}&code={code}";
            await emailSender.SendEmail(
                toEmail: user.Email,
                toName: user.Email,
                subject: "Password Recovery",
                htmlContent: @$"You requested a password recovery. To change your password <a href='{redirectUrl}' target='_blank'>Click Here</a>
<br/>If the button doesn't work coy this link: {redirectUrl}",
                plainTextContent: @$"You requested a password recovery. To change your password use the next link: {redirectUrl}"
                );
        }

        public async Task<LoginResponseDto> Signin(UserCreateDto dto)
        {
            await ValidateUserCreateUpdateDto(dto);
            var user = mapper.Map<User>(dto);
            user.Password = HashEncryption.ComputeSHA256Hash(dto.Password);
            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return BuildToken(user);
        }
        private async Task ValidateUserCreateUpdateDto(UserCreateUpdateAbstractDto userCreateUpdateDto, int idUpdate = 0)
        {
            if (await dbContext.Users.Where(u =>
                (u.Email == userCreateUpdateDto.Email &&
                idUpdate == 0) ||
                (u.Email == userCreateUpdateDto.Email &&
                u.Id != idUpdate)).AnyAsync())
            {
                throw new CustomException(400, "Email already in use.");
            }
        }

        public async Task UpdateMe(UserUpdateDto dto)
        {
            var id = GetUserId();
            var user = await dbContext.Users.FindAsync(id);
            await ValidateUserCreateUpdateDto(dto, user.Id);
            mapper.Map(dto, user);
            await dbContext.SaveChangesAsync();
        }
    }
}
