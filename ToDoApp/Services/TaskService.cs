using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleTables;
using ToDoApp.Models;

namespace ToDoApp.Services
{
    /// <summary>
    /// Provides services for managing tasks, including CRUD operations and task filtering.
    /// </summary>
    public class TaskService
    {
        private readonly TaskRepository _taskRepository;
        private readonly OpenWeatherService _weatherService;
        private readonly ErrorLogger _errorLogger;
        private readonly ValidationService _validationService;

        /// <summary>
        /// Initializes a new instance of the TaskService class.
        /// </summary>
        /// <param name="taskRepository">The task repository for database operations.</param>
        /// <param name="weatherService">The weather service for fetching weather information.</param>
        /// <param name="errorLogger">The error logger for logging errors.</param>
        /// <param name="validationService">The validation service for input validation.</param>
        public TaskService(TaskRepository taskRepository, OpenWeatherService weatherService, ErrorLogger errorLogger, ValidationService validationService)
        {
            _taskRepository = taskRepository;
            _weatherService = weatherService;
            _errorLogger = errorLogger;
            _validationService = validationService;
        }

        /// <summary>
        /// Adds a new task asynchronously.
        /// </summary>
        public async Task AddTaskAsync()
        {
            try
            {
                var task = GetTaskDetailsFromUser();
                if (task == null) return;

                task.WeatherInfo = await _weatherService.GetWeatherAsync(task.City);

                await _taskRepository.AddTaskAsync(task);
                Console.WriteLine("Task added successfully.");
            }
            catch (Exception ex)
            {
                HandleException("Error adding task", ex);
            }
        }

        /// <summary>
        /// Displays all tasks.
        /// </summary>
        public void ViewTasks()
        {
            try
            {
                var tasks = _taskRepository.GetTasks();
                DisplayTasks(tasks);
            }
            catch (Exception ex)
            {
                HandleException("Error viewing tasks", ex);
            }
        }

        /// <summary>
        /// Marks a task as completed.
        /// </summary>
        public void MarkTaskAsCompleted()
        {
            try
            {
                var task = GetTaskById();
                if (task == null) return;

                task.IsCompleted = true;
                _taskRepository.SaveChangesAsync().Wait();
                Console.WriteLine("Task marked as completed.");
            }
            catch (Exception ex)
            {
                HandleException("Error marking task as completed", ex);
            }
        }

        /// <summary>
        /// Removes a task.
        /// </summary>
        public void RemoveTask()
        {
            try
            {
                var task = GetTaskById();
                if (task == null) return;

                _taskRepository.RemoveTask(task);
                Console.WriteLine("Task removed.");
            }
            catch (Exception ex)
            {
                HandleException("Error removing task", ex);
            }
        }

        /// <summary>
        /// Edits an existing task.
        /// </summary>
        public void EditTask()
        {
            try
            {
                var task = GetTaskById();
                if (task == null) return;

                UpdateTaskDetailsFromUser(task);

                _taskRepository.SaveChangesAsync().Wait();
                Console.WriteLine("Task updated.");
            }
            catch (Exception ex)
            {
                HandleException("Error editing task", ex);
            }
        }

        /// <summary>
        /// Filters tasks by completion status.
        /// </summary>
        public void FilterTasksByCompletionStatus()
        {
            try
            {
                var status = GetCompletionStatusFromUser();
                if (status == null) return;

                var tasks = _taskRepository.GetTasks().Where(t => t.IsCompleted == status).ToList();
                DisplayTasks(tasks);
            }
            catch (Exception ex)
            {
                HandleException("Error filtering tasks by completion status", ex);
            }
        }

        /// <summary>
        /// Filters tasks by date range.
        /// </summary>
        public void FilterTasksByDateRange()
        {
            try
            {
                var dateRange = GetDateRangeFromUser();
                if (dateRange == null) return;

                var tasks = _taskRepository.GetTasks().Where(t => t.DueDate >= dateRange.Item1 && t.DueDate <= dateRange.Item2).ToList();
                DisplayTasks(tasks);
            }
            catch (Exception ex)
            {
                HandleException("Error filtering tasks by date range", ex);
            }
        }

