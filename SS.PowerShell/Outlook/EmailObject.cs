using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgKit.Enums;

namespace SS.PowerShell.Outlook
{
    /// <summary>
    /// <para type="description">Used to make a new Outlook E-mail MSG file.</para>
    /// </summary>
    public class EmailObject
    {
        public AddressObject Sender { get; internal set; }
        public string Subject { get; set; }
        public bool Draft { get; set; }
        public List<AttachmentObject> Attachments { get; internal set; } = new List<AttachmentObject>();
        public List<AddressObject> To { get; internal set; } = new List<AddressObject>();
        public List<AddressObject> CC { get; internal set; } = new List<AddressObject>();
        public List<AddressObject> BCC { get; internal set; } = new List<AddressObject>();

        public EmailObject(AddressObject sender, string subject, bool draft)
        {
            this.Sender = sender;
            this.Subject = subject;
            this.Draft = draft;
        }

        public string BodyText { get; set; }
        public string BodyHtml { get; set; }
        public EmailImportance Importance { get; set; }
    }
}
