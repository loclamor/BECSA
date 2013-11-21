using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace SmartHome
{
    /// <summary>
    /// Utils class that create HTTP request send it and retrieve response.
    /// </summary>
    public class HTTPRequest
    {
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Class used to make HTTP request 
        /// </summary>
        private WebRequest _request;
        /// <summary>
        /// HTTP request status, or ERROR if request not done 
        /// </summary>
        public string Status { get; protected set; }
        /// <summary>
        /// HTTP response
        /// </summary>
        public string Response { get; protected set; }
        /// <summary>
        /// True if response retrieved
        /// </summary>
        private bool _responseRetrieved;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create and initialize the HTTP Request 
        /// </summary>
        /// <param name="url">URL desired</param>
        public HTTPRequest(string url)
        {
            /* Create Request from url */
            Status = "";
            Response = "";
            _responseRetrieved = false;
            _request = WebRequest.Create(url);
            _request.Method = "GET";
            _responseRetrieved = false;
        }
        /// <summary>
        /// Create and initialize the HTTP Request with a defined message
        /// </summary>
        /// <param name="url">URL desired</param>
        /// <param name="msg">Message to send</param>
        public HTTPRequest(string url, Message msg) 
            : this(url + "?" + msg.ToHTTPRequest())
        {
            ;
        }

        /// <summary>
        /// Retrieve response of the HTTP Request
        /// </summary>
        /// <returns>HTTP response</returns>
        public string GetResponse()
        {
            /* Retrieve Response */
            if (!_responseRetrieved) {
                try {
                    /* Retrieve Response & Status */
                    WebResponse res = _request.GetResponse();
                    Status = ((HttpWebResponse)res).StatusDescription;
                    /* Get Response content */
                    Stream resStream = res.GetResponseStream();
                    StreamReader s = new StreamReader(resStream);
                    Response = s.ReadToEnd();
                    /* Close streams */
                    s.Close();
                    resStream.Close();
                    res.Close();
                    _responseRetrieved = true;
                } catch {
                    Status = "ERROR";
                    _responseRetrieved = true;
                }
            }
            /* Return response */
            return Response;
        }

    }
}
