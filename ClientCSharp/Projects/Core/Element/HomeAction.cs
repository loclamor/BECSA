using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SmartHome
{
    /// <summary>
    /// Represent an Action of the SmartHome
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class HomeAction
    {
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Unique ID of this action
        /// </summary>
        public int Id { get; protected set; }
        /// <summary>
        /// Action to do
        /// </summary>
        public string Type { get; protected set; }
        /// <summary>
        /// Parameters of this action
        /// </summary>
        private List<string> _params;
        /// <summary>
        /// Sended date of the action
        /// </summary>
        public DateTime SendedDate { get; protected set; }
        /// <summary>
        /// Name of the sender of this action. Reserved for future use
        /// </summary>
        public string SenderIdentifier { get; protected set; }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Initialize
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Private contrusctor used to initialize class members
        /// </summary>
        private HomeAction() {
            Id = -1;
            Type = "";
            _params = new List<string>();
            SendedDate = new DateTime();
            SenderIdentifier = "";
        }
        /// <summary>
        /// Create a action from a JSON Object. (cf. doc serveur web.pdf) 
        /// </summary>
        /// <param name="json">JSON object to read</param>
        public HomeAction(JSON json)
            : this() 
        {
            Id = json.Get("id").GetIntValue();
            Type = StringUtils.UTF8ToASCII(json.Get("action").GetStringValue());
            _params.Clear();
            JSON paramsJSON = json.Get("params");
            if (paramsJSON != null) {
                for(int i = 0; i < paramsJSON.Count; i++) {
                    _params.Add(StringUtils.UTF8ToASCII(paramsJSON.Get(i).GetStringValue()));
                }
            }
            try {
                SendedDate = DateTime.Parse(json.Get("envoie").GetStringValue());
            } catch (SystemException) {
                SendedDate = new DateTime(1, 1, 1);
            }
            SenderIdentifier = WebUtility.HtmlDecode(json.Get("from").GetStringValue());
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Property - get 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check if it's a specific type
        /// </summary>
        /// <param name="type">Type desired</param>
        /// <param name="ignoreCase">True to ignore case</param>
        /// <returns>True if is desired type</returns>
        public bool IsType(string type, bool ignoreCase = false) {
            return (string.Compare(Type, type, ignoreCase) == 0);
        }
        /// <summary>
        /// Check if it's one of specific types
        /// </summary>
        /// <param name="types">Types desired</param>
        /// <returns>True if is one of desired types</returns>
        public bool IsTypeOneOf(params string[] types) {
            for (int i = 0; i < types.Length; i++) {
                if (string.Compare(Type, types[i], false) == 0) {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Check if it's one of specific types
        /// </summary>
        /// <param name="types">Types desired</param>
        /// <returns>True if is one of desired types</returns>
        public bool IsTypeOneOfIgnoringCase(params string[] types) {
            for (int i = 0; i < types.Length; i++) {
                if (string.Compare(Type, types[i], true) == 0) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Convert Sended date to "yyyy-MM-dd"
        /// </summary>
        public string SendedDateString {
            get {
                return SendedDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// Get parameters count of this action
        /// </summary>
        public int ParamCount {
            get {
                return _params.Count;
            }
        }
        /// <summary>
        /// Check by id if this action have a parameter
        /// </summary>
        /// <param name="id">Id of the desired parameter</param>
        /// <returns>True if parameter present</returns>
        public bool ContainsParam(int id) {
            return ((id >= 0) && (id < _params.Count));
        }
        /// <summary>
        /// Get parameter value by id
        /// </summary>
        /// <param name="id">Id of the desired parameter</param>
        /// <param name="defaultValue">Default value to return if parameter not found</param>
        /// <returns>Parameter value if found otherwise defaultValue</returns>
        public string GetParam(int id, string defaultValue = "") {
            if ((id >= 0) && (id < _params.Count)) {
                return _params[id];
            } else {
                return defaultValue;
            }
        }
        /// <summary>
        /// Get parameter integer value by id
        /// </summary>
        /// <param name="id">Id of the desired parameter</param>
        /// <param name="defaultValue">Default value to return if parameter not found</param>
        /// <returns>Parameter integer value if found otherwise defaultValue</returns>
        public int GetIntParam(int id, int defaultValue = 0) {
            if ((id >= 0) && (id < _params.Count)) {
                try {
                    return StringUtils.StringToInt(_params[id]);
                } catch {
                    return 0;
                }
            } else {
                return defaultValue;
            }
        }
        /// <summary>
        /// Get parameter double value by id
        /// </summary>
        /// <param name="id">Id of the desired parameter</param>
        /// <param name="defaultValue">Default value to return if parameter not found</param>
        /// <returns>Parameter double value if found otherwise defaultValue</returns>
        public double GetDoubleParam(int id, double defaultValue = 0) {
            if ((id >= 0) && (id < _params.Count)) {
                try {
                    return StringUtils.StringToDouble(_params[id]);
                } catch {
                    return 0;
                }
            } else {
                return defaultValue;
            }
        }
        /// <summary>
        /// Get parameter boolean value by id
        /// </summary>
        /// <param name="id">Id of the desired parameter</param>
        /// <param name="defaultValue">Default value to return if parameter not found</param>
        /// <returns>Parameter boolean value if found otherwise defaultValue</returns>
        public bool GetBoolParam(int id, bool defaultValue = false) {
            if ((id >= 0) && (id < _params.Count)) {
                return ((string.Compare(_params[id], "true", true) == 0) || (string.Compare(_params[id], "1", true) == 0));
            } else {
                return defaultValue;
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////
        // Compare
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Compare this home action to another one, and check if there are perfectly identical.
        /// </summary>
        /// <param name="a">Action to compare to</param>
        /// <returns>True if identical</returns>
        public bool isEgalTo(HomeAction a) {
            if ((Id == a.Id) && (string.Compare(Type, a.Type) == 0)
                && (SendedDate.CompareTo(a.SendedDate) == 0)
                && (string.Compare(SenderIdentifier, a.SenderIdentifier) == 0) && (_params.Count == a._params.Count)) {
                /* Compare all parameters */ 
                for(int i = 0; i < _params.Count; i++) {
                    if (string.Compare(_params[i], a._params[i]) != 0) return false;
                }
                return true;
            } else {
                return false;
            }
        }
		/// <summary>
		/// Compare this home action to another one, and check if there are identical.
		/// </summary>
		/// <param name="a">Action to compare to</param>
		/// <returns>True if identical</returns>
		public bool isIdenticalTo(HomeAction a) {
			if ((string.Compare(Type, a.Type) == 0)
				&& (string.Compare(SenderIdentifier, a.SenderIdentifier) == 0) && (_params.Count == a._params.Count)) {
				/* Compare all parameters */
				for (int i = 0; i < _params.Count; i++) {
					if (string.Compare(_params[i], a._params[i]) != 0) return false;
				}
				return true;
			} else {
				return false;
			}
		}

        ///////////////////////////////////////////////////////////////////////////////////////////
        // ToString 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert this action into an human-readable string
        /// </summary>
        /// <returns>String representation of this action</returns>
        override public string ToString() {
            string m = "action " + Id + ": type=" + Type + " envoie=" + SendedDateString;
            if (SenderIdentifier.Length > 0) {
                m += " from=" + SenderIdentifier;
            }
            m += " params=[";
            for (int i = 0; i < _params.Count; i++) {
                if (i > 0) {
                    m += ",";
                }
                m += _params[i];
            }
            m += "]";
            return m;
        }


    }
}