        /// <summary>
        /// Gets task details from user input.
        /// </summary>
        /// <returns>A new ToDoTask object with user-provided details.</returns>
        private ToDoTask GetTaskDetailsFromUser()
        {
            Console.Write("Enter title (Maximum 20 characters): ");
            var title = Console.ReadLine();
            if (title.Length > 20)
            {
                Console.WriteLine("Title cannot be more than 20 characters.");
                return null;
            }

            Console.Write("Enter description (Maximum 45 characters): ");
            var description = Console.ReadLine();
            if (description.Length > 45)
            {
                Console.WriteLine("Description cannot be more than 45 characters.");
                return null;
            }

            Console.Write("Enter due date (yyyy-MM-dd, e.g., 2024-12-30): ");
            var dueDateInput = Console.ReadLine();
            if (!_validationService.IsValidDate(dueDateInput, out var dueDate))
            {
                Console.WriteLine("Invalid date. Please enter a valid date in yyyy-MM-dd format.");
                return null;
            }

            Console.Write("Enter city name for weather information (if your city name is not listed, weather information may not show correctly): ");
            var cityName = Console.ReadLine();

            return new ToDoTask
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                IsCompleted = false,
                City = cityName
            };
        }

        /// <summary>
        /// Updates task details from user input.
        /// </summary>
        /// <param name="task">The task to update.</param>
        private void UpdateTaskDetailsFromUser(ToDoTask task)
        {
            Console.Write("Enter new title (leave blank to keep current, Maximum 20 characters): ");
            var title = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(title) && title.Length > 20)
            {
                Console.WriteLine("Title cannot be more than 20 characters.");
                return;
            }

            Console.Write("Enter new description (leave blank to keep current, Maximum 45 characters): ");
            var description = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(description) && description.Length > 45)
            {
                Console.WriteLine("Description cannot be more than 45 characters.");
                return;
            }

            Console.Write("Enter new due date (leave blank to keep current, yyyy-MM-dd, e.g., 2024-12-30): ");
            var dueDateInput = Console.ReadLine();
            DateTime dueDate = task.DueDate;
            if (!string.IsNullOrWhiteSpace(dueDateInput))
            {
                if (!_validationService.IsValidDate(dueDateInput, out dueDate))
                {
                    Console.WriteLine("Invalid date. Please enter a valid date in yyyy-MM-dd format.");
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(title))
                task.Title = title;
            if (!string.IsNullOrWhiteSpace(description))
                task.Description = description;
            if (!string.IsNullOrWhiteSpace(dueDateInput))
                task.DueDate = dueDate;
        }

        /// <summary>
        /// Retrieves a task by its ID from user input.
        /// </summary>
        /// <returns>The task with the specified ID, or null if not found.</returns>
        private ToDoTask GetTaskById()
        {
            Console.Write("Enter task Id: ");
            var id = int.Parse(Console.ReadLine());
            return _taskRepository.FindTask(id);
        }

        /// <summary>
        /// Gets the completion status from user input.
        /// </summary>
        /// <returns>A boolean value representing the completion status, or null if input is invalid.</returns>
        private bool? GetCompletionStatusFromUser()
        {
            Console.Write("Enter completion status (yes/no): ");
            var statusInput = Console.ReadLine().ToLower();
            if (statusInput == "yes")
                return true;
            if (statusInput == "no")
                return false;

            Console.WriteLine("Invalid status. Please enter 'yes' or 'no'.");
            return null;
        }

        /// <summary>
        /// Gets a date range from user input.
        /// </summary>
        /// <returns>A tuple containing the start and end dates, or null if input is invalid.</returns>
        private Tuple<DateTime, DateTime>? GetDateRangeFromUser()
        {
            Console.Write("Enter start date (yyyy-MM-dd): ");
            var startDateInput = Console.ReadLine();
            if (!_validationService.IsValidDate(startDateInput, out var startDate))
            {
                Console.WriteLine("Invalid date. Please enter a valid date in yyyy-MM-dd format.");
                return null;
            }

            Console.Write("Enter end date (yyyy-MM-dd): ");
            var endDateInput = Console.ReadLine();
            if (!_validationService.IsValidDate(endDateInput, out var endDate))
            {
                Console.WriteLine("Invalid date. Please enter a valid date in yyyy-MM-dd format.");
                return null;
            }

            return new Tuple<DateTime, DateTime>(startDate, endDate);
        }

        /// <summary>
        /// Displays tasks in a table format.
        /// </summary>
        /// <param name="tasks">The tasks to display.</param>
        private void DisplayTasks(IEnumerable<ToDoTask> tasks)
        {
            var table = new ConsoleTable("Id", "Title", "Description", "Due Date", "Completed", "WeatherInfo");

            foreach (var task in tasks)
            {
                table.AddRow(task.Id, task.Title, task.Description, task.DueDate.ToString("yyyy-MM-dd"), task.IsCompleted ? "Yes" : "No", task.WeatherInfo);
            }

            table.Write();
        }

        /// <summary>
        /// Handles exceptions by logging the error and displaying a message.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <param name="ex">The exception to log.</param>
        private void HandleException(string message, Exception ex)
        {
            Console.WriteLine($"{message}: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            _errorLogger.LogError(message, ex);
        }
    }
}
