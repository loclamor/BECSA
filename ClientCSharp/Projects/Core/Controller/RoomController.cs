using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Home rooms controller
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class RoomController : Controller
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Room availaible from home server
        /// </summary>
        private Dictionary<int, Room> _rooms;


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Contructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="home">Home to control</param>
        public RoomController(Home home)
            : base(home)
        {
            _rooms = new Dictionary<int, Room>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Rooms - set
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create an new room
        /// </summary>
        /// <remarks>To take in account the room creation you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="name">Name of the new room</param>
        /// <param name="haveLight">Indicate if the new room have light</param>
        /// <param name="haveFlap">Indicate if the new room have flap</param>
        /// <param name="haveDoor">Indicate if the new room have door</param>
        /// <returns>Server response</returns>
        public HomeResponse CreateNewRoom(string name, bool haveLight = true, bool haveFlap = true, bool haveDoor = false) {
            /* Init */
            HomeRequest msgGET = new HomeRequest();
            msgGET.Set("controller", "piece")
                .Set("action", "creer");
            HomeRequest msgPOST = new HomeRequest(); 
            msgPOST.Set("piece", name)
                .Set("alumiere", (haveLight ? "true" : "false"))
                .Set("avolet", (haveFlap ? "true" : "false"))
                .Set("aporte", (haveDoor ? "true" : "false"));
            /* Execute */
            HTTPRequest r = new HTTPRequest(_home.HomeURI, msgGET, msgPOST);
            return HomeResponse.Create(new JSON(r.GetResponse()));
        }
        /// <summary>
        /// Remove an room
        /// </summary>
        /// <remarks>To take in account the room deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="room">Room to remove</param>
        /// <returns>Server response</returns>
        public HomeResponse Remove(Room room) {
            if (room != null) {
                return Remove(room.Id);
            } else {
                return null;
            }
        }
        /// <summary>
        /// Remove a room by index
        /// </summary>
        /// <remarks>To take in account the room deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="index">Index of the room to remove</param>
        /// <returns>Server response</returns>
        public HomeResponse RemoveByIndex(int index) {
            return Remove(GetByIndex(index));
        }
        /// <summary>
        /// Remove a room
        /// </summary>
        /// <remarks>To take in account the room deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="name">Name of the room to remove</param>
        /// <param name="ignoreCase">True to ignore case during seeking room</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking room</param>
        /// <returns>Server response</returns>
        public HomeResponse Remove(string name, bool ignoreCase = true, bool ignoreAccent = true) {
            return Remove(Get(name, ignoreCase, ignoreAccent));
        }
        /// <summary>
        /// Remove a room by identifier number
        /// </summary>
        /// <remarks>To take in account the room deletion you need to refresh home (<see cref="Home.Refresh"/>)</remarks>
        /// <param name="identifier">Identier number of the room to remove</param>
        /// <returns>Server response</returns>
        public HomeResponse Remove(int identifier) {
            /* Init */
            HomeRequest msg = new HomeRequest();
            msg.Set("controller", "piece")
                .Set("action", "supprimer")
                .Set("id", identifier.ToString());
            /* Execute */
            HTTPRequest r = new HTTPRequest(_home.HomeURI, msg);
            return HomeResponse.Create(new JSON(r.GetResponse()));
        }



        ///////////////////////////////////////////////////////////////////////////////////////////
        // Rooms - get
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Refresh list of rooms and status of each rooms.
        /// </summary>
        /// <remarks>
        /// Will automatically fired room update and home update event if needed and configured 
        /// (<see cref="SmartHome.Home.RegisterRoomUpdateEvent"/>, <see cref="SmartHome.Home.RegisterHomeUpdateEvent"/>)
        /// </remarks>
        /// <param name="json">JSON to load, containing all rooms datas</param>
        /// <returns>True if refreshed without errors</returns>
        public bool Refresh(JSON json) {
            /* Check if needed */
            if (json == null) return true;
            /* Retrieve all Pieces from JSON response */
            Dictionary<int, Room> newRooms = new Dictionary<int, Room>();
            for (int i = 0, max = json.Count; i < max; i++) {
                JSON jsonRoom = ((json.Type == JSON.ValueType.ARRAY) ? json.Get(i) : json);
                newRooms.Add(jsonRoom.Get("id").GetIntValue(-1), new Room(jsonRoom));
                /* Manage when it's not an array but a simply object */
                if (json.Type == JSON.ValueType.OBJECT) break;
            }
            /* Count how many Piece element have changed */
            List<Room> lightUpdated = new List<Room>();
            List<Room> doorUpdated = new List<Room>();
            List<Room> flapUpdated = new List<Room>();
            List<Room> roomAdded = new List<Room>();
            List<Room> roomRemoved = new List<Room>();
            int haveLightCount = 0;
            int haveDoorCount = 0;
            int haveFlapCount = 0;
            int lightOnCount = 0;
            int doorOpenCount = 0;
            int flapOpenCount = 0;
            foreach (KeyValuePair<int, Room> pair in newRooms) {
                Room p = pair.Value;
                /* Check if there is an update from older Piece */
                Room oldPiece = Get(pair.Key);
                if ((oldPiece != null) && (!(oldPiece.isEgalTo(p)))) {
                    /* Count the light/door/flap change */
                    if (p.LumiereAllumer != oldPiece.LumiereAllumer) lightUpdated.Add(p);
                    if (p.PorteDeverrouiller != oldPiece.PorteDeverrouiller) doorUpdated.Add(p);
                    if (p.VoletOuvert != oldPiece.VoletOuvert) flapUpdated.Add(p);
                }
                if (oldPiece == null) {
                    /* Room added: store in a list all the room added in order to throw event after list of room changed */
                    roomAdded.Add(p);
                }
                /* Count element "on" */
                if (p.ALumiere) haveLightCount++;
                if (p.APorte) haveDoorCount++;
                if (p.AVolet) haveFlapCount++;
                if ((p.ALumiere) && (p.LumiereAllumer)) lightOnCount++;
                if ((p.APorte) && (p.PorteDeverrouiller)) doorOpenCount++;
                if ((p.AVolet) && (p.VoletOuvert)) flapOpenCount++;
            }
            /* Check room removed */
            foreach (KeyValuePair<int, Room> pair in _rooms) {
                if (!(newRooms.ContainsKey(pair.Key))) {
                    /* Room removed: store in a list all the room added in order to throw event after list of room changed */
                    roomRemoved.Add(pair.Value);
                }
            }
            /* Copy new rooms */
            _rooms = newRooms;
            /* Make home event */ 
            if (_home.OnRoomUpdate != null) {
                /* Remove */
                foreach(Room r in roomRemoved) {
                    _home.OnRoomUpdate(_home, r, Home.RoomUpdateKind.RoomRemoved);
                }
                /* Add */
                foreach(Room r in roomAdded) {
                    _home.OnRoomUpdate(_home, r, Home.RoomUpdateKind.NewRoomAdded);
                }
            }
            if ((_home.OnHomeUpdate != null) && ((roomRemoved.Count > 0) || (roomAdded.Count > 0))) {
                /* Room list changed */
                _home.OnHomeUpdate(_home, Home.HomeUpdateKind.RoomListChanged);
            }
            /* Call delegates for light */
            if ((lightUpdated.Count > 1) && (lightOnCount == 0)) {
                /* All light switched to off */
                if (_home.OnHomeUpdate != null) _home.OnHomeUpdate(_home, Home.HomeUpdateKind.SwitchOffAllRoomLight);
            } else if ((lightUpdated.Count > 1) && (lightOnCount == haveLightCount)) {
                /* All light switched to on */
                if (_home.OnHomeUpdate != null) _home.OnHomeUpdate(_home, Home.HomeUpdateKind.SwitchOnAllRoomLight);
            } else if (_home.OnRoomUpdate != null) {
                /* Call delegate for each Piece where light have been updated */
                foreach (Room p in lightUpdated) {
                    _home.OnRoomUpdate(_home, p, (p.LumiereAllumer ? Home.RoomUpdateKind.SwitchOnRoomLight : Home.RoomUpdateKind.SwitchOffRoomLight));
                }
            }
            /* Call delegates for door: same algo */
            if ((doorUpdated.Count > 1) && (doorOpenCount == 0)) {
                if (_home.OnHomeUpdate != null) _home.OnHomeUpdate(_home, Home.HomeUpdateKind.CloseAllDoor);
            } else if ((doorUpdated.Count > 1) && (doorOpenCount == haveDoorCount)) {
                if (_home.OnHomeUpdate != null) _home.OnHomeUpdate(_home, Home.HomeUpdateKind.OpenAllDoor);
            } else if (_home.OnRoomUpdate != null) {
                foreach (Room p in doorUpdated) {
                    _home.OnRoomUpdate(_home, p, (p.PorteDeverrouiller ? Home.RoomUpdateKind.OpenRoomDoor : Home.RoomUpdateKind.CloseRoomDoor));
                }
            }
            /* Call delegates for flap: same algo */
            if ((flapUpdated.Count > 1) && (flapOpenCount == 0)) {
                if (_home.OnHomeUpdate != null) _home.OnHomeUpdate(_home, Home.HomeUpdateKind.CloseAllFlap);
            } else if ((flapUpdated.Count > 1) && (flapOpenCount == haveFlapCount)) {
                if (_home.OnHomeUpdate != null) _home.OnHomeUpdate(_home, Home.HomeUpdateKind.OpenAllFlap);
            } else if (_home.OnRoomUpdate != null) {
                foreach (Room p in flapUpdated) {
                    _home.OnRoomUpdate(_home, p, (p.VoletOuvert ? Home.RoomUpdateKind.OpenRoomFlap : Home.RoomUpdateKind.CloseRoomFlap));
                }
            }
            /* Done: return true */
            return true;
        }

        /// <summary>
        /// Get the number of rooms
        /// </summary>
        /// <returns>Room count</returns>
        public int Count {
            get {
                return _rooms.Count;
            }
        }
        /// <summary>
        /// Get a room by identifier
        /// </summary>
        /// <param name="id">Identifier of the desired room</param>
        /// <returns>Room found Or null if no room found</returns>
        public Room Get(int id) {
            if (_rooms.ContainsKey(id)) {
                return _rooms[id];
            } else {
                return null;
            }
        }
        /// <summary>
        /// Get a room by name
        /// </summary>
        /// <param name="name">Name of the desired room</param>
        /// <param name="ignoreCase">True to ignore case during seeking room</param>
        /// <param name="ignoreAccent">True to ignore accent during seeking room</param>
        /// <returns>Room found Or null if no room found</returns>
        public Room Get(string name, bool ignoreCase = true, bool ignoreAccent = true) {
            foreach (KeyValuePair<int, Room> p in _rooms) {
                if (StringUtils.IdenticalString(p.Value.Nom, name, ignoreCase, ignoreAccent)) {
                    return p.Value;
                }
            }
            return null;
        }
        /// <summary>
        /// Get a room by index
        /// </summary>
        /// <param name="index">Index of the desired room</param>
        /// <returns>Room found Or null if no room found</returns>
        public Room GetByIndex(int index) {
            if ((index >= 0) && (index < _rooms.Count)) {
                foreach (KeyValuePair<int, Room> p in _rooms) {
                    if (index == 0) {
                        return p.Value;
                    }
                    index--;
                }
            }
            return null;
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
            foreach (KeyValuePair<int, Room> p in _rooms) {
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
