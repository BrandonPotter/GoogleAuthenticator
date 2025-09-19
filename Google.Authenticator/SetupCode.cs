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

        /// <summary>
        /// The Raw otp:// url
        /// </summary>
        public string SetupUrl { get; internal set; }

        public SetupCode() { }

        public SetupCode(string account, string manualEntryKey, string qrCodeSetupImageUrl, string setupUrl)
        {
            Account = account;
            ManualEntryKey = manualEntryKey;
            QrCodeSetupImageUrl = qrCodeSetupImageUrl;
            SetupUrl = setupUrl;
        }
    }
}