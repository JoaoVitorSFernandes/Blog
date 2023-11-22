using System.Net;
using System.Net.Mail;
using Blog.ViewModels;

namespace Blog.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configruation)
        => _configuration = configruation;

    public bool Send(string toName, string toEmail, string subject, string body)
    {
        var smtp = new StmpConfigurations();
        _configuration.GetSection("SmtpConfigurations").Bind(smtp);
        
        var smtpClient = new SmtpClient(smtp.Host, smtp.Port);
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(smtp.UserEmail, smtp.Password);
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

        var emailMessage = new MailMessage();
        emailMessage.From = new MailAddress(smtp.UserEmail, smtp.UserName);
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
        catch (Exception)
        {
            return false;
        }
    }
}