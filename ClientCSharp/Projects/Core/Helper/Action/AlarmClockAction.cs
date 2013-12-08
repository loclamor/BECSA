using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Action for alarm clock
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class AlarmClockAction : AbstractAction
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Enums
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Action recognized
        /// </summary>
        public enum ActionType
        {
            // None for the moment
        };

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Action to do on alarm clock
        /// </summary>
        public ActionType Action { get; protected set; }
        /// <summary>
        /// Can be <see cref="SmartHome.AlarmClock.Id"/> or -1.
        /// </summary>
        /// <remarks>-1 = All</remarks>
        public int AlarmClockId { get; protected set; }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize an alarm clock action
        /// </summary>
        /// <param name="act">Action to do with alarm clock</param>
        /// <param name="alarmClockId">Alarm clock identifier number to control</param>
        public AlarmClockAction(ActionType act, int alarmClockId = -1) {
            Action = act;
            AlarmClockId = alarmClockId;
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
            // None for the moment
            return null;
        }

    }

}
