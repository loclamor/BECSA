using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SmartHome
{
    /// <summary>
    /// Represent a song of the SmartHome
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class Song
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Unique identifier of this song
        /// </summary>
        public int Id { get; protected set; }
        /// <summary>
        /// Artist name of this song
        /// </summary>
        public string Artist { get; protected set; }
        /// <summary>
        /// Title of this song
        /// </summary>
        public string Title { get; protected set; }
        /// <summary>
        /// Indicate that this home element have been just added
        /// </summary>
        public bool IsNewOne { get; protected set; }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Initialize
        ///////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Private contrusctor used to initialize class members
        /// </summary>
        private Song() {
            Id = -1;
            Artist = "";
            Title = "";
            IsNewOne = true;
        }
        /// <summary>
        /// Create an song from a JSON Object. (cf. doc serveur web.pdf) 
        /// </summary>
        /// <param name="json">JSON object to read</param>
        public Song(JSON json)
            : this() 
        {
            Id = json.Get("id").GetIntValue(-1);
            Refresh(json);
            IsNewOne = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Refresh
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Refresh this song from a JSON Object. (cf. doc serveur web.pdf)
        /// </summary>
        /// <param name="json">JSON object to read</param>
        public void Refresh(JSON json) {
            Title = StringUtils.UTF8ToASCII(WebUtility.HtmlDecode(json.Get("title").GetStringValue(Title)));
            Artist = StringUtils.UTF8ToASCII(WebUtility.HtmlDecode(json.Get("artist").GetStringValue(Artist)));
            IsNewOne = false;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Compare
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Compare this song to another one, and check if there are identical.
        /// </summary>
        /// <param name="s">Song to compare to</param>
        /// <returns>True if identical</returns>
        public bool isEgalTo(Song s) {
            return ((Id == s.Id) && (string.Compare(Artist, s.Artist) == 0) && (string.Compare(Title, s.Title) == 0));
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // ToString 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert this song into an human-readable string
        /// </summary>
        /// <returns>String representation of this song</returns>
        override public string ToString() {
            return "son id=" + Id + ", title=" + Title + ", artist=" + Artist;
        }


    }
}
