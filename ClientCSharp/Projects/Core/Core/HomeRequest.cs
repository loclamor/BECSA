using System;
using System.Collections.Generic;
using System.Text;


namespace SmartHome
{
    /// <summary>
    /// Home request composer. Use user-defined parameters to produce a message ready to send
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class HomeRequest
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// User-defined parameters
        /// </summary>
        protected Dictionary<string, string> _params;


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create an empty message
        /// </summary>
        public HomeRequest() {
            _params = new Dictionary<string,string>();
        }
        /// <summary>
        /// Read a message from a string, formatted like URL parameters.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="request"></param>
        public HomeRequest(string request) {
            ExplodeHTTPRequest(request);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Param management
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check if no parameters present
        /// </summary>
        /// <returns>True if no parameters found</returns>
        public bool IsEmpty() {
            return (_params.Count == 0);
        }
        /// <summary>
        /// Check if parameters present
        /// </summary>
        /// <returns>True if parameters found</returns>
        public bool IsNotEmpty() {
            return (_params.Count > 0);
        }
        /// <summary>
        /// Clear all parameters defined
        /// </summary>
        /// <returns>This object to enable chained call (example: msg.Clear().Set("done","ok");)</returns>
        public HomeRequest Clear() {
            _params.Clear();
            return this;
        }
        /// <summary>
        /// Modify or Add an parameters
        /// </summary>
        /// <param name="name">Name of the parameter to change or create</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>This object to enable chained call (example: msg.Set("test","42").Set("done","ok");)</returns>
        public HomeRequest Set(string name, string value = "") {
            if (_params.ContainsKey(name)) {
                _params[name] = value.Trim();
            } else {
                _params.Add(name, value.Trim());
            }
            return this;
        }
        /// <summary>
        /// Remove a parameter
        /// </summary>
        /// <param name="name">Name of the desired parameter</param>
        /// <returns>This object to enable chained call (example: msg.Remove("test").Set("done","ok");)</returns>
        public HomeRequest Remove(string name) {
            if (_params.ContainsKey(name)) {
                _params.Remove(name);
            }
            return this;
        }
        /// <summary>
        /// Get a parameter value by name
        /// </summary>
        /// <param name="name">Name of the desired parameter</param>
        /// <returns>Value of the parameter</returns>
        public string Get(string name) {
            if (_params.ContainsKey(name)) {
                return _params[name];
            } else {
                return "";
            }
        }
        /// <summary>
        /// Get a parameter value by id
        /// </summary>
        /// <param name="id">Id of the desired parameter</param>
        /// <returns>Value of the parameter</returns>
        public string Get(int id) {
            if ((id >= 0) && (id < _params.Count)) {
                foreach (KeyValuePair<string, string> pair in _params) {
                    if (id == 0) return pair.Value;
                    id--;
                }
                return "";
            } else {
                return "";
            }
        }
        /// <summary>
        /// Get a parameter name
        /// </summary>
        /// <param name="id">Id of the desired parameter</param>
        /// <returns></returns>
        public string GetName(int id) {
            if ((id >= 0) && (id < _params.Count)) {
                foreach (KeyValuePair<string, string> pair in _params) {
                    if (id == 0) return pair.Key;
                    id--;
                }
                return "";
            } else {
                return "";
            }
        }
        /// <summary>
        /// Get the number of parameters
        /// </summary>
        public int Count {
            get {
                return _params.Count;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Convert functions
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Retrieve all parameters formatted like URL parameters.
        /// </summary>
        /// <returns>Parameters list formatted like URL parameters.</returns>
        public string ToHTTPRequest() {
            string request = System.String.Empty;
            /* Encode all parameters */
            int id = 0;
            foreach (KeyValuePair<string, string> pair in _params) {
                if (id > 0) {
                    request += "&";
                }
                request += Uri.EscapeDataString(pair.Key);
                if (pair.Value.Length > 0) {
                    request += "=" + Uri.EscapeDataString(pair.Value);
                }
                id++;
            }
            /* Return request */
            return request;
        }

        /// <summary>
        /// Set parameters from string 
        /// </summary>
        /// <param name="request">Parameters list formatted like URL parameters.</param>
        public void ExplodeHTTPRequest(string request) {
            /* Initialize */ 
            int nameStart = 0;
            int nameEnd = 0;
            int valueStart = 0;
            int mode = 0; /* 0 = Wait '=', 1 = Wait '&' */ 
            /* Retrieve all parameters */ 
            for (int i = 0, max = request.Length; i <= max; i++) {
                if (mode == 0) {
                    /* Wait '=' */
                    if (i == max) {
                        /* Param with no value */
                        Set(Uri.EscapeUriString(request.Substring(nameStart, i - nameStart)));
                    } else if (request[i] == '=') {
                        /* Param with value */ 
                        nameEnd = i;
                        valueStart = i + 1;
                    }
                } else if (mode == 1) {
                    /* Wait '&' */
                    if ((i == max) || (request[i] == '&')) {
                        Set(Uri.EscapeUriString(request.Substring(nameStart, nameEnd - nameStart)),
                            Uri.EscapeUriString(request.Substring(valueStart, i - valueStart)));
                    }
                }
            }
        }

    }
}
