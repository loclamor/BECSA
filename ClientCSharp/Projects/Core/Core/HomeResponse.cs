using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome
{
    /// <summary>
    /// Represent the server of Home response (cf. doc serveur web.pdf for more details)
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class HomeResponse
    {
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Enum
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Home serveur status (cf. doc serveur web.pdf)
        /// </summary>
        public enum StatusType {
			/// <summary>
			/// Connexion error, response not retrieved
			/// </summary>
			ConnectionError = -1,
            /// <summary>
            /// Wait code response
            /// </summary>
            Pending = 0,                
            /// <summary>
            /// Unknwon code
            /// </summary>
            Unknwon = -1,               
            /// <summary>
            /// success opération effectuée avec succès
            /// </summary>
            Done = 200,                  
            /// <summary>
            /// success opération effectuée, une Entite a été créée
            /// </summary>
            DoneEntityCreated = 201,     
            /// <summary>
            /// success opération effectuée, mais sans garantie de résultat
            /// </summary>
            DoneWithoutGuarantee = 202,  
            /// <summary>
            /// error échec, paramètre(s) incorrect(s)
            /// </summary>
            FailBadParameters = 400,     
            /// <summary>
            /// error échec, l’Entite n’a pas été trouvée
            /// </summary>
            FailNotFound = 404,         
            /// <summary>
            /// error échec, l’opération n’est pas disponible
            /// </summary>
            FailUnknownOperator = 415,   
            /// <summary>
            /// error échec, erreur indéterminée
            /// </summary>
            Fail = 520                  
        }

       
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Code of the response
        /// </summary>
        public StatusType Status { get; protected set; }
        /// <summary>
        /// Indicate if the response success
        /// </summary>
        public bool OK { get; protected set; }
        /// <summary>
        /// Code signification
        /// </summary>
        public string Message { get; protected set; }
        /// <summary>
        /// JSON object returned by server
        /// </summary>
        public JSON Data { get; protected set; }


		///////////////////////////////////////////////////////////////////////////////////////////
		// Constructor
		///////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Default construtor
		/// </summary>
		public HomeResponse() {
			Status = StatusType.ConnectionError;
			OK = false;
			Message = "";
			Data = new JSON();
		}


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Property - get
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Retrieve code of the response from Status
        /// </summary>
        /// <returns>The code of the home server response</returns>
        public int Code() {
            return (int)(Status);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructor
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Extract JSON from home server response and fill this object with it 
        /// </summary>
        /// <param name="json">JSON object to parse</param>
        public HomeResponse(JSON json) {
            Data = json;
            OK = (string.Compare(json.Get("status").GetStringValue("error"), "success") == 0);
            Message = json.Get("message").GetStringValue();
            int code = json.GetIntValue(-1);
            switch (code) {
                case 200: Status = StatusType.Done; break;
                case 201: Status = StatusType.DoneEntityCreated; break;
                case 202: Status = StatusType.DoneWithoutGuarantee; break;
                case 400: Status = StatusType.FailBadParameters; break;
                case 404: Status = StatusType.FailNotFound; break;
                case 415: Status = StatusType.FailUnknownOperator; break;
                case 520: Status = StatusType.Fail; break;
                default:
                    Status = StatusType.Unknwon;
                    break;
            }
        }
        /// <summary>
        /// Create a HomeResponse with JSON object received from home server
        /// </summary>
        /// <param name="json">JSON object to parse</param>
        /// <returns>The corresponding HomeResponse of JSON object</returns>
        public static HomeResponse Create(JSON json) {
            return new HomeResponse(json);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // ToString 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert this Reponse into an human-readable string
        /// </summary>
        /// <returns>String representation of this song</returns>
        override public string ToString() {
            return Data.ToString();
        }


    }
}
