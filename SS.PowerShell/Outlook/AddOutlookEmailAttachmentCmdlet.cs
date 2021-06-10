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
    /// Add attachment to message.
    /// <para type="synopsis">Add attachment to message.</para>
    /// <para type="description">Add attachment to message.</para>
    /// <example>
    ///   <title>Default usage.</title>
    ///   <code>$msg | Add-OutlookEmailAttachment -File ".\directort\file.txt"</code>
    /// </example>
    /// <example>
    ///   <title>Inline content usage.</title>
    ///   <code>$msg | Add-OutlookEmailAttachment -File ".\directort\crocodile.jpg" -Inline -ContentId "tick-tock"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "OutlookEmailAttachment")]
    public class AddOutlookEmailAttachmentCmdlet : PSCmdlet
    {
        private string _File;
        private bool _Continue = true;

        /// <summary>
        /// <para type="description">Email Object to have attachment added.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull]
        public EmailObject Email { get; set; }

        /// <summary>
        /// <para type="description">Filename and/or path of attachment.</para>
        /// </summary>
        [Parameter(Position = 0, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string File { get; set; }

        /// <summary>
        /// <para type="description">Is attachment in-line.</para>
        /// </summary>
        [Parameter()]
        public SwitchParameter Inline { get; set; }

        /// <summary>
        /// <para type="description">What is the content id of the in-line attachment.</para>
        /// </summary>
        [Parameter()]
        public string ContentId { get; set; }

        protected override void BeginProcessing()
        {
            // Convert path to absolute path relative to calling script's/prompt's path
            this._File = this.GetUnresolvedProviderPathFromPSPath(this.File);
            WriteDebug(string.Format("File: {0}", this._File));

            if (!IO.File.Exists(this._File))
            {
                WriteError(new ErrorRecord(new IO.FileNotFoundException($"File {this.File} doesn't exist!"), "FileNotFound,SS.PowerShell.AddOutlookEmailAttachmentCmdlet", ErrorCategory.ObjectNotFound, this._File));
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

            this.Email.Attachments.Add(new AttachmentObject {
                File = this._File,
                Inline = this.Inline,
                ContentId = this.ContentId
            });

            base.ProcessRecord();
        }

    }
}
