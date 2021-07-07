namespace Google.Authenticator
{
    public class SetupCode
    {
        public string Account { get; internal set; }
        public string ManualEntryKey { get; internal set; }
        /// <summary>
        /// Base64-encoded PNG image
        /// </summary>
        public string QrCodeSetupImageUrl { get; internal set; }

        public SetupCode() { }

        public SetupCode(string account, string manualEntryKey, string qrCodeSetupImageUrl)
        {
            Account = account;
            ManualEntryKey = manualEntryKey;
            QrCodeSetupImageUrl = qrCodeSetupImageUrl;
        }
    }
}