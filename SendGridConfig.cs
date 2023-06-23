using System.ComponentModel.DataAnnotations;

namespace OnlineShopPoC
{
    public class SendGridConfig
    {
        [Required]
        public string ApiKey { get; set; }
        [Required, EmailAddress]
        public string FromEmail { get; set; }
    }
}