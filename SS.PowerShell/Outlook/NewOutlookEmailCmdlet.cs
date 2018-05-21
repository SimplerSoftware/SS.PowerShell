using MsgKit;
using MsgKit.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SS.PowerShell.Outlook
{
    /// <summary>
    /// Create a new Email Object for creating a Outlook MSG file.
    /// <para type="synopsis">Create a new Email Object for creating a Outlook MSG file.</para>
    /// <para type="description">Create a new Email Object for creating a Outlook MSG file.</para>
    /// <example>
    ///   <title>Default usage. </title>
    ///   <code>New-OutlookEmail </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "OutlookEmail")]
    [OutputType(typeof(EmailObject))]
    public class NewOutlookEmailCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The full E-mail address of the Sender of the E-mail.</para>
        /// </summary>
        [Parameter()]
        public string SenderEmail { get; set; }

        /// <summary>
        /// <para type="description">The display name of the Sender of the E-mail.</para>
        /// </summary>
        [Parameter()]
        public string SenderDisplayName { get; set; }

        /// <summary>
        /// <para type="description">The subject of the E-mail.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateNotNull]
        public string Subject { get; set; }

        /// <summary>
        /// <para type="description">If the message is set as a draft message.</para>
        /// </summary>
        [Parameter()]
        public SwitchParameter Draft { get; set; } = false;

        /// <summary>
        /// <para type="description">The text body of the E-mail.</para>
        /// </summary>
        [Parameter()]
        public string BodyText { get; set; }

        /// <summary>
        /// <para type="description">The html body of the E-mail.</para>
        /// </summary>
        [Parameter()]
        public string BodyHtml { get; set; }

        /// <summary>
        /// <para type="description">Relative priority of the message.</para>
        /// </summary>
        [Parameter()]
        public EmailImportance Importance { get; set; } = EmailImportance.Normal;

        protected override void ProcessRecord()
        {
            AddressObject sender = null;
            if (!string.IsNullOrWhiteSpace(this.SenderEmail))
                sender = new AddressObject(this.SenderEmail, this.SenderDisplayName);

            var email = new EmailObject(sender, this.Subject, this.Draft)
            {
                BodyText = this.BodyText,
                BodyHtml = this.BodyHtml,
                Importance = this.Importance
                //IconIndex = MessageIconIndex.UnsentMail;
            };

            WriteObject(email);

            base.ProcessRecord();
        }
    }
}
