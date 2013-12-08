using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{

    /// <summary>
    /// Helper class that manage action that home can do
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public class ActionMapHelper
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Home's actions recognizable
        /// </summary>
        private Dictionary<string, List<AbstractAction>> _actions;


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize ActionMapHelper
        /// </summary>
        public ActionMapHelper() {
            _actions = new Dictionary<string, List<AbstractAction>>();
        }



        ///////////////////////////////////////////////////////////////////////////////////////////
        // VoiceActions - getters
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get an sentence by index
        /// </summary>
        /// <param name="index">Sentence index desired</param>
        /// <returns>Sentence found or "" if not found</returns>
        public string Get(int index) {
            /* Seek sentence */
            if ((index >= 0) && (index < _actions.Count)) {
                foreach(KeyValuePair<string, List<AbstractAction>> pair in _actions) {
                    if (index == 0) {
                        return pair.Key;
                    }
                    index--;
                }
            }
            /* Not found: return "" */
            return "";
        }
        /// <summary>
        /// Get vocal action by index of an sentence
        /// </summary>
        /// <param name="sentence">Sentence desired</param>
        /// <param name="actionIndex">Vocal action index desired</param>
        /// <returns>Vocal action found or null if not found</returns>
        public AbstractAction GetAction(string sentence, int actionIndex) {
            /* Seek action */
            if ((actionIndex >= 0) && (_actions.ContainsKey(sentence))) {
                if (actionIndex < _actions[sentence].Count) {
                    return _actions[sentence].ElementAt(actionIndex);
                }
            }
            /* Not found: return null */
            return null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // VoiceActions - management
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Clear actions
        /// </summary>
        public void Clear() {
            _actions.Clear();
        }
        /// <summary>
        /// Map an action
        /// </summary>
        /// <param name="toSentence">Sentence to recognize</param>
        /// <param name="action">Action to map with</param>
        public void Add(string toSentence, AbstractAction action) {
            if (!(_actions.ContainsKey(toSentence))) {
                _actions.Add(toSentence, new List<AbstractAction>());
            }
            _actions[toSentence].Add(action);
        }
        /// <summary>
        /// Remove an sentence
        /// </summary>
        /// <param name="sentence">Sentence desired</param>
        /// <returns>True if sentence was present</returns>
        public bool Remove(string sentence) {
            if (_actions.ContainsKey(sentence)) {
                _actions.Remove(sentence);
                return true;
            } else {
                return false;
            }
        }
        /// <summary>
        /// Remove an associated action of an sentence
        /// </summary>
        /// <param name="toSentence">Sentence desired</param>
        /// <param name="actionIndex">Action index to remove</param>
        /// <returns>True if the action was associated to the sentence</returns>
        public bool RemoveAction(string toSentence, int actionIndex) {
            /* Seek action and erase it */
            if ((actionIndex >= 0) && (_actions.ContainsKey(toSentence))) {
                if (actionIndex < _actions[toSentence].Count) {
                    _actions[toSentence].RemoveAt(actionIndex);
                    return true;
                }
            }
            /* Not removed: return false */
            return false;
        }
        /// <summary>
        /// Number of sentence recognized
        /// </summary>
        public int Count {
            get {
                return _actions.Count;
            }
        }
        /// <summary>
        /// Number of action associated with an sentence
        /// </summary>
        /// <param name="ofSentence">Sentence desired</param>
        /// <returns>Number of action associated with the sentence</returns>
        public int ActionCount(string ofSentence) {
            if (_actions.ContainsKey(ofSentence)) {
                return _actions[ofSentence].Count;
            } else {
                return 0;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // VocalActions - Execution
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute all action of an sentence
        /// </summary>
        /// <param name="onHome"></param>
        /// <param name="ofSentence"></param>
        /// <returns></returns>
        public bool ExecuteAction(Home onHome, string ofSentence) {
            if (_actions.ContainsKey(ofSentence)) {
                foreach(AbstractAction a in _actions[ofSentence]) {
                    a.Execute(onHome);
                }
                return true;
            } else {
                return false;
            }
        }


    }
}
