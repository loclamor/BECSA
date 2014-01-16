using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SmartHome;

namespace TextViewClient
{
    /// <summary>
    /// C# client of SmartHome, provide text feedback of home.
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
        const string IDENTIFIER = "sharp"; /* webapp */
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


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Home
        ///////////////////////////////////////////////////////////////////////////////////////////
       
        /// <summary>
        /// Home loop
        /// </summary>
        static void HomeThread() {
            while (running) {
                home.Refresh();
                Thread.Sleep(100);
            }
        }
        /// <summary>
        /// Event fired when a room have been updated
        /// </summary>
        /// <param name="h">Home updated</param>
        /// <param name="p">Room updated</param>
        /// <param name="act">Update kind</param>
        static void RoomUpdate(Home h, Room p, Home.RoomUpdateKind act) {
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
            } else if (act == Home.RoomUpdateKind.NewRoomAdded) {
                s = "Room added : " + p.ToString();
            } else if (act == Home.RoomUpdateKind.RoomRemoved) {
                s = "Room removed : " + p.ToString();
            }
            if (s.Length > 0) {
                System.Console.WriteLine(s);
            }
        }
        /// <summary>
        /// Event fired when the home have been updated
        /// </summary>
        /// <param name="h">Home updated</param>
        /// <param name="act">Update kind</param>
        static void HomeUpdate(Home h, Home.HomeUpdateKind act) {
            string s = "";
            string s2 = "";
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
            } else if (act == Home.HomeUpdateKind.RoomListChanged) {
                s = "La liste des pieces à changé";
                s2 = h.Pieces.ToString();
            } else if (act == Home.HomeUpdateKind.AlarmClockListChanged) {
                s = "La liste des reveils à changé";
                s2 = h.Reveils.ToString();
            } else if (act == Home.HomeUpdateKind.SongListChanged) {
                s = "La liste des chansons à changé";
                s2 = h.Hifi.ToString();
            }
            if (s.Length > 0) {
                System.Console.WriteLine(s);
            }
            if (s2.Length > 0) {
                System.Console.WriteLine(s2);
            }
        }
        /// <summary>
        /// Event fired when an alarm clock is updated
        /// </summary>
        /// <param name="h">Home updated</param>
        /// <param name="a">Alarm clock updated</param>
        /// <param name="act">Update kind</param>
        static void AlarmClockUpdate(Home h, AlarmClock a, Home.AlarmClockUpdateKind act) {
            string s = "";
            if (act == Home.AlarmClockUpdateKind.AlarmClockRing) {
                s = "Sonnerie activer: " + a.Name;
            } else if (act == Home.AlarmClockUpdateKind.AlarmClockUpdated) {
                s = "Réveils modifier: " + a.Name;
            } else if (act == Home.AlarmClockUpdateKind.NewAlarmClockSet) {
                s = "Reveils added : " + a.ToString();
            } else if (act == Home.AlarmClockUpdateKind.AlarmClockRemoved) {
                s = "Reveils removed : " + a.ToString();
            }
            if (s.Length > 0) {
                System.Console.WriteLine(s);
            }
        }
        /// <summary>
        /// Event fired when a song is updated
        /// </summary>
        /// <param name="h">Home updated</param>
        /// <param name="o">Song updated</param>
        /// <param name="act">Update kind</param>
        static void HifiUpdate(Home h, Song o, Home.HifiUpdateKind act) {
            string s = "";
            if (act == Home.HifiUpdateKind.SongUpdated) {
                s = "Son modifier: " + o.Title;
            } else if (act == Home.HifiUpdateKind.NewSongAdded) {
                s = "Son added : " + o.ToString();
            } else if (act == Home.HifiUpdateKind.SongRemoved) {
                s = "Son removed : " + o.ToString();
            }
            if (s.Length > 0) {
                System.Console.WriteLine(s);
            }
        }
        /// <summary>
        /// Event fired when an action have been received
        /// </summary>
        /// <param name="h">Home updated</param>
        /// <param name="a">Action received</param>
        static void ActionReceived(Home h, HomeAction a) {
            System.Console.WriteLine(a.ToString());
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
			JSON jsonConfig = new JSON(FileUtils.GetFileContent("config.json"));
			string serverURI = (jsonConfig.Contains("server") ? jsonConfig.Get("server").GetStringValue(SERVER_URI) : SERVER_URI);
			int serverRefreshFreq = (jsonConfig.Contains("refresh_frequency") ? jsonConfig.Get("refresh_frequency").GetIntValue(SERVER_REFRESH_FREQUENCY) : SERVER_REFRESH_FREQUENCY);
			if (serverRefreshFreq < 0) serverRefreshFreq = SERVER_REFRESH_FREQUENCY;
			System.Console.WriteLine("Server: " + serverURI);
			System.Console.WriteLine("RefreshFrequency:" + serverRefreshFreq.ToString() + "ms");
			home = new Home(serverURI, IDENTIFIER, serverRefreshFreq);
            home.Refresh();
            home.RegisterEvent(HomeUpdate);
            home.RegisterEvent(RoomUpdate);
            home.RegisterEvent(AlarmClockUpdate);
            home.RegisterEvent(HifiUpdate);
            home.RegisterEvent(ActionReceived);
            running = true;
            /* Run Home thread */
            Thread homeThread = new Thread(HomeThread);
            homeThread.Start();
            System.Console.WriteLine(home.Hifi.RemoveSong(home.Hifi.GetSong("rust in peace")));
            /* Run a loop that wait User action */
            System.Console.WriteLine(">> Les actions effectuées sur la maison s'inscriront ici");
            System.Console.WriteLine(">> Pour quitter appuyer sur 'q'");
            while (running) {
                ConsoleKeyInfo key = System.Console.ReadKey();
                if ((key.KeyChar == 'q') || (key.KeyChar == 'Q')) {
                    running = false;
                }
            }
        }

    }
}
