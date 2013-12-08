using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SmartHome;
using Microsoft.Kinect;
using System.IO;

namespace KinectController
{

    /// <summary>
    /// C# client of SmartHome, provide an example of home implementation.
    /// </summary>
    class Program
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

#if TEST_LOCAL
        /// <summary>
        /// Server location
        /// </summary>
        const string SERVER_URI = "http://127.0.0.1:80/csa/server/server.php";
#else
        /// <summary>
        /// Server location
        /// </summary>
        const string SERVER_URI = "http://becsa.mondophoto.fr/server/";
#endif
        /// <summary>
        /// Identifier used by server
        /// </summary>
        const string IDENTIFIER = "kinect";
        /// <summary>
        /// Home refresh frequency
        /// </summary>
        const int SERVER_REFRESH_FREQUENCY = 500;
        /// <summary>
        /// Home controller/viewer
        /// </summary>
        static private Home home;
        /// <summary>
        /// Indicate if the application is still running
        /// </summary>
        static private bool running;
        /// <summary>
        /// Kinect controller
        /// </summary>
        static private KinectController kinect;
        /// <summary>
        /// Scenario 0
        /// </summary>
        static private Scenario0 scenario0;
        /// <summary>
        /// Scenario 1
        /// </summary>
        static private Scenario1 scenario1;
        /// <summary>
        /// Scenario 2
        /// </summary>
        static private Scenario2 scenario2;
        

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Home
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Home loop
        /// </summary>
        static void HomeThread() {
            while (running) {
                home.Refresh();
                Thread.Sleep(SERVER_REFRESH_FREQUENCY);
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Main
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args">List of arguments passed to the program</param>
        static void Main(string[] args) {
            /* Initialize Home */
            home = new Home(SERVER_URI, IDENTIFIER, SERVER_REFRESH_FREQUENCY);
            home.Refresh();
            running = true;
            /* Run Home thread */
            Thread homeThread = new Thread(HomeThread);
            homeThread.Start();
            /* Initialize Kinect */
            kinect = new KinectController();
            scenario0 = new Scenario0(home, kinect);
            scenario1 = new Scenario1(home, kinect);
            scenario2 = new Scenario2(home, kinect);
            /* Run a loop that wait User action */
            System.Console.WriteLine(">> Ce controller detect la présence et effectue des actions en conséquences.");
            System.Console.WriteLine(">> Scénarios disponible:");
            System.Console.WriteLine("\t1. Controle de la lumiere et des volets suivant la présence");
            System.Console.WriteLine("\t2. Allumage des lumières à l’entré dans une pièce et extinction après la sortie");
            System.Console.WriteLine("\t3. La maison reconnait les secours et ouvre automatiquement la porte");
            System.Console.WriteLine("\t4. Quitter ou appuyer sur 'q'");
            System.Console.WriteLine(">> Votre choix> ");
            kinect.Start();
            while (running) {
                int m = System.Console.ReadKey().KeyChar;
                switch (m) {
                    case '1':
                        kinect.CurrentScenario = scenario0;
                        break;
                    case '2':
                        kinect.CurrentScenario = scenario1;
                        break;
                    case '3':
                        kinect.CurrentScenario = scenario2;
                        break;
                    case '4': case 'q': case 'Q':
                        running = false;
                        break;
                }
            }
            /* Finalize */ 
            kinect.Stop();
        }


    }
}
