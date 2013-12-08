using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{

    /// <summary>
    /// Extra actions availible for SmartHome. Contains all the actions not implemented in other Action helper class.
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class ExtraAction : AbstractAction
    {
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Enums
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Action recognized
        /// </summary>
        public enum ActionType
        {
            /// <summary>
            /// Make speaker goto silence mode
            /// </summary>
            SILENCE
        };

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Action to do
        /// </summary>
        public ActionType Action { get; protected set; }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize an extra action
        /// </summary>
        /// <param name="act">Action to do</param>
        public ExtraAction(ActionType act) {
            Action = act;
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
            if (Action == ActionType.SILENCE) {
                return onHome.Actions.SendAction("synthese", "silence");
            } else {
                return null;
            }
        }

    }

}
