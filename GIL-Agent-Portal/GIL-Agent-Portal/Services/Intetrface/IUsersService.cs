using GIL_Agent_Portal.DTOs;
using GIL_Agent_Portal.Models;

namespace GIL_Agent_Portal.Services.Intetrface
{
    public interface IUsersService
    {
        List<Users> GetAllUserRegisterdList();
        List<Users> GetAllUAgentList();
        bool UserRegister(Users users);
        UserLoginResponseDto LoginUser(UserLoginRequestDto request);
        void ApproveUser(string userId);
        void RejectUser(string userId);
        Users UserUpdate(updateUser users);
        Users UserProfileUpdate(Users users);
        int UpdateUserPassword(updatePassword request);
        Users GetAgentDetails(string userId);
        bool resetForgotPassword(string Email);
        bool updatePassword(UpdatePassword updatePassword);

    }
}
