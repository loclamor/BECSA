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
            /* Retrieve all Pieces from JSON response */
            for (int i = 0, max = json.Count; i < max; i++) {
                JSON jsonAct = ((json.Type == JSON.ValueType.ARRAY) ? json.Get(i) : json);
                if (!(_actionsReceived.Contains(jsonAct.Get("id").GetIntValue()))) {
                    HomeAction a = new HomeAction(jsonAct);
                    _home.OnActionReceived(_home, a);
                    _actionsReceived.Add(a.Id);
                }
                /* Manage when it's not an array but a simply object */
                if (json.Type == JSON.ValueType.OBJECT) break;
            }
            /* Done: return true */
            return true;
        }

    }
}
