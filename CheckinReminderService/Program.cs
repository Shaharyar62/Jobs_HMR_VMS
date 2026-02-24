using Microsoft.Extensions.Configuration;
using Logger;

namespace CheckinReminderService
{
    class Program
    {
        static async Task Main()
        {
            Logger.Logger.Write("Service Start", "Check-In Reminder Service Started", "", Logger.Logger.LogType.InformationLog);

            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ApiSettings? apiSettings = config.GetSection("ApiSettings").Get<ApiSettings>();

            if (apiSettings == null || string.IsNullOrWhiteSpace(apiSettings.EndpointUrl))
            {
                Logger.Logger.Write("Configuration Error", "ApiSettings are missing or EndpointUrl is not set", "", Logger.Logger.LogType.ErrorLog);
                Console.WriteLine("Error: API configuration is missing. Check appsettings.json.");
                return;
            }

            try
            {
                string response = await ApiService.CallApiAsync(apiSettings);

                Logger.Logger.Write("Reminders Sent", "SendCheckinReminders completed successfully", response, Logger.Logger.LogType.InformationLog);
                Console.WriteLine("Check-in reminder job completed successfully.");
                Console.WriteLine($"Response: {response}");
            }
            catch (Exception ex)
            {
                Logger.Logger.Write("Service Error", "Exception occurred while sending reminders", ex.Message + " | StackTrace: " + ex.StackTrace, Logger.Logger.LogType.ErrorLog);
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
