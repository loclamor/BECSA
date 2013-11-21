using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome
{
    /// <summary>
    /// JSON reader
    /// </summary>
    public class JSON
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Protected enums/structures
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Type of token availible
        /// </summary>
        protected enum TokenType
        {
            /// <summary>
            /// Simple word, format: ("_" | [a-z] | [A-Z]) + ("_" | [a-z] | [A-Z] | [0-9])*
            /// </summary>
            TOK_WORD,
            /// <summary>
            /// Symbol char.
            /// </summary>
            TOK_SYMBOL,
            /// <summary>
            /// Number, format: (0 + "." + [0-9]*) | [0-9]*
            /// </summary>
            TOK_NUMBER,
            /// <summary>
            /// String, format: "\"" + CHARS + "\""
            /// </summary>
            /// <remarks>This parser respect the escaped sequence defined by JSON.</remarks>
            TOK_STRING,
            /// <summary>
            /// End of File. Always the last token returned by <see cref="NextToken(string, Token)">NextToken</see>.
            /// </summary>
            TOK_EOF
        }
        /// <summary>
        /// Structure representing a Token used by inter alia <see cref="NextToken(string, Token)">NextToken</see>
        /// </summary>
        protected class Token
        {
            /// <summary>
            /// Type of this token
            /// </summary>
            public TokenType type;
            /// <summary>
            /// Start position of this token (used mainly for <see cref="NextToken(string, Token)">NextToken</see>)
            /// </summary>
            public int start;
            /// <summary>
            /// Length of this token (used mainly for <see cref="NextToken(string, Token)">NextToken</see>)
            /// </summary>
            public int len;
            /// <summary>
            /// Value of this token
            /// </summary>
            public string value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Enum
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Value availible in JSON
        /// </summary>
        public enum ValueType
        {
            /// <summary>
            /// Empty JSON Object (cf. json.org :: value empty)
            /// </summary>
            EMPTY,
            /// <summary>
            /// String JSON Object: "..." (cf. json.org :: value string)
            /// </summary>
            STRING,
            /// <summary>
            /// Number JSON Object (cf. json.org :: value number)
            /// </summary>
            NUMBER,
            /// <summary>
            /// JSON Object: {...} (cf. json.org :: value object)
            /// </summary>
            OBJECT,
            /// <summary>
            /// Array JSON Object: [...] (cf. json.org :: value array)
            /// </summary>
            ARRAY,
            /// <summary>
            /// Boolean JSON Object: true or false (cf. json.org :: value true, false)
            /// </summary>
            BOOLEAN
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Parent of this JSON object. Can be null.
        /// </summary>
        /// <remarks>If return null Then Root element</remarks>
        public JSON Parent { get; protected set; }
        /// <summary>
        /// Private list of JSON childs.
        /// </summary>
        /// <remarks>Concern only JSON Object and JSON Array, since other type cannot have named child (cf. json specification)</remarks>
        private List<JSON> _childs;
        /// <summary>
        /// Name of this JSON object
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Type of this JSON Object (cf. ValueType, json specification)
        /// </summary>
        public ValueType Type { get; protected set; }
        /// <summary>
        /// String value of this JSON object
        /// </summary>
        /// <remarks>To get a usable value in specific format you can use the helpers methods:
        ///             <see cref="GetBoolValue">GetBoolValue</see>, 
        ///             <see cref="GetIntValue">GetIntValue</see>, 
        ///             <see cref="GetNumberValue">GetNumberValue</see>, 
        ///             <see cref="GetStringValue">GetStringValue</see>.
        /// </remarks>
        public string Value {  get; protected set; }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructors
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Construct an empty JSON Object
        /// </summary>
        /// <remarks>Since this class is only a Reader of JSON, the next step from this point is to call <code>Parse</code> method.</remarks>
        public JSON() {
            Parent = null;
            _childs = new List<JSON>();
            Name = "";
            Type = ValueType.EMPTY;
            Value = "";
        }
        /// <summary>
        /// Construct a JSON Object from a string
        /// </summary>
        /// <remarks>Equivalent to using <code>Parse</code> method</remarks>
        /// <param name="str">String to parse</param>
        public JSON(string str) 
            : this()
        {
            Parse(str);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Childs
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get a child by his position in the child list.
        /// </summary>
        /// <remarks>Concern only JSON Array and JSON Object, since other type cannot have named child (cf. json specification)</remarks>
        /// <param name="id">Position of the desired child</param>
        /// <returns>Child in question Or this object if child not found.</returns>
        public JSON Get(int id) {
            if ((id >= 0) && (id < _childs.Count)) {
                return _childs[id];
            } else {
                return this;
            }
        }
        /// <summary>
        /// Get a child by his name
        /// </summary>
        /// <remarks>Concern only JSON Object, since other type cannot have named child (cf. json specification)</remarks>
        /// <param name="name">Name of the desired child</param>
        /// <returns>Child in question Or this object if child not found.</returns>
        public JSON Get(string name) {
            foreach (JSON c in _childs) {
                if (string.Compare(c.Name, name) == 0) {
                    return c;
                }
            }
            return this;
        }
        /// <summary>
        /// Get child count
        /// </summary>
        /// <remarks>Concern only JSON Object and JSON Array, since other type cannot have child (cf. json specification)</remarks>
        public int Count {
            get {
                return _childs.Count;
            }
        }
        


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Value
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Retrieve boolean value of this JSON Object
        /// </summary>
        /// <param name="defaultValue">Value returned if this JSON Object is an Empty object</param>
        /// <returns>Boolean value of this JSON Object Or defaultValue</returns>
        public bool GetBoolValue(bool defaultValue = false) {
            if (Type != ValueType.EMPTY) {
                return (string.Compare(Value, "true", true) == 0);
            } else {
                return defaultValue;
            }
        }
        /// <summary>
        /// Retrieve integer value of this JSON Object
        /// </summary>
        /// <param name="defaultValue">Value returned if this JSON Object is an Empty object</param>
        /// <returns>Integer value of this JSON Object Or defaultValue</returns>
        public int GetIntValue(int defaultValue = 0) {
            if (Type != ValueType.EMPTY) {
                try {
                    return int.Parse(Value);
                } catch {
                    return 0;
                }
            } else {
                return defaultValue;
            }
        }
        /// <summary>
        /// Retrieve double value of this JSON Object
        /// </summary>
        /// <param name="defaultValue">Value returned if this JSON Object is an Empty object</param>
        /// <returns>Double value of this JSON Object Or defaultValue</returns>
        public double GetNumberValue(double defaultValue = 0) {
            if (Type != ValueType.EMPTY) {
                try {
                    return double.Parse(Value);
                } catch {
                    return 0;
                }
            } else {
                return defaultValue;
            }
        }
        /// <summary>
        /// Retrieve string value of this JSON Object
        /// </summary>
        /// <param name="defaultValue">Value returned if this JSON Object is an Empty object</param>
        /// <returns>String value of this JSON Object Or defaultValue</returns>
        public string GetStringValue(string defaultValue = "") {
            if (Type != ValueType.EMPTY) {
                return Value;
            } else {
                return defaultValue;
            }
        }
        
        

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Parse
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Load JSON data from a string
        /// </summary>
        /// <param name="str">String to load</param>
        /// <returns>True if no errors encountered</returns>
        public bool Parse(string str) {
            /* Initialize */
            Token tok = new Token();
            tok.start = 0;
            tok.len = 0;
            _childs.Clear();
            Value = "";
            Parent = null;
            /* Read the first '{' */ 
            tok = NextToken(str, tok);
            if (!IsTokenEgalTo(tok, "{")) return false;
            /* Analyse each tokens */
            Type = ValueType.OBJECT;
            tok = NextToken(str, tok);
            return privParseChild(str, ref tok);
        }
        /// <summary>
        /// Load JSON data from a string
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <param name="tok">Current token to analyse</param>
        /// <returns>True if parsed without errors (correct syntax, etc ...)</returns>
        private bool privParse(string str, ref Token tok) {
            /* JSON: Read Name */ 
            if ((Parent == null) || (Parent.Type == ValueType.OBJECT)) {
                /* No parent Or Or Parent wait object: Wait child with name */ 
                if (tok.type != TokenType.TOK_STRING) return false;
                Name = GetTokenValue(tok);
                /* JSON: Read ':' */ 
                tok = NextToken(str, tok);
                if (!IsTokenEgalTo(tok, ":")) return false;
                /* JSON: Read Value */ 
                tok = NextToken(str, tok);
            }
            /* JSON: Read Value */
            if (tok.type == TokenType.TOK_STRING) {
                /* No child: String value */
                Type = ValueType.STRING;
                Value = GetTokenValue(tok);
            } else if (tok.type == TokenType.TOK_NUMBER) {
                /* No child: Number value */
                Type = ValueType.NUMBER;
                Value = GetTokenValue(tok);
            } else if ((IsTokenEgalTo(tok, "true")) || (IsTokenEgalTo(tok, "false"))) {
                /* No child: Boolean value */
                Type = ValueType.BOOLEAN;
                Value = GetTokenValue(tok);
            } else if (IsTokenEgalTo(tok, "{")) {
                /* Child */
                Type = ValueType.OBJECT;
                tok = NextToken(str, tok);
            } else if (IsTokenEgalTo(tok, "[")) {
                /* Array child */
                Type = ValueType.ARRAY;
                tok = NextToken(str, tok);
            } else {
                /* EOF or Illegal Char: Error! */
                return false;
            }
            /* JSON: Read Child if have child */
            return (privParseChild(str, ref tok));
        }
        /// <summary>
        /// Load JSON childs from a string
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <param name="tok">Current token to analyse</param>
        /// <returns>True if parsed without errors (correct syntax, etc ...)</returns>
        private bool privParseChild(string str, ref Token tok) {
            /* Parse child */ 
            if ((Type == ValueType.OBJECT) || (Type == ValueType.ARRAY)) {
                while (true) {
                    /* Read Token */
                    if ((IsTokenEgalTo(tok, "}")) || (IsTokenEgalTo(tok, "]"))) {
                        /* End of Child */
                        break;
                    }
                    /* Add Child & Parse */
                    JSON child = new JSON();
                    _childs.Add(child);
                    child.Parent = this;
                    if (!(child.privParse(str, ref tok))) {
                        /* Error! */
                        return false;
                    }
                    /* JSON Read: ',' | '}' | ']' */
                    tok = NextToken(str, tok);
                    if ((IsTokenEgalTo(tok, "}")) || (IsTokenEgalTo(tok, "]"))) {
                        /* End of Child */
                        break;
                    }
                    if (!IsTokenEgalTo(tok, ",")) return false;
                    tok = NextToken(str, tok);
                }
            }
            /* Done: retrun true */
            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // ToString 
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert JSON object to a human-readable string.
        /// </summary>
        /// <returns>String representation of this JSON object</returns>
        override public string ToString() {
            return ToString("");
        }
        /// <summary>
        /// Convert JSON object to a human-readable string.
        /// </summary>
        /// <param name="tabIndent">Indentation used for each new line</param>
        /// <returns>String representation of this JSON object</returns>
        private string ToString(string tabIndent) {
            string tmp = "";
            /* Add name */
            if ((Parent != null) && (Parent.Type != ValueType.ARRAY)) { 
                tmp += "\"" + Name + "\"";
                tmp += ": ";
            }
            /* Add value */
            if (Type == ValueType.STRING) {
                tmp += "\"" + Value + "\"";
            } else if ((Type == ValueType.NUMBER) || (Type == ValueType.BOOLEAN)) {
                tmp += Value;
            }
            /* Begin Child */
            if (_childs.Count > 0) {
                string childTabIndent = tabIndent + "\t";
                if (Type == ValueType.OBJECT) {
                    tmp += "{\n" + childTabIndent;
                } else if (Type == ValueType.ARRAY) {
                    tmp += "[\n" + childTabIndent;
                }
            
                /* Add Child */
                int childId = 0;
                foreach (JSON c in _childs) {
                    if (childId > 0) {
                        tmp += ",\n" + childTabIndent;
                    }
                    tmp += c.ToString(childTabIndent);
                    childId++;
                }
                /* End Child */
                if (Type == ValueType.OBJECT) {
                    tmp += "\n" + tabIndent + "}";
                } else if (Type == ValueType.ARRAY) {
                    tmp += "\n" + tabIndent + "]";
                }
            }
            /* Return result */
            return tmp;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // Protected - Token functions
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Retrieve the token just after a gived token.
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <param name="fromToken">Start after this token</param>
        /// <returns>Token after fromToken</returns>
        protected Token NextToken(string str, Token fromToken) {
            return NextToken(str, fromToken.start + fromToken.len);
        }

        /// <summary>
        /// Retrieve token from a position.
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <param name="fromPos">Start position</param>
        /// <returns>The token at fromPos</returns>
        protected Token NextToken(string str, int fromPos = 0) {
            /* Initialize */
            int mode = 0; /* 0 = Wait Token Type, 1 = Wait String End, 2 = Wait Word End, 3 = Wait Number End */
            int oldPos = 0;
            int numberDotFound = 0;
            Token tok = new Token();
            tok.type = TokenType.TOK_EOF;
            tok.start = str.Length;
            tok.len = 0;
            /* Seek next tokens */
            for (int i = fromPos, max = str.Length; i <= max; i++) {
                /* Get char */ 
                char c = ((i < max) ? (str[i]) : ' ');
                /* Analyse char */ 
                if (mode == 0) {
                    /* 0 - Wait Token Type */
                    switch (c) {
                        case ' ': case '\t': case '\n': case '\r':
                            /* WhiteSpace - ignore */
                            break;
                        case '\"': 
                            /* String */
                            oldPos = i;
                            mode = 1;
                            break;
                        case '-': 
                            /* Can be number or symbol */ 
                            if ((i + 1 < max) && (str[i+1] >= '0') && (str[i+1] <= '9')) {
                                /* Number */ 
                                oldPos = i;
                                numberDotFound = 0;
                                mode = 3;
                            } else {
                                /* Done: Symbol */ 
                                tok.type = TokenType.TOK_SYMBOL;
                                tok.value = str.Substring(i,1);
                                tok.start = i;
                                tok.len = 1;
                                i = max + 1;
                            }
                            break;
                        default:
                            if ((c == '_') || ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z'))) {
                                /* Word */
                                oldPos = i;
                                mode = 2;
                            } else if ((c >= '0') && (c <= '9')) {
                                /* Number */
                                oldPos = i;
                                numberDotFound = 0;
                                mode = 3;
                            } else {
                                /* Done: Symbol */
                                tok.type = TokenType.TOK_SYMBOL;
                                tok.value = str.Substring(i, 1);
                                tok.start = i;
                                tok.len = 1;
                                i = max + 1;
                            }
                            break;
                    }
                } else if (mode == 1) {
                    /* 1 - Wait String End */
                    if (c == '\\') {
                        /* Ignore next */
                        i++;
                    } else if (c == '\"') {
                        /* Done: End of string found */
                        tok.type = TokenType.TOK_STRING;
                        tok.value = str.Substring(oldPos, i-oldPos+1);
                        tok.start = oldPos;
                        tok.len = i-oldPos+1;
                        i = max + 1;
                    }
                } else if (mode == 2) {
                    /* 2 - Wait Word End */
                    if ((c == '_') || ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z')) || ((c >= '0') && (c <= '9'))) {
                        /* Step */ 
                    } else {
                        /* Done: End of word found */
                        tok.type = TokenType.TOK_WORD;
                        tok.value = str.Substring(oldPos, i - oldPos);
                        tok.start = oldPos;
                        tok.len = i - oldPos;
                        i = max + 1;
                    }
                } else if (mode == 3) {
                    /* 3 - Wait Number End */
                    if ((c == '.') && (numberDotFound == 0)) {
                        numberDotFound++;
                    } else if ((c >= '0') && (c <= '9')) {
                        /* Step */
                    } else {
                        /* Done: End of word found */
                        tok.type = TokenType.TOK_NUMBER;
                        tok.value = str.Substring(oldPos, i - oldPos);
                        tok.start = oldPos;
                        tok.len = i - oldPos;
                        i = max + 1;
                    }
                }
            }
            /* Return token */
            return tok;
        }

        /// <summary>
        /// Check if a token is egal to a value.
        /// </summary>
        /// <param name="tok">Token to test</param>
        /// <param name="value">Value to test</param>
        /// <returns>True if tok == value</returns>
        protected bool IsTokenEgalTo(Token tok, string value) {
            return (string.Compare(tok.value, value) == 0);
        }

        /// <summary>
        /// Retrieve value of a token.
        /// </summary>
        /// <param name="tok">Token considered</param>
        /// <returns>Value of tok</returns>
        protected string GetTokenValue(Token tok) {
            if (tok.type == TokenType.TOK_STRING) {
                /* Remove surround string char And interpret Escape Sequences */ 
                string tmp = tok.value.Substring(1, tok.value.Length - 2);
                for(int i = 0, max = tmp.Length; i < max; i++) {
                    if ((i + 1 < max) && (tmp[i] == '\\')) {
                        /* (cf. json.org :: Escape Sequences */ 
                        switch (tmp[i+1]) {
                            case '\"': case '\\': case '/':
                                tmp.Remove(i,1);
                                max--;
                                break;
                            case 'b':
                                tmp.Remove(i,2).Insert(i,"\b");
                                max--;
                                break;
                            case 'f':
                                tmp.Remove(i,2).Insert(i,"\f");
                                max--;
                                break;
                            case 'n':
                                tmp.Remove(i,2).Insert(i,"\n");
                                max--;
                                break;
                            case 'r':
                                tmp.Remove(i,2).Insert(i,"\r");
                                max--;
                                break;
                            case 't':
                                tmp.Remove(i,2).Insert(i,"\t");
                                max--;
                                break;
                            case 'u':
                                /* Four-hex-digits */
                                // TODO: Implements Four-hex-digits for Escape Sequences full support
                                break;
                        }
                    }
                }
                /* Return String value */ 
                return tmp;
            } else if (tok.type == TokenType.TOK_NUMBER) {
                /* Format number */
                // TODO: Add Format number
                return tok.value;
            } else {
                return tok.value;
            }
        }

    }
}
