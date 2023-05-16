using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace GoogleAuthenticator.WebCoreSample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string QRImageUrl;
        public string Key;
        public string ManualEntryKey;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;

        }

        public void OnGet()
        {
            byte[] key = new byte[6];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            Key = Convert.ToBase64String(key);

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            SetupCode setupInfo = tfa.GenerateSetupCode("我 & You", "user@example.com", Key, false, 3);
            QRImageUrl = setupInfo.QrCodeSetupImageUrl;
            ManualEntryKey = setupInfo.ManualEntryKey;
        }

        public void OnPost()
        {
            Key = Request.Form["Key"];
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            bool result = tfa.ValidateTwoFactorPIN(Key, Request.Form["txtCode"]);

            SetupCode setupInfo = tfa.GenerateSetupCode("我 & You", "user@example.com", Key, false, 3);
            QRImageUrl = setupInfo.QrCodeSetupImageUrl;
            ManualEntryKey = setupInfo.ManualEntryKey;

            TempData["ValidationResult"] = $"{Request.Form["txtCode"]} is {(result ? "" : "not ")}a valid PIN at UTC time {DateTime.UtcNow.ToString()}";
        }
    }
}