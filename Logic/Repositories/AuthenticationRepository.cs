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

        public AuthenticationRepository(
            OrionTekDbContext _dbContext,
            IConfiguration _configuration,
            IMapper _mapper,
            IHttpContextAccessor _httpContextAccessor
            )
        {
            mapper = _mapper;
            httpContext = _httpContextAccessor.HttpContext;
            configuration = _configuration;
            dbContext = _dbContext;
        }
        public async Task ChangePassword(ChangePasswordDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserFromEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;
            return await dbContext.Users
                .Where(u =>
                    u.Email == email)
                .FirstOrDefaultAsync();
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
            var userEmail = GetUserEmail();
            var user = await dbContext.Users.Where(u => u.Email == userEmail).FirstOrDefaultAsync();
            return mapper.Map<UserDto>(user);
        }
        private string GetUserEmail()
        {
            var token = httpContext.Request.Headers["Authorization"].ToString();
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            string userEmail = null;
            try
            {
                userEmail = (tokenHandler.ReadToken(token) as JwtSecurityToken)?.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            }
            catch
            {

            }
            return userEmail;
        }
        public async Task PasswordRecovery(PasswordRecoveryDto passwordRecoveryDto)
        {
            throw new NotImplementedException();
        }

        public async Task RequestPasswordRecovery(RequestPasswordRecoveryDto requestPasswordRecoveryDto)
        {
            throw new NotImplementedException();
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
                u.Email == userCreateUpdateDto.Email &&
                idUpdate == 0 ||
                u.Email == userCreateUpdateDto.Email &&
                u.Id != idUpdate).AnyAsync())
            {
                throw new CustomException(400, "Email already in use.");
            }
        }
    }
}
