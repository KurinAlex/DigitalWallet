﻿using DigitalWallet.Services.Options;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SendGrid;
using SendGrid.Helpers.Mail;

namespace DigitalWallet.Services;

public class EmailSender(IOptions<EmailSenderOptions> optionsAccessor, ILogger<EmailSender> logger) : IEmailSender
{
    private readonly ILogger _logger = logger;

    private EmailSenderOptions Options => optionsAccessor.Value;

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        ArgumentException.ThrowIfNullOrEmpty(Options.SendGridKey, nameof(optionsAccessor.Value.SendGridKey));
        ArgumentException.ThrowIfNullOrEmpty(Options.SenderEmail, nameof(optionsAccessor.Value.SenderEmail));

        await Execute(Options.SendGridKey, Options.SenderEmail, subject, message, toEmail);
    }

    public async Task Execute(string apiKey, string fromEmail, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage
        {
            From = new EmailAddress(fromEmail, "Password Recovery"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation(
            response.IsSuccessStatusCode ? "Email to {ToEmail} queued successfully!" : "Failure Email to {ToEmail}",
            toEmail);
    }
}