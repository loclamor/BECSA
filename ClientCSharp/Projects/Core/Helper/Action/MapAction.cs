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
		/// <summary>
		/// Enable to avoid interpreting multi-response if many webapp runned (cf. doc serveur web). Default: -1 use an computed one <see cref="Home.ComputeLastRequestUniqueId"/>.
		/// </summary>
		public int UniqueRequestId { get; protected set; }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize an map action
        /// </summary>
        /// <param name="act">Action to do with map</param>
		/// <param name="locationId">Location identifier number desired</param>
		/// <param name="uniqueRequestId">
		/// Unique request identifier desired <see cref="UniqueRequestId">UniqueRequestId</see>. 
		/// Default: -1 use an computed one <see cref="Home.ComputeLastRequestUniqueId"/>.
		/// </param>
		public MapAction(ActionType act, int locationId, int uniqueRequestId = -1) {
            Action = act;
            LocationId = locationId;
            Location = "";
			UniqueRequestId = uniqueRequestId;
        }

        /// <summary>
        /// Initialize an map action
        /// </summary>
        /// <param name="act">Action to do with map</param>
		/// <param name="location">Location desired</param>
		/// <param name="uniqueRequestId">
		/// Unique request identifier desired <see cref="UniqueRequestId">UniqueRequestId</see>. 
		/// Default: -1 use an computed one <see cref="Home.ComputeLastRequestUniqueId"/>.
		/// </param>
		public MapAction(ActionType act, string location = "", int uniqueRequestId = -1) {
            Action = act;
            LocationId = -1;
			Location = location;
			UniqueRequestId = uniqueRequestId;
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
				int uRequestId = ((UniqueRequestId == -1) ? onHome.ComputeLastRequestUniqueId() : UniqueRequestId);
                if (LocationId != -1) {
					return onHome.Actions.SendAction("webapp", "trafic", LocationId.ToString(), uRequestId.ToString());
                } else {
					return onHome.Actions.SendAction("webapp", "trafic", Location, uRequestId.ToString());
                }
			} else if (Action == ActionType.ROAD_DIRECTIONS) {
				int uRequestId = ((UniqueRequestId == -1) ? onHome.ComputeLastRequestUniqueId() : UniqueRequestId);
                if (LocationId != -1) {
					return onHome.Actions.SendAction("webapp", "itineraire", LocationId.ToString(), uRequestId.ToString());
                } else {
					return onHome.Actions.SendAction("webapp", "itineraire", Location, uRequestId.ToString());
                }
            } else {
                return null;
            }
        }

    }
}
