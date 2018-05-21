using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.PowerShell.Outlook
{
    /// <summary>
    /// Contains the relative priority of a message.
    /// <para type="description">Contains the relative priority of a message.</para>
    /// </summary>
    public enum EmailImportance
    {
        /// <summary>
        /// The message has low importance.
        /// </summary>
        Low = 0,
        /// <summary>
        /// The message has normal importance.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The message has high importance.
        /// </summary>
        High = 2
    }
}
