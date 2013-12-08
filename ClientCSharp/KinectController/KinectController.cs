using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.IO;
using System.Threading;


namespace KinectController
{
    /// <summary>
    /// Kinect controller
    /// </summary>
    class KinectController
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Kinect sensor
        /// </summary>
        private KinectSensor sensor;
        /// <summary>
        /// Skeletons detected
        /// </summary>
        private Skeleton[] skeletons;
        /// <summary>
        /// Count of skeletons detected
        /// </summary>
        public int SkeleteDetectedCount { get; protected set; }
        /// <summary>
        /// Indicate if the kinect controller is still running
        /// </summary>
        private bool _running;
        /// <summary>
        /// Thread to simulated kinect action
        /// </summary>
        private Thread _thread;
        /// <summary>
        /// Private: Current scenario
        /// </summary>
        private Scenario _currentScenario;


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Property
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Current scenario
        /// </summary>
        public Scenario CurrentScenario { 
            get {
                return _currentScenario;
            }
            set {
                SkeleteDetectedCount = 0;
                _currentScenario = value;
                if (_currentScenario != null) {
                    _currentScenario.Start();
                }
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize 
        /// </summary>
        public KinectController() {
            SkeleteDetectedCount = 0;
            _running = true;
            try {
                sensor = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);
                if (sensor == null) {
                } else {
                    sensor.SkeletonStream.Enable();
                    sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
                    sensor.Start();
                }
            } catch (IOException) {
                sensor = null;
            }
            if (sensor == null) {
                System.Console.WriteLine(">> WARNING: Ce client nécessite Kinect pour fonctionner correctement.");
                System.Console.WriteLine(">> Tapez 'e' (enter) pour simuler l'entré d'une personne.");
                System.Console.WriteLine(">> Tapez 'l' (leave) pour simuler la sortie d'une personne.");
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Control
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Start the kinect controller
        /// </summary>
        /// <returns>True if start initialized</returns>
        public bool Start() {
            SkeleteDetectedCount = 0;
            if (sensor != null) {
                try {
                    sensor = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);
                    sensor.SkeletonStream.Enable();
                    sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
                    sensor.Start();
                    return true;
                } catch (IOException) {
                    sensor = null;
                    return false;
                }
            } else {
                _running = true;
                _thread = new Thread(KinectSimulatorThread);
                _thread.Start();
                return true;
            }
        }

        /// <summary>
        /// Reset skeletons count
        /// </summary>
        public void Reset() {
            SkeleteDetectedCount = -1;
        }

        /// <summary>
        /// Stop the kinect controller
        /// </summary>
        public void Stop() {
            if (sensor != null) {
                try {
                    sensor.Stop();
                } catch (IOException) {
                    ;
                }
            }
            _running = false;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Kinect
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Detect skeleton
        /// </summary>
        /// <param name="sender">Object that send the event</param>
        /// <param name="e">Event data</param>
        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e) {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) {
                if (skeletonFrame != null) {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }
            int skltCnt = 0;
            if (skeletons != null) {
                foreach (Skeleton s in skeletons) {
                    if (s.TrackingState == SkeletonTrackingState.Tracked) {
                        skltCnt++;
                    }
                }
            }
            if (SkeleteDetectedCount != skltCnt) {
                SkeleteDetectedCount = skltCnt;
                if (CurrentScenario != null) {
                    CurrentScenario.OnSkeleteCountChanged(SkeleteDetectedCount);
                }
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Kinect simulation
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Used to replace Kinect sensor.
        /// </summary>
        private void KinectSimulatorThread() {
            while (_running) {
                ConsoleKeyInfo key = System.Console.ReadKey();
                if ((key.KeyChar == 'e') || (key.KeyChar == 'E')) {
                    /* Enter */
                    if (SkeleteDetectedCount < 0) SkeleteDetectedCount = 0;
                    SkeleteDetectedCount++;
                    CurrentScenario.OnSkeleteCountChanged(SkeleteDetectedCount);
                } else if ((key.KeyChar == 'l') || (key.KeyChar == 'L')) {
                    /* Leave */
                    if (SkeleteDetectedCount > 0) {
                        SkeleteDetectedCount = SkeleteDetectedCount - 1;
                        CurrentScenario.OnSkeleteCountChanged(SkeleteDetectedCount);
                    }
                }
            }
        }
    


    }
}
