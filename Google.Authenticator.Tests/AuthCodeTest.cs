using System;
using Xunit;
using Shouldly;

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
    }
}
