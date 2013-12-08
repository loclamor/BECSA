using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Action for Hifi
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class HifiAction : AbstractAction
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
            /// Play an random song
            /// </summary>
            PLAY_RANDOM_SONG,
            /// <summary>
            /// Play a specific song
            /// </summary>
            PLAY_SONG,
            /// <summary>
            /// Next song
            /// </summary>
            NEXT_SONG,
            /// <summary>
            /// Previous song
            /// </summary>
            PREVIOUS_SONG,
            /// <summary>
            /// Stop all songs
            /// </summary>
            STOP_ALL_SONG,
            /// <summary>
            /// List all songs
            /// </summary>
            LIST_ALL_SONGS
        };
            
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Action to do on Hifi or song
        /// </summary>
        public ActionType Action { get; protected set; }
        /// <summary>
        /// Can be <see cref="SmartHome.Song.Id"/> or -1.
        /// </summary>
        /// <remarks>-1 = All</remarks>
        public int SongId { get; protected set; }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize an hifi action
        /// </summary>
        /// <param name="act">Action to do with hifi or song</param>
        /// <param name="songId">Song identifier number to control</param>
        public HifiAction(ActionType act, int songId = -1) {
            Action = act;
            SongId = songId;
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
            if (Action == ActionType.PLAY_SONG) {
                return onHome.Actions.SendAction("webapp", "playSongId", SongId.ToString());
            } else if (Action == ActionType.PLAY_RANDOM_SONG) {
                return onHome.Actions.SendAction("webapp", "playSong");
            } else if (Action == ActionType.NEXT_SONG) {
                return onHome.Actions.SendAction("webapp", "nextSong");
            } else if (Action == ActionType.PREVIOUS_SONG) {
                return onHome.Actions.SendAction("webapp", "previousSong");
            } else if (Action == ActionType.STOP_ALL_SONG) {
                return onHome.Actions.SendAction("webapp", "pauseSong");
            } else if (Action == ActionType.LIST_ALL_SONGS) {
                return onHome.Actions.SendAction("synthese", "listSongs");
            } else {
                return null;
            }
        }


    }
}
