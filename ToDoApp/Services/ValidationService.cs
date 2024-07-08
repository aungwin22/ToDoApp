using System;

namespace ToDoApp.Services
{
    /// <summary>
    /// Provides validation services for various input types.
    /// </summary>
    public class ValidationService
    {
        /// <summary>
        /// Validates the provided date input string.
        /// </summary>
        /// <param name="dateInput">The date input string in yyyy-MM-dd format.</param>
        /// <param name="date">The parsed DateTime value if valid.</param>
        /// <returns>True if the date is valid, otherwise false.</returns>
        public bool IsValidDate(string dateInput, out DateTime date)
        {
            // Try to parse the date input string into a DateTime object with the specified format
            if (DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                // Validate the year, month, and day components of the parsed date
                if (date.Year < 1 || date.Year > 9999 || date.Month < 1 || date.Month > 12 || date.Day < 1 || date.Day > DateTime.DaysInMonth(date.Year, date.Month))
                {
                    return false;
                }
                return true; // The date is valid
            }
            return false; // The date is invalid
        }
    }
}
