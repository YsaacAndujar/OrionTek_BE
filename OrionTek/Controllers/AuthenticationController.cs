using Logic.Dtos.Authentication;
using Logic.Dtos.Users;
using Logic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OrionTek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationRepository authenticationRepository;

        public AuthenticationController(IAuthenticationRepository _authenticationRepository)
        {
            authenticationRepository = _authenticationRepository;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await authenticationRepository.Login(loginRequestDto);
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

        [HttpPost("requestPasswordRecovery")]
        public async Task<ActionResult> RequestPasswordRecovery([FromBody] RequestPasswordRecoveryDto requestPasswordRecovery)
        {
            await authenticationRepository.RequestPasswordRecovery(requestPasswordRecovery);
            return Ok();
        }

        [HttpPut("PasswordRecovery")]
        public async Task<ActionResult> PasswordRecovery([FromBody] PasswordRecoveryDto passwordRecovery)
        {
            await authenticationRepository.PasswordRecovery(passwordRecovery);
            return Ok();
        }
    }
}
