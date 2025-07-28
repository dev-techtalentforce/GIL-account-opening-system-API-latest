using System.ComponentModel.DataAnnotations;

namespace GIL_Agent_Portal.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? RefreshToken { get; set; }   
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? mobile { get; set; }
        public string? PanCard { get; set; }
        public string? ReferralCode { get; set; }
        public string? Address { get; set; }
        public bool? status { get; set; }
        public int? BlockStatus { get; set; }

    }

    public class  updateUser
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public bool? status { get; set; }
        public int? BlockStatus { get; set; }
    }
    public class updatePassword
    {
        public int UserId { get; set; }
        public string? PasswordHash { get; set; }
    }
}
