
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
        Task<User> GetUser(int? id);
        Task<UserDto> Me();
        Task UpdateMe(UserUpdateDto dto);
    }
}
