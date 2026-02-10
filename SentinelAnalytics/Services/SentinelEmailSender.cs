using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System.Text;

namespace SentinelAnalytics.Services;

public class SentinelEmailSender(IConfiguration config, ILogger<SentinelEmailSender> logger) : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        logger.LogInformation($"[SENTINEL EMAIL SERVICE] To: {email} | Subject: {subject}");
        logger.LogInformation($"[CONTENT]: {htmlMessage}");

        var smtpHost = config["Email:SmtpHost"];
        var smtpPort = int.Parse(config["Email:SmtpPort"] ?? "587");
        var smtpUser = config["Email:SmtpUser"];
        var smtpPass = config["Email:SmtpPass"];

        var message = new MimeMessage();
        message.Sender = MailboxAddress.Parse(smtpUser);
        message.Subject = subject;

        message.From.Add(new MailboxAddress("SentinelAnalytics", smtpUser));
        message.To.Add(MailboxAddress.Parse(email));

        StringBuilder bodysb = new StringBuilder();
        bodysb.Append("<p style='margin-top:0; padding-top:0;'>");
        bodysb.Append(htmlMessage);
        bodysb.Append("</p>");

        var builder = new BodyBuilder();
        builder.HtmlBody = bodysb.ToString();
        message.Body = builder.ToMessageBody();

        try
        {
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(smtpHost, 587, SecureSocketOptions.Auto);
            await smtp.AuthenticateAsync(smtpUser, smtpPass);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error with sending email");
        }
    }
}