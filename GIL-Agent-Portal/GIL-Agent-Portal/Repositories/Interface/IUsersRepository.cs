﻿using GIL_Agent_Portal.DTOs;
using GIL_Agent_Portal.Models;
using System.Collections.Generic;
using System.Numerics;

namespace GIL_Agent_Portal.Repositories.Interface
{
    public interface IUsersRepository
    {
        List<Users> GetAllUserRegisterdList();
        List<UsersResponseList> GetAllUAgentList();
        UsersResponseList GetAgentLoginData(string id);
        bool UserRegister(Users users);
        UserLoginResponseDto LoginUser(UserLoginRequestDto request);
        string GetUserEmailById(string userId);
        Users UserUpdate(updateUser users);
        Users UserProfileUpdate(Users users);
        Users GetAgentDetails(string userId);
        string GetEmailByUserId(string email);
        bool ResetForgotPassword(ResetPassword resetPassword);
        bool updatePassword(UpdatePassword updatePassword);
        int UpdateUserPassword(updatePassword request);




    }
}
