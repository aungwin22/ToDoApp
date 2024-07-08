using System;
using System.IO;
using System.Threading.Tasks;

namespace ToDoApp.Services
{
    /// <summary>
    /// Provides functionalities to log and upload error messages.
    /// but uploading part is to enhance for real project not for now :D
    /// </summary>
    public class ErrorLogger
    {
        // Path to the error log file
        private readonly string _logFilePath = "error_log.txt";

        /// <summary>
        /// Logs the provided error message and exception details to a log file.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="ex">The exception to log.</param>
        public void LogError(string message, Exception ex)
        {
            var logMessage = $"{DateTime.Now}: {message}\n{ex}\n";            
            File.AppendAllText(_logFilePath, logMessage);     // Append the log message to the error log file
        }

        /// <summary>
        /// Uploads the error logs to a remote server asynchronously.
        /// This is just a demostration purpose only when we are using like mobile application etc.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Sends the log content to a remote server asynchronously.
        /// </summary>
        /// <param name="logContent">The log content to send.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SendLogsToServerAsync(string logContent)
        {
            // Implement the logic to send logs to a remote server
            // For example, you can use HttpClient to send a POST request
             await Task.Delay(1000); // Simulate network delay
            Console.WriteLine("Logs uploaded to server successfully.");
        }
    }
}
