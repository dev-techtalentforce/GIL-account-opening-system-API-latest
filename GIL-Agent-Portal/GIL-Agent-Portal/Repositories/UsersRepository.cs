using GIL_Agent_Portal.DataContext;
using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.HttpResults;
using Dapper;
using GIL_Agent_Portal.DTOs;
using System.Data.Common;

namespace GIL_Agent_Portal.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<UsersRepository> _logger;

        public UsersRepository(IConfiguration configuration, IDbConnection dbConnection, ILogger<UsersRepository> logger)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection"); // update name if different
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public List<Users> GetAllUserRegisterdList()
        {
            List<Users> userList = new List<Users>();
            var sp = "user_RegistrationAgent_get";

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    var result = db.Query<Users>(sp, commandType: CommandType.StoredProcedure);
                    userList = result.ToList();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // e.g., _logger.LogError(ex, "Error in GetAll");
            }

            return userList;
        }
        public List<UsersResponseList> GetAllUAgentList()
        {
            List<UsersResponseList> userList = new List<UsersResponseList>();
            var sp = "UsersGetAllAgent";

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    var result = db.Query<UsersResponseList>(sp, commandType: CommandType.StoredProcedure);
                    userList = result.ToList();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // e.g., _logger.LogError(ex, "Error in GetAll");
            }

            return userList;
        }



        public bool UserRegister(Users users)
        {
            var sp = "users_add";
            var parameters = new DynamicParameters();

            parameters.Add("@FirstName", users.FirstName);
            parameters.Add("@LastName", users.LastName);
            parameters.Add("@Email", users.Email);
            parameters.Add("@PasswordHash", users.PasswordHash);
            //parameters.Add("@RoleId", users.RoleId);
            parameters.Add("@IsActive", users.IsActive);
            parameters.Add("@RefreshToken", users.RefreshToken);
            parameters.Add("@RefreshTokenExpiryTime", users.RefreshTokenExpiryTime);
            parameters.Add("@Mobile", users.mobile);
            parameters.Add("@PanCard", users.PanCard);
            parameters.Add("@ReferralCode", users.ReferralCode);
            parameters.Add("@Address", users.Address);

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    var result = db.ExecuteScalar<string>(sp, parameters, commandType: CommandType.StoredProcedure);
                    return !string.IsNullOrEmpty(result);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // e.g., _logger.LogError(ex, "Error in AddUsersRecord");
                return false;
            }
        }

        public UserLoginResponseDto LoginUser(UserLoginRequestDto request)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Email", request.Email, DbType.String);
                    parameters.Add("@Password", request.PasswordHash, DbType.String);

                    var result = db.QueryFirstOrDefault<UserLoginResponseDto>(
                        "UserLogin",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result; // returning the result here
                }
            }
            catch (Exception ex)
            {
                // Log ex here if needed
                throw new Exception("An error occurred while logging in: " + ex.Message);
            }
        }

        public string GetUserEmailById(string userId)
        {
            try
            {
                var query = "SELECT Email FROM Users WHERE UserId = @UserId";
                return _dbConnection.QuerySingleOrDefault<string>(query, new { UserId = userId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving email for UserId {userId}: {ex.Message}", ex);
            }
        }

        Users IUsersRepository.UserUpdate(updateUser users)
        {

            var sp = "status_action_update";
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", users.UserId);
            //parameters.Add("@FirstName", users.FirstName);
            //parameters.Add("@LastName", users.LastName);
            //parameters.Add("@Email", users.Email, DbType.String);
            //parameters.Add("@PasswordHash", users.PasswordHash);
            //parameters.Add("@RoleId", users.RoleId);
            //parameters.Add("@IsActive", users.IsActive);
            //parameters.Add("@RefreshToken", users.RefreshToken);
            //parameters.Add("@RefreshTokenExpiryTime", users.RefreshTokenExpiryTime);
            //parameters.Add("@Mobile", users.mobile);
            //parameters.Add("@PanCard", users.PanCard); // Adjusted
            //parameters.Add("@ReferralCode", users.ReferralCode);
            parameters.Add("@BlockStatus", users.BlockStatus);
            parameters.Add("@status", users.status);
            parameters.Add("@nsdl_status", users.nsdl_status);

            try
            {
                _logger.LogInformation("Updating user with UserId: {UserId}", users.UserId);
                var updatedUser = _dbConnection.QuerySingleOrDefault<Users>(sp, parameters, commandType: CommandType.StoredProcedure);

                if (updatedUser == null)
                {
                    _logger.LogWarning("No user updated for UserId: {UserId}", users.UserId);
                    throw new Exception("No record found or updated for the specified UserId.");
                }

                //_logger.LogInformation("User updated successfully with email: {Email}", updatedUser.Email);
                return updatedUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with UserId: {UserId}", users.UserId);
                throw new Exception($"Error updating user: {ex.Message}", ex);
            }

        }

        public Users UserProfileUpdate(Users users)
        {

            var sp = "user_profile_update";
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", users.UserId);
            parameters.Add("@FirstName", users.FirstName);
            parameters.Add("@LastName", users.LastName);
            parameters.Add("@Email", users.Email, DbType.String);
            parameters.Add("@Address", users.Address);
            parameters.Add("@Mobile", users.mobile);


            try
            {
                _logger.LogInformation("Updating user with UserId: {UserId}", users.UserId);
                var userProfileUpdate = _dbConnection.QuerySingleOrDefault<Users>(sp, parameters, commandType: CommandType.StoredProcedure);

                if (userProfileUpdate == null)
                {
                    _logger.LogWarning("No user updated for UserId: {UserId}", users.UserId);
                    throw new Exception("No record found or updated for the specified UserId.");
                }

                _logger.LogInformation("User updated successfully with email: {Email}", userProfileUpdate.Email);
                return userProfileUpdate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with UserId: {UserId}", users.UserId);
                throw new Exception($"Error updating user: {ex.Message}", ex);
            }
        }

        public int UpdateUserPassword(updatePassword request)
        {
            const string sp = "UpdateUsersPassword";
            var parameters = new DynamicParameters();
            parameters.Add("@PasseordHash", request.PasswordHash);
            parameters.Add("@UserId", request.UserId);

            try
            {
                _logger.LogInformation("Updating password for UserId: {UserId}", request.UserId);

                // Call stored procedure and get return value
                var result = _dbConnection.ExecuteScalar<int>(sp, parameters, commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Stored procedure return value: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password for UserId: {UserId}", request.UserId);
                throw new Exception($"Error updating user password: {ex.Message}", ex);
            }
        }

        public Users GetAgentDetails(string userId)
        {
            var sp = "Users_get_by_id";
            var parameters = new DynamicParameters();

            parameters.Add("@UserId", userId);

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    var result = db.QuerySingleOrDefault<Users>(sp, parameters, commandType: CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // e.g., _logger.LogError(ex, "Error in AddUsersRecord");
                return null;
            }
        }

        public string GetEmailByUserId(string email)
        {
            try
            {
                var query = "SELECT UserId FROM Users WHERE Email = @email";
                return _dbConnection.QuerySingleOrDefault<string>(query, new { Email = email });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving email for UserId {email}: {ex.Message}", ex);
            }
        }

        public bool ResetForgotPassword(ResetPassword resetPassword)
        {
            var sp = "resetForgotPassword_add";
            var parameters = new DynamicParameters();

            parameters.Add("@UserId", resetPassword.UserId);
            parameters.Add("@Email", resetPassword.Email);
            parameters.Add("@Token", resetPassword.Token);

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    var result = db.ExecuteScalar<int>(sp, parameters, commandType: CommandType.StoredProcedure);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // e.g., _logger.LogError(ex, "Error in AddUsersRecord");
                return false;
            }
        }

        public bool updatePassword(UpdatePassword updatePassword)
        {
            var sp = "UpdateUsersPassword";
            var parameters = new DynamicParameters();

            parameters.Add("@PasseordHash", updatePassword.Password);
            parameters.Add("@Token", updatePassword.Token);

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    var result = db.ExecuteScalar<int>(sp, parameters, commandType: CommandType.StoredProcedure);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // e.g., _logger.LogError(ex, "Error in AddUsersRecord");
                return false;
            }
        }

        public UsersResponseList GetAgentLoginData(string id)
        {
            var sp = "Users_get_by_id";
            var parameters = new DynamicParameters();

            parameters.Add("@UserId", id);

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    db.Open();
                    var result = db.QuerySingleOrDefault<UsersResponseList>(sp, parameters, commandType: CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // e.g., _logger.LogError(ex, "Error in AddUsersRecord");
                return null;
            }
        }
    }
}

