using SendGrid;
using Serilog;

namespace OnlineShopPoC
{
    /// <summary>
    /// Decorator class that adds logging functionality to an email sender.
    /// </summary>
    public class EmailSenderLoggingDecorator : IEmailSender
    {
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Initializes a new instance of the EmailSenderLoggingDecorator class.
        /// </summary>
        /// <param name="emailSender">The underlying email sender to decorate.</param>
        public EmailSenderLoggingDecorator(IEmailSender emailSender)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }

        /// <summary>
        /// Sends an email asynchronously with logging.
        /// </summary>
        /// <param name="recipient">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        /// <returns>A Task representing the asynchronous operation with the response.</returns>
        public async Task<Response> SendEmailAsync(string recipient, string subject, string body)
        {
            Log.Information($"Sending email to {recipient}... {subject}, {body}");
            var response = await _emailSender.SendEmailAsync(recipient, subject, body);

            if(!response.IsSuccessStatusCode)
            {
                
                Log.Information("Sending Email Failed! Retrying...");
                Log.Information($"Sending email to {recipient}... {subject}, {body}");
                var retryresponse = await _emailSender.SendEmailAsync(recipient, subject, body);
                if(!retryresponse.IsSuccessStatusCode) 
                {
                    Log.Information("Retry Failed! Please Try Again Later!");
                }
                else if(retryresponse.IsSuccessStatusCode)
                {
                    Log.Information("Retry Successful!");
                }
            }
            else
            {
                Log.Information("Email Sent Succesfully!");
            }
            return response;
        }
    }
}
