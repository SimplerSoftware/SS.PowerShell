using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SS.PowerShell.Outlook
{
    public class AttachmentObject
    {
        public string File { get; internal set; }
        public string ContentId { get; internal set; } = "";
        public bool Inline { get; internal set; }
    }
}
