using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// String utils static functions
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    class StringUtils
    {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Static - Convert functions
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert a string encoded in UTF8 to ASCII. Example "Biblioth\u00e8que" -> "Bibliothèque"
        /// </summary>
        /// <param name="utf8EncodedString">UTF8 string to convert</param>
        /// <returns>ASCII string</returns>
        static public string UTF8ToASCII(string utf8EncodedString) {
            //return string.Format(utf8EncodedString);
            StringBuilder result = new StringBuilder(utf8EncodedString);
            for (int i = 0; i < result.Length; i++) {
                if ((i + 6 < result.Length) && (result[i] == '\\') && (result[i + 1] == 'u') && (result[i + 2] == '0') && (result[i + 3] == '0')) {
                    /* Retrieve 2 last char: Hex number */
                    char c1 = result[i + 4];
                    char c2 = result[i + 5];
                    int hex1, hex2;
                    if ((c1 >= '0') && (c1 <= '9')) {
                        hex1 = c1 - '0';
                    } else if ((c1 >= 'a') && (c1 <= 'f')) {
                        hex1 = c1 - 'a' + 10;
                    } else if ((c1 >= 'A') && (c1 <= 'F')) {
                        hex1 = c1 - 'A' + 10;
                    } else {
                        /* Not hex digit: step to the next one */
                        continue;
                    }
                    if ((c2 >= '0') && (c2 <= '9')) {
                        hex2 = c2 - '0';
                    } else if ((c2 >= 'a') && (c2 <= 'f')) {
                        hex2 = c2 - 'a' + 10;
                    } else if ((c2 >= 'A') && (c2 <= 'F')) {
                        hex2 = c2 - 'A' + 10;
                    } else {
                        /* Not hex digit: step to the next one */
                        continue;
                    }
                    /* Replace in result by ascii char */
                    result[i] = (char)((hex1 * 16) + hex2);
                    result.Remove(i + 1, 5);
                }
            }
            return result.ToString();
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
        /// Convert a string into an integer (take in account '.' and ',')
        /// </summary>
        /// <param name="str">String desired</param>
        /// <returns>Integer number of the string</returns>
        static public int StringToInt(string str) {
            int i;
            try {
                i = Convert.ToInt32(StringToDouble(str));
            } catch {
                i = 0;
            }
            return i;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Static - Compare functions
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Test if two string are identical
        /// </summary>
        /// <param name="str1">First string to compare</param>
        /// <param name="str2">Second string to compare</param>
        /// <param name="ignoreCase">True to ignore case during comparaison</param>
        /// <param name="ignoreAccent">True to ignore accent during comparaison</param>
        /// <returns>True if string identical</returns>
        static public bool IdenticalString(string str1, string str2, bool ignoreCase = false, bool ignoreAccent = false) {
            /* Initialize */
            if (str1.Length != str2.Length) return false;
            if (!ignoreAccent) {
                return (string.Compare(str1, str2, ignoreCase) == 0);
            }
            /* Compare each char */
            for (int i = 0; i < str1.Length; i++) {
                char c1 = RemoveAccent(str1[i]);
                char c2 = RemoveAccent(str2[i]);
                if (ignoreCase) {
                    if ((c1 >= 'A') && (c1 <= 'Z')) c1 = (char)(c1 + 32);
                    if ((c2 >= 'A') && (c2 <= 'Z')) c2 = (char)(c2 + 32);
                }
                if (c1 != c2) return false;
            }
            /* Done: string identical */
            return true;
        }

        /// <summary>
        /// Convert accent char into corresponding non-accent char
        /// </summary>
        /// <param name="c">Char to convert</param>
        /// <returns>Corresponding non-accent char</returns>
        static public char RemoveAccent(char c) {
            switch (c) {
                /* Upper chars */
                case 'Š': return 'S';
                case 'Ž': return 'Z';
                case 'Ÿ':
                case 'Ý':
                    return 'Y';
                case 'Ç': return 'C';
                case 'Ð': return 'D';
                case 'Ñ': return 'N';
                case 'Ø': return 'O';
                /* Lower chars */
                case 'š': return 's';
                case 'ž': return 'z';
                case 'ÿ':
                case 'ý':
                    return 'y';
                case 'ç': return 'c';
                case 'ñ': return 'n';
                case 'ø': return 'o';
                /* Range chars */
                default:
                    /* Upper chars */
                    if ((c >= 192) && (c <= 197)) {
                        return 'A';
                    } else if ((c >= 200) && (c <= 203)) {
                        return 'E';
                    } else if ((c >= 204) && (c <= 207)) {
                        return 'I';
                    } else if ((c >= 210) && (c <= 214)) {
                        return 'O';
                    } else if ((c >= 217) && (c <= 220)) {
                        return 'U';
                    /* Lower chars */
                    }else if ((c >= 224) && (c <= 229)) {
                        return 'a';
                    } else if ((c >= 232) && (c <= 235)) {
                        return 'e';
                    } else if ((c >= 236) && (c <= 239)) {
                        return 'i';
                    } else if ((c >= 242) && (c <= 246)) {
                        return 'o';
                    } else if ((c >= 249) && (c <= 252)) {
                        return 'u';
                    } else {
                        return c;
                    }
            }
        }

    }
}
