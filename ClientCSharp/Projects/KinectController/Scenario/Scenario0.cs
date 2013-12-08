using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome;

namespace KinectController
{
    /// <summary>
    /// Wait a skeletton Then 
    ///     If day Then 
    ///         Switch off Light
    ///         Open flap
    ///     If night Then
    ///         Switch on Light
    ///         Close flap
    /// When skeletton Leave
    ///     Switch off light
    ///     Close flap
    ///     Close door
    /// </summary>
    class Scenario0 : Scenario
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Room used in this scenario
        /// </summary>
        private Room _room;


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize base scenario
        /// </summary>
        /// <param name="h">Home to use</param>
        /// <param name="k">Kinect controller to use</param>
        public Scenario0(Home h, KinectController k)
            : base(h, k, 5) 
        {
            
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Events
        ///////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Intialize this scenario
        /// </summary>
        public override void Init() {
            _room = _home.Pieces.Get("bureau");
            if ((_room == null) || (!(_room.ALumiere)) || (!(_room.APorte)) || (!(_room.AVolet))) {
                System.Console.WriteLine("<ERROR: La piece bureau n'est pas bien configuré ou inexistante>");
                return;
            }
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
                _home.Portes.Deverrouiller(_room);
                if ((DateTime.Now.Hour >= 8) && (DateTime.Now.Hour <= 20)) {
                    _home.Volets.Fermer(_room);
                    _home.Lumieres.Allumer(_room);
                } else {
                    _home.Volets.Ouvrir(_room);
                    _home.Lumieres.Eteindre(_room);
                }
                _home.Actions.SendAction("synthese", "unmute");
            } else if (a.IsType("unmuteOK")) {
                /* Step in scenario */
                if (ScenarioState == 0) {
                    ScenarioState = 1;
                    _kinect.Reset();
                    System.Console.WriteLine("<0: Initializement Scenario terminer>");
                }
            } else if (a.IsType("sayOK")) {
                if (string.Compare(a.GetParam(0), "sc0_step1") == 0) {
                    /* Presence detected */
                    if ((DateTime.Now.Hour >= 8) && (DateTime.Now.Hour <= 20)) {
                        /* Day: Open Flap & Switch off light */
                        _home.Volets.Ouvrir(_room);
                        _home.Lumieres.Eteindre(_room);
                    } else {
                        /* Night: Switch on light & Close Flap */
                        _home.Volets.Fermer(_room);
                        _home.Lumieres.Allumer(_room);
                    }
                    ScenarioState = 3;
                } else if (string.Compare(a.GetParam(0), "sc0_step2") == 0) {
                    /* Presence detected leave: Close doors, flaps, lights */
                    _home.Volets.Fermer(_room);
                    _home.Portes.Verrouiller(_room);
                    _home.Lumieres.Eteindre(_room);
                    ScenarioState = 5;
                }
            }
        }

        /// <summary>
        /// Event triggered when the number of skeleton detected changed
        /// </summary>
        /// <param name="newCount">New skeleton detected count</param>
        public override void OnSkeleteCountChanged(int newCount) {
            if ((ScenarioState == 1) && (newCount >= 1)) {
                /* Presence detected */
                System.Console.WriteLine("<1: Présence détecter>");
                if ((DateTime.Now.Hour >= 8) && (DateTime.Now.Hour <= 20)) {
                    /* Day: Open Flap & Switch off light */
                    _home.Actions.SendAction("synthese", "say", "Bienvenue dans le bureau, puisqu'il fait jour je vais ouvrir les volets et eteindre les lumieres.", "kinect", "sc0_step1");
                } else {
                    /* Night: Switch on light & Close Flap */
                    _home.Actions.SendAction("synthese", "say", "Bienvenue dans le bureau, puisqu'il fait nuit je vais fermer les volets et allumer les lumieres.", "kinect", "sc0_step1");
                }
                ScenarioState = 2;
            } else if ((ScenarioState == 3) && (newCount == 0)) {
                /* Presence detected leave: Close doors, flaps, lights */
                System.Console.WriteLine("<2: Plus de présence détecter>");
                _home.Actions.SendAction("synthese", "say", "Vous êtes sortie du bureau je vais donc éteindre les lumieres et fermer la porte.", "kinect", "sc0_step2");
                ScenarioState = 4;
            }
        }

    }
}
