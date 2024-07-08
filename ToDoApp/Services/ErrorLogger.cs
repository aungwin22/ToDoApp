using System;
using System.IO;
using System.Threading.Tasks;

namespace ToDoApp.Services
{
    public class ErrorLogger
    {
        private readonly string _logFilePath = "error_log.txt";

        public void LogError(string message, Exception ex)
        {
            var logMessage = $"{DateTime.Now}: {message}\n{ex}\n";
            File.AppendAllText(_logFilePath, logMessage);
        }

        public async Task UploadLogsAsync()
        {
            if (File.Exists(_logFilePath))
            {
                try
                {
                    var logContent = await File.ReadAllTextAsync(_logFilePath);
                    // Simulate sending logs to a server
                    await SendLogsToServerAsync(logContent);

                    // Clear the log file after successful upload
                    File.WriteAllText(_logFilePath, string.Empty);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to upload logs: {ex.Message}");
                }
            }
        }

        private async Task SendLogsToServerAsync(string logContent)
        {
            // Implement the logic to send logs to a remote server
            // For example, you can use HttpClient to send a POST request
            // await Task.Delay(1000); // Simulate network delay
            //Console.WriteLine("Logs uploaded to server successfully.");
        }
    }
}
