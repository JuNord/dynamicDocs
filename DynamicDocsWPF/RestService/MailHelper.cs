using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace RestService
{
    public static class MailHelper
    {
        public static bool SendMail(string target, string subject, string message)
        {
            var regex = new Regex("^([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})$");
            
            
            if (regex.IsMatch(target) && !string.IsNullOrWhiteSpace(subject) && !string.IsNullOrWhiteSpace(message))
            {
                var mail = new MailMessage("noreply@atiw.de", target)
                {
                    Subject = subject, 
                    Body = message
                };
                var client = new SmtpClient
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    Credentials = new NetworkCredential("noreply@atiw.de","1234"),
                    EnableSsl = true
                };
                
                client.Send(mail);

                return true;
            }

            return false;
        }
    }
}