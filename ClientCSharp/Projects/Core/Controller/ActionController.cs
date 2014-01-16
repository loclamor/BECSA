using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Home actions controller
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class ActionController : Controller
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Private: List of all ids of action already received and treated
        /// </summary>
        private HashSet<int> _actionsReceived;
		/// <summary>
		/// Private: Indicate if duplicate action are avoided
		/// </summary>
		private bool _avoidDuplicateActions;
		/// <summary>
		/// Private: Frequency for refreshing the list used to avoid duplicate action
		/// </summary>
		private int _avoidDuplicateActionFrequency;
		/// <summary>
		/// Private: Last tick when the list used to avoid duplicate action was cleared
		/// </summary>
		private int _avoidDuplicateActionTick;
		/// <summary>
		/// Private: List of action used to know if an action are duplicate or not
		/// </summary>
		private List<HomeAction> _previousActions;
		/// <summary>
		/// Private: List of type of action allowed to be duplicated
		/// </summary>
		private HashSet<string> _actionTypeDuplicateException;


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Contructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="home">Home to control</param>
        public ActionController(Home home)
            : base(home)
        {
            _actionsReceived = new HashSet<int>();
			_avoidDuplicateActions = false;
			_avoidDuplicateActionFrequency = 0;
			_avoidDuplicateActionTick = 0;
			_previousActions = new List<HomeAction>();
			_actionTypeDuplicateException = new HashSet<string>();
        }


		///////////////////////////////////////////////////////////////////////////////////////////
		// Configure
		///////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Enable/Disable avoiding duplicate action
		/// </summary>
		/// <remarks><see cref="SetDuplicateActionException"/> allow to let some action to be duplicated</remarks>
		/// <param name="activate">If true Then avoid duplicate action</param>
		/// <param name="frequency">Frequency used to refresh the list used to avoid duplicate action</param>
		public void AvoidDuplicateAction(bool activate, int frequency = 1000) {
			if (activate != _avoidDuplicateActions) {
				_avoidDuplicateActionFrequency = frequency;
				_avoidDuplicateActionTick = 0;
				_previousActions = new List<HomeAction>();
				_avoidDuplicateActions = activate;
			}
		}

		/// <summary>
		/// Indicate if duplicate action are avoided
		/// </summary>
		public bool AvoidingDuplicateAction {
			get {
				return _avoidDuplicateActions;
			}
		}
		/// <summary>
		/// Frequency for refreshing the list used to avoid duplicate action
		/// </summary>
		public int AvoidingDuplicateActionFrequency {
			get {
				return _avoidDuplicateActionFrequency;
			}
		}

		/// <summary>
		/// Clear list of duplicate action type exception
		/// </summary>
		public void ClearDuplicateActionException() {
			_actionTypeDuplicateException.Clear();
		}
		/// <summary>
		/// Clear and set new duplicate action type exception
		/// </summary>
		/// <param name="actionTypes">Type of actions allowed to be duplicate</param>
		public void SetDuplicateActionException(params string[] actionTypes) {
			_actionTypeDuplicateException.Clear();
			foreach (string s in actionTypes) {
				if (!(_actionTypeDuplicateException.Contains(s))) {
					_actionTypeDuplicateException.Add(s);
				}
			}
		}
		/// <summary>
		/// Number of duplicate action exception
		/// </summary>
		public int DuplicateActionExceptionCount {
			get {
				return _actionTypeDuplicateException.Count;
			}
		}
		/// <summary>
		/// Get a duplicate action exception at a gived index
		/// </summary>
		/// <param name="index">Index desired</param>
		/// <returns>Action types allowed to be duplicate Or "" if index not found</returns>
		public string GetDuplicateActionException(int index) {
			if ((index >= 0) && (index < _actionTypeDuplicateException.Count)) {
				foreach (string s in _actionTypeDuplicateException) {
					if (index == 0) {
						return s;
					}
					index--;
				}
			}
			return "";
		}
		/// <summary>
		/// Test if have an action type allowed to be duplicate
		/// </summary>
		/// <param name="actionType">Action type desired</param>
		/// <returns>True if allowed, false otherwise</returns>
		public bool HaveDuplicateActionException(string actionType) {
			return (_actionTypeDuplicateException.Contains(actionType));
		}


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Send
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Send an action
        /// </summary>
        /// <param name="dest">Identifier that will receive the action</param>
        /// <param name="action">Action to send</param>
        /// <param name="param">List of action parameters to send</param>
        /// <returns>Home server response</returns>
        public HomeResponse SendAction(string dest, string action, params string[] param) {
            /* Init */
            HomeRequest msgGET = new HomeRequest();
            msgGET.Set("controller", "action")
                .Set("action", "post");
            HomeRequest msgPOST = new HomeRequest();
            msgPOST.Set("action", action)
                .Set("dest", dest);
            for(int i = 0; i < param.Length; i++) {
                msgPOST.Set("param" + i.ToString(), param[i]);
            }
			/* Clear duplicate action */
			_previousActions.Clear();
            /* Execute */
            HTTPRequest r = new HTTPRequest(_home.HomeURI, msgGET, msgPOST);
			if (r.Execute()) {
				return HomeResponse.Create(new JSON(r.GetResponse()));
			} else {
				return new HomeResponse();
			}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Refresh
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Retrieve all actions and triggered action received event.
        /// </summary>
        /// <remarks>
        /// Will automatically fired action received event if configured (<see cref="SmartHome.Home.RegisterActionReceivedEvent"/>). 
        /// Action are not persisted, this means that if action received event is not configured then the actions cannot be retrieved after.
        /// </remarks>
        /// <param name="json">JSON to load, containing all actions</param>
        /// <returns>True if refreshed without errors</returns>
        public bool Refresh(JSON json) {
            /* Check if needed */
            if ((json == null) || (_home.OnActionReceived == null)) return true;
			/* Check if need to refresh avoid duplication list */
			if (_avoidDuplicateActions) {
				if ((System.Environment.TickCount - _avoidDuplicateActionTick) >= _avoidDuplicateActionFrequency) {
					_previousActions.Clear();
					_avoidDuplicateActionTick = System.Environment.TickCount;
				}
			}
            /* Retrieve all Pieces from JSON response */
            for (int i = 0, max = json.Count; i < max; i++) {
                JSON jsonAct = ((json.Type == JSON.ValueType.ARRAY) ? json.Get(i) : json);
                if (!(_actionsReceived.Contains(jsonAct.Get("id").GetIntValue()))) {
                    HomeAction a = new HomeAction(jsonAct);
					bool doRecvAct = true;
					if ((_avoidDuplicateActions) && (!(_actionTypeDuplicateException.Contains(a.Type)))) { 
						doRecvAct = (!(isDuplicatedAction(a)));
						_previousActions.Add(a);
					}
					if (doRecvAct) {
						_home.OnActionReceived(_home, a);
					}
					_actionsReceived.Add(a.Id);
                }
                /* Manage when it's not an array but a simply object */
                if (json.Type == JSON.ValueType.OBJECT) break;
            }
            /* Done: return true */
            return true;
        }

		///////////////////////////////////////////////////////////////////////////////////////////
		// Protected methods
		///////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Check if the action is an duplicate action
		/// </summary>
		/// <param name="act">Action to test</param>
		/// <returns>True if the action is an duplicate action</returns>
		private bool isDuplicatedAction(HomeAction act) {
			foreach (HomeAction a in _previousActions) {
				if (act.isIdenticalTo(a)) return true;
			}
			return false;
		}


    }
}
