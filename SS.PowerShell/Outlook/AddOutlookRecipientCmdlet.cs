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
    /// Add recipient to message.
    /// <para type="synopsis">Add recipient to message.</para>
    /// <para type="description">Add recipient to message.</para>
    /// <example>
    ///   <title>Default usage.</title>
    ///   <code>$msg | Add-OutlookRecipient -EmailAddress hook@example.com -DisplayName "Captain"</code>
    /// </example>
    /// <example>
    ///   <title>Add CC usage.</title>
    ///   <code>$msg | Add-OutlookRecipient -EmailAddress smee@example.com -DisplayName "Smee" -Type CC</code>
    /// </example>
    /// <example>
    ///   <title>Add BCC usage.</title>
    ///   <code>$msg | Add-OutlookRecipient -EmailAddress lostboys@example.com -DisplayName "Lost Boys" -Type BCC</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "OutlookRecipient")]
    public class AddOutlookRecipientCmdlet : PSCmdlet
    {
        private bool _Continue = true;

        /// <summary>
        /// <para type="description">Email Object to have recipient added.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull]
        public EmailObject Email { get; set; }

        /// <summary>
        /// <para type="description">Email address of recipient.</para>
        /// </summary>
        [Parameter(Position = 0, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string EmailAddress { get; set; }

        /// <summary>
        /// <para type="description">Display name of recipient.</para>
        /// </summary>
        [Parameter(Position = 1, Mandatory = false)]
        public string DisplayName { get; set; }

        /// <summary>
        /// <para type="description">Type of recipient for the message.</para>
        /// </summary>
        [Parameter(Position = 2, Mandatory = false)]
        public RecipientType Type { get; set; } = RecipientType.To;

        protected override void BeginProcessing()
        {


            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            if (Email.To.Any(r => r.EmailAddress.Equals(this.EmailAddress, StringComparison.OrdinalIgnoreCase)) ||
                Email.CC.Any(r => r.EmailAddress.Equals(this.EmailAddress, StringComparison.OrdinalIgnoreCase)) ||
                Email.BCC.Any(r => r.EmailAddress.Equals(this.EmailAddress, StringComparison.OrdinalIgnoreCase)))
            {
                WriteError(new ErrorRecord(new Exception($"Recipient with email address {this.EmailAddress} already added to message!"), "DuplicateRecipient,SS.PowerShell.AddOutlookRecipientCmdlet", ErrorCategory.ResourceExists, this.EmailAddress));
                _Continue = false;
            }

            WriteDebug(string.Format("Continue: {0}", _Continue));
            if (!_Continue)
            {
                return;
            }
            switch (this.Type)
            {
                case RecipientType.CC:
                    this.Email.CC.Add(new AddressObject(this.EmailAddress, $"{this.DisplayName}"));
                    break;
                case RecipientType.BCC:
                    this.Email.BCC.Add(new AddressObject(this.EmailAddress, $"{this.DisplayName}"));
                    break;
                default:
                    this.Email.To.Add(new AddressObject(this.EmailAddress, $"{this.DisplayName}"));
                    break;
            }

            base.ProcessRecord();
        }

    }
}
