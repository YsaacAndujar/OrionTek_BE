
using Data.Models;
using Logic.Dtos.Authentication;
using Logic.Dtos.Users;

namespace Logic.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task RequestPasswordRecovery(RequestPasswordRecoveryDto requestPasswordRecoveryDto);
        Task ChangePassword(ChangePasswordDto dto);
        Task PasswordRecovery(PasswordRecoveryDto passwordRecoveryDto);
        Task<LoginResponseDto> GetTokenPasswordRecovery(GetTokenPasswordRecoveryDto dto);
        Task<User> GetUserFromEmail(string email);
        Task<UserDto> Me();
    }
}
