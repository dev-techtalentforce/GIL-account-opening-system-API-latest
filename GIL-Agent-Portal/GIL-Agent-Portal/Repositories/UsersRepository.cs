﻿using GIL_Agent_Portal.DataContext;
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



        public bool UserRegister(Users users)
        {
            var sp = "users_add";
            var parameters = new DynamicParameters();

            parameters.Add("@FirstName", users.FirstName);
            parameters.Add("@LastName", users.LastName);
            parameters.Add("@Email", users.Email);
            parameters.Add("@PasswordHash", users.PasswordHash);
            parameters.Add("@RoleId", users.RoleId);
            parameters.Add("@IsActive", users.IsActive);
            parameters.Add("@RefreshToken", users.RefreshToken);
            parameters.Add("@RefreshTokenExpiryTime", users.RefreshTokenExpiryTime);
            parameters.Add("@Mobile", users.mobile);
            parameters.Add("@PanCrad", users.PanCard);
            parameters.Add("@ReferralCode", users.ReferralCode);
            parameters.Add("@Address", users.Address);

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

        public string GetUserEmailById(int userId)
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

            var sp = "user_update";
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
            //parameters.Add("@Address", users.Address);
            parameters.Add("@status", users.status);

            try
            {
                _logger.LogInformation("Updating user with UserId: {UserId}", users.UserId);
                var updatedUser = _dbConnection.QuerySingleOrDefault<Users>(sp, parameters, commandType: CommandType.StoredProcedure);
                if (updatedUser == null)
                {
                    _logger.LogWarning("No user updated for UserId: {UserId}", users.UserId);
                    throw new Exception("No record found or updated for the specified UserId.");
                }
                _logger.LogInformation("User updated successfully with email: {Email}", updatedUser.Email);
                return updatedUser ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with UserId: {UserId}", users.UserId);
                throw new Exception($"Error updating user: {ex.Message}", ex);
            }

        }
    }
}

