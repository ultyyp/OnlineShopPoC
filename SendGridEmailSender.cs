﻿using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Options;

namespace OnlineShopPoC
{
    /// <summary>
    /// Implementation of the email sender using SendGrid service.
    /// </summary>
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridClient _client;
        private readonly SendGridConfig _sendGridConfig;

        /// <summary>
        /// Initializes a new instance of the SendGridEmailSender class.
        /// </summary>
        /// <param name="config">The config gotten from appsettings.json.</param>
        public SendGridEmailSender(IOptionsSnapshot<SendGridConfig> config)
        {
            ArgumentNullException.ThrowIfNull(config);
            _sendGridConfig = config.Value;
            _client = new SendGridClient(_sendGridConfig.ApiKey);
        }

        

        /// <summary>
        /// Sends an email asynchronously using SendGrid.
        /// </summary>
        /// <param name="recipient">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            ArgumentNullException.ThrowIfNull(recipient);
            ArgumentNullException.ThrowIfNull(subject);
            ArgumentNullException.ThrowIfNull(body);

            var myEmail = Environment.GetEnvironmentVariable("myemail");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_sendGridConfig.FromEmail, "OnlineShopPoC"),
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = $"<strong>{body}</strong>"
            };

            msg.AddTo(new EmailAddress(recipient, "To user"));
            await _client.SendEmailAsync(msg).ConfigureAwait(false);
        }
    }
}
