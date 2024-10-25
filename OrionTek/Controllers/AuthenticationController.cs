using Logic.Dtos.Authentication;
using Logic.Dtos.Users;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrionTek.Middlewares;

namespace OrionTek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthCustom]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationRepository authenticationRepository;

        public AuthenticationController(IAuthenticationRepository _authenticationRepository)
        {
            authenticationRepository = _authenticationRepository;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await authenticationRepository.Login(loginRequestDto);
            return Ok(loginResponse);
        }
        
        [AllowAnonymous]
        [HttpPost("Signin")]
        public async Task<ActionResult<LoginResponseDto>> Signin([FromBody] UserCreateDto dto)
        {
            var loginResponse = await authenticationRepository.Signin(dto);
            return Ok(loginResponse);
        }

        [HttpGet("Me")]
        public async Task<ActionResult<UserDto>> Me()
        {
            return Ok(await authenticationRepository.Me());
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            await authenticationRepository.ChangePassword(dto);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("RequestPasswordRecovery")]
        public async Task<ActionResult> RequestPasswordRecovery([FromBody] RequestPasswordRecoveryDto requestPasswordRecovery)
        {
            await authenticationRepository.RequestPasswordRecovery(requestPasswordRecovery);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPut("PasswordRecovery")]
        public async Task<ActionResult> PasswordRecovery([FromBody] PasswordRecoveryDto passwordRecovery)
        {
            await authenticationRepository.PasswordRecovery(passwordRecovery);
            return Ok();
        }
    }
}
