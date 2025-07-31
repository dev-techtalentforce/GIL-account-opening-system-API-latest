namespace GIL_Agent_Portal.Models
{
    public class ResetPassword
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public DateTime? SendDate { get; set; }
        public bool? Status { get; set; }
    }

    public class UpdatePassword
    {
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
