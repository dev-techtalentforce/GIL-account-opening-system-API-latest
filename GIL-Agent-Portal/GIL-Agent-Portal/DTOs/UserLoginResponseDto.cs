﻿namespace GIL_Agent_Portal.DTOs
{
    public class UserLoginResponseDto
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public int? nsdl_status { get; set; }         
        public string KYCStatus { get; set; }
        public string ReferralCode { get; set; }
        public int? RoleId { get; set; }
    }
}
