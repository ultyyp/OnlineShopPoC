using SendGrid;
using System.Net;

namespace OnlineShopPoC.Interfaces
{
    /// <summary>
    /// Interface for sending emails.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="recipient">The email recipient.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="body">The email body.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SendEmailAsync(string recipient, string subject, string body);
    }
}
