using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
	/// <summary>
	/// File utils static functions
	/// </summary>
	/// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
	public class FileUtils
	{
		///////////////////////////////////////////////////////////////////////////////////////////
		// Get functions
		///////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Retrieve file content
		/// </summary>
		/// <param name="path">File desired</param>
		/// <returns>Return file content or empty string if error</returns>
		public static string GetFileContent(string path) {
			string content = "";
			try {
				content = File.ReadAllText(path);
			} catch {
				/*NOP*/
				;
			}
			return content;
		}



	}

}
