using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    /// <summary>
    /// Represent an generic action.
    /// </summary>
    /// <remarks>Authors: Dorian RODDE, Vivian RODDE</remarks>
    public abstract class AbstractAction
    {
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Execute action
        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute the action
        /// </summary>
        /// <param name="onHome">Home desired</param>
        /// <returns>Home response</returns>
        public abstract HomeResponse Execute(Home onHome);

    }
}
