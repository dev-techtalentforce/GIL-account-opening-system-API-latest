using GIL_Agent_Portal.DTOs;
using GIL_Agent_Portal.Models;
using System.Collections.Generic;
using System.Numerics;

namespace GIL_Agent_Portal.Repositories.Interface
{
    public interface IUsersRepository
    {
        List<Users> GetAllUserRegisterdList();
        List<Users> GetAllUAgentList();
        bool UserRegister(Users users);
        UserLoginResponseDto LoginUser(UserLoginRequestDto request);
        string GetUserEmailById(int userId);
        Users UserUpdate(updateUser users);
        Users UpdatePassword(Users user);
        Users UserProfileUpdate(Users users);



    }
}
