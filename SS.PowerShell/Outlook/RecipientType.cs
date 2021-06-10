using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.PowerShell.Outlook
{
    /// <summary>
    /// Contains the type of recipient for a message.
    /// <para type="description">Contains the type of recipient for a message.</para>
    /// </summary>
    public enum RecipientType
    {
        /// <summary>
        /// Standard To recipient.
        /// </summary>
        To = 0,
        /// <summary>
        /// Carbon copy(CC) recipient.
        /// </summary>
        CC = 1,
        /// <summary>
        /// Blind carbon copy(BCC) recipient.
        /// </summary>
        BCC = 2,
    }
}
