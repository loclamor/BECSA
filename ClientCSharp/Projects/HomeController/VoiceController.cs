using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome;


namespace HomeController
{
    /// <summary>
    /// Vocal home controller: Map a vocal sentence to an list of action to execute.
    /// </summary>
    class VoiceController : ActionMapHelper
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Enums
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Vocal mode supported
        /// </summary>
        public enum VocalModeType
        {
            /// <summary>
            /// Permit to control room element by voice
            /// </summary>
            DefaultVocalMode,
            /// <summary>
            /// In this mode only response to emergency question are possible
            /// </summary>
            EmergencyVocalMode
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Private member: Current vocal mode, use <see cref="VoiceController.ChangeVocalMode"/> method to change it.
        /// </summary>
        private VocalModeType _vocalMode;

        /// <summary>
        /// Directions pre-configured
        /// </summary>
        private Dictionary<string, int> Directions;


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Vocal action recognizable
        /// </summary>
        public VoiceController() {
            _vocalMode = VocalModeType.DefaultVocalMode;
            Directions = new Dictionary<string, int>();
            Directions.Add("aux courses", 1);
            Directions.Add("faires les courses", 1);
            Directions.Add("a une brocante", 2);
            Directions.Add("a la brocante la plus proche", 2);
            Directions.Add("chez le medecin", 3);
            Directions.Add("chez le veterinaire", 4);
            Directions.Add("a l'hopital", 5);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Vocal Mode - management
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get Current vocal mode
        /// </summary>
        public VocalModeType VocalMode {
            get {
                return _vocalMode;
            }
        }
        /// <summary>
        /// Change vocal mode
        /// </summary>
        /// <param name="mode">Vocal mode desired</param>
        /// <param name="forHome">Home associated</param>
        public void ChangeVocalMode(VocalModeType mode, Home forHome) {
            InitAction(mode, forHome);
            _vocalMode = mode;
        }
        /// <summary>
        /// Refresh vocal actions
        /// </summary>
        /// <param name="withHome">HOme associated</param>
        public void Refresh(Home withHome) {
            InitAction(_vocalMode, withHome);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Private methods
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize all vocal action follow an vocal mode
        /// </summary>
        /// <param name="mode">Vocal mode desired</param>
        /// <param name="forHome">Home associated</param>
        private void InitAction(VocalModeType mode, Home forHome) {
            /* Initialize */
            _vocalMode = mode;
            Clear();
            /* Add actions */
            if (mode == VocalModeType.DefaultVocalMode) {
                /* Add all lights controllers */
                Add("allume toutes les lumieres.", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON));
                Add("allume tous.", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON));
                Add("eteint toutes les lumieres.", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.OFF));
                Add("eteint tous", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.OFF));
                Add("allumer toutes les lumieres.", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON));
                Add("allumer tous", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON));
                Add("eteindre toutes les lumieres.", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.OFF));
                Add("eteindre tous.", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.OFF));
                /* Add all doors controllers */
                Add("dévérouille toutes les portes.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON));
                Add("dévérouille tous.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON));
                Add("vérouille toutes les portes.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF));
                Add("vérouille tous.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF));
                Add("dévérouiller toutes les portes.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON));
                Add("dévérouiller tous.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON));
                Add("vérouiller toutes les portes.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF));
                Add("vérouiller tous.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF));
                Add("ouvre toutes les portes.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON));
                Add("ferme toutes les portes.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF));
                Add("ouvrir toutes les portes.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON));
                Add("fermer toutes les portes.", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF));
                /* Add all flaps controllers */
                Add("ouvre toutes les volets.", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.ON));
                Add("ouvre tous.", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.ON));
                Add("ferme toutes les volets.", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF));
                Add("ferme tous.", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF));
                Add("ouvrir toutes les volets.", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.ON));
                Add("ouvrir tous.", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.ON));
                Add("fermer toutes les volets.", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF));
                Add("fermer tous.", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF));
                /* Extra */
                Add("mode invasion zombie.", new RoomAction(
                    RoomAction.ControllerType.VOLET | RoomAction.ControllerType.LUMIERE | RoomAction.ControllerType.PORTE, 
                    RoomAction.ActionType.OFF));
                Add("ferme tout les volets et allume la lumiere.", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF));
                Add("ferme tout les volets et allume la lumiere.", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON, 1));
                Add("ferme les volets et allume la lumiere.", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF, 1));
                Add("ferme les volets et allume la lumiere.", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON, 1));
                /* Add all rooms */
                for (int i = 0; i < forHome.Pieces.Count; i++) {
                    Room r = forHome.Pieces.GetByIndex(i);
                    /* Add lights */
                    Add("allume la lumiere de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON, r.Id));
                    Add("allumer la lumiere de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON, r.Id));
                    Add("allume la lumiere du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON, r.Id));
                    Add("allumer la lumiere du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON, r.Id));
                    Add("allume " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON, r.Id));
                    Add("allumer " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.ON, r.Id));
                    Add("eteindre la lumiere de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.OFF, r.Id));
                    Add("eteint la lumiere de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.OFF, r.Id));
                    Add("eteindre la lumiere du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.OFF, r.Id));
                    Add("eteint la lumiere du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.OFF, r.Id));
                    Add("eteint " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.OFF, r.Id));
                    Add("eteindre " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.LUMIERE, RoomAction.ActionType.OFF, r.Id));
                    /* Add doors */
                    Add("dévérouille la porte de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON, r.Id));
                    Add("dévérouiller la porte de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON, r.Id));
                    Add("dévérouille la porte du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON, r.Id));
                    Add("dévérouiller la porte du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON, r.Id));
                    Add("dévérouille " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON, r.Id));
                    Add("dévérouiller " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON, r.Id));
                    Add("vérouille la porte de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF, r.Id));
                    Add("vérouiller la porte de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF, r.Id));
                    Add("vérouille la porte du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF, r.Id));
                    Add("vérouiller la porte du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF, r.Id));
                    Add("vérouille " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF, r.Id));
                    Add("vérouiller " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF, r.Id));
                    Add("ouvre la porte de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON, r.Id));
                    Add("ouvrir la porte de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON, r.Id));
                    Add("fermer la porte de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF, r.Id));
                    Add("ferme la porte de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF, r.Id));
                    Add("ouvre la porte du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON, r.Id));
                    Add("ouvrir la porte du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.ON, r.Id));
                    Add("fermer la porte du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF, r.Id));
                    Add("ferme la porte du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.PORTE, RoomAction.ActionType.OFF, r.Id));
                    /* Add flaps */
                    Add("ouvre le volet de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.ON, r.Id));
                    Add("ouvrir volet de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.ON, r.Id));
                    Add("fermer le volet de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF, r.Id));
                    Add("ferme le volet de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF, r.Id));
                    Add("ouvre le volet du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.ON, r.Id));
                    Add("ouvrir volet du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.ON, r.Id));
                    Add("fermer le volet du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF, r.Id));
                    Add("ferme le volet du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF, r.Id));
                    Add("ouvre les volets de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.ON, r.Id));
                    Add("fermer les volets de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF, r.Id));
                    Add("ferme les volets de la " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF, r.Id));
                    Add("ouvre les volets du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.ON, r.Id));
                    Add("fermer les volets du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF, r.Id));
                    Add("ferme les volets du " + r.Nom + ".", new RoomAction(RoomAction.ControllerType.VOLET, RoomAction.ActionType.OFF, r.Id));
                }
                /* Add Hifi */
                Add("joue une musique aleatoire.", new HifiAction(HifiAction.ActionType.PLAY_RANDOM_SONG));
                Add("joue une chanson aleatoirement.", new HifiAction(HifiAction.ActionType.PLAY_RANDOM_SONG));
                Add("met en pause la musique.", new HifiAction(HifiAction.ActionType.STOP_ALL_SONG));
                Add("pause.", new HifiAction(HifiAction.ActionType.STOP_ALL_SONG));
                Add("stop la musique.", new HifiAction(HifiAction.ActionType.STOP_ALL_SONG));
                Add("musique suivante.", new HifiAction(HifiAction.ActionType.NEXT_SONG));
                Add("piste suivante.", new HifiAction(HifiAction.ActionType.NEXT_SONG));
                Add("chanson suivante.", new HifiAction(HifiAction.ActionType.NEXT_SONG));
                Add("musique precedente.", new HifiAction(HifiAction.ActionType.PREVIOUS_SONG));
                Add("piste precedente.", new HifiAction(HifiAction.ActionType.PREVIOUS_SONG));
                Add("chanson precedente.", new HifiAction(HifiAction.ActionType.PREVIOUS_SONG));
                Add("listes les chansons disponibles.", new HifiAction(HifiAction.ActionType.LIST_ALL_SONGS));
                Add("listes les musiques disponibles.", new HifiAction(HifiAction.ActionType.LIST_ALL_SONGS));
                for(int i = 0; i < forHome.Hifi.SongCount; i++) {
                    Song s = forHome.Hifi.GetSongByIndex(i);
                    /* With title only */
                    Add("lance la musique " + s.Title + ".", new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                    Add("lance la chanson " + s.Title + ".", new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                    Add("lance le son " + s.Title + ".", new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                    Add("ecouter la musique " + s.Title + ".", new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                    Add("ecouter la chanson " + s.Title + ".", new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                    Add("ecouter " + s.Title, new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                    /* With title + artist */
                    Add("lance la musique " + s.Title + " de " + s.Artist + ".", new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                    Add("lance la chanson " + s.Title + " de " + s.Artist + ".", new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                    Add("lance " + s.Title + " de " + s.Artist + ".", new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                    Add("ecouter la musique " + s.Title + " de " + s.Artist + ".", new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                    Add("ecouter la chanson " + s.Title + " de " + s.Artist + ".", new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                    Add("ecouter " + s.Title + " de " + s.Artist + ".", new HifiAction(HifiAction.ActionType.PLAY_SONG, s.Id));
                }
                /* Add Weather */ 
                int currentWeekDay = DateUtils.WeekDayToNumber();
                string[] userQuestion = { "qu'elle est", "qu'elle sera", "donne moi", "indique moi" };
				for(int i = 0; i < userQuestion.Length; i++) {
					Add(userQuestion[i] + " la meteo d'aujourdhui.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, currentWeekDay));
                    Add(userQuestion[i] + " la meteo de demain.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, DateUtils.WeekDayAdd(currentWeekDay, 1)));
                    Add(userQuestion[i] + " la meteo du lendemain.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, DateUtils.WeekDayAdd(currentWeekDay, 1)));
                    Add(userQuestion[i] + " la meteo d'apres demain.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, DateUtils.WeekDayAdd(currentWeekDay, 2)));
					Add(userQuestion[i] + " la meteo du sur lendemain.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, DateUtils.WeekDayAdd(currentWeekDay, 2)));
					Add(userQuestion[i] + " la meteo de lundi.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, 1));
					Add(userQuestion[i] + " la meteo de mardi.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, 2));
					Add(userQuestion[i] + " la meteo de mercredi.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, 3));
					Add(userQuestion[i] + " la meteo de jeudi.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, 4));
					Add(userQuestion[i] + " la meteo de vendredi.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, 5));
					Add(userQuestion[i] + " la meteo de samedi.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, 6));
                    Add(userQuestion[i] + " la meteo de dimanche.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, 7));
                    /* Next week */ 
                    for (int j = 1; j < 5; j++) {
                        int d = DateUtils.WeekDayAdd(currentWeekDay, j);
                        Add(userQuestion[i] + " la meteo de " + DateUtils.WeekDayNumberToString(d) + " prochain.", new WeatherAction(WeatherAction.ActionType.GET_WEATHER, d));
                    }
				}
                /* Add Map */
                foreach (KeyValuePair<string, int> pair in Directions) {
                    for (int i = 0; i < userQuestion.Length; i++) {
                        Add(userQuestion[i] + " l'itineraire pour allez " + pair.Key + ".", new MapAction(MapAction.ActionType.ROAD_DIRECTIONS, pair.Value));
                        Add(userQuestion[i] + " l'itineraire pour se rendre " + pair.Key + ".", new MapAction(MapAction.ActionType.ROAD_DIRECTIONS, pair.Value));
                        Add(userQuestion[i] + " le chemin pour allez " + pair.Key + ".", new MapAction(MapAction.ActionType.ROAD_DIRECTIONS, pair.Value));
                        Add(userQuestion[i] + " le chemin pour se rendre " + pair.Key + ".", new MapAction(MapAction.ActionType.ROAD_DIRECTIONS, pair.Value));
                        Add(userQuestion[i] + " l'état du traffic pour allez " + pair.Key + ".", new MapAction(MapAction.ActionType.ROAD_TRAFFIC, pair.Value));
                        Add(userQuestion[i] + " l'état du traffic pour se rendre " + pair.Key + ".", new MapAction(MapAction.ActionType.ROAD_TRAFFIC, pair.Value));
                    }
                }
                /* Add AlarmClock */
                /* NONE for this current version */
                /* Extra action */
                Add("silence.", new ExtraAction(ExtraAction.ActionType.SILENCE));
                Add("silence.", new HifiAction(HifiAction.ActionType.STOP_ALL_SONG));
            } else if (mode == VocalModeType.EmergencyVocalMode) {
                /* Emergency mode */
                Add("oui je vais bien.", new EmergencyAction(EmergencyAction.ActionType.EMERGENCY_RESPONSE_OK));
            }
        }


    }
}
