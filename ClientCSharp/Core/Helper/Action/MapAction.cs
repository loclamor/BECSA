using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Action for map
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class MapAction : AbstractAction
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
            /// Retrieve road traffic
            /// </summary>
            ROAD_TRAFFIC,
            /// <summary>
            /// Retrieve directions for specific location
            /// </summary>
            ROAD_DIRECTIONS
        };

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Action to do with map
        /// </summary>
        public ActionType Action { get; protected set; }
        /// <summary>
        /// Desired location id. If -1 Then use <see cref="Location">Location</see> text instead.
        /// </summary>
        public int LocationId { get; protected set; }
        /// <summary>
        /// Desired location used only when <see cref="LocationId">LocationId</see> == -1
        /// </summary>
        public string Location { get; protected set; }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize an map action
        /// </summary>
        /// <param name="act">Action to do with map</param>
        /// <param name="locationId">Location identifier number desired</param>
        public MapAction(ActionType act, int locationId) {
            Action = act;
            LocationId = locationId;
            Location = "";
        }

        /// <summary>
        /// Initialize an map action
        /// </summary>
        /// <param name="act">Action to do with map</param>
        /// <param name="location">Location desired</param>
        public MapAction(ActionType act, string location = "") {
            Action = act;
            LocationId = -1;
            Location = location;
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
            if (Action == ActionType.ROAD_TRAFFIC) {
                if (LocationId != -1) {
                    return onHome.Actions.SendAction("webapp", "trafic", LocationId.ToString());
                } else {
                    return onHome.Actions.SendAction("webapp", "trafic", Location);
                }
            } else if (Action == ActionType.ROAD_DIRECTIONS) {
                if (LocationId != -1) {
                    return onHome.Actions.SendAction("webapp", "itineraire", LocationId.ToString());
                } else {
                    return onHome.Actions.SendAction("webapp", "itineraire", Location);
                }
            } else {
                return null;
            }
        }

    }
}
