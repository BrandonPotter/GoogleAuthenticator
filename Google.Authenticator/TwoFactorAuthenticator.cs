using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Google.Authenticator
{
    /// <summary>
    /// modified from
    /// http://brandonpotter.com/2014/09/07/implementing-free-two-factor-authentication-in-net-using-google-authenticator/
    /// https://github.com/brandonpotter/GoogleAuthenticator
    /// With elements borrowed from https://github.com/stephenlawuk/GoogleAuthenticator
    /// </summary>
    public class TwoFactorAuthenticator
    {
        private static readonly DateTime _epoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private TimeSpan DefaultClockDriftTolerance { get; set; }

        public TwoFactorAuthenticator() => DefaultClockDriftTolerance = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Generate a setup code for a Google Authenticator user to scan
        /// </summary>
        /// <param name="issuer">Issuer ID (the name of the system, i.e. 'MyApp'),
        /// can be omitted but not recommended https://github.com/google/google-authenticator/wiki/Key-Uri-Format
        /// </param>
        /// <param name="accountTitleNoSpaces">Account Title (no spaces)</param>
        /// <param name="accountSecretKey">Account Secret Key</param>
        /// <param name="secretIsBase32">Flag saying if accountSecretKey is in Base32 format or original secret</param>
        /// <param name="qrPixelsPerModule">Number of pixels per QR Module (2 pixels give ~ 100x100px QRCode,
        /// should be 10 or less)</param>
        /// <returns>SetupCode object</returns>
        public SetupCode GenerateSetupCode(
            string issuer,
            string accountTitleNoSpaces,
            string accountSecretKey,
            bool secretIsBase32,
            int qrPixelsPerModule = 3)
        {
            var key = secretIsBase32
                ? Base32Encoding.ToBytes(accountSecretKey)
                : Encoding.UTF8.GetBytes(accountSecretKey);

            return GenerateSetupCode(issuer, accountTitleNoSpaces, key, qrPixelsPerModule);
        }

        /// <summary>
        /// Generate a setup code for a Google Authenticator user to scan
        /// </summary>
        /// <param name="issuer">Issuer ID (the name of the system, i.e. 'MyApp'), can be omitted but not
        /// recommended https://github.com/google/google-authenticator/wiki/Key-Uri-Format </param>
        /// <param name="accountTitleNoSpaces">Account Title (no spaces)</param>
        /// <param name="accountSecretKey">Account Secret Key as byte[]</param>
        /// <param name="qrPixelsPerModule">Number of pixels per QR Module
        /// (2 = ~120x120px QRCode, should be 10 or less)</param>
        /// <param name="generateQrCode"></param>
        /// <returns>SetupCode object</returns>
        public SetupCode GenerateSetupCode(
            string issuer,
            string accountTitleNoSpaces,
            byte[] accountSecretKey,
            int qrPixelsPerModule = 3,
            bool generateQrCode = true)
        {
            if (string.IsNullOrWhiteSpace(accountTitleNoSpaces))
            {
                throw new NullReferenceException("Account Title is null");
            }

            accountTitleNoSpaces = RemoveWhitespace(Uri.EscapeUriString(accountTitleNoSpaces));
            var encodedSecretKey = Base32Encoding.ToString(accountSecretKey);

            var provisionUrl = string.IsNullOrWhiteSpace(issuer)
                ? $"otpauth://totp/{accountTitleNoSpaces}?secret={encodedSecretKey.Trim('=')}"
                //  https://github.com/google/google-authenticator/wiki/Conflicting-Accounts
                // Added additional prefix to account otpauth://totp/Company:joe_example@gmail.com
                // for backwards compatibility
                : $"otpauth://totp/{UrlEncode(issuer)}:{accountTitleNoSpaces}?secret={encodedSecretKey.Trim('=')}&issuer={UrlEncode(issuer)}";

            return new SetupCode(
                accountTitleNoSpaces,
                encodedSecretKey.Trim('='),
                generateQrCode ? GenerateQrCodeUrl(qrPixelsPerModule, provisionUrl) : "");
        }

        private static string GenerateQrCodeUrl(int qrPixelsPerModule, string provisionUrl)
        {
            var qrCodeUrl = "";
            try
            {
                using (var qrGenerator = new QRCodeGenerator())
                using (var qrCodeData = qrGenerator.CreateQrCode(provisionUrl, QRCodeGenerator.ECCLevel.Q))
                using (var qrCode = new QRCode(qrCodeData))
                using (var qrCodeImage = qrCode.GetGraphic(qrPixelsPerModule))
                using (var ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    qrCodeUrl = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
                }
            }
            catch (TypeInitializationException e)
            {
                if (e.InnerException != null
                    && e.InnerException.GetType() == typeof(DllNotFoundException)
                    && e.InnerException.Message.Contains("libgdiplus"))
                {
                    throw new MissingDependencyException(
                        "It looks like libgdiplus has not been installed - see" +
                        " https://github.com/codebude/QRCoder/issues/227",
                        e);
                }
            }
            catch (System.Runtime.InteropServices.ExternalException e)
            {
                if (e.Message.Contains("GDI+") && qrPixelsPerModule > 10)
                {
                    throw new QRException(
                        $"There was a problem generating a QR code. The value of {nameof(qrPixelsPerModule)}" +
                        " should be set to a value of 10 or less for optimal results.",
                        e);
                }
            }

            return qrCodeUrl;
        }

        private static string RemoveWhitespace(string str) =>
            new string(str.Where(c => !char.IsWhiteSpace(c)).ToArray());

        private string UrlEncode(string value)
        {
            var result = new StringBuilder();
            var validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            foreach (var symbol in value)
            {
                if (validChars.IndexOf(symbol) == -1)
                {
                    result.AppendFormat("%{0:X2}", (int) symbol);
                }
                else
                {
                    result.Append(symbol);
                }
            }

            return result.Replace(" ", "%20").ToString();
        }

        public string GeneratePINAtInterval(
            string accountSecretKey,
            long counter,
            int digits = 6,
            bool secretIsBase32 = false)
        {
            return GenerateHashedCode(accountSecretKey, counter, secretIsBase32, digits);
        }

        private string GenerateHashedCode(string secret,
            long iterationNumber,
            bool secretIsBase32,
            int digits = 6)
        {
            return GenerateHashedCode(
                secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret),
                iterationNumber,
                digits);
        }

        private string GenerateHashedCode(byte[] key, long iterationNumber, int digits = 6)
        {
            var counter = BitConverter.GetBytes(iterationNumber);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(counter);

            var hmac = new HMACSHA1(key);
            var hash = hmac.ComputeHash(counter);
            var offset = hash[hash.Length - 1] & 0xf;

            // Convert the 4 bytes into an integer, ignoring the sign.
            var binary =
                ((hash[offset] & 0x7f) << 24)
                | (hash[offset + 1] << 16)
                | (hash[offset + 2] << 8)
                | hash[offset + 3];

            var password = binary % (int) Math.Pow(10, digits);
            return password.ToString(new string('0', digits));
        }

        private long GetCurrentCounter() => GetCurrentCounter(DateTime.UtcNow, _epoch, 30);

        private long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep) =>
            (long) (now - epoch).TotalSeconds / timeStep;

        public bool ValidateTwoFactorPIN(
            string accountSecretKey,
            string twoFactorCodeFromClient,
            bool secretIsBase32 = false)
        {
            return ValidateTwoFactorPIN(accountSecretKey, twoFactorCodeFromClient, DefaultClockDriftTolerance,
                secretIsBase32);
        }

        public bool ValidateTwoFactorPIN(
            string accountSecretKey,
            string twoFactorCodeFromClient,
            TimeSpan timeTolerance,
            bool secretIsBase32 = false)
        {
            return GetCurrentPINs(accountSecretKey, timeTolerance, secretIsBase32)
                .Any(c => c == twoFactorCodeFromClient);
        }

        public string GetCurrentPIN(string accountSecretKey, bool secretIsBase32 = false) =>
            GeneratePINAtInterval(accountSecretKey, GetCurrentCounter(), secretIsBase32: secretIsBase32);

        public string GetCurrentPIN(string accountSecretKey, DateTime now, bool secretIsBase32 = false) =>
            GeneratePINAtInterval(accountSecretKey, GetCurrentCounter(now, _epoch, 30));

        public string[] GetCurrentPINs(string accountSecretKey, bool secretIsBase32 = false) =>
            GetCurrentPINs(accountSecretKey, DefaultClockDriftTolerance, secretIsBase32);

        public string[] GetCurrentPINs(string accountSecretKey, TimeSpan timeTolerance, bool secretIsBase32 = false)
        {
            var codes = new List<string>();
            var iterationCounter = GetCurrentCounter();
            var iterationOffset = 0;

            if (timeTolerance.TotalSeconds > 30)
            {
                iterationOffset = Convert.ToInt32(timeTolerance.TotalSeconds / 30.00);
            }

            var iterationStart = iterationCounter - iterationOffset;
            var iterationEnd = iterationCounter + iterationOffset;

            for (var counter = iterationStart; counter <= iterationEnd; counter++)
            {
                codes.Add(GeneratePINAtInterval(accountSecretKey, counter, secretIsBase32: secretIsBase32));
            }

            return codes.ToArray();
        }
    }
}