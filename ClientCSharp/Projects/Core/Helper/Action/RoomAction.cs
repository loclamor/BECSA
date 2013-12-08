using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Action for room
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class RoomAction : AbstractAction
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Enums
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Controller recognized
        /// </summary>
        public enum ControllerType
        {
            /// <summary>
            /// Undefined controller
            /// </summary>
            UNDEFINED,
            /// <summary>
            /// Light controller
            /// </summary>
            LUMIERE = 1,
            /// <summary>
            /// Door controller
            /// </summary>
            PORTE = 2,
            /// <summary>
            /// Flap controller
            /// </summary>
            VOLET = 4
        }
        /// <summary>
        /// Action recognized
        /// </summary>
        public enum ActionType
        {
            /// <summary>
            /// Undefined action
            /// </summary>
            UNDEFINED,
            /// <summary>
            /// On action, correspond to: Allumer, Ouvrir volet, Déverouiller porte, Lancer son, Activer reveil
            /// </summary>
            ON,
            /// <summary>
            /// Off action, correspond to: Eteindre, Fermer volet, Verouiller porte, Stoppper son, Désactiver reveil
            /// </summary>
            OFF
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Element of room to control
        /// </summary>
        public ControllerType Controller { get; protected set; }
        /// <summary>
        /// Action to do on the controller
        /// </summary>
        public ActionType Action { get; protected set; }
        /// <summary>
        /// Can be <see cref="SmartHome.Room.Id"/> or -1.
        /// </summary>
        /// <remarks>-1 = All</remarks>
        public int RoomId { get; protected set; }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize a room action
        /// </summary>
        /// <param name="ctrl">Room element desired</param>
        /// <param name="act">Action to do with the controller</param>
        /// <param name="roomId">Room identifier number to control</param>
        public RoomAction(ControllerType ctrl, ActionType act, int roomId = -1) {
            Controller = ctrl;
            Action = act;
            RoomId = roomId;
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
            /* Classic controller */ 
            if ((Controller & ControllerType.LUMIERE) == ControllerType.LUMIERE) {
                /* Lumieres */
                if (RoomId == -1) {
                    /* Toute les lumieres */ 
                    switch (Action) {
                        case ActionType.ON: onHome.Lumieres.AllumerTout(); break;
                        case ActionType.OFF: onHome.Lumieres.EteindreTout(); break;
                    }
                } else {
                    /* Room */
                    switch (Action) {
                        case ActionType.ON: onHome.Lumieres.Allumer(RoomId); break;
                        case ActionType.OFF: onHome.Lumieres.Eteindre(RoomId); break;
                    }
                }
            } 
            if ((Controller & ControllerType.PORTE) == ControllerType.PORTE) {
                /* Portes */
                if (RoomId == -1) {
                    /* Toute les portes */
                    switch (Action) {
                        case ActionType.ON: onHome.Portes.DeverrouillerTout(); break;
                        case ActionType.OFF: onHome.Portes.VerrouillerTout(); break;
                    }
                } else {
                    /* Room */
                    switch (Action) {
                        case ActionType.ON: onHome.Portes.Deverrouiller(RoomId); break;
                        case ActionType.OFF: onHome.Portes.Verrouiller(RoomId); break;
                    }
                }
            } 
            if ((Controller & ControllerType.VOLET) == ControllerType.VOLET) {
                /* Volets */
                if (RoomId == -1) {
                    /* Tous les volets */
                    switch (Action) {
                        case ActionType.ON: onHome.Volets.OuvrirTout(); break;
                        case ActionType.OFF: onHome.Volets.FermerTout(); break;
                    }
                } else {
                    /* Room */
                    switch (Action) {
                        case ActionType.ON: onHome.Volets.Ouvrir(RoomId); break;
                        case ActionType.OFF: onHome.Volets.Fermer(RoomId); break;
                    }
                }
            }
            return null;
        }

    }
}
