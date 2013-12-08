using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{

    /// <summary>
    /// Date utils static functions
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class DateUtils
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Static - WeekDay functions
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert Day of week into integer number.
        /// </summary>
        /// <returns>Integer number (1=Monday, 2=Tuesday, ..., 7=Sunday)</returns>
        static public int WeekDayToNumber() {
            return WeekDayToNumber(DateTime.Now.DayOfWeek);
        }
        /// <summary>
        /// Convert Day of week into integer number.
        /// </summary>
        /// <param name="date">Date desired</param>
        /// <returns>Integer number (1=Monday, 2=Tuesday, ..., 7=Sunday)</returns>
        static public int WeekDayToNumber(DateTime date) {
            return WeekDayToNumber(date.DayOfWeek);
        }
        /// <summary>
        /// Convert Day of week into integer number.
        /// </summary>
        /// <param name="dayOfWeek">Day of week desired</param>
        /// <returns>Integer number (1=Monday, 2=Tuesday, ..., 7=Sunday)</returns>
        static public int WeekDayToNumber(DayOfWeek dayOfWeek) {
            switch (dayOfWeek) {
                case DayOfWeek.Monday: return 1; 
                case DayOfWeek.Tuesday: return 2; 
                case DayOfWeek.Wednesday: return 3; 
                case DayOfWeek.Thursday: return 4; 
                case DayOfWeek.Friday: return 5; 
                case DayOfWeek.Saturday: return 6; 
                case DayOfWeek.Sunday: return 7; 
                default: return 1; 
            }
        }
        /// <summary>
        /// Convery day of week number into french string
        /// </summary>
        /// <param name="dayNumber">Day of week number desired</param>
        /// <returns>French day name</returns>
        static public string WeekDayNumberToString(int dayNumber) {
            switch (dayNumber) {
                case 1: return "Lundi";
                case 2: return "Mardi";
                case 3: return "Mercredi";
                case 4: return "Jeudi";
                case 5: return "Vendredi";
                case 6: return "Samedi";
                case 7: return "Dimanche";
                default: return "";
            }
        }
        /// <summary>
        /// Get valid day id (1..7) of an int
        /// </summary>
        /// <param name="dayNumber">Day int number desired</param>
        /// <returns>Return valid day id (1..7)</returns>
        static public int WeekDayOfInt(int dayNumber) {
            if (dayNumber < 1) dayNumber = 1;
            return ((dayNumber - 1) % 7) + 1;
        }
        /// <summary>
        /// Add to day of week number
        /// </summary>
        /// <param name="dayNumber">Day of week number desired</param>
        /// <param name="dayCountToAdd">Count of day to add</param>
        /// <returns>Valid day of week number after day addition</returns>
        static public int WeekDayAdd(int dayNumber, int dayCountToAdd) {
            dayNumber = WeekDayOfInt(dayNumber);
            return WeekDayOfInt(dayNumber + dayCountToAdd);
        }

    }
}
