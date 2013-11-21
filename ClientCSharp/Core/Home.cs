using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome
{
    /// <summary>
    /// Home main controller which interact directly with the Home server
    /// </summary>
    public class Home
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Enum
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Action that an room can do
        /// </summary>
        public enum RoomUpdateKind
        {
            /// <summary>
            /// Indicate that a room light have been switched to on
            /// </summary>
            SwitchOnRoomLight,
            /// <summary>
            /// Indicate that a room light have been switched to off
            /// </summary>
            SwitchOffRoomLight,
            /// <summary>
            /// Indicate that a room door have been opened
            /// </summary>
            OpenRoomDoor,
            /// <summary>
            /// Indicate that a room door have been closed
            /// </summary>
            CloseRoomDoor,
            /// <summary>
            /// Indicate that a room flap have been opened
            /// </summary>
            OpenRoomFlap,
            /// <summary>
            /// Indicate that a room flap have been closed
            /// </summary>
            CloseRoomFlap
        }
        /// <summary>
        /// Action that the home can do
        /// </summary>
        public enum HomeUpdateKind
        {
            /// <summary>
            /// Indicate that all home lights have been switched to on
            /// </summary>
            SwitchOnAllRoomLight,
            /// <summary>
            /// Indicate that all home lights have been switched to off
            /// </summary>
            SwitchOffAllRoomLight,
            /// <summary>
            /// Indicate that all home doors have been opened
            /// </summary>
            OpenAllDoor,
            /// <summary>
            /// Indicate that all home doors have been closed
            /// </summary>
            CloseAllDoor,
            /// <summary>
            /// Indicate that all home flaps have been opened
            /// </summary>
            OpenAllFlap,
            /// <summary>
            /// Indicate that all home flaps have been closed
            /// </summary>
            CloseAllFlap,
            /// <summary>
            /// Indicate that some room(s) have been added or removed
            /// </summary>
            RoomCountChanged
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Events
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Event function to detect when a Room have been updated
        /// </summary>
        /// <param name="p">Room updated</param>
        /// <param name="updateKind">Update kind</param>
        public delegate void PieceUpdate(Piece p, RoomUpdateKind updateKind);
        /// <summary>
        /// Event function to detect when the homve have been updated
        /// </summary>
        /// <param name="updateKind">Update kind</param>
        public delegate void HomeUpdate(HomeUpdateKind updateKind);


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Room availaible from home server
        /// </summary>
        private Dictionary<string, Piece> _pieces;
        /// <summary>
        /// Home server URI
        /// </summary>
        public string HomeURI { get; set; }
        /// <summary>
        /// Event called when a room have been updated
        /// </summary>
        private PieceUpdate _onPieceUpdate;
        /// <summary>
        /// Event called when home have been updated
        /// </summary>
        private HomeUpdate _onHomeUpdate;
        /// <summary>
        /// Last System.Environment.TickCount when a refresh was done
        /// </summary>
        private int _lastUpdateTick;
        /// <summary>
        /// Frequency of refresh in milliseconds
        /// </summary>
        private int RefreshFrequency { get; set; }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize an empty Home. HomeURI need to be set after before using refresh.
        /// </summary>
        public Home() {
            _pieces = new Dictionary<string, Piece>();
            RefreshFrequency = 0;
            _lastUpdateTick = 0;
            HomeURI = "";
        }
        /// <summary>
        /// Initialize Home with the location of the Home server set
        /// </summary>
        /// <param name="uri">URI of the Home server</param>
        /// <param name="refreshFrequency">Frequency of refresh in milliseconds (set to 0 to have direct refresh)</param>
        public Home(string uri, int refreshFrequency = 1000) 
            : this()
        {
            HomeURI = uri;
            RefreshFrequency = refreshFrequency;
        }
        

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Event
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Register an update event for room
        /// </summary>
        /// <param name="update">Update function called when a room have been update</param>
        public void RegisterPieceUpdateEvent(PieceUpdate update) {
            if (_onPieceUpdate == null) {
                _onPieceUpdate = new PieceUpdate(update);
            } else {
                _onPieceUpdate += update;
            }
        }
        /// <summary>
        /// Register an update event for home
        /// </summary>
        /// <param name="update">Update function called when home have been update</param>
        public void RegisterHomeUpdateEvent(HomeUpdate update) {
            if (_onHomeUpdate == null) {
                _onHomeUpdate = new HomeUpdate(update);
            } else {
                _onHomeUpdate += update;
            }
        }
        /// <summary>
        /// Unregister an update event for room
        /// </summary>
        /// <param name="update">Update function to unregister</param>
        public void UnregisterPieceUpdateEvent(PieceUpdate update) {
            if (_onPieceUpdate != null) {
                _onPieceUpdate -= update;
            }
        }
        /// <summary>
        /// Unregister an update event for home
        /// </summary>
        /// <param name="update">Update function to unregister</param>
        public void UnregisterHomeUpdateEvent(HomeUpdate update) {
            if (_onHomeUpdate != null) {
                _onHomeUpdate -= update;
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////
        // Refresh
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Refresh list of rooms and status of each rooms.
        /// </summary>
        /// <remarks>Will automatically fired room update and home update event if needed and configured (<see cref="SmartHome.Home.RegisterPieceUpdateEvent"/>, <see cref="SmartHome.Home.RegisterHomeUpdateEvent"/>)</remarks>
        /// <returns>Response of the Home server</returns>
        public HomeResponse Refresh() {
            /* Check if time to refresh */
            if (RefreshFrequency > 0) {
                if ((System.Environment.TickCount - _lastUpdateTick) < RefreshFrequency) {
                    return null;
                }
            }
            /* Get Pieces from server */
            HomeResponse res = ExecuteAction("piece", "lister");
            JSON jsonPieces = res.Data.Get("pieces");
            /* Retrieve all Pieces from JSON response */
            Dictionary<string, Piece> newPieces = new Dictionary<string, Piece>();
            for (int i = 0, max = jsonPieces.Count; i < max; i++) {
                JSON jsonPiece = jsonPieces.Get(i);
                newPieces.Add(jsonPiece.Get("nom").Value, new Piece(jsonPiece));
            }
            /* Count how many Piece element have changed */ 
            List<Piece> lightUpdated = new List<Piece>();
            List<Piece> doorUpdated = new List<Piece>();
            List<Piece> flapUpdated = new List<Piece>();
            int haveLightCount = 0;
            int haveDoorCount = 0;
            int haveFlapCount = 0;
            int lightOnCount = 0;
            int doorOpenCount = 0;
            int flapOpenCount = 0;
            foreach (KeyValuePair<string,Piece> pair in newPieces) {
                Piece p = pair.Value;
                /* Check if there is an update from older Piece */ 
                Piece oldPiece = GetPiece(p.Nom);
                if ((oldPiece != null) && (oldPiece != p)) {
                    /* Count the light/door/flap change */ 
                    if (p.LumiereAllumer != oldPiece.LumiereAllumer) lightUpdated.Add(p);
                    if (p.PorteDeverrouiller != oldPiece.PorteDeverrouiller) doorUpdated.Add(p);
                    if (p.VoletOuvert != oldPiece.VoletOuvert) flapUpdated.Add(p);
                }
                /* Count element "on" */
                if (p.ALumiere) haveLightCount++;
                if (p.APorte) haveDoorCount++;
                if (p.AVolet) haveFlapCount++;
                if ((p.ALumiere) && (p.LumiereAllumer)) lightOnCount++;
                if ((p.APorte) && (p.PorteDeverrouiller)) doorOpenCount++;
                if ((p.AVolet) && (p.VoletOuvert)) flapOpenCount++;
            }
            /* Copy new piece */
            int oldPieceCount = _pieces.Count;
            _pieces = newPieces;
            /* Make event if piece count changed */
            if ((_onHomeUpdate != null) && (oldPieceCount != newPieces.Count)) {
                _onHomeUpdate(HomeUpdateKind.RoomCountChanged);
            }
            /* Call delegates for light */
            if ((lightUpdated.Count > 0) && (lightOnCount == 0)) {
                /* All light switched to off */ 
                if (_onHomeUpdate != null) _onHomeUpdate(HomeUpdateKind.SwitchOffAllRoomLight);
            } else if ((lightUpdated.Count > 0) && (lightOnCount == haveLightCount)) {
                /* All light switched to on */
                if (_onHomeUpdate != null) _onHomeUpdate(HomeUpdateKind.SwitchOnAllRoomLight);
            } else if (_onPieceUpdate != null) {
                /* Call delegate for each Piece where light have been updated */
                foreach (Piece p in lightUpdated) {
                    _onPieceUpdate(p, (p.LumiereAllumer ? RoomUpdateKind.SwitchOnRoomLight : RoomUpdateKind.SwitchOffRoomLight));
                }
            }
            /* Call delegates for door: same algo */
            if ((doorUpdated.Count > 0) && (doorOpenCount == 0)) {
                if (_onHomeUpdate != null) _onHomeUpdate(HomeUpdateKind.CloseAllDoor);
            } else if ((doorUpdated.Count > 0) && (doorOpenCount == haveDoorCount)) {
                if (_onHomeUpdate != null) _onHomeUpdate(HomeUpdateKind.OpenAllDoor);
            } else if (_onPieceUpdate != null) {
                foreach (Piece p in doorUpdated) {
                    _onPieceUpdate(p, (p.PorteDeverrouiller ? RoomUpdateKind.OpenRoomDoor : RoomUpdateKind.CloseRoomDoor));
                }
            }
            /* Call delegates for flap: same algo */
            if ((flapUpdated.Count > 0) && (flapOpenCount == 0)) {
                if (_onHomeUpdate != null) _onHomeUpdate(HomeUpdateKind.CloseAllFlap);
            } else if ((flapUpdated.Count > 0) && (flapOpenCount == haveFlapCount)) {
                if (_onHomeUpdate != null) _onHomeUpdate(HomeUpdateKind.OpenAllFlap);
            } else if (_onPieceUpdate != null) {
                foreach (Piece p in flapUpdated) {
                    _onPieceUpdate(p, (p.VoletOuvert ? RoomUpdateKind.OpenRoomFlap : RoomUpdateKind.CloseRoomFlap));
                }
            }
            /* Refresh update tick */
            if (RefreshFrequency > 0) {
                _lastUpdateTick = System.Environment.TickCount;
            }
            /* Return server response */
            return res;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Pieces
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get the number of the room
        /// </summary>
        /// <returns>Room count</returns>
        public int GetPieceCount() {
            return _pieces.Count;
        }
        /// <summary>
        /// Get a room by name
        /// </summary>
        /// <param name="name">Name of the desired room</param>
        /// <returns>Room found Or null if no room found</returns>
        public Piece GetPiece(string name) {
            if (_pieces.ContainsKey(name)) {
                return _pieces[name];
            } else {
                return null;
            }
        }
        /// <summary>
        /// Get a room by id
        /// </summary>
        /// <param name="id">Id of the desired room</param>
        /// <returns>Room found Or null if no room found</returns>
        public Piece GetPiece(int id) {
            if ((id >= 0) && (id < _pieces.Count)) {
                foreach (KeyValuePair<string, Piece> p in _pieces) {
                    if (id == 0) {
                        return p.Value;
                    }
                    id--;
                }
            }
            return null;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Lumiere
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Switch on light of a room
        /// </summary>
        /// <param name="piece">Room name desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse AllumerLumiere(string piece) {
            return ExecuteAction("lumiere", "allumer", piece);
        }
        /// <summary>
        /// Switch off light of a room
        /// </summary>
        /// <param name="piece">Room name desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse EteindreLumiere(string piece) {
            return ExecuteAction("lumiere", "eteindre", piece);
        }
        /// <summary>
        /// Switch on light of a room
        /// </summary>
        /// <param name="piece">Room desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse AllumerLumiere(Piece piece) {
            return ExecuteAction("lumiere", "allumer", piece.Nom);
        }
        /// <summary>
        /// Switch off light of a room
        /// </summary>
        /// <param name="piece">Room desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse EteindreLumiere(Piece piece) {
            return ExecuteAction("lumiere", "eteindre", piece.Nom);
        }
        /// <summary>
        /// Switch on all light
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse AllumerTout() {
            return ExecuteAction("lumiere", "allumerTout");
        }
        /// <summary>
        /// Switch off all light
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse EteindreTout() {
            return ExecuteAction("lumiere", "eteindreTout");
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Porte
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Lock the door of a room
        /// </summary>
        /// <param name="piece">Room name desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse VerrouillerPorte(string piece) {
            return ExecuteAction("porte", "verrouiller", piece);
        }
        /// <summary>
        /// Unlock the door of a room
        /// </summary>
        /// <param name="piece">Rom name desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse DeverrouillerPorte(string piece) {
            return ExecuteAction("porte", "deverrouiller", piece);
        }
        /// <summary>
        /// Lock the door of a room
        /// </summary>
        /// <param name="piece">Room desired</param>
        /// <returns></returns>
        public HomeResponse VerrouillerPorte(Piece piece) {
            return ExecuteAction("porte", "verrouiller", piece.Nom);
        }
        /// <summary>
        /// Unlock the door of a room
        /// </summary>
        /// <param name="piece">Rom desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse DeverrouillerPorte(Piece piece) {
            return ExecuteAction("porte", "deverrouiller", piece.Nom);
        }
        /// <summary>
        /// Lock all the door of home
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse VerrouillerTout() {
            return ExecuteAction("porte", "verrouillerTout");
        }
        /// <summary>
        /// UnLock all the door of home
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse DeverrouillerTout() {
            return ExecuteAction("porte", "deverrouillerTout");
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Volet
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Open flaps of a room
        /// </summary>
        /// <param name="piece">Room name desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse OuvrirVolet(string piece) {
            return ExecuteAction("volet", "ouvrir", piece);
        }
        /// <summary>
        /// Close flaps of a room
        /// </summary>
        /// <param name="piece">Room name desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse FermerVolet(string piece) {
            return ExecuteAction("volet", "fermer", piece);
        }
        /// <summary>
        /// Open flaps of a room
        /// </summary>
        /// <param name="piece">Room desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse OuvrirVolet(Piece piece) {
            return ExecuteAction("volet", "ouvrir", piece.Nom);
        }
        /// <summary>
        /// Close flaps of a room
        /// </summary>
        /// <param name="piece">Room desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse FermerVolet(Piece piece) {
            return ExecuteAction("volet", "fermer", piece.Nom);
        }
        /// <summary>
        /// Open all flaps of home
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse OuvrirTout() {
            return ExecuteAction("volet", "ouvrirTout");
        }
        /// <summary>
        /// Close all flaps of home
        /// </summary>
        /// <returns>Home server response</returns>
        public HomeResponse FermerTout() {
            return ExecuteAction("volet", "fermerTout");
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Action functions
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute an action to a controller of the home
        /// </summary>
        /// <remarks>For an expansive details of controller/action availaible cf. doc serveur web.pdf</remarks>
        /// <param name="controller">Controller desired</param>
        /// <param name="action">Action desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse ExecuteAction(string controller, string action) {
            Message msg = new Message();
            msg.Set("controller", controller)
                .Set("action", action);
            HTTPRequest r = new HTTPRequest(HomeURI, msg);
            return HomeResponse.Create(new JSON(r.GetResponse()));
        }
        /// <summary>
        /// Execute an action to a controller of the home
        /// </summary>
        /// <remarks>For an expansive details of controller/action availaible cf. doc serveur web.pdf</remarks>
        /// <param name="controller">Controller desired</param>
        /// <param name="action">Action desired</param>
        /// <param name="piece">Room name desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse ExecuteAction(string controller, string action, string piece) {
            Message msg = new Message();
            msg.Set("controller", controller)
                .Set("action", action)
                .Set("piece", piece);
            HTTPRequest r = new HTTPRequest(HomeURI, msg);
            return HomeResponse.Create(new JSON(r.GetResponse()));
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // ToString 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert home into human-readable string
        /// </summary>
        /// <returns>String representation of this home</returns>
        override public string ToString() {
            string m = "";
            int id = 0;
            foreach (KeyValuePair<string, Piece> p in _pieces) {
                if (id > 0) {
                    m += "\n";
                }
                m += p.Value.ToString();
                id++;
            }
            return m;
        }

    }
}
