using Demo.Abstractions;
using System.Diagnostics;

namespace Demo.Implementors
{
    public class ConsoleEmailSender : IEmailSender
    {
        public void SendEmail(string email, string body)
        {
            Debug.WriteLine($"To: {email} with: {body}");
        }
    }
}
