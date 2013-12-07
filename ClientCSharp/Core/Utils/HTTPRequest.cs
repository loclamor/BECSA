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
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class HTTPRequest
    {
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Enum
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// HTTP Request mode.
        /// </summary>
        public enum Method
        {
            /// <summary>
            /// Message data send by url. (cf. HTTP GET documentation)
            /// </summary>
            GET,
            /// <summary>
            /// Message data incorporate in message data. (cf. HTTP POST documentation) 
            /// </summary>
            POST
        }

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
        public HTTPRequest(string url) {
            /* Create Request from url */
            Status = "";
            Response = "";
            _responseRetrieved = false;
            _request = WebRequest.Create(url);
            _request.Method = "GET";
        }
        /// <summary>
        /// Create and initialize the HTTP Request with a defined message
        /// </summary>
        /// <param name="url">URL desired</param>
        /// <param name="msg">Message to send</param>
        /// <param name="method">Method to use</param>
        public HTTPRequest(string url, HomeRequest msg, Method method = Method.GET) {
            if (method == Method.POST) {
                init(url, null, msg);
            } else {
                init(url, msg, null);
            }
        }
        /// <summary>
        /// Create and initialize the HTTP Request with a defined message
        /// </summary>
        /// <param name="url">URL desired</param>
        /// <param name="msgGET">Message to send by GET method</param>
        /// <param name="msgPOST">Message to send by POST method</param>
        public HTTPRequest(string url, HomeRequest msgGET, HomeRequest msgPOST) {
            init(url, msgGET, msgPOST);
        }
        /// <summary>
        /// Create and initialize the HTTP Request with a defined message
        /// </summary>
        /// <param name="url">URL desired</param>
        /// <param name="msgGET">Message to send by GET method</param>
        /// <param name="msgPOST">Message to send by POST method</param>
        private void init(string url, HomeRequest msgGET, HomeRequest msgPOST) {
            /* Initialize */
            Status = "";
            Response = "";
            _responseRetrieved = false;
            /* Create Request from url */
            if ((msgGET != null) && (msgGET.Count > 0)) {
                _request = WebRequest.Create(url + "?" + msgGET.ToHTTPRequest());
            } else {
                _request = WebRequest.Create(url);
            }
            /* Add POST data if have one */
            try {
                if (msgPOST != null) {
                    /* POST */
                    _request.Method = "POST";
                    _request.ContentType = "application/x-www-form-urlencoded";
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] data = encoding.GetBytes(msgPOST.ToHTTPRequest());
                    _request.ContentLength = data.Length;
                    Stream s = _request.GetRequestStream();
                    s.Write(data, 0, data.Length);
                    s.Close();
                } else {
                    /* GET */
                    _request.Method = "GET";
                }
            } catch {
                ;
            }
        }


        /// <summary>
        /// Retrieve response of the HTTP Request
        /// </summary>
        /// <returns>HTTP response</returns>
        public string GetResponse() {
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
