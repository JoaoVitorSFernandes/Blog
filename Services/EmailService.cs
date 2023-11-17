using System.Net;
using System.Net.Mail;

namespace Blog.Services;

public class EmailService
{
    public bool Send(string toName, string toEmail, string subject, string body)
    {
        var smtpClient   = new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port);        
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(Configuration.Smtp.UserEmail, Configuration.Smtp.Password);
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

        var emailMessage = new MailMessage();
        emailMessage.From = new MailAddress(Configuration.Smtp.UserEmail, Configuration.Smtp.UserName);
        emailMessage.Subject = subject;
        emailMessage.Body = body;
        emailMessage.IsBodyHtml = true;
        emailMessage.Priority = MailPriority.Normal;
        emailMessage.To.Add(new MailAddress(toEmail, toName));
        
        try
        {
            smtpClient.Send(emailMessage);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}