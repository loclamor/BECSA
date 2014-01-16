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
    /// <summary>
    /// C# client of SmartHome, provide vocal feedback of home.
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
        const string IDENTIFIER = "synthese";
        /// <summary>
        /// Home refresh frequency
        /// </summary>
        const int SERVER_REFRESH_FREQUENCY = 1000;
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
        /// <summary>
        /// Room where light have switched to on
        /// </summary>
        static private string _roomLightOn = "";
        /// <summary>
        /// Room where light have switched to off
        /// </summary>
        static private string _roomLightOff = "";
        /// <summary>
        /// Room where door have been opened
        /// </summary>
        static private string _roomDoorOn = "";
        /// <summary>
        /// Room where door have been closed
        /// </summary>
        static private string _roomDoorOff = "";
        /// <summary>
        /// Room where flap have been opened
        /// </summary>
        static private string _roomFlapOn = "";
        /// <summary>
        /// Room where flap have been closed
        /// </summary>
        static private string _roomFlapOff = "";
        /// <summary>
        /// Mute mode
        /// </summary>
        static private bool _muteMode = false;
        /// <summary>
        /// Stop all current speak until next home refresh
        /// </summary>
        static private bool _silence = false;
        /// <summary>
        /// Texts to say
        /// </summary>
        static private List<string> _textToSay = new List<string>();
        /// <summary>
        /// Directions pre-configured
        /// </summary>
        static private Dictionary<string, int> Directions;
        /// <summary>
        /// Trafic sentence follow percent
        /// </summary>
        static private string[] TraficPercentSentence = {
            "fluide aucun bouchon à l'horizon",
            "plus ou moins fluide",
            "plutôt encombré",
            "moyennement encombré",
            "très encombrés",
            "complètement encombrés"
        };
        /// <summary>
        /// Weather day name.
        /// </summary>
        static private string[] WeatherDayId = {
            "Lundi",
            "Mardi",
            "Mercredi",
            "Jeudi",
            "Vendredi",
            "Samedi",
            "Dimanche"
        };
        /// <summary>
        /// Weather sentence follow condition. (cf. http://developer.yahoo.com/weather/)
        /// </summary>
        static private string[] WeatherSentence = {
            "présence de tornades", // tornado
            "présence de tempête tropical", // tropical storm
            "présence d'ouragan", // hurricane
            "présence d'orages violents", // severe thunderstorms
            "présence d'orages", // thunderstorms
            "le temps sera partagée entre de la pluie et de la neige", // mixed rain and snow
            "le temps sera partagée entre de la pluie et de la neige fondue", // mixed rain and sleet
            "le temps sera partagée entre de la neige et de la neige fondue", // mixed snow and sleet
            "présence de bruine verglaçante", // freezing drizzle
            "présence de bruine", // drizzle
            "présence de pluie verglaçante", // freezing rain
            "présence d'averses", // showers
            "présence d'averses", // showers
            "présence d'averses de neige", // snow flurries
            "présence de légères averses de neige", // light snow showers
            "présence de rafales de neige", // blowing snow
            "présence de neige", // snow
            "présence de grêle", // hail
            "présence de neige fondue", // sleet
            "présence de poussière", // dust
            "le temps sera brumeux", // foggy
            "présence de brume", // haze
            "le temps sera enfumé", // smoky
            "présence de tempête", // blustery
            "le temps sera venteux", // windy
            "le temps sera froid", // cold
            "le temps sera nuageux", // cloudy
            "nuit nuageux", // mostly cloudy (night)
            "jour nuageux", // mostly cloudy (day)
            "nuit partiellement nuageuse", // partly cloudy (night)
            "jour partiellement nuageux", // partly cloudy (day)
            "nuit dégagé", // clear (night)
            "le temps sera ensoleillé", // sunny
            "nuit passable", // fair (night)
            "jour passable", // fair (day)
            "le temps sera partagée entre de la pluie et de la grêle", // mixed rain and hail
            "le temps sera chaud", // hot
            "présence d'orages isolés", // isolated thunderstorms
            "présence d'orages dispersés", // scattered thunderstorms
            "présence d'orages dispersés", // scattered thunderstorms
            "présence d'averses dispersés", // scattered showers
            "le temps sera très neigeux", // heavy snow
            "présence d'averses de neige éparses", // scattered snow showers
            "le temps sera très neigeux", // heavy snow
            "le temps sera partiellement nuageux", // partly cloudy
            "présence d'averses orageuses", // thundershowers
            "présence d'averses neigeuses", // snow showers
            "présence d'averses orageuses isolée", // isolated thundershowers
        };

		/// <summary>
		/// Contains all RequestUniqueId already received and treated
		/// </summary>
		static private HashSet<int> _uniqueRequestIdTreats = new HashSet<int>();
		/// <summary>
		/// Used to reset all unique request id after 10 minute without received action that use unique request id
		/// </summary>
		static private int _lastUniqueRequestIdResetTick = 0;
		


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Home
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Home loop
        /// </summary>
        static void HomeThread() {
            while (running) {
                /* Refresh */
                home.Refresh();
                /* Read all text */
                if (!_silence) {
                    foreach (string s in _textToSay) {
                        if (_muteMode) break;
                        _reader.SpeakAsync(s);
                    }
                }
                _textToSay.Clear();
                _silence = false;
                /* Avoid taking all CPU */
                Thread.Sleep(SERVER_REFRESH_FREQUENCY);
            }
        }

        /// <summary>
        /// Speak a text and write it on console in asyncrone mode
        /// </summary>
        /// <param name="text">Text desired</param>
        /// <param name="prefix">Prefix desired</param>
        /// <param name="suffix">Suffix desired</param>
        static private void SpeakAsyncText(string text, string prefix = "", string suffix = "") {
            if (text.Length > 0) {
                string s = prefix + text + suffix;
                System.Console.WriteLine(s);
                if (!_muteMode) {
                    _textToSay.Add(s);
                }
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
                if (_roomLightOn.Length > 0) _roomLightOn += ", ";
                _roomLightOn += p.Nom;
            } else if (act == Home.RoomUpdateKind.SwitchOffRoomLight) {
                if (_roomLightOff.Length > 0) _roomLightOff += ", ";
                _roomLightOff += p.Nom;
            } else if (act == Home.RoomUpdateKind.CloseRoomDoor) {
                if (_roomDoorOff.Length > 0) _roomDoorOff += ", ";
                _roomDoorOff += p.Nom;
            } else if (act == Home.RoomUpdateKind.OpenRoomDoor) {
                if (_roomDoorOn.Length > 0) _roomDoorOn += ", ";
                _roomDoorOn += p.Nom;
            } else if (act == Home.RoomUpdateKind.OpenRoomFlap) {
                if (_roomFlapOn.Length > 0) _roomFlapOn += ", ";
                _roomFlapOn += p.Nom;
            } else if (act == Home.RoomUpdateKind.CloseRoomFlap) {
                if (_roomFlapOff.Length > 0) _roomFlapOff += ", ";
                _roomFlapOff += p.Nom;
            } else if (act == Home.RoomUpdateKind.NewRoomAdded) {
                s = "Pièces ajouté : " + p.ToString();
            } else if (act == Home.RoomUpdateKind.RoomRemoved) {
                s = "Pièces enlevées : " + p.ToString();
            }
            SpeakAsyncText(s);
        }
        /// <summary>
        /// Event fired when the home have been updated
        /// </summary>
        /// <param name="h">Home updated</param>
        /// <param name="act">Update kind</param>
        static void HomeUpdate(Home h, Home.HomeUpdateKind act) {
            string s = "";
            string s2 = "";
            if (act == Home.HomeUpdateKind.BeginUpdate) {
                /* Begin update */
                _roomLightOn = "";
                _roomLightOff = "";
                _roomDoorOn = "";
                _roomDoorOff = "";
                _roomFlapOn = "";
                _roomFlapOff = "";
                return;
            } else if (act == Home.HomeUpdateKind.SwitchOnAllRoomLight) {
                s = "Toutes les lumières sont allumées.";
            } else if (act == Home.HomeUpdateKind.SwitchOffAllRoomLight) {
                s = "Toutes les lumières sont éteintes.";
            } else if (act == Home.HomeUpdateKind.OpenAllDoor) {
                s = "Toutes les portes sont dévérouillée.";
            } else if (act == Home.HomeUpdateKind.CloseAllDoor) {
                s = "Toutes les portes sont vérouillée.";
            } else if (act == Home.HomeUpdateKind.OpenAllFlap) {
                s = "Tous les volets sont ouverts.";
            } else if (act == Home.HomeUpdateKind.CloseAllFlap) {
                s = "Tous les volets sont fermés.";
            } else if (act == Home.HomeUpdateKind.RoomListChanged) {
                //s = "La liste des pieces à changé";
                s2 = h.Pieces.ToString();
            } else if (act == Home.HomeUpdateKind.AlarmClockListChanged) {
                //s = "La liste des reveils à changé";
                s2 = h.Reveils.ToString();
            } else if (act == Home.HomeUpdateKind.SongListChanged) {
                //s = "La liste des chansons à changé";
                s2 = h.Hifi.ToString();
            } else if (act == Home.HomeUpdateKind.EndUpdate) {
                /* End update */
                SpeakAsyncText(_roomLightOn, "Lumière allumée : ", ".");
                SpeakAsyncText(_roomLightOff, "Lumière eteinte : ", ".");
                SpeakAsyncText(_roomDoorOn, "Porte dévérouillée : ", ".");
                SpeakAsyncText(_roomDoorOff, "Porte vérouillée : ", ".");
                SpeakAsyncText(_roomFlapOn, "Volet ouvert : ", ".");
                SpeakAsyncText(_roomFlapOff, "Volet fermé : ", ".");
                return;
            }
            SpeakAsyncText(s);
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
                s = "Sonnerie activer: " + a.Name + ".";
            } else if (act == Home.AlarmClockUpdateKind.AlarmClockUpdated) {

                System.Console.WriteLine(h.Reveils.Get("Test"));
                System.Console.WriteLine(a);
                s = "Réveils modifier: " + a.Name + ".";
            } else if (act == Home.AlarmClockUpdateKind.NewAlarmClockSet) {
                s = "Reveils ajouté : " + a.ToString() + ".";
            } else if (act == Home.AlarmClockUpdateKind.AlarmClockRemoved) {
                s = "Reveils supprimer : " + a.ToString() + ".";
            }
            SpeakAsyncText(s);
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
                s = "La chanson " + o.Title + " de " + o.Artist + " à été modifier en " + h.Hifi.GetSong(o.Id) + ".";
            } else if (act == Home.HifiUpdateKind.NewSongAdded) {
                s = "Nouvelle chanson disponible: " + o.Title + " de " + o.Artist + ".";
            } else if (act == Home.HifiUpdateKind.SongRemoved) {
                s = "La chanson " + o.Title + " de " + o.Artist + " n'est plus disponible.";
            }
            System.Console.WriteLine(s);
        }
        /// <summary>
        /// Convert a string into a double (take in account '.' and ',')
        /// </summary>
        /// <param name="str">String desired</param>
        /// <returns>Double number of the string</returns>
        static public double StringToDouble(string str) {
            double d1, d2;
            try {
                d1 = double.Parse(str.Replace(',', '.'));
            } catch {
                d1 = 0;
            }
            try {
                d2 = double.Parse(str.Replace('.', ','));
            } catch {
                d2 = 0;
            }
            if (d1 == d2) {
                return d1;
            } else if (d1 != 0) {
                return d1;
            } else {
                return d2;
            }
        }
        /// <summary>
        /// Event fired when an action have been received
        /// </summary>
        /// <param name="h">Home updated</param>
        /// <param name="a">Action received</param>
        static void ActionReceived(Home h, HomeAction a) {
            if (a.IsType("say")) {
                /* say(<string> text, [<string> sender], [<string> tag]) */
                _silence = false;
                SpeakAsyncText(a.GetParam(0));
                if (a.ParamCount == 2) {
                    h.Actions.SendAction(a.GetParam(1), "sayOK");
                } else if (a.ParamCount == 3) {
                    h.Actions.SendAction(a.GetParam(1), "sayOK", a.GetParam(2));
                }
            } else if ((a.IsType("meteoReponse")) && (a.ParamCount > 0)) {
				/* meteoReponse(<int> dayId, <int> temperature, <int> maxTemperature, <int> minTemperature, <int> cloudyState, [<int> requestUniqueId]) */
                int currentWeekDay = DateUtils.WeekDayToNumber();
                string s = "";
				int p1 = a.GetIntParam(0, 1);
				/* Manage UniqueRequestId */
				if ((a.ParamCount >= 6) && (IsUniqueRequestIdTreated(a.GetIntParam(5,-1)))) {
					/* Already treated */
					return;
				} else if ((a.ParamCount == 2) && (IsUniqueRequestIdTreated(a.GetIntParam(1, -1)))) {
					/* Already treated */
					return;
				}
                /* Get Day name */                  
                if ((p1 >= 1) && (p1 <= WeatherDayId.Length)) {
                    /* Check day */
                    if ((a.ParamCount == 5) && (p1 == currentWeekDay)) {
                        s = "Aujourd'hui";
                    } else if ((a.ParamCount == 5) && (p1 == DateUtils.WeekDayAdd(currentWeekDay, 1))) {
                        s = "Demain";
                    } else {
                        s = DateUtils.WeekDayNumberToString(p1);
                    }
                }
                /* Write weather data */
                if (a.ParamCount >= 5) {
                    int p5 = a.GetIntParam(4, -1);
                    if ((p5 >= 0) && (p5 < WeatherSentence.Length)) {
                        s += " " + WeatherSentence[p5];
                    }
                    s += ", la température sera de " + a.GetDoubleParam(1, 0).ToString()
                        + "°c, la maximale sera de " + a.GetDoubleParam(2, 0).ToString()
                        + "°c, et la minimal sera " + a.GetDoubleParam(3, 0).ToString() + "°c.";
                } else {
                    /* Pas de météo disponible */
                    s += " la météo est non disponible.";
                }
                /* Read it */
                s += " Pour plus de prévisions la météo à été préparer et est affiché sur votre tablette.";
                SpeakAsyncText(s);

            } else if (a.IsType("traficReponse")) {
                /* traficReponse(<int> destLocationId, <int> percent, [<int> requestUniqueId]) */
                int p1 = a.GetIntParam(0, -1);
                int p2 = (int)(((a.GetDoubleParam(1, 0)/100.0) * (double)(TraficPercentSentence.Length)));
                if (p2 < 0) p2 = 0;
				if (p2 >= TraficPercentSentence.Length) p2 = TraficPercentSentence.Length - 1;
				/* Manage UniqueRequestId */
				if ((a.ParamCount >= 3) && (IsUniqueRequestIdTreated(a.GetIntParam(2,-1)))) {
					/* Already treated */
					return;
				}
				/* Manage response */
                foreach (KeyValuePair<string, int> pair in Directions) {
                    if (pair.Value == p1) {
                        SpeakAsyncText("Le traffic pour " + pair.Key + " est " + TraficPercentSentence[p2] + ".");
                        break;
                    }
                }
            } else if (a.IsType("itineraireReponse")) {
				/* itinairaireReponse(<int> destLocationId, [<int> requestUniqueId]) */
				int p1 = a.GetIntParam(0, -1);
				/* Manage UniqueRequestId */
				if ((a.ParamCount >= 2) && (IsUniqueRequestIdTreated(a.GetIntParam(1,-1)))) {
					/* Already treated */
					return;
				}
				/* Manage response */
                foreach (KeyValuePair<string, int> pair in Directions) {
                    if (pair.Value == p1) {
                        SpeakAsyncText("L'itineraire pour " + pair.Key + " à était préparé et est disponible sur votre téléphone.");
                        break;
                    }
                }
            } else if (a.IsType("listSongs")) {
                /* listSongs() */
                string s = "Musique disponibles:\n";
                for (int i = 0; i < home.Hifi.SongCount; i++) {
                    Song sg = home.Hifi.GetSongByIndex(i);
                    s += sg.Title + " de " + sg.Artist + "\n";
                }
                SpeakAsyncText(s);
            } else if (a.IsType("silence")) {
                /* silence */
                _silence = true;
                _reader.SpeakAsyncCancelAll();
                _textToSay.Clear();
            } else if (a.IsType("mute")) {
                /* Mute */
                _muteMode = true;
                _reader.SpeakAsyncCancelAll();
                h.Actions.SendAction("kinect", "muteOK");
            } else if (a.IsType("unmute")) {
                /* Unmute */
                _muteMode = false;
                _silence = true;
                h.Actions.SendAction("kinect", "unmuteOK");
            }
            System.Console.WriteLine(a.ToString());
        }

		/// <summary>
		/// Check if the action have not been already treated
		/// </summary>
		/// <param name="uniqueRequestId">Unique request id received</param>
		/// <returns>True if action have been already treated, false otherwise</returns>
		static private bool IsUniqueRequestIdTreated(int uniqueRequestId) {
			/* Check if unique request id treated */
			if (uniqueRequestId != -1) {
				/* Reset request id treats list if necessary: reset after 10 minutes */
				if ((System.Environment.TickCount - _lastUniqueRequestIdResetTick) >= 600000) {
					/* Reset */
					_uniqueRequestIdTreats.Clear();
					_lastUniqueRequestIdResetTick = System.Environment.TickCount;
				}
				/* Check if request id present in treated id */
				if (_uniqueRequestIdTreats.Contains(uniqueRequestId)) {
					/* Return: action already treated */
					return true;
				} else {
					_uniqueRequestIdTreats.Add(uniqueRequestId);
				}
			}
			/* Return false - action not treated */
			return false;
		}


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Main
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args">List of arguments passed to the program</param>
		static void Main(string[] args) {
            /* Initialize dictionarie */
            Directions = new Dictionary<string, int>();
            Directions.Add("allez faires les courses", 1);
            Directions.Add("allez a la brocante la plus proche", 2);
            Directions.Add("allez chez le medecin", 3);
            Directions.Add("allez chez le veterinaire", 4);
            Directions.Add("allez a l'hopital", 5);
            /* Initialize Home */
			JSON jsonConfig = new JSON(FileUtils.GetFileContent("config.json"));
			string serverURI = (jsonConfig.Contains("server") ? jsonConfig.Get("server").GetStringValue(SERVER_URI) : SERVER_URI);
			int serverRefreshFreq = (jsonConfig.Contains("refresh_frequency") ? jsonConfig.Get("refresh_frequency").GetIntValue(SERVER_REFRESH_FREQUENCY) : SERVER_REFRESH_FREQUENCY);
			if (serverRefreshFreq < 0) serverRefreshFreq = SERVER_REFRESH_FREQUENCY;
			System.Console.WriteLine("Server: " + serverURI);
			System.Console.WriteLine("RefreshFrequency:" + serverRefreshFreq.ToString() + "ms");
			home = new Home(serverURI, IDENTIFIER, serverRefreshFreq);
			//home.Actions.AvoidDuplicateAction(true, 5000);
			//home.Actions.SetDuplicateActionException("silence", "mute", "unmute");
            home.Refresh();
            home.RegisterEvent(HomeUpdate);
            home.RegisterEvent(RoomUpdate);
            home.RegisterEvent(AlarmClockUpdate);
            home.RegisterEvent(HifiUpdate);
            home.RegisterEvent(ActionReceived);
            running = true;
            /* Initialize Speech */
            _reader = new SpeechSynthesizer();
            /* Run Home thread */
            Thread homeThread = new Thread(HomeThread);
            homeThread.Start();
            /* Run a loop that wait User action */
            System.Console.WriteLine(">> Les actions effectuées sur la maison seront lues et s'inscriront ici");
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
