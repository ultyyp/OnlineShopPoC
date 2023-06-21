using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Reflection;

namespace OnlineShopPoC
{
    /// <summary>
    /// Background service that sends startup and status notification emails.
    /// </summary>
    public class AppStartedNotificatorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IClock _clock;

        /// <summary>
        /// Initializes a new instance of the AppStartedNotificatorBackgroundService class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="clock">The clock used to get the local date and time.</param>
        public AppStartedNotificatorBackgroundService(IServiceProvider serviceProvider, IClock clock)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        /// <summary>
        /// Executes the background service asynchronously.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var localServiceProvider = scope.ServiceProvider;
            var emailSender = localServiceProvider.GetRequiredService<IEmailSender>();

            await SendStartupEmail(emailSender);
            Console.WriteLine("Startup Email Sent");

            using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
            Stopwatch sw = Stopwatch.StartNew();
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await SendStatusEmail(emailSender);
                Console.WriteLine("Status Email Sent");
            }
        }

        /// <summary>
        /// Sends a startup email notification.
        /// </summary>
        /// <param name="emailSender">The email sender service.</param>
        private async Task SendStartupEmail(IEmailSender emailSender)
        {
            var toEmail = Environment.GetEnvironmentVariable("myemail2");
            await emailSender.SendEmailAsync(toEmail, "Server Started", "The server has been started on " + _clock.GetLocalDate().ToString());
        }

        /// <summary>
        /// Sends a status email notification.
        /// </summary>
        /// <param name="emailSender">The email sender service.</param>
        private async Task SendStatusEmail(IEmailSender emailSender)
        {
            var toEmail = Environment.GetEnvironmentVariable("myemail2");
            await emailSender.SendEmailAsync(toEmail, "Server Started", "Server is working properly on " + _clock.GetLocalDate().ToString()
                + " with a RAM usage of " + GetMemoryUsage() + "B");
        }

        /// <summary>
        /// Retrieves the memory usage of the current process.
        /// </summary>
        /// <returns>The memory usage in bytes.</returns>
        private string GetMemoryUsage()
        {
            try
            {
                string fname = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);

                ProcessStartInfo ps = new ProcessStartInfo("tasklist");
                ps.Arguments = "/fi \"IMAGENAME eq " + fname + ".*\" /FO CSV /NH";
                ps.RedirectStandardOutput = true;
                ps.CreateNoWindow = true;
                ps.UseShellExecute = false;
                var p = Process.Start(ps);
                if (p.WaitForExit(1000))
                {
                    var s = p.StandardOutput.ReadToEnd().Split('\"');
                    return s[9].Replace("\"", "");
                }
            }
            catch { }
            return "Unable to get memory usage";
        }
    }
}
