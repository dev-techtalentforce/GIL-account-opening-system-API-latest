﻿using GIL_Agent_Portal.DTOs;
using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories;
using GIL_Agent_Portal.Services;
using GIL_Agent_Portal.Services.Intetrface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GIL_Agent_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUsersService _usersService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<UsersService> _logger;

        public UsersController(IUsersService usersServicey, IJwtTokenGenerator jwtTokenGenerator, ILogger<UsersService> logger)
        {
            _usersService = usersServicey;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }



        [Authorize]
        [HttpGet("LoginAuthorization")]
        public IActionResult GetLoginAuthorization()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new { Email = email, Role = role });
        }

        [HttpGet("GetAllRegistrationList")]

        public IActionResult GetAllRegistrationList()
        {
            var result = _usersService.GetAllUserRegisterdList();
            return Ok(result);
        }

        [HttpGet("GetAllAgentList")]

        public IActionResult GetAllUAgentList()
        {
            var result = _usersService.GetAllUAgentList();
            return Ok(result);
        }

        [HttpGet("GetAgentLoginData/{id}")]
        public IActionResult GetAgentLoginData([FromRoute]  string id)
        {
            var result = _usersService.GetAgentLoginData(id);
            return Ok(result);
        }
        [HttpPost("UserRegistration")]
        public IActionResult Register([FromBody] Users users)
        {
            if (users == null)
                return BadRequest("User data is required.");

            var isRegistered = _usersService.UserRegister(users);

            if (isRegistered)
                return Ok(new { message = "User registered successfully." });

            return StatusCode(500, new { message = "User registration failed." });
        }



        [HttpPost("UserLogin")]
        public IActionResult Login([FromBody] UserLoginRequestDto request)
        {
            try
            {
                // Authenticate the user
                var user = _usersService.LoginUser(request);

                if (user == null)
                {
                    return Unauthorized("Invalid credentials");
                }

                // Generate JWT token
                var token = _jwtTokenGenerator.GenerateToken(user);

                // Optionally, generate a refresh token as well
                var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

                // Save refresh token in database or cache for future validation
                user.RefreshToken = refreshToken;
                //_usersService.UpdateUserRefreshToken(user.UserId, refreshToken);

                // Create response DTO
                var response = new UserLoginResponseDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                    KYCStatus = user.KYCStatus,
                    ReferralCode = user.ReferralCode,
                    RoleId = user.RoleId,
                    Mobile=user.Mobile,
                    Pancard=user.Pancard,
                    Address=user.Address
                    //AgentId = user.AgentId,
                };

                // Return JWT token and user details
                return Ok(new { token, response });
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        //[HttpPost("approve/{userId}")]
        //public IActionResult ApproveUser(int userId)
        //{
        //    try
        //    {
        //        _usersService.ApproveUser(userId);
        //        return Ok("Approval email sent successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost("reject/{userId}")]
        //public IActionResult RejectUser(int userId)
        //{
        //    try
        //    {
        //        _usersService.RejectUser(userId);
        //        return Ok("Rejection email sent successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpPost("update")]
        public IActionResult UserUpdate([FromBody] updateUser users)
        {
            try
            {
                _logger.LogInformation("Updating user with UserId: {UserId}", users?.UserId);
                if (users == null)
                {
                    _logger.LogWarning("UserId is null or invalid in UpdateUser request");
                    return BadRequest("UserId cannot be null or invalid.");
                }
                // 1. Approve/Reject logic based on status
                if (users.status.HasValue)
                {
                    if (users.status.Value == false)
                    {
                        _logger.LogInformation("Rejecting user with UserId: {UserId}", users.UserId);
                        _usersService.RejectUser(users.UserId);
                    }
                    else
                    {
                        _logger.LogInformation("Approving user with UserId: {UserId}", users.UserId);
                        _usersService.ApproveUser(users.UserId);
                    }
                }
                var updatedUser = _usersService.UserUpdate(users);

                return Ok(new
                {
                    Message = "User updated successfully",
                    StatusAction = users.status.HasValue ? (users.status.Value ? "Approved" : "Rejected") : "No status change",
                    BlockStatusAction = users.BlockStatus.HasValue ? $"BlockStatus changed to {users.BlockStatus}" : "No block change",
                    UpdatedUser = updatedUser
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user with UserId: {UserId}", users?.UserId);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("update-password")]
        public IActionResult UpdateUserPassword([FromBody] updatePassword request)
        {
            try
            {
                var updatedUser = _usersService.UpdateUserPassword(request);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateUserPassword endpoint");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("GetAgentDetails")]

        public IActionResult GetAgentDetails([FromBody] Users users)
        {
            var result = _usersService.GetAgentDetails(users.UserId);
            return Ok(result);
        }

        [HttpPost("resetForgotPassword")]
        public IActionResult resetForgotPassword([FromBody] ResetPassword resetPassword)
        {
            var result = _usersService.resetForgotPassword(resetPassword.Email);
            return Ok(result);
        }

        [HttpPost("upadatePassword")]
        public IActionResult upadatePassword([FromBody] UpdatePassword updatePassword)
        {
            var result = _usersService.updatePassword(updatePassword);
            return Ok(result);
        }
    }
}
