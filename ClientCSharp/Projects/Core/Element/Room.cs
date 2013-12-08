using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace SmartHome
{
    /// <summary>
    /// Represent a room of the SmartHome
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class Room
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Unique identifier of this room
        /// </summary>
        public int Id { get; protected set; }
        /// <summary>
        /// Name of this room
        /// </summary>
        public string Nom { get; protected set; }
        /// <summary>
        /// Light on/off state of this room
        /// </summary>
        /// <remarks>Returns true if the light is on</remarks>
        public bool LumiereAllumer {get; protected set;}
        /// <summary>
        /// Door locked/unlocked state of this room
        /// </summary>
        /// <remarks>Returns true if the door is lock</remarks>
        public bool PorteDeverrouiller {get; protected set;}
        /// <summary>
        /// Flap opened/closed state of this room
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
        private Room() {
            Nom = "";
            LumiereAllumer = false;
            PorteDeverrouiller = false;
            VoletOuvert = false;
            ALumiere = false;
            APorte = false;
            AVolet = false;
            IsNewOne = true;
        }
        /// <summary>
        /// Create a room from a JSON Object. (cf. doc serveur web.pdf) 
        /// </summary>
        /// <param name="json">JSON object to read</param>
        public Room(JSON json) 
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
        /// Refresh this room from a JSON Object. (cf. doc serveur web.pdf)
        /// </summary>
        /// <param name="json">JSON object to read</param>
        public void Refresh(JSON json) {
            Nom = StringUtils.UTF8ToASCII(WebUtility.HtmlDecode(json.Get("nom").GetStringValue(Nom)));
            LumiereAllumer = json.Get("lumiereAllumee").GetBoolValue(LumiereAllumer);
            PorteDeverrouiller = !(json.Get("porteVerrouillee").GetBoolValue(PorteDeverrouiller));
            VoletOuvert = json.Get("voletOuvert").GetBoolValue(VoletOuvert);
            ALumiere = json.Get("aLumiere").GetBoolValue(ALumiere);
            APorte = json.Get("aPorte").GetBoolValue(APorte);
            AVolet = json.Get("aVolet").GetBoolValue(AVolet);
            IsNewOne = false;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Compare
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Compare this room to another one, and check if there are identical.
        /// </summary>
        /// <param name="r">Room to compare to</param>
        /// <returns>True if identical</returns>
        public bool isEgalTo(Room r) {
            return ((string.Compare(Nom, r.Nom) == 0)
                && (LumiereAllumer == r.LumiereAllumer) && (PorteDeverrouiller == r.PorteDeverrouiller) && (VoletOuvert == r.VoletOuvert)
                && (ALumiere == r.ALumiere) && (APorte == r.APorte) && (AVolet == r.AVolet));
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // ToString 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert this room into an human-readable string
        /// </summary>
        /// <returns>String representation of this room</returns>
        override public string ToString() {
            string m = "piece [" + Id + "," + Nom + "]:";
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
