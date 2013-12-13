using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome
{
    /// <summary>
    /// Home main controller which interact directly with the Home server
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class Home
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Enums
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Room events handleable
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
            CloseRoomFlap,
            /// <summary>
            /// Indicate that a new room have been setted
            /// </summary>
            /// <remarks>
            ///     This event is triggered always after 
            ///     <see cref="SmartHome.Home.RoomUpdateKind.RoomRemoved">RoomRemoved</see> 
            ///     and before
            ///     <see cref="SmartHome.Home.HomeUpdateKind.RoomListChanged">RoomListChanged</see>
            /// </remarks>
            NewRoomAdded,
            /// <summary>
            /// Indicate that an room have been removed
            /// </summary>
            /// <remarks>
            ///     This event is triggered always before 
            ///     <see cref="SmartHome.Home.RoomUpdateKind.NewRoomAdded">NewRoomAdded</see> and 
            ///     <see cref="SmartHome.Home.HomeUpdateKind.RoomListChanged">RoomListChanged</see>
            /// </remarks>
            RoomRemoved
        }
        /// <summary>
        /// Home events handleable
        /// </summary>
        public enum HomeUpdateKind
        {
            /// <summary>
            /// Update begin
            /// </summary>
            BeginUpdate,
            /// <summary>
            /// End of update
            /// </summary>
            EndUpdate,
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
            /// <remarks>
            ///     This event is triggered always after 
            ///     <see cref="SmartHome.Home.RoomUpdateKind.NewRoomAdded">NewRoomAdded</see> and 
            ///     <see cref="SmartHome.Home.RoomUpdateKind.RoomRemoved">RoomRemoved</see>
            /// </remarks>
            RoomListChanged,
            /// <summary>
            /// Indicate that some alarm clock(s) have been added or removed
            /// </summary>
            /// <remarks>
            ///     This event is triggered always after 
            ///     <see cref="SmartHome.Home.AlarmClockUpdateKind.NewAlarmClockSet">NewAlarmClockSet</see> and 
            ///     <see cref="SmartHome.Home.AlarmClockUpdateKind.AlarmClockRemoved">AlarmClockRemoved</see>
            /// </remarks>
            AlarmClockListChanged,
            /// <summary>
            /// Indicate that some song(s) have been added or removed
            /// </summary>
            /// <remarks>
            ///     This event is triggered always after 
            ///     <see cref="SmartHome.Home.HifiUpdateKind.NewSongAdded">NewSongAdded</see> and 
            ///     <see cref="SmartHome.Home.HifiUpdateKind.SongRemoved">SongRemoved</see>
            /// </remarks>
            SongListChanged
        }
        /// <summary>
        /// Alarm clock events handleable
        /// </summary>
        public enum AlarmClockUpdateKind
        {
            /// <summary>
            /// Indicate that an alarm clock ringing
            /// </summary>
            AlarmClockRing,
            /// <summary>
            /// Indicate that an alarm clock have been updated
            /// </summary>
            /// <remarks>On the event AlarmClock parameter is the old alarm clock. You can compare change by get new one by it's id, since event are fired after update done</remarks>
            AlarmClockUpdated,
            /// <summary>
            /// Indicate that a new alarm clock have been setted
            /// </summary>
            /// <remarks>
            ///     This event is triggered always after 
            ///     <see cref="SmartHome.Home.AlarmClockUpdateKind.AlarmClockRemoved">AlarmClockRemoved</see> 
            ///     and before 
            ///     <see cref="SmartHome.Home.HomeUpdateKind.AlarmClockListChanged">AlarmClockListChanged</see>
            /// </remarks>
            NewAlarmClockSet,
            /// <summary>
            /// Indicate that an alarm clock have been removed
            /// </summary>
            /// <remarks>
            ///     This event is triggered always before 
            ///     <see cref="SmartHome.Home.AlarmClockUpdateKind.NewAlarmClockSet">NewAlarmClockSet</see> and 
            ///     <see cref="SmartHome.Home.HomeUpdateKind.AlarmClockListChanged">AlarmClockListChanged</see>
            /// </remarks>
            AlarmClockRemoved
        }
        /// <summary>
        /// Hifi events handleable
        /// </summary>
        public enum HifiUpdateKind
        {
            /// <summary>
            /// Indicate that a song have been updated
            /// </summary>
            /// <remarks>On the event Song parameter is the old song. You can compare change by get new one by it's id, since event are fired after update done</remarks>
            SongUpdated,
            /// <summary>
            /// Indicate that a new song is available
            /// </summary>
            /// <remarks>
            ///     This event is always triggered after 
            ///     <see cref="SmartHome.Home.HifiUpdateKind.SongRemoved">SongRemoved</see> 
            ///     and before 
            ///     <see cref="SmartHome.Home.HomeUpdateKind.SongListChanged">SongListChanged</see>
            /// </remarks>
            NewSongAdded,
            /// <summary>
            /// Indicate that a song have been removed
            /// </summary>
            /// <remarks>
            ///     This event is always triggered before 
            ///     <see cref="SmartHome.Home.HifiUpdateKind.NewSongAdded">NewSongAdded</see> and 
            ///     <see cref="SmartHome.Home.HomeUpdateKind.SongListChanged">SongListChanged</see>
            /// </remarks>
            SongRemoved
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Events
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Delegate able to treat a room event
        /// </summary>
        /// <param name="h">Home</param>
        /// <param name="r">Room updated</param>
        /// <param name="updateKind">Update kind</param>
        public delegate void RoomUpdate(Home h, Room r, RoomUpdateKind updateKind);
        /// <summary>
        /// Delegate able to treat an home event
        /// </summary>
        /// <param name="h">Home</param>
        /// <param name="updateKind">Update kind</param>
        public delegate void HomeUpdate(Home h, HomeUpdateKind updateKind);
        /// <summary>
        /// Delegate able to treat an alarm clock event
        /// </summary>
        /// <param name="h">Home</param>
        /// <param name="a">Alarm clock updated</param>
        /// <param name="updateKind">Update kind</param>
        public delegate void AlarmClockUpdate(Home h, AlarmClock a, AlarmClockUpdateKind updateKind);
        /// <summary>
        /// Delegate able to treat an hifi event
        /// </summary>
        /// <param name="h">Home</param>
        /// <param name="s">Song updated</param>
        /// <param name="updateKind">Update kind</param>
        public delegate void HifiUpdate(Home h, Song s, HifiUpdateKind updateKind);
        /// <summary>
        /// Delegate able to treat an action received
        /// </summary>
        /// <param name="h">Home</param>
        /// <param name="a">Action received</param>
        public delegate void ActionReceived(Home h, HomeAction a);


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        // Global
        /// <summary>
        /// Home server URI
        /// </summary>
        public string HomeURI { get; set; }
        /// <summary>
        /// Identifier used by home to receive/send message
        /// </summary>
        public string HomeIdentifier { get; set; }
        /// <summary>
        /// Frequency of refresh in milliseconds
        /// </summary>
        public int RefreshFrequency { get; set; }

        // Events
        /// <summary>
        /// Event called when a room have been updated
        /// </summary>
        public RoomUpdate OnRoomUpdate { get; protected set;} 
        /// <summary>
        /// Event called when home have been updated
        /// </summary>
        public HomeUpdate OnHomeUpdate { get; protected set; } 
        /// <summary>
        /// Event called when an alarm clock is added/updated/removed
        /// </summary>
        public AlarmClockUpdate OnAlarmClockUpdate { get; protected set; }
        /// <summary>
        /// Event called when an song is added/updated/removed
        /// </summary>
        public HifiUpdate OnHifiUpdate { get; protected set; } 
        /// <summary>
        /// Event called when an action have been received
        /// </summary>
        public ActionReceived OnActionReceived { get; protected set;} 

        // Private members
        /// <summary>
        /// Last System.Environment.TickCount when a refresh was done
        /// </summary>
        private int _lastUpdateTick;
        /// <summary>
        /// Basic home controller
        /// </summary>
        private Controller _basicController;

        // Controllers
        /// <summary>
        /// Room controller
        /// </summary>
        public RoomController Pieces { get; protected set; }
        /// <summary>
        /// Light controller
        /// </summary>
        public LightController Lumieres { get; protected set; }
        /// <summary>
        /// Door controller
        /// </summary>
        public DoorController Portes { get; protected set; }
        /// <summary>
        /// Flap controller
        /// </summary>
        public FlapController Volets { get; protected set; }
        /// <summary>
        /// Alarm clock controller
        /// </summary>
        public AlarmClockController Reveils { get; protected set; }
        /// <summary>
        /// Hifi controller
        /// </summary>
        public HifiController Hifi { get; protected set; }
        /// <summary>
        /// Action controller
        /// </summary>
        public ActionController Actions { get; protected set; }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize an empty Home. HomeURI need to be set after before using refresh.
        /// </summary>
        public Home() {
            /* Global */ 
            HomeURI = "";
            HomeIdentifier = "";
            RefreshFrequency = 0;
            /* Private members */ 
            _lastUpdateTick = 0;
            _basicController = new Controller(this);
            /* Controllers */
            Pieces = new RoomController(this);
            Lumieres = new LightController(this);
            Portes = new DoorController(this);
            Volets = new FlapController(this);
            Reveils = new AlarmClockController(this);
            Hifi = new HifiController(this);
            Actions = new ActionController(this);
        }
        /// <summary>
        /// Initialize Home with the location of the Home server set
        /// </summary>
        /// <param name="uri">URI of the Home server</param>
        /// <param name="identifier">Identifier used by home server. Can be empty.</param>
        /// <param name="refreshFrequency">Frequency of refresh in milliseconds (set to 0 to have direct refresh)</param>
        public Home(string uri, string identifier = "", int refreshFrequency = 1000) 
            : this()
        {
            HomeURI = uri;
            HomeIdentifier = identifier;
            RefreshFrequency = refreshFrequency;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Refresh
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Synchronous refreshing of home state (<see cref="RoomController.Refresh"/>, <see cref="AlarmClockController.Refresh"/>, <see cref="HifiController.Refresh"/>, <see cref="ActionController.Refresh"/>)
        /// </summary>
        /// <remarks>
        /// Will automatically fire all the registered events.
        /// Will update also Hifi datas however if <see cref="Home.HomeIdentifier">HomeIdentifier</see> is set the home server response returned will be only the response of the request "getState" on "maison" (cf. doc serveur web for more details)
        /// Nonetheless if Home identifier is not set then the response returned will be the response of "lister" on "pieces" controller (cf. doc serveur web)
        /// </remarks>
        /// <returns>Response of the Home server</returns>
        public HomeResponse Refresh() {
            /* Check if time to refresh */
            if (RefreshFrequency > 0) {
                if ((System.Environment.TickCount - _lastUpdateTick) < RefreshFrequency) {
                    return null;
                }
            }
            /* Get refresh data from home */
            HomeResponse returnRes;
            JSON roomJSONRes = null;
            JSON alarmClockJSONRes = null;
            JSON actionJSONRes = null;
            JSON hifiJSONRes = null;
            HomeRequest msg = new HomeRequest();
            HTTPRequest request;
            if (HomeIdentifier.Length > 0) { 
                /* Get Home state from server by "maison" controller */
                msg.Set("controller", "maison").Set("action", "getState").Set("dest", HomeIdentifier).Set("hifi");
                request = new HTTPRequest(HomeURI, msg);
                HomeResponse homeStateRes = HomeResponse.Create(new JSON(request.GetResponse()));
                JSON stateJSON = homeStateRes.Data.Get("state");
                roomJSONRes = stateJSON.Get("pieces");
                alarmClockJSONRes = stateJSON.Get("reveils");
                actionJSONRes = stateJSON.Get("actions");
                /* Check if hifi was retrieved */
                if (stateJSON.Contains("songs")) {
                    hifiJSONRes = stateJSON.Get("songs");
                } else {
                    /* Get Hifi state from server */
                    msg.Clear().Set("controller", "hifi").Set("action", "lister");
                    request = new HTTPRequest(HomeURI, msg);
                    HomeResponse hifiRes = HomeResponse.Create(new JSON(request.GetResponse()));
                    hifiJSONRes = hifiRes.Data.Get("songs");
                }
                /* Set return response */ 
                returnRes = homeStateRes;
            } else {
                /* Get Room state from server */
                msg.Set("controller", "pieces").Set("action", "lister");
                request = new HTTPRequest(HomeURI, msg);
                HomeResponse roomRes = HomeResponse.Create(new JSON(request.GetResponse()));
                roomJSONRes = roomRes.Data.Get("pieces");
                /* Get Alarm clocks state from server */
                msg.Set("controller", "reveil").Set("action", "lister");
                request = new HTTPRequest(HomeURI, msg);
                HomeResponse alarmClockRes = HomeResponse.Create(new JSON(request.GetResponse()));
                alarmClockJSONRes = alarmClockRes.Data.Get("reveils");
                /* Set return response */
                returnRes = roomRes;
            }
            /* Begin refresh */
            if (OnHomeUpdate != null) {
                OnHomeUpdate(this, HomeUpdateKind.BeginUpdate);
            }
            /* Refresh rooms */
            Pieces.Refresh(roomJSONRes);
            /* Refresh alarm clocks */
            Reveils.Refresh(alarmClockJSONRes);
            /* Refresh hifi */
            Hifi.Refresh(hifiJSONRes);
            /* Refresh actions */
            Actions.Refresh(actionJSONRes);
            /* End refresh */
            if (OnHomeUpdate != null) {
                OnHomeUpdate(this, HomeUpdateKind.EndUpdate);
            }
            /* Refresh update tick */
            if (RefreshFrequency > 0) {
                _lastUpdateTick = System.Environment.TickCount;
            }
            /* Return server response */
            return returnRes;
        }



        ///////////////////////////////////////////////////////////////////////////////////////////
        // Controller functions
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute an action to a controller of the home
        /// </summary>
        /// <remarks>For an expansive details of controller/action availaible cf. doc serveur web.pdf</remarks>
        /// <param name="controller">Controller desired</param>
        /// <param name="action">Action desired</param>
        /// <returns>Home server response</returns>
        public HomeResponse ExecuteAction(string controller, string action) {
            return _basicController.ExecuteAction(controller, action);
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
            return _basicController.ExecuteAction(controller, action);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Event
        ///////////////////////////////////////////////////////////////////////////////////////////

        // Register

        /// <summary>
        /// Register an update event for home
        /// </summary>
        /// <remarks>Multiple functions can be registered. When the event is triggered, all registered functions are called in order they were added.</remarks>
        /// <param name="update">Update function called when home have been update</param>
        public void RegisterEvent(HomeUpdate update) {
            RegisterHomeUpdateEvent(update);
        }
        /// <summary>
        /// Register an update event for room
        /// </summary>
        /// <remarks>Multiple functions can be registered. When the event is triggered, all registered functions are called in order they were added.</remarks>
        /// <param name="update">Update function called when a room have been update</param>
        public void RegisterEvent(RoomUpdate update) {
            RegisterRoomUpdateEvent(update);
        }
        /// <summary>
        /// Register an update event for alarm clock
        /// </summary>
        /// <remarks>Multiple functions can be registered. When the event is triggered, all registered functions are called in order they were added.</remarks>
        /// <param name="update">Update function called when an alarm clock is added/updated/removed</param>
        public void RegisterEvent(AlarmClockUpdate update) {
            RegisterAlarmClockUpdateEvent(update);
        }
        /// <summary>
        /// Register an update event for hifi
        /// </summary>
        /// <remarks>Multiple functions can be registered. When the event is triggered, all registered functions are called in order they were added.</remarks>
        /// <param name="update">Update function called when an song is added/updated/removed</param>
        public void RegisterEvent(HifiUpdate update) {
            RegisterHifiUpdateEvent(update);
        }
        /// <summary>
        /// Register an function called when Home receive an Action
        /// </summary>
        /// <remarks>Multiple functions can be registered. When the home receive an action, all registered functions are called in order they were added.</remarks>
        /// <param name="fun">Function to call when Home receive an Action</param>
        public void RegisterEvent(ActionReceived fun) {
            RegisterActionReceivedEvent(fun);
        }
        /// <summary>
        /// Register an update event for home
        /// </summary>
        /// <remarks>Multiple functions can be registered. When the event is triggered, all registered functions are called in order they were added.</remarks>
        /// <param name="update">Update function called when home have been update</param>
        public void RegisterHomeUpdateEvent(HomeUpdate update) {
            if (OnHomeUpdate == null) {
                OnHomeUpdate = new HomeUpdate(update);
            } else {
                OnHomeUpdate += update;
            }
        }
        /// <summary>
        /// Register an update event for room
        /// </summary>
        /// <remarks>Multiple functions can be registered. When the event is triggered, all registered functions are called in order they were added.</remarks>
        /// <param name="update">Update function called when a room have been update</param>
        public void RegisterRoomUpdateEvent(RoomUpdate update) {
            if (OnRoomUpdate == null) {
                OnRoomUpdate = new RoomUpdate(update);
            } else {
                OnRoomUpdate += update;
            }
        }
        /// <summary>
        /// Register an update event for alarm clock
        /// </summary>
        /// <remarks>Multiple functions can be registered. When the event is triggered, all registered functions are called in order they were added.</remarks>
        /// <param name="update">Update function called when an alarm clock is added/updated/removed</param>
        public void RegisterAlarmClockUpdateEvent(AlarmClockUpdate update) {
            if (OnAlarmClockUpdate == null) {
                OnAlarmClockUpdate = new AlarmClockUpdate(update);
            } else {
                OnAlarmClockUpdate += update;
            }
        }
        /// <summary>
        /// Register an update event for hifi
        /// </summary>
        /// <remarks>Multiple functions can be registered. When the event is triggered, all registered functions are called in order they were added.</remarks>
        /// <param name="update">Update function called when an song is added/updated/removed</param>
        public void RegisterHifiUpdateEvent(HifiUpdate update) {
            if (OnHifiUpdate == null) {
                OnHifiUpdate = new HifiUpdate(update);
            } else {
                OnHifiUpdate += update;
            }
        }
        /// <summary>
        /// Register an function called when Home receive an Action
        /// </summary>
        /// <remarks>Multiple functions can be registered. When the home receive an action, all registered functions are called in order they were added.</remarks>
        /// <param name="fun">Function to call when Home receive an Action</param>
        public void RegisterActionReceivedEvent(ActionReceived fun) {
            if (OnActionReceived == null) {
                OnActionReceived = new ActionReceived(fun);
            } else {
                OnActionReceived += fun;
            }
        }



        // Unregister

        /// <summary>
        /// Unregister an update event for home
        /// </summary>
        /// <param name="update">Update function to unregister</param>
        public void UnregisterEvent(HomeUpdate update) {
            if (OnHomeUpdate != null) {
                OnHomeUpdate -= update;
            }
        }
        /// <summary>
        /// Unregister an update event for room
        /// </summary>
        /// <param name="update">Update function to unregister</param>
        public void UnregisterEvent(RoomUpdate update) {
            if (OnRoomUpdate != null) {
                OnRoomUpdate -= update;
            }
        }
        /// <summary>
        /// Unregister an update event for home
        /// </summary>
        /// <param name="update">Update function to unregister</param>
        public void UnregisterEvent(AlarmClockUpdate update) {
            if (OnAlarmClockUpdate != null) {
                OnAlarmClockUpdate -= update;
            }
        }
        /// <summary>
        /// Unregister an update event for home
        /// </summary>
        /// <param name="update">Update function to unregister</param>
        public void UnregisterEvent(HifiUpdate update) {
            if (OnHifiUpdate != null) {
                OnHifiUpdate -= update;
            }
        }
        /// <summary>
        /// Unregister an event for home
        /// </summary>
        /// <param name="fun">Update function to unregister</param>
        public void UnregisterEvent(ActionReceived fun) {
            if (OnActionReceived != null) {
                OnActionReceived -= fun;
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////
        // ToString 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert home into human-readable string
        /// </summary>
        /// <returns>String representation of this home</returns>
        override public string ToString() {
            return Pieces.ToString() + "\n" + Reveils.ToString() + "\n" + Hifi.ToString();
        }

    }
}
