namespace Demo.Abstractions
{
    public interface IEmailSender
    {
        void SendEmail(string email, string body);
    }
}
