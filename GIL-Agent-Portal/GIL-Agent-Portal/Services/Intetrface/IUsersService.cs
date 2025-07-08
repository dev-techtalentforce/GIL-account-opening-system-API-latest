using GIL_Agent_Portal.DTOs;
using GIL_Agent_Portal.Models;

namespace GIL_Agent_Portal.Services.Intetrface
{
    public interface IUsersService
    {
        List<Users> GetAllUserRegisterdList();
        bool UserRegister(Users users);
        UserLoginResponseDto LoginUser(UserLoginRequestDto request);
        void ApproveUser(int userId);
        void RejectUser(int userId);
        Users UserUpdate(updateUser users);


    }
}
