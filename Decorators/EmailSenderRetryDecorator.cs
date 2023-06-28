using Microsoft.Extensions.Options;
using OnlineShopPoC.Interfaces;
using OnlineShopPoC.Objects;
using Polly.Retry;
using Polly;
using SendGrid;
using Serilog;
using Serilog.Extensions.Logging;
using System.Net;
using Newtonsoft.Json.Linq;

namespace OnlineShopPoC.Decorators
{
    /// <summary>
    /// Decorator class that adds logging functionality to an email sender.
    /// </summary>
    public class EmailSenderRetryDecorator : IEmailSender
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EmailSenderRetryDecorator> _logger;
        private readonly int _emailRetryAttempts;

        /// <summary>
        /// Initializes a new instance of the EmailSenderLoggingDecorator class.
        /// </summary>
        /// <param name="emailSender">The underlying email sender to decorate.</param>
        /// <param name="logger">Logger to log the operations.</param>
        /// <param name="configuration">Used to retrieve the ammount of retry attempts.</param>
        public EmailSenderRetryDecorator(IEmailSender emailSender, ILogger<EmailSenderRetryDecorator> logger, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailRetryAttempts = configuration.GetValue<int>("EmailRetryAttemtps");
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
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(retryCount: _emailRetryAttempts, sleepDurationProvider: (attemptCount) => TimeSpan.FromSeconds(attemptCount * 2),
                onRetry: (exception, sleepDuration, attemptNumber, context) =>
                {
                    _logger.LogWarning(
                    exception, "Error while sending email, Attempt number: {Attempt}. Retrying in {Time}...", attemptNumber, sleepDuration);
                });


            var result = await policy.ExecuteAndCaptureAsync(
                                        () => _emailSender.SendEmailAsync(recipient, subject, body));

            if (result.Outcome == OutcomeType.Failure)
            {
                _logger.LogError(result.FinalException, "There was an error while sending email");
            }

        }   
    }
}
