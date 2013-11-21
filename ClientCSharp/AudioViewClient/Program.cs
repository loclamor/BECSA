using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Speech.Synthesis;
using SmartHome;

namespace AudioViewClient
{
    class Program
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Server location
        /// </summary>
        const string SERVER_URI = "http://becsa.mondophoto.fr/server/"; /* http://127.0.0.1:80/csa/server/server.php */
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
        /// Used to read text
        /// </summary>
        static private SpeechSynthesizer _reader;


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Home
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Home loop
        /// </summary>
        static void HomeThread() {
            while (running) {
                home.Refresh();
            }
        }
        /// <summary>
        /// Event fired when a room have been updated
        /// </summary>
        /// <param name="p">Room updated</param>
        /// <param name="act">Update kind</param>
        static void PieceUpdate(Piece p, Home.RoomUpdateKind act) {
            string s = "";
            if (act == Home.RoomUpdateKind.SwitchOnRoomLight) {
                s = "Lumière allumée : " + p.Nom;
            } else if (act == Home.RoomUpdateKind.SwitchOffRoomLight) {
                s = "Lumière éteinte : " + p.Nom;
            } else if (act == Home.RoomUpdateKind.CloseRoomDoor) {
                s = "Porte verrouillée : " + p.Nom;
            } else if (act == Home.RoomUpdateKind.OpenRoomDoor) {
                s = "Porte déverouillée : " + p.Nom;
            } else if (act == Home.RoomUpdateKind.OpenRoomFlap) {
                s = "Volet ouvert : " + p.Nom;
            } else if (act == Home.RoomUpdateKind.CloseRoomFlap) {
                s = "Volet fermé : " + p.Nom;
            }
            System.Console.WriteLine(s);
            _reader.SpeakAsync(s);
        }
        /// <summary>
        /// Event fired when the home have been updated
        /// </summary>
        /// <param name="act">Update kind</param>
        static void HomeUpdate(Home.HomeUpdateKind act) {
            string s = "";
            if (act == Home.HomeUpdateKind.SwitchOnAllRoomLight) {
                s = "Toutes les lumières sont allumées";
            } else if (act == Home.HomeUpdateKind.SwitchOffAllRoomLight) {
                s = "Toutes les lumières sont éteintes";
            } else if (act == Home.HomeUpdateKind.OpenAllDoor) {
                s = "Toutes les portes sont déverouillée";
            } else if (act == Home.HomeUpdateKind.CloseAllDoor) {
                s = "Toutes les portes sont verouillée";
            } else if (act == Home.HomeUpdateKind.OpenAllFlap) {
                s = "Tous les volets sont ouverts";
            } else if (act == Home.HomeUpdateKind.CloseAllFlap) {
                s = "Tous les volets sont fermés";
            }
            System.Console.WriteLine(s);
            _reader.SpeakAsync(s);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Main
        ///////////////////////////////////////////////////////////////////////////////////////////

        static void Main(string[] args) {
            /* Initialize Home */
            home = new Home(SERVER_URI, SERVER_REFRESH_FREQUENCY);
            home.Refresh();
            home.RegisterHomeUpdateEvent(HomeUpdate);
            home.RegisterPieceUpdateEvent(PieceUpdate);
            running = true;
            /* Initialize Speech */
            _reader = new SpeechSynthesizer();
            /* Run Home thread */
            Thread homeThread = new Thread(HomeThread);
            homeThread.Start();
            /* Run a loop that wait User action */
            System.Console.WriteLine(">> Les actions effectuées sur la maison seront lues et s'inscriront ici");
            System.Console.WriteLine(">> Pour quitter appuyer sur n'importe quelle touche");
            while (running) {
                ConsoleKeyInfo key = System.Console.ReadKey();
                running = false;
            }
        }


    }
}
