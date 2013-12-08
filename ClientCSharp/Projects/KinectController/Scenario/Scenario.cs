using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome;

namespace KinectController
{
    /// <summary>
    /// Generic scenario
    /// </summary>
    abstract class Scenario
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Home used by this scenario
        /// </summary>
        protected Home _home;
        /// <summary>
        /// Kinect controller sensor used by this scenario
        /// </summary>
        protected KinectController _kinect;
        /// <summary>
        /// Count of state availible in this scenario
        /// </summary>
        public int ScenarioStateCount { get; protected set; }
        /// <summary>
        /// Current Scenario state
        /// </summary>
        public int ScenarioState { get; protected set; }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize generic scenario
        /// </summary>
        /// <param name="h">Home to use</param>
        /// <param name="k">Kinect controller to use</param>
        /// <param name="stateCount">State count of this scenario</param>
        public Scenario(Home h, KinectController k, int stateCount) {
            _home = h;
            _kinect = k;
            ScenarioStateCount = stateCount;
            ScenarioState = 0;
        }

        /// <summary>
        /// Start scenario
        /// </summary>
        public void Start() {
            System.Console.WriteLine("\n<Lancement du scenario: " + ScenarioStateCount.ToString() + " étapes>");
            ScenarioState = 0;
            _home.RegisterEvent(ActionReceived);
            Init();
            while ((ScenarioState >= 0) && (ScenarioState < ScenarioStateCount)) {
                /* Wait until scenario end */
                if (!Step()) break;
                System.Threading.Thread.Sleep(1);
            }
            _home.UnregisterEvent(ActionReceived);
            System.Console.WriteLine("<Fin du scenario>");
            _kinect.CurrentScenario = null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Events
        ///////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Event triggered when the number of skeleton detected changed
        /// </summary>
        /// <param name="newCount">New skeleton detected count</param>
        public void SkeleteCountChanged(int newCount) {
            if (ScenarioState < ScenarioStateCount) {
                OnSkeleteCountChanged(newCount);
            }
        }
        /// <summary>
        /// Event triggered when an action have been received by home
        /// </summary>
        /// <param name="h">Home concerned</param>
        /// <param name="a">Action received</param>
        public void ActionReceived(Home h, HomeAction a) {
            if (ScenarioState < ScenarioStateCount) {
                OnActionReceived(h, a);
            }
        }
        
        /// <summary>
        /// Intialize this scenario
        /// </summary>
        public virtual void Init() {
            /*NOP*/
        }

        /// <summary>
        /// Step in scenario
        /// </summary>
        /// <returns>False if no step remaining: stop the scenario</returns>
        public virtual bool Step() {
            return true;
        }

        /// <summary>
        /// Event triggered when the number of skeleton detected changed
        /// </summary>
        /// <param name="newCount">New skeleton detected count</param>
        public virtual void OnSkeleteCountChanged(int newCount) {
            /*NOP*/
            ;
        }

        
        /// <summary>
        /// Event triggered when an action have been received by home
        /// </summary>
        /// <param name="h">Home concerned</param>
        /// <param name="a">Action received</param>
        public virtual void OnActionReceived(Home h, HomeAction a) {
            /*NOP*/;
        }

    }
}
