using System;

namespace ToDoApp.Services
{
    /// <summary>
    /// Provides user interface services for displaying menus and capturing user input.
    /// </summary>
    public class UserInterfaceService
    {
        /// <summary>
        /// Displays the main menu of the ToDo List application.
        /// </summary>
        public void ShowMenu()
        {
            Console.WriteLine("\nToDo List Application"); // Display the application title
            Console.WriteLine("1. Add new task"); // Option to add a new task
            Console.WriteLine("2. View all tasks"); // Option to view all tasks
            Console.WriteLine("3. Mark task as completed"); // Option to mark a task as completed
            Console.WriteLine("4. Remove task"); // Option to remove a task
            Console.WriteLine("5. Edit task"); // Option to edit a task
            Console.WriteLine("6. Filter tasks by completion status"); // Option to filter tasks by completion status
            Console.WriteLine("7. Filter tasks by date range"); // Option to filter tasks by date range
            Console.WriteLine("8. Exit"); // Option to exit the application
            Console.Write("Enter your choice: "); // Prompt the user to enter their choice
        }
    }
}
