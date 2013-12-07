using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome;

namespace KinectController
{
    /// <summary>
    /// State 0:
    ///     1. Send an action to "home_controller" "change_mode"("emergency")
    ///     2. Send to "synthese" "say"(emergency_question[1..n]) each 10 seconde 
    ///         If "home_controller" send response "response_emergency_done" to "kinect" Then goto state 2
    ///         If all emergency question asked Then goto state 1
    /// State 1:
    ///     1. Wait 2 skeleton representing emergency people 
    ///         Then Open entry door and switch on light And goto state 2
    /// State 2:
    ///     1. End of scenario, emergency done or cancelled
    /// </summary>
    class Scenario2 : Scenario
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Private: Room representing the main door of the home
        /// </summary>
        private Room _roomEntry;
        /// <summary>
        /// Private: Last tick used to check if 10s elapsed
        /// </summary>
        private int _lastTick;
        /// <summary>
        /// Private: Interval for emergency question
        /// </summary>
        private int _sentenceInterval = 8000;
        /// <summary>
        /// Private: Current question asked
        /// </summary>
        private int _currentEmergencyQuestion;
        /// <summary>
        /// Sentences that home say each 10 secondes until user answer.
        /// </summary>
        static private string[] _emergencyQuestions = 
            {
                "Est-ce que vous allez bien ?",
                "Veuillez répondre, allez vous bien ?",
                "Si vous ne répondez pas je vais appeler les secours, pour vous venir en aide, êtes vous conscient ?"
            };
        /// <summary>
        /// Sentence that home say when emergency have been called
        /// </summary>
        static private string _emergencyCallText = "Les secours ont été prevenus, et devrait arrivé rapidement pour vous secourrir";
        /// <summary>
        /// Sentence that home say when emergency are not needed
        /// </summary>
        static private string _emergencyUndoText = "Merci de votre réponse.";
        /// <summary>
        /// Sentence that home say when emergency arrive
        /// </summary>
        static private string _emergencyArrivedText = "Les secours sont arrivés et vont venir vous secourrir.";
        /// <summary>
        /// Sentence that home say for emergency
        /// </summary>
        static private string _emergencyDestText = "A l'attention des secours, le propriétaire à fait un malaise dans le bureau. Pour vous facilité l'intervention je vais ouvrir toutes les portes et allumer les lumieres.";
        

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize base scenario
        /// </summary>
        /// <param name="h">Home to use</param>
        /// <param name="k">Kinect controller to use</param>
        public Scenario2(Home h, KinectController k)
            : base(h, k, 5) {
            ;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Events
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Intialize this scenario
        /// </summary>
        public override void Init() {
            /* Init room entree */
            _roomEntry = _home.Pieces.Get("entree", true);
            if ((_roomEntry == null) || (!(_roomEntry.ALumiere)) || (!(_roomEntry.APorte))) {
                System.Console.WriteLine("<ERROR: La piece entree n'est pas bien configuré ou inexistante>");
                return;
            }
            System.Console.WriteLine("<0: Initializement Scenario en cours>");
            _home.Actions.SendAction("synthese", "mute");
        }

        /// <summary>
        /// Step in scenario
        /// </summary>
        /// <returns>False if no step remaining: stop the scenario</returns>
        public override bool Step() {
            if (ScenarioState == 1) {
                /* State 0: Send emergency questions each 8 seconde */
                if ((System.Environment.TickCount - _lastTick) > _sentenceInterval) {
                    if (_currentEmergencyQuestion >= _emergencyQuestions.Length) {
                        _home.Actions.SendAction("synthese", "say", _emergencyCallText);
                        /* No user answer: call emergency */
                        System.Console.WriteLine("<1: Secours appeler>");
                        ScenarioState = 3;
                        _kinect.Reset();
                        return true;
                    }
                    /* Step */
                    _home.Actions.SendAction("synthese", "say", _emergencyQuestions[_currentEmergencyQuestion], "kinect", "sc2_step_" + _currentEmergencyQuestion.ToString());
                    ScenarioState = 2;
                }
            }
            return true;
        }

        /// <summary>
        /// Event triggered when an action have been received by home
        /// </summary>
        /// <param name="h">Home concerned</param>
        /// <param name="a">Action received</param>
        public override void OnActionReceived(Home h, HomeAction a) {
            if (a.IsType("muteOK")) {
                /* Initialize */
                _home.Portes.VerrouillerTout();
                _home.Lumieres.EteindreTout();
                /* State 0: Send action "change_mode"("emergency") to "home_controller" */
                _home.Actions.SendAction("home_controller", "change_mode", "emergency");
                _home.Actions.SendAction("synthese", "unmute");
            } else if (a.IsType("unmuteOK")) {
                /* Step in scenario */
                if (ScenarioState == 0) {
                    _lastTick = 0;
                    _currentEmergencyQuestion = 0;
                    _sentenceInterval = 8000;
                    ScenarioState = 1;
                    _kinect.Reset();
                    System.Console.WriteLine("<0: Initializement Scenario terminer>");
                }
            } else if (a.IsType("response_emergency_done")) {
                /* Scenario end */
                if (ScenarioState == 1) {
                    ScenarioState = 5;
                    _home.Actions.SendAction("synthese", "silence");
                    _home.Actions.SendAction("synthese", "say", _emergencyUndoText);
                    System.Console.WriteLine("<1: Secours annuler>");
                    _home.Actions.SendAction("home_controller", "change_mode", "default");
                }
            } else if (a.IsType("sayOK")) {
                if (string.Compare(a.GetParam(0), "sc2_end") == 0) {
                    /* End of scenario */
                    _home.Lumieres.AllumerTout(); /*.Allumer(_roomEntry);*/
                    _home.Portes.DeverrouillerTout();
                    ScenarioState = 5;
                } else if ((ScenarioState == 2) && (string.Compare(a.GetParam(0), "sc2_step_" + _currentEmergencyQuestion.ToString()) == 0)) {
                    /* Question emergency sayed */
                    ScenarioState = 1;
                    _currentEmergencyQuestion++;
                    if (_currentEmergencyQuestion >= _emergencyQuestions.Length) {
                        _sentenceInterval = 12000;
                    }
                    _lastTick = System.Environment.TickCount;
                }
            }
        }

        /// <summary>
        /// Event triggered when the number of skeleton detected changed
        /// </summary>
        /// <param name="newCount">New skeleton detected count</param>
        public override void OnSkeleteCountChanged(int newCount) {
            if (ScenarioState == 3) {
                if (newCount == 2) { 
                    /* 2 skeleton detected: Open the entry door and switch on light */
                    ScenarioState = 4;
                    System.Console.WriteLine("<2: Secours arrivé>");
                    _home.Actions.SendAction("synthese", "silence");
                    _home.Actions.SendAction("synthese", "say", _emergencyArrivedText);
                    _home.Actions.SendAction("synthese", "say", _emergencyDestText, "kinect", "sc2_end");
                    _home.Actions.SendAction("home_controller", "change_mode", "default");
                }
            }
        }

    }
}
