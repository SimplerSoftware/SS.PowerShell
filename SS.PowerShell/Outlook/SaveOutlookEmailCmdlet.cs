using MsgKit;
using MsgKit.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using IO=System.IO;

namespace SS.PowerShell.Outlook
{
    /// <summary>
    /// Save a Email Object as a Outlook MSG file.
    /// <para type="synopsis">Save a Email Object as a Outlook MSG file.</para>
    /// <para type="description">Save a Email Object as a Outlook MSG file.</para>
    /// <example>
    ///   <title>Default usage. </title>
    ///   <code>Save-OutlookEmail </code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsData.Save, "OutlookEmail", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
    public class SaveOutlookEmailCmdlet : PSCmdlet
    {
        private string _File;
        private bool _Continue = true;

        /// <summary>
        /// <para type="description">Email Object to be saved as Outlook MSG file.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull]
        public EmailObject Email { get; set; }

        /// <summary>
        /// <para type="description">Filename and/or path for Outlook MSG file.</para>
        /// </summary>
        [Parameter(Position = 0, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string File { get; set; }

        /// <summary>
        /// <para type="description">Force to overwrite file without prompting if it already exists.</para>
        /// </summary>
        [Parameter()]
        public SwitchParameter Force { get; set; } = false;

        protected override void BeginProcessing()
        {
            // Convert path to absolute path relative to calling script's/prompt's path
            this._File = this.GetUnresolvedProviderPathFromPSPath(this.File);
            WriteDebug(string.Format("File: {0}", this._File));

            if (!this.Force && IO.File.Exists(this._File) && !this.ShouldContinue($"File {this.File} already exists, do you want to overwrite?", "Overwrite"))
            {
                WriteError(new ErrorRecord(new IO.IOException($"File {this.File} already exists!", -2147024816), "FileExists,SS.PowerShell.SaveOutlookEmailCmdlet", ErrorCategory.ResourceExists, this._File));
                _Continue = false;
            }

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            WriteDebug(string.Format("Continue: {0}", _Continue));
            if (!_Continue)
            {
                return;
            }

            Sender sender = null;
            if (this.Email.Sender != null)
                sender = new Sender(this.Email.Sender.EmailAddress, this.Email.Sender.DisplayName);
            using (var email = new Email(sender, this.Email.Subject, this.Email.Draft))
            {
                this.Email.To.ForEach(r => email.Recipients.AddTo(r.EmailAddress, r.DisplayName));
                this.Email.CC.ForEach(r => email.Recipients.AddCc(r.EmailAddress, r.DisplayName));
                this.Email.BCC.ForEach(r => email.Recipients.AddBcc(r.EmailAddress, r.DisplayName));
                email.BodyText = this.Email.BodyText;
                email.BodyHtml = this.Email.BodyHtml;
                email.Importance = (MessageImportance)this.Email.Importance;
                //email.IconIndex = this.Email.Draft ? MessageIconIndex.UnsentMail : MessageIconIndex.NewMail;
                email.Priority = MessagePriority.PRIO_NORMAL;
                //email.ReceivedOn;
                //email.SentOn;
                this.Email.Attachments.ForEach(x => email.Attachments.Add(x.File, isInline: x.Inline, contentId: x.ContentId));
                email.Save(this._File);
            }

            base.ProcessRecord();
        }

    }
}
