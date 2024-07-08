using System;

namespace ToDoApp.Services
{
    public class UserInterfaceService
    {
        public void ShowMenu()
        {
            Console.WriteLine("\nToDo List Application");
            Console.WriteLine("1. Add new task");
            Console.WriteLine("2. View all tasks");
            Console.WriteLine("3. Mark task as completed");
            Console.WriteLine("4. Remove task");
            Console.WriteLine("5. Edit task");
            Console.WriteLine("6. Filter tasks by completion status");
            Console.WriteLine("7. Filter tasks by date range");
            Console.WriteLine("8. Exit");
            Console.Write("Enter your choice: ");
        }
    }
}
