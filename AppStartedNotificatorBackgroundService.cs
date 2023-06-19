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
    /// Background service that sends email notifications on application startup and periodically.
    /// </summary>
    public class AppStartedNotificatorBackgroundService : BackgroundService
    {
        private readonly IEmailSender _emailSender;
        private readonly IClock _clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppStartedNotificatorBackgroundService"/> class.
        /// </summary>
        /// <param name="emailSender">The email sender service.</param>
        /// <param name="clock">The clock service.</param>
        public AppStartedNotificatorBackgroundService(IEmailSender emailSender, IClock clock)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        /// <summary>
        /// Executes the background service logic.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token that can be used to stop the service.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await SendStartupEmail();
            Console.WriteLine("Startup Email Sent");

            using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
            Stopwatch sw = Stopwatch.StartNew();
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await SendStatusEmail();
                Console.WriteLine("Status Email Sent");
            }
        }

        /// <summary>
        /// Sends the startup email notification.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SendStartupEmail()
        {
            var toEmail = Environment.GetEnvironmentVariable("myemail2");
            await _emailSender.SendEmailAsync(toEmail, "Server Started", "The server has been started on " + _clock.GetLocalDate().ToString());
        }

        /// <summary>
        /// Sends the status email notification.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SendStatusEmail()
        {
            var toEmail = Environment.GetEnvironmentVariable("myemail2");
            await _emailSender.SendEmailAsync(toEmail, "Server Started", "Server is working properly on " + _clock.GetLocalDate().ToString()
                + " with a RAM usage of " + GetMemoryUsage() + "B");
        }

        /// <summary>
        /// Retrieves the memory usage of the application.
        /// </summary>
        /// <returns>A string representing the memory usage.</returns>
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
