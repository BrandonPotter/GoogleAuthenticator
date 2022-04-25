using System.Text;
using Shouldly;
using Xunit;

namespace Google.Authenticator.Tests
{
    public class AuthCodeTest
    {
        [Fact]
        public void BasicAuthCodeTest()
        {
            var secretKey = "PJWUMZKAUUFQKJBAMD6VGJ6RULFVW4ZH";
            var expected = "551508";

            var tfa = new TwoFactorAuthenticator();

            var currentTime = 1416643820;

            // I actually think you are supposed to divide the time by 30 seconds?
            // Maybe need an overload that takes a DateTime?
            var actual = tfa.GeneratePINAtInterval(secretKey, currentTime, 6);

            actual.ShouldBe(expected);
        }
        
        [Fact]
        public void Base32AuthCodeTest()
        {
            var secretKey = Base32Encoding.ToString(Encoding.UTF8.GetBytes("PJWUMZKAUUFQKJBAMD6VGJ6RULFVW4ZH"));
            var expected = "551508";

            var tfa = new TwoFactorAuthenticator();

            var currentTime = 1416643820;

            // I actually think you are supposed to divide the time by 30 seconds?
            // Maybe need an overload that takes a DateTime?
            var actual = tfa.GeneratePINAtInterval(secretKey, currentTime, 6, true);

            actual.ShouldBe(expected);
        }
    }
}