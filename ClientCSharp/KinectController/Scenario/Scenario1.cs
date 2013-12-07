using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome;

namespace KinectController
{
    /// <summary>
    /// Switch light state follow skeleton presence and an list of room to cross 
    ///     Each time a skeletton is detected Then 
    ///         Switch on light of room
    ///     When skeletton leave Then
    ///         Switch off light of room
    ///         And step to next room of the room list
    /// </summary>
    class Scenario1 : Scenario
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// List of room to cross
        /// </summary>
        private List<Room> _roomsToCross;
        /// <summary>
        /// Current room crossed
        /// </summary>
        private int _currentRoomIndex;
        static private string[] _roomNamesToCross = 
            {
                "chambre",
                "salon",
                "bibliotheque",
                "couloir de la cave",
                "cave",
                "salle a manger",
                "arriere cuisine",
                "cuisine"
            };

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize base scenario
        /// </summary>
        /// <param name="h">Home to use</param>
        /// <param name="k">Kinect controller to use</param>
        public Scenario1(Home h, KinectController k)
            : base(h, k, 2) 
        {
            _roomsToCross = new List<Room>();
            _currentRoomIndex = 0;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Events
        ///////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Intialize this scenario
        /// </summary>
        public override void Init() {
            /* Init room to travel */
            _roomsToCross.Clear();
            foreach (string s in _roomNamesToCross) {
                Room r = _home.Pieces.Get(s);
                if ((r != null) && (r.ALumiere)) {
                    _roomsToCross.Add(r);
                }
            }
            _currentRoomIndex = 0;
            /* Init Room state */        
            System.Console.WriteLine("<0: Initializement Scenario en cours>");
            _home.Actions.SendAction("synthese", "mute");
        }

        /// <summary>
        /// Event triggered when an action have been received by home
        /// </summary>
        /// <param name="h">Home concerned</param>
        /// <param name="a">Action received</param>
        public override void OnActionReceived(Home h, HomeAction a) {
            if (a.IsType("muteOK")) {
                /* Init Room state follow time */
                _home.Lumieres.AllumerTout();
                foreach (Room r in _roomsToCross) {
                    _home.Lumieres.Eteindre(r);
                }
                _home.Actions.SendAction("synthese", "unmute");
            } else if (a.IsType("unmuteOK")) {
                /* Step in scenario */
                if (ScenarioState == 0) {
                    System.Console.WriteLine("<0: Initializement Scenario terminer>");
                    ScenarioState = 1;
                    _kinect.Reset();
                }
            }
        }


        /// <summary>
        /// Event triggered when the number of skeleton detected changed
        /// </summary>
        /// <param name="newCount">New skeleton detected count</param>
        public override void OnSkeleteCountChanged(int newCount) {
            if (ScenarioState > 0) {
                if (newCount >= 1) {
                    /* Room enter */
                    if (_currentRoomIndex < _roomsToCross.Count) {
                        _home.Lumieres.Allumer(_roomsToCross[_currentRoomIndex]);
                        System.Console.WriteLine("<0: Entre dans la piece \'" + _roomsToCross[_currentRoomIndex].Nom + "\'");
                    }
                } else {
                    /* Room Leave */
                    if (_currentRoomIndex < _roomsToCross.Count) {
                        _home.Lumieres.Eteindre(_roomsToCross[_currentRoomIndex]);
                        System.Console.WriteLine("<0: Sort de la piece \'" + _roomsToCross[_currentRoomIndex].Nom + "\'");
                    }
                    /* Check if all room travelled */
                    _currentRoomIndex++;
                    if (_currentRoomIndex >= _roomsToCross.Count) {
                        /* End of scenario */ 
                        ScenarioState = 2;
                    }

                }
            }
        }

    }
}
