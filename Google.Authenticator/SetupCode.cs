using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Google.Authenticator
{
    public class SetupCode
    {
        public string Account { get; internal set; }
        public string AccountSecretKey { get; internal set; }
        public string ManualEntryKey { get; internal set; }
        /// <summary>
        /// Base64-encoded PNG image
        /// </summary>
        public string QrCodeSetupImageUrl { get; internal set; }
    }
}
