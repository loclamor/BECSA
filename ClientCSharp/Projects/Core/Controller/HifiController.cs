using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Home hifi controller
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class HifiController : Controller
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Alarm clocks of home
        /// </summary>
        private List<Song> _songs;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Contructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="home">Home to control</param>
        public HifiController(Home home)
            : base(home)
        {
            _songs = new List<Song>();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Hifi - set
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Add a song
        /// </summary>
        /// <remarks>To take in account the song creation you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="title">Title of the song to add</param>
        /// <param name="artist">Artist of the song to add</param>
        /// <returns>Server response</returns>
        public HomeResponse AddSong(string title, string artist) {
            /* Init */
            HomeRequest msgGET = new HomeRequest();
            msgGET.Set("controller", "hifi")
                .Set("action", "ajouter");
            HomeRequest msgPOST = new HomeRequest();
            msgPOST.Set("title", title)
                .Set("artist", artist);
            /* Execute */
            HTTPRequest r = new HTTPRequest(_home.HomeURI, msgGET, msgPOST);
            return HomeResponse.Create(new JSON(r.GetResponse()));
        }
        /// <summary>
        /// Remove a song
        /// </summary>
        /// <remarks>To take in account the song deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="song">Song to remove</param>
        /// <returns>Server response</returns>
        public HomeResponse RemoveSong(Song song) {
            if (song != null) {
                return RemoveSong(song.Id);
            } else {
                return null;
            }
        }
        /// <summary>
        /// Remove a song by index
        /// </summary>
        /// <remarks>To take in account the song deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="index">Index of song to remove</param>
        /// <returns>Server response</returns>
        public HomeResponse RemoveSongByIndex(int index) {
            return RemoveSong(GetSongByIndex(index));
        }
        /// <summary>
        /// Remove a song by title
        /// </summary>
        /// <remarks>To take in account the song deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="title">Title of song to remove</param>
        /// <param name="ignoreCase">True to ignore case during seeking song by title</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking song by title</param>
        /// <returns>Server response</returns>
        public HomeResponse RemoveSong(string title, bool ignoreCase = true, bool ignoreAccent = true) {
            return RemoveSong(GetSong(title, ignoreCase, ignoreAccent));
        }
        /// <summary>
        /// Remove a song by identifier number
        /// </summary>
        /// <remarks>To take in account the song deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="identifier">Identifier number of song to remove</param>
        /// <returns>Server response</returns>
        public HomeResponse RemoveSong(int identifier) {
            /* Init */
            HomeRequest msg = new HomeRequest();
            msg.Set("controller", "hifi")
                .Set("action", "supprimer")
                .Set("id", identifier.ToString());
            /* Execute */
            HTTPRequest r = new HTTPRequest(_home.HomeURI, msg);
            return HomeResponse.Create(new JSON(r.GetResponse()));
        }



        ///////////////////////////////////////////////////////////////////////////////////////////
        // Song - get
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get the number of the songs
        /// </summary>
        /// <returns>Songs count</returns>
        public int SongCount {
            get {
                return _songs.Count;
            }
        }
        /// <summary>
        /// Get a song by title
        /// </summary>
        /// <remarks>This function return the first song found with corresponding title, since there can be song with identical name.</remarks>
        /// <param name="title">Title of the desired song</param>
        /// <param name="ignoreCase">True to ignore case during seeking song by title</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking song by title</param>
        /// <returns>Song found Or null if no song found</returns>
        public Song GetSong(string title, bool ignoreCase = true, bool ignoreAccent = true) {
            foreach (Song s in _songs) {
                if (StringUtils.IdenticalString(s.Title, title, ignoreCase, ignoreAccent)) {
                    return s;
                }
            }
            return null;
        }
        /// <summary>
        /// Get a song by index
        /// </summary>
        /// <param name="index">Index of the desired song</param>
        /// <returns>Song found Or null if no song found</returns>
        public Song GetSongByIndex(int index) {
            if ((index >= 0) && (index < _songs.Count)) {
                return _songs.ElementAt(index);
            } else {
                return null;
            }
        }
        /// <summary>
        /// Get a song by identifier number
        /// </summary>
        /// <param name="identifierNumber">Identifier number of the desired song</param>
        /// <returns>Song found Or null if no song found</returns>
        public Song GetSong(int identifierNumber) {
            foreach (Song s in _songs) {
                if (s.Id == identifierNumber) {
                    return s;
                }
            }
            return null;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Refresh
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Refresh list of songs.
        /// </summary>
        /// <remarks>Will automatically fired hifi update event if needed and configured (<see cref="SmartHome.Home.RegisterHifiUpdateEvent"/>)</remarks>
        /// <param name="json">JSON to load, containing all songs</param>
        /// <returns>True if refreshed without errors</returns>
        public bool Refresh(JSON json) {
            /* Check if needed */
            if (json == null) return true;
            /* Retrieve all Pieces from JSON response */
            List<Song> newSongs = new List<Song>();
            for (int i = 0, max = json.Count; i < max; i++) {
                JSON jsonSong = ((json.Type == JSON.ValueType.ARRAY) ? json.Get(i) : json);
                newSongs.Add(new Song(jsonSong));
                /* Manage when it's not an array but a simply object */
                if (json.Type == JSON.ValueType.OBJECT) break;
            }
            /* Stock all change in order to fire event after update completed */
            List<Song> songAdded = new List<Song>();
            List<Song> songRemoved = new List<Song>();
            List<Song> songUpdated = new List<Song>();
            foreach (Song s in newSongs) {
                /* Check if there is an update from older song */
                Song oldSong = GetSong(s.Id);
                if (oldSong != null) {
                    if (!(oldSong.isEgalTo(s))) {
                        /* Song updated */
                        songUpdated.Add(oldSong);
                    }
                } else {
                    /* Song added */
                    songAdded.Add(s);
                }
            }
            /* Check song removed */
            foreach (Song oldS in _songs) {
                bool found = false;
                foreach (Song newS in newSongs) {
                    if (newS.Id == oldS.Id) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    songRemoved.Add(oldS);
                }
            }
            /* Copy new rooms */
            _songs = newSongs;
            /* Make home event */
            if (_home.OnHifiUpdate != null) {
                /* Remove */
                foreach (Song s in songRemoved) {
                    _home.OnHifiUpdate(_home, s, Home.HifiUpdateKind.SongRemoved);
                }
                /* Add */
                foreach (Song s in songAdded) {
                    _home.OnHifiUpdate(_home, s, Home.HifiUpdateKind.NewSongAdded);
                }
                /* Updated */
                foreach (Song s in songUpdated) {
                    _home.OnHifiUpdate(_home, s, Home.HifiUpdateKind.SongUpdated);
                }
            }
            if ((_home.OnHomeUpdate != null) && ((songRemoved.Count > 0) || (songAdded.Count > 0))) {
                /* Room list changed */
                _home.OnHomeUpdate(_home, Home.HomeUpdateKind.SongListChanged);
            }
            /* Done: return true */ 
            return true;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // ToString 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert songs into human-readable string
        /// </summary>
        /// <returns>String representation of all song</returns>
        override public string ToString() {
            string m = "";
            int id = 0;
            foreach (Song s in _songs) {
                if (id > 0) {
                    m += "\n";
                }
                m += s.ToString();
                id++;
            }
            return m;
        }

    }
}