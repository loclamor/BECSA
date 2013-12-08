using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Home flaps controller
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class FlapController : Controller
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Contructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="home">Home to control</param>
        public FlapController(Home home)
            : base(home)
        {
            ;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Volet
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Open flaps of a room
        /// </summary>
        /// <param name="name">Room name desired</param>
        /// <param name="ignoreCase">True to ignore case during seeking room</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking room</param>
        /// <returns>Home server response</returns>
        public HomeResponse Ouvrir(string name, bool ignoreCase = true, bool ignoreAccent = true) {
            return Ouvrir(_home.Pieces.Get(name, ignoreCase, ignoreAccent));
        }
        /// <summary>
        /// Close flaps of a room
        /// </summary>
        /// <param name="name">Room name desired</param>
        /// <param name="ignoreCase">True to ignore case during seeking room</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking room</param>
        /// <returns>Home server response</returns>
        public HomeResponse Fermer(string name, bool ignoreCase = true, bool ignoreAccent = true) {
            return Fermer(_home.Pieces.Get(name, ignoreCase, ignoreAccent));
        }
        /// <summary>
        /// Open flaps of a room
        /// </summary>
        /// <param name="id">Room identifier desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Ouvrir(int id) {
            return Ouvrir(_home.Pieces.Get(id));
        }
        /// <summary>
        /// Close flaps of a room
        /// </summary>
        /// <param name="id">Room identifier desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Fermer(int id) {
            return Fermer(_home.Pieces.Get(id));
        }
        /// <summary>
        /// Open flaps of a room
        /// </summary>
        /// <param name="room">Room desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Ouvrir(Room room) {
            if (room != null) {
                return ExecuteAction("volet", "ouvrir", room);
            } else {
                return null;
            }
        }
        /// <summary>
        /// Close flaps of a room
        /// </summary>
        /// <param name="room">Room desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Fermer(Room room) {
            if (room != null) {
                return ExecuteAction("volet", "fermer", room);
            } else {
                return null;
            }
        }
        /// <summary>
        /// Open all flaps of home
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse OuvrirTout() {
            return ExecuteAction("volet", "ouvrirTout");
        }
        /// <summary>
        /// Close all flaps of home
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse FermerTout() {
            return ExecuteAction("volet", "fermerTout");
        }


    }
}
