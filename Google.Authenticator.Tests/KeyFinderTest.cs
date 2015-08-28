using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Google.Authenticator.Tests
{
    [TestClass]
    public class KeyFinderTest
    {
        public static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [TestMethod]
        public void FindIterationNumber()
        {
            string secretKey = "PJWUMZKAUUFQKJBAMD6VGJ6RULFVW4ZH";
            string targetCode = "267762";

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var mins = DateTime.UtcNow.Subtract(_epoch).TotalMinutes;

            long currentTime = 1416643820;

            for (long i = currentTime; i >= 0; i=i-60)
            {
                var result = tfa.GeneratePINAtInterval(secretKey, i, 6);
                if (result == targetCode)
                {
                    Assert.IsTrue(true);
                }
            }

            Assert.IsTrue(false);
        }
    }
}
