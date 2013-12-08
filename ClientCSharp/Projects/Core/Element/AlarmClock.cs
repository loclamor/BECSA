using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Represent an Alarm clock of the SmartHome
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class AlarmClock
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Enum
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Day availaible for alarm clock.
        /// </summary>
        /// <remarks>Value are power of 2 in order to enable combinaison as: Days.Monday + Days.Sunday</remarks>
        public enum WeekDays 
        {
            /// <summary>
            /// No day defined
            /// </summary>
            None = 0,
            /// <summary>
            /// Monday -- (Doc comment present only to avoid compiler doc warnings)
            /// </summary>
            Monday = 1,
            /// <summary>
            /// Tuesday -- (Doc comment present only to avoid compiler doc warnings)
            /// </summary>
            Tuesday = 2,
            /// <summary>
            /// Wednesday -- (Doc comment present only to avoid compiler doc warnings)
            /// </summary>
            Wednesday = 4,
            /// <summary>
            /// Thursday -- (Doc comment present only to avoid compiler doc warnings)
            /// </summary>
            Thursday = 8,
            /// <summary>
            /// Friday -- (Doc comment present only to avoid compiler doc warnings)
            /// </summary>
            Friday = 16,
            /// <summary>
            /// Saturday -- (Doc comment present only to avoid compiler doc warnings)
            /// </summary>
            Saturday = 32,
            /// <summary>
            /// Sunday -- (Doc comment present only to avoid compiler doc warnings)
            /// </summary>
            Sunday = 64,
            /// <summary>
            /// Day of the week from Monday to Friday included
            /// </summary>
            Week = Monday | Tuesday | Wednesday | Thursday | Friday,
            /// <summary>
            /// Day of the weekend from Saturday to Sunday included
            /// </summary>
            WeekEnd = Saturday | Sunday,
            /// <summary>
            /// All the day
            /// </summary>
            All = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Alarm clock identifier
        /// </summary>
        public int Id { get; protected set; }
        /// <summary>
        /// Alarm clock name
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Hour part of the time when alarm clock will ring (<see cref="AlarmClock.Minute">Minute</see>, <see cref="AlarmClock.HourString">HourString</see>, <see cref="AlarmClock.HourDateTime">HourDateTime</see>)
        /// </summary>
        public int Hour { get; protected set; }
        /// <summary>
        /// Minute part of the time when alarm clock will ring (<see cref="AlarmClock.Hour">Hour</see>)
        /// </summary>
        public int Minute { get; protected set; }
        /// <summary>
        /// Days alarm clock have to ring
        /// </summary>
        public WeekDays Days { get; protected set; }
        /// <summary>
        /// Week count alarm clock have to ring
        /// </summary>
        public int WeekRepetition { get; protected set; }
        /// <summary>
        /// Last date the alarm clock have rang.
        /// </summary>
        public DateTime LastRing { get; protected set; }
        /// <summary>
        /// Private: Indicate if alarm clock is activate. To know if the alarm clock is active use <see cref="AlarmClock.Active">Active property</see>
        /// </summary>
        private bool _isActive;
        /// <summary>
        /// Private: Indicate if alarm clock is ringing. To know if the alarm clock is ringing use <see cref="AlarmClock.Ringing">Ringing property</see>
        /// </summary>
        private bool _isRinging;
        /// <summary>
        /// Private: Indicate the count of ring already done.
        /// </summary>
        private int _ringingCounter;
        /// <summary>
        /// Indicate that this home element have been just added
        /// </summary>
        public bool IsNewOne { get; protected set; }
        

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Initialize
        ///////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Private contrusctor used to initialize class members
        /// </summary>
        private AlarmClock() {
            Id = -1;
            Name = "";
            Hour = 0;
            Minute = 0;
            WeekRepetition = 0;
            LastRing = new DateTime();
            _isActive = false;
            _isRinging = false;
            _ringingCounter = 0;
            IsNewOne = true;
        }
        /// <summary>
        /// Create an alarm clock from a JSON Object. (cf. doc serveur web.pdf) 
        /// </summary>
        /// <param name="json">JSON object to read</param>
        /// <param name="ringingCounter">Ringing counter to set (<see cref="AlarmClockController.Refresh"/>)</param>
        public AlarmClock(JSON json, int ringingCounter=0)
            : this() 
        {
            Id = json.Get("id").GetIntValue(-1);
            _ringingCounter = ringingCounter;
            Refresh(json);
            IsNewOne = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Refresh
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Refresh this alarm clock from a JSON Object. (cf. doc serveur web.pdf)
        /// </summary>
        /// <param name="json">JSON object to read</param>
        public void Refresh(JSON json) {
            /* General */
            Name = StringUtils.UTF8ToASCII(WebUtility.HtmlDecode(json.Get("nom").GetStringValue(Name)));
            string s = json.Get("heure").GetStringValue(HourString);
            /* Heure */
            int i = s.IndexOf(':');
            if (i >= 0) {
                Hour = StringUtils.StringToInt(s.Substring(0, i));
                if ((Hour < 0) || (Hour >= 24)) Hour = 0;
                int i2 = s.IndexOf(':', i+1);
                if (i2 > 0) {
                    Minute = StringUtils.StringToInt(s.Substring(i+1, i2 - i - 1));
                } else {
                    Minute = StringUtils.StringToInt(s.Substring(i+1));
                }
                if ((Minute < 0) || (Minute >= 60)) Minute = 0;
            }
            /* Jour */
            s = json.Get("jour").GetStringValue(WeekDaysToString(Days));
            if (s.Length == 13) {
                Days = WeekDays.None;
                if (s[0] == '1') Days |= WeekDays.Monday;
                if (s[2] == '1') Days |= WeekDays.Tuesday;
                if (s[4] == '1') Days |= WeekDays.Wednesday;
                if (s[6] == '1') Days |= WeekDays.Thursday;
                if (s[8] == '1') Days |= WeekDays.Friday;
                if (s[10] == '1') Days |= WeekDays.Saturday;
                if (s[12] == '1') Days |= WeekDays.Sunday;
            }
            /* LastRing */
            try {
                LastRing = DateTime.Parse(json.Get("lastRing").GetStringValue(LastRingString));
            } catch (SystemException) {
                LastRing = new DateTime(1,1,1);
            }
            /* Other */
            WeekRepetition = json.Get("repetition").GetIntValue(WeekRepetition);
            _isRinging = json.Get("sonne").GetBoolValue(_isRinging);
            if (_isRinging) {
                _ringingCounter++;
            } else {
                _ringingCounter = 0;
            }
            _isActive = json.Get("actif").GetBoolValue(_isActive);
            IsNewOne = false;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Property - get
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Indicate if this alarm clock is active
        /// </summary>
        public bool Active {
            get {
                return _isActive;
            }
        }
        /// <summary>
        /// Indicate if this alarm clock is ringing
        /// </summary>
        public bool Ringing {
            get {
                return (_isRinging);
            }
        }
        /// <summary>
        /// Indicate how many times the alarm clock have rang
        /// </summary>
        /// <remarks>This value is mainly used by AlarmClockController in order to make a only one ring. (<see cref="AlarmClockController.Refresh"/>)</remarks>
        public int RingingCount {
            get {
                return _ringingCounter;
            }
        }

        /// <summary>
        /// Convert Hour and Minute to HOUR:MINUTE (format HH:MM)
        /// </summary>
        public string HourString {
            get {
                return (((Hour < 10) ? "0" : "") + Hour + ((Minute < 10) ? ":0" : ":") + Minute);
            }
        }
        /// <summary>
        /// Convert Hour and Minute into a DateTime object
        /// </summary>
        public DateTime HourDateTime {
            get {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Hour, Minute, 0);
            }
        }
        /// <summary>
        /// Convert LastRing date to "yyyy-MM-dd"
        /// </summary>
        public string LastRingString {
            get {
                return LastRing.ToString("yyyy-MM-dd");
            }
        }
        /// <summary>
        /// Convert Days to string
        /// </summary>
        public string DaysString {
            get {
                return WeekDaysToString(Days, true);
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Compare
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Compare this alarm clock to another one, and check if there are identical.
        /// </summary>
        /// <param name="a">AlarmClock to compare to</param>
        /// <returns>True if identical</returns>
        public bool isEgalTo(AlarmClock a) {
            if (a._isRinging) {
                return ((Id == a.Id) && (string.Compare(Name, a.Name) == 0) && (Hour == a.Hour) && (Minute == a.Minute)
                    && (Days == a.Days) && ((WeekRepetition == a.WeekRepetition) || (WeekRepetition - 1 == a.WeekRepetition)));
            } else {
                return ((Id == a.Id) && (string.Compare(Name, a.Name) == 0) && (Hour == a.Hour) && (Minute == a.Minute)
                    && (Days == a.Days) && (WeekRepetition == a.WeekRepetition));
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // ToString 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert this alarm clock into an human-readable string
        /// </summary>
        /// <returns>String representation of this alarm clock</returns>
        override public string ToString() {
            return "reveil [id=" + Id + ", name=" + Name + "]: heure=" + HourString + " jour=" + DaysString + " sonne=" + Ringing + " actif=" + Active + " derniereSonnerie=" + LastRing;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Static utils functions
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert days enum value into string understandable by home server
        /// </summary>
        /// <param name="days">Days desired</param>
        /// <param name="useDayNameInsteadOfOneOrZero">
        /// If false then will use 1 or 0 to indicate if <see cref="AlarmClock.WeekDays">WeekDays</see> present in days. 
        /// If true then will use french day name (since it's a french project)
        /// </param>
        /// <returns>String understandable by home server</returns>
        static public string WeekDaysToString(WeekDays days, bool useDayNameInsteadOfOneOrZero = false) {
            string m = "";
            if (useDayNameInsteadOfOneOrZero) {
                m += (((days & WeekDays.Monday) == WeekDays.Monday) ? "Lundi," : "");
                m += (((days & WeekDays.Tuesday) == WeekDays.Tuesday) ? "Mardi," : "");
                m += (((days & WeekDays.Wednesday) == WeekDays.Wednesday) ? "Mercredi," : "");
                m += (((days & WeekDays.Thursday) == WeekDays.Thursday) ? "Jeudi," : "");
                m += (((days & WeekDays.Friday) == WeekDays.Friday) ? "Vendredi," : "");
                m += (((days & WeekDays.Saturday) == WeekDays.Saturday) ? "Samedi," : "");
                m += (((days & WeekDays.Sunday) == WeekDays.Sunday) ? "Dimanche," : "");
                if (m.Length > 0) {
                    m = m.Remove(m.Length - 1);
                }
            } else {
                m += (((days & WeekDays.Monday) == WeekDays.Monday) ? "1" : "0") + ",";
                m += (((days & WeekDays.Tuesday) == WeekDays.Tuesday) ? "1" : "0") + ",";
                m += (((days & WeekDays.Wednesday) == WeekDays.Wednesday) ? "1" : "0") + ",";
                m += (((days & WeekDays.Thursday) == WeekDays.Thursday) ? "1" : "0") + ",";
                m += (((days & WeekDays.Friday) == WeekDays.Friday) ? "1" : "0") + ",";
                m += (((days & WeekDays.Saturday) == WeekDays.Saturday) ? "1" : "0") + ",";
                m += (((days & WeekDays.Sunday) == WeekDays.Sunday) ? "1" : "0");
            }
            return m;
        }


    }
}
