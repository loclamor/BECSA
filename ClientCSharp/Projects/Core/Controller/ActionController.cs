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
		private List<HomeAction> _previousActions;


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
        }


		///////////////////////////////////////////////////////////////////////////////////////////
		// Configure
		///////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Enable/Disable avoiding duplicate action
		/// </summary>
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
			_previousActions = null;
            /* Execute */
            HTTPRequest r = new HTTPRequest(_home.HomeURI, msgGET, msgPOST);
            return HomeResponse.Create(new JSON(r.GetResponse()));
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
				if ((System.Environment.TickCount - _avoidDuplicateActionTick) < _avoidDuplicateActionFrequency) {
					_previousActions.Clear();
					_avoidDuplicateActionTick = System.Environment.TickCount;
				}
			}
            /* Retrieve all Pieces from JSON response */
            for (int i = 0, max = json.Count; i < max; i++) {
                JSON jsonAct = ((json.Type == JSON.ValueType.ARRAY) ? json.Get(i) : json);
                if (!(_actionsReceived.Contains(jsonAct.Get("id").GetIntValue()))) {
                    HomeAction a = new HomeAction(jsonAct);
					if (!(isDuplicatedAction(a))) {
						_home.OnActionReceived(_home, a);
					}
					if (_avoidDuplicateActions) {
						_previousActions.Add(a);
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
			if (_avoidDuplicateActions) {
				foreach (HomeAction a in _previousActions) {
					if (act.isIdenticalTo(a)) return true;
				}
			}
			return false;
		}


    }
}
