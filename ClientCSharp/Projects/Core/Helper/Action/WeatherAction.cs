using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Action for weather
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class WeatherAction : AbstractAction
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Enums
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Action recognized
        /// </summary>
        public enum ActionType
        {
            /// <summary>
            /// Retrieve weather for a specific date
            /// </summary>
            GET_WEATHER
        };

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Action to do with weather
        /// </summary>
        public ActionType Action { get; protected set; }
        /// <summary>
        /// Desired week day: -1=Use <see cref="WeatherDate">WeatherDate</see> instead of, 1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday, 7=Sunday
        /// </summary>
        public int WeatherWeekDay { get; protected set; }
        /// <summary>
        /// Desired date used only if <see cref="WeatherWeekDay">WeatherWeekDay</see> == -1
        /// </summary>
        public DateTime WeatherDate { get; protected set; }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize an weather action
        /// </summary>
        /// <param name="act">Action to do with weather</param>
        public WeatherAction(ActionType act) {
            Action = act;
            WeatherWeekDay = -1;
            WeatherDate = DateTime.Now;
        }
        /// <summary>
        /// Initialize an weather action
        /// </summary>
        /// <param name="act">Action to do with weather</param>
        /// <param name="weatherWeekDay">Week desired</param>
        public WeatherAction(ActionType act, int weatherWeekDay) {
            Action = act;
            WeatherWeekDay = DateUtils.WeekDayOfInt(weatherWeekDay);
            WeatherDate = DateTime.Now;
        }
        /// <summary>
        /// Initialize an weather action
        /// </summary>
        /// <param name="act">Action to do with weather</param>
        /// <param name="weatherDate">Date desired</param>
        public WeatherAction(ActionType act, DateTime weatherDate) {
            Action = act;
            WeatherWeekDay = -1;
            WeatherDate = weatherDate;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Execute action
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute the action
        /// </summary>
        /// <param name="onHome">Home desired</param>
        /// <returns>Home response</returns>
        public override HomeResponse Execute(Home onHome) {
            if (Action == ActionType.GET_WEATHER) {
                if (WeatherWeekDay != -1) {
                    return onHome.Actions.SendAction("webapp", "meteo", WeatherWeekDay.ToString());
                } else {
                    return onHome.Actions.SendAction("webapp", "getWeather", WeatherDate.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            } else {
                return null;
            }
        }


    }

}
