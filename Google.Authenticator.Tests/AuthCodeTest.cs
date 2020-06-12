using System;
using Xunit;
using Shouldly;
using System.Text;

namespace Google.Authenticator.Tests
{
    public class AuthCodeTest
    {
        [Fact]
        public void BasicAuthCodeTest()
        {
            string secretKey = "PJWUMZKAUUFQKJBAMD6VGJ6RULFVW4ZH";
            string expected = "551508";

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            
            long currentTime = 1416643820;

            // I actually think you are supposed to divide the time by 30 seconds? Maybe need an overload that takes a DateTime?
            var actual = tfa.GeneratePINAtInterval(secretKey, currentTime, 6);

            actual.ShouldBe(expected);   
        }

        [Fact]
        public void ValidateTwoFactorPIN_WithByteArray_Succeeds()
        {
            // Arrange
            var secretKeyByteArray = Encoding.UTF8.GetBytes("PJWUMZKAUUFQKJBAMD6VGJ6RULFVW4ZH");

            var tfa = new TwoFactorAuthenticator();

            long currentTime = GetCurrentCounter();
            var currentCode = tfa.GeneratePINAtInterval(secretKeyByteArray, currentTime, 6);

            // Act/Assert
            Assert.True(tfa.ValidateTwoFactorPIN(secretKeyByteArray, currentCode));
        }

        private static long GetCurrentCounter()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds / 30;
        }
    }
}
