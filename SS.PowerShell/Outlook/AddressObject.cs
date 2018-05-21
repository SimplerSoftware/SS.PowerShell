using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.PowerShell.Outlook
{
    public class AddressObject
    {
        public string EmailAddress { get; set; }

        public string DisplayName { get; set; }

        public AddressObject(string EmailAddress, string DisplayName)
        {
            this.EmailAddress = EmailAddress;
            this.DisplayName = DisplayName;
        }
    }
}
