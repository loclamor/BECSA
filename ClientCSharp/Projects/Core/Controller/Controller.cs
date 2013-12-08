using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Home basic controller
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class Controller
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Home controlled
        /// </summary>
        protected Home _home;


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructor
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize a basic controller
        /// </summary>
        /// <param name="home">Home to control</param>
        public Controller(Home home) {
            _home = home;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Action functions
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute an action to a controller of the home
        /// </summary>
        /// <remarks>For an expansive details of controller/action availaible cf. doc serveur web.pdf</remarks>
        /// <param name="controller">Controller desired</param>
        /// <param name="action">Action desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse ExecuteAction(string controller, string action) {
            HomeRequest msg = new HomeRequest();
            msg.Set("controller", controller)
                .Set("action", action);
            HTTPRequest r = new HTTPRequest(_home.HomeURI, msg);
            return HomeResponse.Create(new JSON(r.GetResponse()));
        }
        /// <summary>
        /// Execute an action to a controller of a home room's
        /// </summary>
        /// <remarks>For an expansive details of controller/action availaible cf. doc serveur web.pdf</remarks>
        /// <param name="controller">Controller desired</param>
        /// <param name="action">Action desired</param>
        /// <param name="room">Room desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse ExecuteAction(string controller, string action, Room room) {
            HomeRequest msg = new HomeRequest();
            msg.Set("controller", controller)
                .Set("action", action)
                .Set("id", room.Id.ToString());
            HTTPRequest r = new HTTPRequest(_home.HomeURI, msg);
            return HomeResponse.Create(new JSON(r.GetResponse()));
        }


    }
}
