using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Home lights controller
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class LightController : Controller
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Contructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="home">Home to control</param>
        public LightController(Home home)
            : base(home)
        {
            ;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Lumiere
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Switch on light of a room
        /// </summary>
        /// <param name="name">Room name desired</param>
        /// <param name="ignoreCase">True to ignore case during seeking room</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking room</param>
        /// <returns>Home server response</returns>
        public HomeResponse Allumer(string name, bool ignoreCase = true, bool ignoreAccent = true) {
            return Allumer(_home.Pieces.Get(name, ignoreCase, ignoreAccent));
        }
        /// <summary>
        /// Switch off light of a room
        /// </summary>
        /// <param name="name">Room name desired</param>
        /// <param name="ignoreCase">True to ignore case during seeking room</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking room</param>
        /// <returns>Home server response</returns>
        public HomeResponse Eteindre(string name, bool ignoreCase = true, bool ignoreAccent = true) {
            return Eteindre(_home.Pieces.Get(name, ignoreCase, ignoreAccent));
        }
        /// <summary>
        /// Switch on light of a room
        /// </summary>
        /// <param name="id">Room identifier desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Allumer(int id) {
            return Allumer(_home.Pieces.Get(id));
        }
        /// <summary>
        /// Switch off light of a room
        /// </summary>
        /// <param name="id">Room identifier desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Eteindre(int id) {
            return Eteindre(_home.Pieces.Get(id));
        }
        /// <summary>
        /// Switch on light of a room
        /// </summary>
        /// <param name="room">Room desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Allumer(Room room) {
            if (room != null) {
                return ExecuteAction("lumiere", "allumer", room);
            } else {
                return null;
            }
        }
        /// <summary>
        /// Switch off light of a room
        /// </summary>
        /// <param name="room">Room desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse Eteindre(Room room) {
            if (room != null) {
                return ExecuteAction("lumiere", "eteindre", room);
            } else {
                return null;
            }
        }


        /// <summary>
        /// Switch on all light
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse AllumerTout() {
            return ExecuteAction("lumiere", "allumerTout");
        }
        /// <summary>
        /// Switch off all light
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse EteindreTout() {
            return ExecuteAction("lumiere", "eteindreTout");
        }


    }
}
