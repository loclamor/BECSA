using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Home alarm clocks controller
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class AlarmClockController : Controller
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Alarm clocks of home
        /// </summary>
        private List<AlarmClock> _alarmClocks;


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Contructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="home">Home to control</param>
        public AlarmClockController(Home home)
            : base(home)
        {
            _alarmClocks = new List<AlarmClock>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // AlarmClock - set
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create an new alarm clock
        /// </summary>
        /// <remarks>To take in account the alarm clock creation you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="name">Name of the new alarm clock</param>
        /// <param name="hour">Hour when alarm clock need to ring</param>
        /// <param name="minute">Minute when alarm clock need to ring</param>
        /// <param name="days">Days alarm clock have to rings</param>
        /// <param name="weekCount">Number of weeks before the alarm clock goto off mode. If 0 Then alarm clock not activate Else If -1 Then infinite</param>
        /// <returns>Server response</returns>
        public HomeResponse CreateNewAlarmClock(string name, int hour, int minute, AlarmClock.WeekDays days, int weekCount = 1) {
            /* Init */
            HomeRequest msgGET = new HomeRequest();
            msgGET.Set("controller", "reveil")
                .Set("action", "creer");
            HomeRequest msgPOST = new HomeRequest();
            msgPOST.Set("reveil", name)
                .Set("heure", ((hour < 10) ? "0" : "") + hour.ToString() + ((minute < 10) ? ":0" : ":") + minute.ToString())
                .Set("jours", AlarmClock.WeekDaysToString(days))
                .Set("repetition", weekCount.ToString());
            /* Execute */
            HTTPRequest r = new HTTPRequest(_home.HomeURI, msgGET, msgPOST);
            return HomeResponse.Create(new JSON(r.GetResponse()));
        }
        /// <summary>
        /// Remove an alarm clock
        /// </summary>
        /// <remarks>To take in account the alarm clock deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="clock">Alarm clock to remove</param>
        /// <returns>Server response</returns>
        public HomeResponse Remove(AlarmClock clock) {
            if (clock != null) {
                return Remove(clock.Id);
            } else {
                return null;
            }
        }
        /// <summary>
        /// Remove an alarm clock by index
        /// </summary>
        /// <remarks>To take in account the alarm clock deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="index">Index of alarm clock to remove</param>
        /// <returns>Server response</returns>
        public HomeResponse RemoveByIndex(int index) {
            return Remove(GetByIndex(index));
        }
        /// <summary>
        /// Remove an alarm clock
        /// </summary>
        /// <remarks>To take in account the alarm clock deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="name">Name of alarm clock to remove</param>
        /// <param name="ignoreCase">True to ignore case during seeking alarm clock</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking alarm clock</param>
        /// <returns>Server response</returns>
        public HomeResponse Remove(string name, bool ignoreCase = true, bool ignoreAccent = true) {
            return Remove(Get(name, ignoreCase, ignoreAccent));
        }
        /// <summary>
        /// Remove an alarm clock by identifier number
        /// </summary>
        /// <remarks>To take in account the alarm clock deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="identifier">Identifier number of alarm clock to remove</param>
        /// <returns>Server response</returns>
        public HomeResponse Remove(int identifier) {
            /* Init */
            HomeRequest msg = new HomeRequest();
            msg.Set("controller", "reveil")
                .Set("action", "supprimer")
                .Set("id", identifier.ToString());
            /* Execute */
            HTTPRequest r = new HTTPRequest(_home.HomeURI, msg);
            return HomeResponse.Create(new JSON(r.GetResponse()));
        }

        /// <summary>
        /// Activate/Desactivate an alarm clock by identifier number
        /// </summary>
        /// <param name="identifier">Identifier number of alarm clock to active/desactivate</param>
        /// <param name="activeState">True to activate the alarm clock false to desactivate</param>
        /// <returns>Server response</returns>
        public HomeResponse ActiveAlarmClock(int identifier, bool activeState) {
            /* Init */
            HomeRequest msg = new HomeRequest();
            msg.Set("controller", "reveil")
                .Set("action", (activeState ? "activer" : "desactiver"))
                .Set("id", identifier.ToString());
            /* Execute */
            HTTPRequest r = new HTTPRequest(_home.HomeURI, msg);
            return HomeResponse.Create(new JSON(r.GetResponse()));
        }
        /// <summary>
        /// Activate/Desactivate an alarm clock by name
        /// </summary>
        /// <param name="name">Name of the alarm clock to active/desactivate</param>
        /// <param name="ignoreCase">True to ignore case during seeking alarm clock</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking alarm clock</param>
        /// <param name="activeState">True to activate the alarm clock false to desactivate</param>
        /// <returns>Server response</returns>
        public HomeResponse ActiveAlarmClock(string name, bool activeState, bool ignoreCase = true, bool ignoreAccent = true) {
            return ActiveAlarmClock(Get(name, ignoreCase, ignoreAccent), activeState);
        }
        /// <summary>
        /// Activate/Desactivate an alarm clock by identifier number
        /// </summary>
        /// <param name="clock">Alarm clock to active/desactivate</param>
        /// <param name="activeState">True to activate the alarm clock false to desactivate</param>
        /// <returns>Server response</returns>
        public HomeResponse ActiveAlarmClock(AlarmClock clock, bool activeState) {
            if (clock != null) {
                return ActiveAlarmClock(clock.Id, activeState);
            } else {
                return null;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // AlarmClock - get
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get the number of the alarm clock
        /// </summary>
        /// <returns>Alarm clock count</returns>
        public int Count {
            get {
                return _alarmClocks.Count;
            }
        }
        /// <summary>
        /// Get an alarm clock by name
        /// </summary>
        /// <param name="name">Name of the desired alarm clock</param>
        /// <param name="ignoreCase">True to ignore case during seeking alarm clock</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking alarm clock</param>
        /// <returns>Alarm clock found Or null if no alarm clock found</returns>
        public AlarmClock Get(string name, bool ignoreCase = true, bool ignoreAccent = true) {
            foreach (AlarmClock a in _alarmClocks) {
                if (StringUtils.IdenticalString(a.Name, name, ignoreCase, ignoreAccent)) {
                    return a;
                }
            }
            return null;
        }
        /// <summary>
        /// Get an alarm clock by index
        /// </summary>
        /// <param name="index">Index of the desired alarm clock</param>
        /// <returns>Alarm clock found Or null if no alarm clock found</returns>
        public AlarmClock GetByIndex(int index) {
            if ((index >= 0) && (index < _alarmClocks.Count)) {
                return _alarmClocks.ElementAt(index);
            } else {
                return null;
            }
        }
        /// <summary>
        /// Get an alarm clock by identifier number
        /// </summary>
        /// <param name="identifierNumber">Identifier number of the desired alarm clock</param>
        /// <returns>Alarm clock found Or null if no alarm clock found</returns>
        public AlarmClock Get(int identifierNumber) {
            foreach (AlarmClock a in _alarmClocks) {
                if (a.Id == identifierNumber) {
                    return a;
                }
            }
            return null;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // AlarmClock - Refresh
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Refresh list of alarm clocks and their status
        /// </summary>
        /// <remarks>Will automatically fired alarm clock update event if needed and configured (<see cref="Home.RegisterAlarmClockUpdateEvent"/>)</remarks>
        /// <param name="json">JSON to load, containing all alarm clocks datas</param>
        /// <returns>True if refreshed without errors</returns> 
        public bool Refresh(JSON json) {
            /* Check if needed */
            if (json == null) return true;
            /* Used to stock all change in order to fire event after update completed */
            List<AlarmClock> alarmClockAdded = new List<AlarmClock>();
            List<AlarmClock> alarmClockRemoved = new List<AlarmClock>();
            List<AlarmClock> alarmClockUpdated = new List<AlarmClock>();
            List<AlarmClock> alarmClockRinging = new List<AlarmClock>();
            List<AlarmClock> newAlarmClocks = new List<AlarmClock>();
            /* Retrieve all AlarmClocks from JSON response */
            for (int i = 0, max = json.Count; i < max; i++) {
                /* Retrieve corresponding old AlarmClock and create new one */
                JSON jsonAlarmClock = ((json.Type == JSON.ValueType.ARRAY) ? json.Get(i) : json);
                AlarmClock oldAlarm = Get(jsonAlarmClock.Get("id").GetIntValue(-1));
                AlarmClock newAlarm = new AlarmClock(jsonAlarmClock, ((oldAlarm != null) ? oldAlarm.RingingCount : 0));
                newAlarmClocks.Add(newAlarm);
                /* Check if there is an update from older alarm clock */
                if (oldAlarm != null) {
                    if (!(oldAlarm.isEgalTo(newAlarm))) {
                        /* Alarm clock updated */
                        alarmClockUpdated.Add(oldAlarm);
                    }
                } else {
                    /* Alarm Clock added */
                    alarmClockAdded.Add(newAlarm);
                }
                /* Manage when it's not an array but a simply object */
                if (json.Type == JSON.ValueType.OBJECT) break;
            }
            /* Check alarm clock removed */
            foreach (AlarmClock oldA in _alarmClocks) {
                bool found = false;
                foreach (AlarmClock newA in newAlarmClocks) {
                    if (newA.Id == oldA.Id) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    alarmClockRemoved.Add(oldA);
                }
            }
            /* Copy new alarm clocks */
            _alarmClocks = newAlarmClocks;
            /* Make home event */
            if (_home.OnAlarmClockUpdate != null) {
                /* Remove */
                foreach (AlarmClock a in alarmClockRemoved) {
                    _home.OnAlarmClockUpdate(_home, a, Home.AlarmClockUpdateKind.AlarmClockRemoved);
                }
                /* Add */
                foreach (AlarmClock a in alarmClockAdded) {
                    _home.OnAlarmClockUpdate(_home, a, Home.AlarmClockUpdateKind.NewAlarmClockSet);
                }
                /* Updated */
                foreach (AlarmClock a in alarmClockUpdated) {
                    _home.OnAlarmClockUpdate(_home, a, Home.AlarmClockUpdateKind.AlarmClockUpdated);
                }
            }
            /* Alarm clock List changed */
            if ((_home.OnHomeUpdate != null) && ((alarmClockRemoved.Count > 0) || (alarmClockAdded.Count > 0))) {
                /* Room list changed */
                _home.OnHomeUpdate(_home, Home.HomeUpdateKind.AlarmClockListChanged);
            }
            /* Make event for AlarmClock that ring */
            if (_home.OnAlarmClockUpdate != null) {
                foreach (AlarmClock a in _alarmClocks) {
                    if ((a.Ringing) && (a.RingingCount == 1)) {
                        _home.OnAlarmClockUpdate(_home, a, Home.AlarmClockUpdateKind.AlarmClockRing);
                    }
                }
            }
            /* Done: return true */
            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // ToString 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert alarm clocks into human-readable string
        /// </summary>
        /// <returns>String representation of all alarm clocks</returns>
        override public string ToString() {
            string m = "";
            int id = 0;
            foreach (AlarmClock a in _alarmClocks) {
                if (id > 0) {
                    m += "\n";
                }
                m += a.ToString();
                id++;
            }
            return m;
        }

    }
}
