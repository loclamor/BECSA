using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartHome;
using System.Speech;
using System.Speech.Recognition;

///
/// TODO: Improve speech recognition
///

namespace HomeController
{

    class Program
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Server location
        /// </summary>
        const string SERVER_URI = "http://becsa.mondophoto.fr/server/"; /*"http://becsa.mondophoto.fr/server/";*/
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
        /// Used to recognize user speak
        /// </summary>
        static private SpeechRecognitionEngine _engine;

        /// <summary>
        /// Home name
        /// </summary>
        static private string _homeName = "Maison";
        /// <summary>
        /// Home actions recognizable.
        /// </summary>
        static private string[] _actionList = {
            //"Allumer lumiere", "Eteindre lumiere", "Allume lumiere", "Eteint lumiere",
            "ouvrir volet", "fermer volet", "ouvre volet", "fermer volet",
            "verrouiller porte", "deverrouiller porte", "verrouille porte", "deverrouille porte", 
            "fermer porte", "ouvrir porte",  "ferme porte", "ouvre porte",
            "allumer", "eteindre", "allume", "eteint",
            "ouvrir", "fermer", "ouvre", "ferme", 
            "verrouiller", "deverrouiller", "verrouille", "deverrouille" 
        };


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
        /// Event fired when the home have been updated
        /// </summary>
        /// <param name="act">Update kind</param>
        static void HomeUpdate(Home.HomeUpdateKind act) {
            if (act == Home.HomeUpdateKind.RoomCountChanged) {
                /* Add first controller */
                Choices detector = new Choices(_homeName);
                Choices controller = new Choices(_actionList);
                /* Add all rooms */
                Choices Rooms = new Choices("Toutes les lumieres", "Toutes les portes", "Tous les volets");
                for (int i = 0, max = home.GetPieceCount(); i < max; i++) {
                    Rooms.Add(home.GetPiece(i).Nom);
                }
                /* Initialize grammar */
                GrammarBuilder grammar = new GrammarBuilder();
                grammar.Append(detector);
                grammar.Append(controller);
                if (home.GetPieceCount() > 0) {
                    grammar.Append(Rooms);
                }
                /* Initialize Speech recognizer */ 
                _engine = new SpeechRecognitionEngine();
                _engine.LoadGrammar(new Grammar(grammar));
                _engine.SetInputToDefaultAudioDevice();
                _engine.RecognizeAsync(RecognizeMode.Multiple);
                _engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Home
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Controller recognized
        /// </summary>
        private enum Controller
        {
            /// <summary>
            /// Undefined controller
            /// </summary>
            UNDEFINED,
            /// <summary>
            /// All light "controller"
            /// </summary>
            ALL_LUMIERE,
            /// <summary>
            /// All door "controller"
            /// </summary>
            ALL_PORTE,
            /// <summary>
            ///  All flap "controller"
            /// </summary>
            ALL_VOLET,
            /// <summary>
            /// Light controller
            /// </summary>
            LUMIERE,
            /// <summary>
            /// Door controller
            /// </summary>
            PORTE,
            /// <summary>
            /// Flap controller
            /// </summary>
            VOLET
        }
        /// <summary>
        /// Action recognized
        /// </summary>
        private enum Action
        {
            /// <summary>
            /// Undefined action
            /// </summary>
            UNDEFINED,
            /// <summary>
            /// On action
            /// </summary>
            ON,
            /// <summary>
            /// Off action
            /// </summary>
            OFF
        }

        /// <summary>
        /// Retrieve controller recognized
        /// </summary>
        /// <param name="result">Recognition result</param>
        /// <returns>Controller found</returns>
        static private Controller RetrieveController(RecognitionResult result) {
            if (result.Text.IndexOf(" lumieres", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.ALL_LUMIERE;
            } else if (result.Text.IndexOf(" portes", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.ALL_PORTE;
            } else if (result.Text.IndexOf(" volets", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.ALL_VOLET;
            /* Lumiere */ 
            } else if (result.Text.IndexOf(" lumiere ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.LUMIERE;
            } else if (result.Text.IndexOf(" allumer ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.LUMIERE;
            } else if (result.Text.IndexOf(" allume ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.LUMIERE;
            } else if (result.Text.IndexOf(" eteindre ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.LUMIERE;
            } else if (result.Text.IndexOf(" eteint ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.LUMIERE;
            /* Porte */ 
            } else if (result.Text.IndexOf(" porte ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.PORTE;
            } else if (result.Text.IndexOf(" ouvre ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.PORTE;
            } else if (result.Text.IndexOf(" ouvrir ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.PORTE;
            } else if (result.Text.IndexOf(" fermer ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.PORTE;
            } else if (result.Text.IndexOf(" ferme ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.PORTE;
            } else if (result.Text.IndexOf(" verrouiller ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.PORTE;
            } else if (result.Text.IndexOf(" verrouille ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.PORTE;
            } else if (result.Text.IndexOf(" deverrouille ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.PORTE;
            } else if (result.Text.IndexOf(" deverrouiller ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.PORTE;
            /* Volet */ 
            } else if (result.Text.IndexOf(" volet ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Controller.VOLET;
            } else {
                return Controller.UNDEFINED;
            }
        }
        /// <summary>
        /// Retrieve action recognized
        /// </summary>
        /// <param name="result">Recognition result</param>
        /// <returns>Controller found</returns>
        static private Action RetrieveAction(RecognitionResult result) {
            if (result.Text.IndexOf(" ouvrir ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.ON;
            } else if (result.Text.IndexOf(" ouvre ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.ON;
            } else if (result.Text.IndexOf(" allumer ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.ON;
            } else if (result.Text.IndexOf(" allume ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.ON;
            } else if (result.Text.IndexOf(" deverrouille ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.ON;
            } else if (result.Text.IndexOf(" deverrouiller ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.ON;
            } else if (result.Text.IndexOf(" fermer ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.OFF;
            } else if (result.Text.IndexOf(" ferme ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.OFF;
            } else if (result.Text.IndexOf(" eteindre ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.OFF;
            } else if (result.Text.IndexOf(" eteint ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.OFF;
            } else if (result.Text.IndexOf(" verrouiller ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.OFF;
            } else if (result.Text.IndexOf(" verrouille ", StringComparison.CurrentCultureIgnoreCase) != -1) {
                return Action.OFF;
            } else {
                return Action.UNDEFINED;
            }

        }
        /// <summary>
        /// Retrieve controller recognized
        /// </summary>
        /// <param name="result">Recognition result</param>
        /// <returns>Room recognized</returns>
        static string RetrieveRoomName(RecognitionResult result) {
            foreach (string s in _actionList) {
                if ((result.Text.Length - _homeName.Length - 1 >= s.Length) && 
                    (string.Compare(s, result.Text.Substring(_homeName.Length + 1, s.Length)) == 0)) {
                    return result.Text.Substring(s.Length + _homeName.Length + 2);
                }
            }
            return "";
        }

        
        /// <summary>
        /// Analyse user speech
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e) {
            if ((e.Result.Words.Count > 0) && (string.Compare(e.Result.Words[0].Text, "maison", true) == 0)) {
                System.Console.WriteLine("<" + e.Result.Text + ">");
                /* Analyse Controller */
                string roomName = "";
                switch (RetrieveController(e.Result)) {
                    /* All lumieres,portes,volets */
                    case Controller.ALL_LUMIERE:
                        switch (RetrieveAction(e.Result)) {
                            case Action.ON: home.AllumerTout(); break;
                            case Action.OFF: home.EteindreTout(); break;
                        }
                        break;
                    case Controller.ALL_PORTE:
                        switch (RetrieveAction(e.Result)) {
                            case Action.ON: home.DeverrouillerTout(); break;
                            case Action.OFF: home.VerrouillerTout(); break;
                        }
                        break;
                    case Controller.ALL_VOLET:
                        switch (RetrieveAction(e.Result)) {
                            case Action.ON: home.OuvrirTout(); break;
                            case Action.OFF: home.FermerTout(); break;
                        }
                        break;
                    /* Lumiere */ 
                    case Controller.LUMIERE:
                        roomName = RetrieveRoomName(e.Result);
                        if (roomName.Length > 0) {
                            switch (RetrieveAction(e.Result)) {
                                case Action.ON: home.AllumerLumiere(roomName); break;
                                case Action.OFF: home.EteindreLumiere(roomName); break;
                            }
                        }
                        break;
                    /* Porte */
                    case Controller.PORTE:
                        roomName = RetrieveRoomName(e.Result);
                        if (roomName.Length > 0) {
                            switch (RetrieveAction(e.Result)) {
                                case Action.ON: home.DeverrouillerPorte(roomName); break;
                                case Action.OFF: home.VerrouillerPorte(roomName); break;
                            }
                        }
                        break;
                    /* Volet */
                    case Controller.VOLET:
                        roomName = RetrieveRoomName(e.Result);
                        if (roomName.Length > 0) {
                            switch (RetrieveAction(e.Result)) {
                                case Action.ON: home.OuvrirVolet(roomName); break;
                                case Action.OFF: home.FermerVolet(roomName); break;
                            }
                        }
                        break;
                }
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Main
        ///////////////////////////////////////////////////////////////////////////////////////////

        static void Main(string[] args) {
            /* Initialize Home */
            home = new Home(SERVER_URI, SERVER_REFRESH_FREQUENCY);
            home.RegisterHomeUpdateEvent(HomeUpdate);
            home.Refresh();
            running = true;
            /* Run Home thread */
            Thread homeThread = new Thread(HomeThread);
            homeThread.Start();
            /* Run a loop that wait User action */
            System.Console.WriteLine(">> Dites une phrase pour controller la maison");
            System.Console.WriteLine(">> Pour quitter appuyer sur n'importe qu'elle touche");
            while (running) {
                ConsoleKeyInfo key = System.Console.ReadKey();
                running = false;
            }
        }


    }
}
