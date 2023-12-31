﻿using OnlineShopPoC.Interfaces;
using SendGrid;
using Serilog;
using Serilog.Extensions.Logging;

namespace OnlineShopPoC.Decorators
{
    /// <summary>
    /// Decorator class that adds logging functionality to an email sender.
    /// </summary>
    public class EmailSenderLoggingDecorator : IEmailSender
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EmailSenderLoggingDecorator> _logger;

        /// <summary>
        /// Initializes a new instance of the EmailSenderLoggingDecorator class.
        /// </summary>
        /// <param name="emailSender">The underlying email sender to decorate.</param>
        public EmailSenderLoggingDecorator(IEmailSender emailSender, ILogger<EmailSenderLoggingDecorator> logger)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sends an email asynchronously with logging.
        /// </summary>
        /// <param name="recipient">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        /// <returns>A Task representing the asynchronous operation with the response.</returns>
        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            _logger.LogInformation("Sending email to {Recipient}... {Subject}, {Body}", recipient, subject, body);
            await _emailSender.SendEmailAsync(recipient, subject, body);
        }
    }
}
