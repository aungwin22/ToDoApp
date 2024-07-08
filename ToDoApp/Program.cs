using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Services;

namespace ToDoApp
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static async Task Main(string[] args)
        {
            using (var db = new AppDbContext())
            {
                // Initialize the database and create tables if they do not exist
                InitDatabase(db);

                // Initialize services
                var weatherService = new OpenWeatherService();
                var errorLogger = new ErrorLogger();
                var taskService = new TaskService(db, weatherService, errorLogger);
                var uiService = new UserInterfaceService();

                // Upload logs at the start of the application
                await errorLogger.UploadLogsAsync();

                // Main application loop
                while (true)
                {
                    // Display the menu to the user
                    uiService.ShowMenu();
                    var choice = Console.ReadLine();

                    // Process user input
                    switch (choice)
                    {
                        case "1":
                            // Add a new task
                            await taskService.AddTaskAsync();
                            break;
                        case "2":
                            // View all tasks
                            taskService.ViewTasks();
                            break;
                        case "3":
                            // Mark a task as completed
                            taskService.MarkTaskAsCompleted();
                            break;
                        case "4":
                            // Remove a task
                            taskService.RemoveTask();
                            break;
                        case "5":
                            // Edit an existing task
                            taskService.EditTask();
                            break;
                        case "6":
                            // Filter tasks by completion status
                            taskService.FilterTasksByCompletionStatus();
                            break;
                        case "7":
                            // Filter tasks by date range
                            taskService.FilterTasksByDateRange();
                            break;
                        case "8":
                            // Exit the application
                            Console.WriteLine("Exiting the application. Press any key to close the console...");
                            Console.ReadKey();
                            // Upload logs before exiting
                            await errorLogger.UploadLogsAsync();
                            return;
                        default:
                            // Handle invalid input
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the database by creating necessary tables if they do not exist.
        /// </summary>
        /// <param name="db">The application database context.</param>
        static void InitDatabase(AppDbContext db)
        {
            string createTableScript = @"
            CREATE TABLE IF NOT EXISTS ToDoTasks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Description TEXT NOT NULL,
                DueDate TEXT NOT NULL,
                IsCompleted BOOLEAN NOT NULL,
                WeatherInfo TEXT
            );";

            using (var connection = db.Database.GetDbConnection() as SqliteConnection)
            {
                connection.Open();

                // Check if the ToDoTasks table exists
                var tableExistsCommand = connection.CreateCommand();
                tableExistsCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='ToDoTasks';";
                var tableName = tableExistsCommand.ExecuteScalar();

                if (tableName == null)
                {
                    // Table does not exist, create it
                    var createTableCommand = connection.CreateCommand();
                    createTableCommand.CommandText = createTableScript;
                    createTableCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
