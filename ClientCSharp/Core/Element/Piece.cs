using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace SmartHome
{
    /// <summary>
    /// Represent a room of the SmartHome
    /// </summary>
    public class Piece
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Name of the Piece
        /// </summary>
        public string Nom { get; protected set; }
        /// <summary>
        /// Light on/off state of the room
        /// </summary>
        /// <remarks>Returns true if the light is on</remarks>
        public bool LumiereAllumer {get; protected set;}
        /// <summary>
        /// Door locked/unlocked state of the room
        /// </summary>
        /// <remarks>Returns true if the door is lock</remarks>
        public bool PorteDeverrouiller {get; protected set;}
        /// <summary>
        /// Flap opened/closed state of the room
        /// </summary>
        /// <remarks>Returns true if the flap is open</remarks>
        public bool VoletOuvert {get; protected set;}
        /// <summary>
        /// Check if a light is present
        /// </summary>
        /// <remarks>Returns true if this room have lights</remarks>
        public bool ALumiere { get; protected set; }
        /// <summary>
        /// Check if a door is present
        /// </summary>
        /// <remarks>Returns true if this room have doors</remarks>
        public bool APorte { get; protected set; }
        /// <summary>
        /// Check if a flap is present
        /// </summary>
        /// <remarks>Returns true if this room have flap</remarks>
        public bool AVolet {get; protected set;}


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Initialize
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create a room from a JSON Object. (cf. doc serveur web.pdf) 
        /// </summary>
        /// <param name="json">JSON object to read</param>
        public Piece(JSON json) {
            Nom = WebUtility.HtmlDecode(json.Get("nom").GetStringValue(Nom));
            LumiereAllumer = json.Get("lumiereAllumee").GetBoolValue(LumiereAllumer);
            PorteDeverrouiller = !(json.Get("porteVerrouillee").GetBoolValue(PorteDeverrouiller));
            VoletOuvert = json.Get("voletOuvert").GetBoolValue(VoletOuvert);
            ALumiere = json.Get("aLumiere").GetBoolValue(ALumiere);
            APorte = json.Get("aPorte").GetBoolValue(APorte);
            AVolet = json.Get("aVolet").GetBoolValue(AVolet);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Refresh
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Refresh the room from a JSON Object. (cf. doc serveur web.pdf)
        /// </summary>
        /// <param name="json">JSON object to read</param>
        public void Refresh(JSON json) {
            LumiereAllumer = json.Get("lumiereAllumee").GetBoolValue(LumiereAllumer);
            PorteDeverrouiller = !(json.Get("porteVerrouillee").GetBoolValue(PorteDeverrouiller));
            VoletOuvert = json.Get("voletOuvert").GetBoolValue(VoletOuvert);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // ToString 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert this room into an human-readable string
        /// </summary>
        /// <returns>String representation of this room</returns>
        override public string ToString() {
            string m = Nom;
            m += ":";
            if (ALumiere) {
                m += " lumiere=" + ((LumiereAllumer ? "allumer" : "eteint"));
            }
            if (APorte) {
                m += " porte=" + ((PorteDeverrouiller ? "devérrouillee" : "vérrouillee"));
            }
            if (AVolet) {
                m += " volet=" + ((VoletOuvert ? "ouvert" : "fermer"));
            }
            return m;
        }

    }
}
