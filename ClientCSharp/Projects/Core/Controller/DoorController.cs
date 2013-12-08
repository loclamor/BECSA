using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Home doors controller
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class DoorController : Controller
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Contructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="home">Home to control</param>
        public DoorController(Home home)
            : base(home)
        {
            ;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Porte
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Lock the door of a room
        /// </summary>
        /// <param name="name">Room name desired</param>
        /// <param name="ignoreCase">True to ignore case during seeking room</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking room</param>
        /// <returns>Home server response</returns>
        public HomeResponse Verrouiller(string name, bool ignoreCase = true, bool ignoreAccent = true) {
            return Verrouiller(_home.Pieces.Get(name, ignoreCase, ignoreAccent));
        }
        /// <summary>
        /// Unlock the door of a room
        /// </summary>
        /// <param name="name">Room name desired</param>
        /// <param name="ignoreCase">True to ignore case during seeking room</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking room</param>
        /// <returns>Home server response</returns>
        public HomeResponse Deverrouiller(string name, bool ignoreCase = true, bool ignoreAccent = true) {
            return Deverrouiller(_home.Pieces.Get(name, ignoreCase, ignoreAccent));
        }
        /// <summary>
        /// Lock the door of a room
        /// </summary>
        /// <param name="id">Room identifier desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Verrouiller(int id) {
            return Verrouiller(_home.Pieces.Get(id));
        }
        /// <summary>
        /// Unlock the door of a room
        /// </summary>
        /// <param name="id">Room identifier desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Deverrouiller(int id) {
            return Deverrouiller(_home.Pieces.Get(id));
        }
        /// <summary>
        /// Lock the door of a room
        /// </summary>
        /// <param name="room">Room desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Verrouiller(Room room) {
            if (room != null) {
                return ExecuteAction("porte", "verrouiller", room);
            } else {
                return null;
            }
        }
        /// <summary>
        /// Unlock the door of a room
        /// </summary>
        /// <param name="room">Room desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Deverrouiller(Room room) {
            if (room != null) {
                return ExecuteAction("porte", "deverrouiller", room);
            } else {
                return null;
            }
        }

        /// <summary>
        /// Lock all the door of home
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse VerrouillerTout() {
            return ExecuteAction("porte", "verrouillerTout");
        }
        /// <summary>
        /// UnLock all the door of home
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse DeverrouillerTout() {
            return ExecuteAction("porte", "deverrouillerTout");
        }


    }
}
