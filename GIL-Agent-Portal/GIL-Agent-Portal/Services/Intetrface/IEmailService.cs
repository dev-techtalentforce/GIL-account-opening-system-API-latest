namespace GIL_Agent_Portal.Services.Intetrface
{
    public interface IEmailService
    {
        void SendEmail(string toEmail, string subject, string message);
    }
}
