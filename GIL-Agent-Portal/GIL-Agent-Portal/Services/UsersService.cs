using GIL_Agent_Portal.DTOs;
using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories;
using GIL_Agent_Portal.Repositories.Interface;
using GIL_Agent_Portal.Services.Intetrface;
using Microsoft.Extensions.Options;

namespace GIL_Agent_Portal.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<UsersService> _logger;
        private readonly URLSettings _urlSettings;
        public UsersService(IUsersRepository usersRepository, IEmailService emailService, ILogger<UsersService> logger, IOptions<URLSettings> options)
        {
            _usersRepository = usersRepository;
            _emailService = emailService;
            _logger = logger;
            _urlSettings = options.Value;
        }

        public bool UserRegister(Users users)
        {
            return _usersRepository.UserRegister(users);
        }

        public List<Users> GetAllUserRegisterdList()
        {
            return _usersRepository.GetAllUserRegisterdList();
        }
        public List<Users> GetAllUAgentList()
        {
            return _usersRepository.GetAllUAgentList();
        }

        public UserLoginResponseDto LoginUser(UserLoginRequestDto request)
        {
            return _usersRepository.LoginUser(request);
        }

        public void ApproveUser(int userId)
        {
            try
            {
                _logger.LogInformation("Approving user with ID: {UserId}", userId);
                var email = _usersRepository.GetUserEmailById(userId);
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("User with ID {UserId} not found or email is empty", userId);
                    throw new Exception($"User with ID {userId} not found or email not available.");
                }
                _emailService.SendEmail(email, "Authentication Approved", "Your authentication has been approved.");
                _logger.LogInformation("Approval email sent to {Email} for user ID {UserId}", email, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to approve user with ID {UserId}", userId);
                throw;
            }
        }

        public void RejectUser(int userId)
        {
            try
            {
                _logger.LogInformation("Rejecting user with ID: {UserId}", userId);
                var email = _usersRepository.GetUserEmailById(userId);
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("User with ID {UserId} not found or email is empty", userId);
                    throw new Exception($"User with ID {userId} not found or email not available.");
                }
                _emailService.SendEmail(email, "Authentication Rejected", "Sorry, you are ineligible for use this.");
                _logger.LogInformation("Rejection email sent to {Email} for user ID {UserId}", email, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reject user with ID {UserId}", userId);
                throw;
            }
        }

        Users IUsersService.UserUpdate(updateUser users)
        {
            try
            {
                _logger.LogInformation("Updating user with UserId: {UserId}", users.UserId);
                if (users == null || users.UserId <= 0) // Updated condition for int
                {
                    _logger.LogWarning("UserId is null or invalid in UpdateUser request");
                    throw new ArgumentException("UserId cannot be null or invalid.", nameof(users.UserId));
                }
                var updatedUser = _usersRepository.UserUpdate(users);
                _logger.LogInformation("User updated successfully with email: {Email}", users.Email);

                return updatedUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user with UserId: {UserId}", users.UserId);
                throw;
            }
        }

        public Users UpdatePassword(Users users)
        {
            try
            {
                _logger.LogInformation("Updating user with UserId: {UserId}", users.UserId);
                if (users == null) // Updated condition for int
                {
                    _logger.LogWarning("UserId is null or invalid in UpdateUser request");
                    throw new ArgumentException("UserId cannot be null or invalid.", nameof(users.UserId));
                }
                var updatedUser = _usersRepository.UpdatePassword(users);
                _logger.LogInformation("User updated successfully with email: {Email}", users.Email);

                return updatedUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user with UserId: {UserId}", users.UserId);
                throw;
            }
        }

        public Users UserProfileUpdate(Users users)
        {
            try
            {

                _logger.LogInformation("Updating user with UserId: {UserId}", users.UserId);
                if (users == null) // Updated condition for int
                {
                    _logger.LogWarning("UserId is null or invalid in UpdateUser request");
                    throw new ArgumentException("UserId cannot be null or invalid.", nameof(users.UserId));
                }
                    var userProfileUpdate = _usersRepository.UserProfileUpdate(users);
                    _logger.LogInformation("User updated successfully with email: {Email}", users.Email);

                    return userProfileUpdate;
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user with UserId: {UserId}", users.UserId);
                throw;
            }
        }

        public Users GetAgentDetails(string userId)
        {
            try
            {
                var result = _usersRepository.GetAgentDetails(userId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get Agent Details {UserId}", userId);
                throw;
            }
        }

        public bool resetForgotPassword(string Email)
        {
            try
            {
                var userId = _usersRepository.GetEmailByUserId(Email);
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception($"User with ID {userId} not found or email not available.");
                }

                Guid newGuid = Guid.NewGuid();

                ResetPassword objResetPassword = new ResetPassword();
                objResetPassword.UserId = userId;
                objResetPassword.Email = Email;
                objResetPassword.Token = newGuid.ToString();

                var result = _usersRepository.ResetForgotPassword(objResetPassword);

                var URL = _urlSettings.UIurl;
                string baseUrl = URL + "reset-password?token=" + newGuid.ToString();

                _emailService.SendEmail(Email, "Reset Forgot Password", "Reset Forgot Password Link Click And Set New Password." + baseUrl);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resetForgotPassword {Email}", Email);
                throw;
            }
        }

        public bool updatePassword(UpdatePassword updatePassword)
        {
            try
            {
                var result = _usersRepository.updatePassword(updatePassword);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to updatePassword {Token}", updatePassword.Token);
                throw;
            }
        }
    }
}
