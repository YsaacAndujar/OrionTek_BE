
using Data.Models;
using Logic.Dtos.Authentication;
using Logic.Dtos.Users;

namespace Logic.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<LoginResponseDto> Signin(UserCreateDto dto);
        Task RequestPasswordRecovery(RequestPasswordRecoveryDto requestPasswordRecoveryDto);
        Task ChangePassword(ChangePasswordDto dto);
        Task PasswordRecovery(PasswordRecoveryDto passwordRecoveryDto);
        Task<User> GetUserFromEmail(string email);
        Task<UserDto> Me();
    }
}
