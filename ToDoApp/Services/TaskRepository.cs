using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Services
{
    /// <summary>
    /// Repository class for managing tasks in the database.
    /// </summary>
    public class TaskRepository
    {
        private readonly AppDbContext _db;
   

        public TaskRepository(AppDbContext db)
        {
            _db = db;
            
        }

        

        /// <summary>
        /// Adds a new task to the database asynchronously.
        /// </summary>
        /// <param name="task">The task to add.</param>
        public async Task AddTaskAsync(ToDoTask task)
        {
            _db.ToDoTasks.Add(task);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all tasks from the database ordered by due date.
        /// </summary>
        /// <returns>A list of tasks ordered by due date.</returns>
        public List<ToDoTask> GetTasks()
        {
            return _db.ToDoTasks.OrderBy(t => t.DueDate).ToList();
        }

        /// <summary>
        /// Finds a task by its ID.
        /// </summary>
        /// <param name="id">The ID of the task to find.</param>
        /// <returns>The task with the specified ID, or null if not found.</returns>
        public ToDoTask? FindTask(int id)
        {
            var task = _db.ToDoTasks.Find(id);
            if (task == null)
            {
              
                return null;
            }

            return task;
        }

        /// <summary>
        /// Saves all changes made in the context to the database asynchronously.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a task from the database.
        /// </summary>
        /// <param name="task">The task to remove.</param>
        public void RemoveTask(ToDoTask task)
        {
            _db.ToDoTasks.Remove(task);
            _db.SaveChanges();
        }
    }
}
