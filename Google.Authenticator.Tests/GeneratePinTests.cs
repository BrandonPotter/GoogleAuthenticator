using Xunit;
using Shouldly;
using System.Text;
using System.Net.NetworkInformation;

namespace Google.Authenticator.Tests
{
    public class GeneratePinTests
    {
        [Fact]
        public void OverloadsReturnSamePIN()
        {
            var secret = "JBSWY3DPEHPK3PXP";
            var secretAsBytes = Encoding.UTF8.GetBytes(secret);
            var secretAsBase32 = Base32Encoding.ToString(secretAsBytes);
            long counter = 54615912;
            var expected = "508826";

            var subject = new TwoFactorAuthenticator();

            var pinFromString = subject.GeneratePINAtInterval(secret, counter);
            var pinFromBytes = subject.GeneratePINAtInterval(secretAsBytes, counter);
            var pinFromBase32 = subject.GeneratePINAtInterval(secretAsBase32, counter, secretIsBase32: true);

            pinFromString.ShouldBe(expected);
            pinFromBytes.ShouldBe(expected);
            pinFromBase32.ShouldBe(expected);
        }
        [Fact]
        public void OverloadsReturnSamePINSHA256()
        {
            var secret = "JBSWY3DPEHPK3PXP";
            var secretAsBytes = Encoding.UTF8.GetBytes(secret);
            var secretAsBase32 = Base32Encoding.ToString(secretAsBytes);
            long counter = 54615912;
            var expected = "087141";

            var subject = new TwoFactorAuthenticator(HashType.SHA256);

            var pinFromString = subject.GeneratePINAtInterval(secret, counter);
            var pinFromBytes = subject.GeneratePINAtInterval(secretAsBytes, counter);
            var pinFromBase32 = subject.GeneratePINAtInterval(secretAsBase32, counter, secretIsBase32: true);

            pinFromString.ShouldBe(expected);
            pinFromBytes.ShouldBe(expected);
            pinFromBase32.ShouldBe(expected);
        }
        [Fact]
        public void OverloadsReturnSamePINSHA512()
        {
            var secret = "JBSWY3DPEHPK3PXP";
            var secretAsBytes = Encoding.UTF8.GetBytes(secret);
            var secretAsBase32 = Base32Encoding.ToString(secretAsBytes);
            long counter = 54615912;
            var expected = "397230";

            var subject = new TwoFactorAuthenticator(HashType.SHA512);

            var pinFromString = subject.GeneratePINAtInterval(secret, counter);
            var pinFromBytes = subject.GeneratePINAtInterval(secretAsBytes, counter);
            var pinFromBase32 = subject.GeneratePINAtInterval(secretAsBase32, counter, secretIsBase32: true);

            pinFromString.ShouldBe(expected);
            pinFromBytes.ShouldBe(expected);
            pinFromBase32.ShouldBe(expected);
        }
    }
}

// private long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep) =>
            //(long) (now - epoch).TotalSeconds / timeStep;