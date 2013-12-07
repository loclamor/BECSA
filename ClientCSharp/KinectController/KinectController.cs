using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.IO;

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
            try {
                sensor = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);
                if (sensor == null) {
                    System.Console.WriteLine(">> ERREUR: Ce client nécessite Kinect pour fonctionner, aucune kinect détecter.");
                } else {
                    sensor.SkeletonStream.Enable();
                    sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
                    sensor.Start();
                }
            } catch (IOException) {
                sensor = null;
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
                return false;
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

    


    }
}
