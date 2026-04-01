using System.Net;
using System.Net.Mail;

namespace ShipzoBackend.Services
{
    public class EmailService
    {
        public static void SendEmail(string to, string subject, string body)
        {
            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("liveeasynotification@gmail.com", "app-password"),
                EnableSsl = true
            };

            smtp.Send("liveeasynotification@gmail.com", to, subject, body);
        }
    }
}
