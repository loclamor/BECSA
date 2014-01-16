using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartHome;
using System.Speech;
using System.Speech.Recognition;


namespace HomeController
{
    /// <summary>
    /// C# client of SmartHome, provide vocal control of home.
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
        const string IDENTIFIER = "home_controller";
        /// <summary>
        /// Home refresh frequency
        /// </summary>
        const int SERVER_REFRESH_FREQUENCY = 5000;
        /// <summary>
        /// Home controller/viewer
        /// </summary>
        static private Home home;
        /// <summary>
        /// Indicate if the application is still running
        /// </summary>
        static private bool running;
        /// <summary>
        /// Used to recognize the speech of the user
        /// </summary>
        static private SpeechRecognitionEngine _engine;
        /// <summary>
        /// Home name
        /// </summary>
        static private string _homeName = "Maison";
        /// <summary>
        /// Home voice controller
        /// </summary>
        static private VoiceController _voiceController;


        ///////////////////////////////////////////////////////////////////////////////////////////
        // VocalActions
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize all vocal action follow an vocal mode
        /// </summary>
        static void InitVocalAction() {
            /* Initialize */
            Choices detector = new Choices(_homeName);
            Choices actions = new Choices();
            /* Append to choices all vocal actions */
            for (int i = 0; i < _voiceController.Count; i++) {
                actions.Add(_voiceController.Get(i));
            }
            /* Initialize grammar */
            GrammarBuilder grammar = new GrammarBuilder();
            grammar.Append(detector);
            grammar.Append(actions);
            /* Initialize Speech recognizer */
            if (_engine != null) {
                _engine.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
            }
            _engine = new SpeechRecognitionEngine();
            _engine.LoadGrammar(new Grammar(grammar));
            _engine.SetInputToDefaultAudioDevice();
            _engine.RecognizeAsync(RecognizeMode.Multiple);
            _engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
        }

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
        /// <summary>
        /// Event fired when the home have been updated
        /// </summary>
        /// <param name="h">Home updated</param>
        /// <param name="act">Update kind</param>
        static void HomeUpdate(Home h, Home.HomeUpdateKind act) {
            if ((act == Home.HomeUpdateKind.RoomListChanged) || (act == Home.HomeUpdateKind.AlarmClockListChanged) || (act == Home.HomeUpdateKind.SongListChanged)) {
                _voiceController.Refresh(home);
                InitVocalAction();
            }
        }
        /// <summary>
        /// Event fired when an action have been received
        /// </summary>
        /// <param name="h">Home updated</param>
        /// <param name="a">Action received</param>
        static void ActionReceived(Home h, HomeAction a) {
            if (a.IsType("change_mode")) {
                if (string.Compare(a.GetParam(0), "emergency") == 0) {
                    _voiceController.ChangeVocalMode(VoiceController.VocalModeType.EmergencyVocalMode, home);
                } else {
                    _voiceController.ChangeVocalMode(VoiceController.VocalModeType.DefaultVocalMode, home);
                }
                InitVocalAction();
            }
            System.Console.WriteLine(a.ToString());
        }



        ///////////////////////////////////////////////////////////////////////////////////////////
        // Speech recognized
        ///////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Analyse user speech
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e) {
            if ((e.Result.Words.Count > 0) && (e.Result.Words[0].Confidence > 0.7) && (string.Compare(e.Result.Words[0].Text, "maison", true) == 0)) {
                System.Console.WriteLine("<" + e.Result.Text + ">");
                _voiceController.ExecuteAction(home, e.Result.Text.Substring(_homeName.Length + 1));
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Main
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Entry points
        /// </summary>
        /// <param name="args">List of arguments passed to the program</param>
        static void Main(string[] args) {
            /* Initialize general */
            _voiceController = new VoiceController();
			/* Initialize Home */
			JSON jsonConfig = new JSON(FileUtils.GetFileContent("config.json"));
			string serverURI = (jsonConfig.Contains("server") ? jsonConfig.Get("server").GetStringValue(SERVER_URI) : SERVER_URI);
			int serverRefreshFreq = (jsonConfig.Contains("refresh_frequency") ? jsonConfig.Get("refresh_frequency").GetIntValue(SERVER_REFRESH_FREQUENCY) : SERVER_REFRESH_FREQUENCY);
			if (serverRefreshFreq < 0) serverRefreshFreq = SERVER_REFRESH_FREQUENCY;
			System.Console.WriteLine("Server: " + serverURI);
			System.Console.WriteLine("RefreshFrequency:" + serverRefreshFreq.ToString() + "ms");
			home = new Home(serverURI, IDENTIFIER, serverRefreshFreq);
            home.RegisterHomeUpdateEvent(HomeUpdate);
            home.RegisterEvent(ActionReceived);
            home.Refresh();
            running = true;
            /* Run Home thread */
            Thread homeThread = new Thread(HomeThread);
            homeThread.Start();
            /* Run a loop that wait User action */
            System.Console.WriteLine(">> Dites une phrase pour contrôler la maison");
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
