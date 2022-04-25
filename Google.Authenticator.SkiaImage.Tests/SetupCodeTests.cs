using Xunit;
using Shouldly;
using System.Text;

namespace Google.Authenticator.Tests
{
    public class SetupCodeTests
    {
        [Fact]
        public void ByteAndStringGeneratesSameSetupCode()
        {
            var secret = "12345678901234567890123456789012";
            var secretAsByteArray = Encoding.UTF8.GetBytes(secret);
            var secretAsBase32 = Base32Encoding.ToString(secretAsByteArray);
            var issuer = "Test";
            var accountName = "TestAccount";
            var expected = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZA";

            var subject = new TwoFactorAuthenticator();

            var setupCodeFromString = subject.GenerateSetupCode(issuer, accountName, secret, false);
            var setupCodeFromByteArray = subject.GenerateSetupCode(issuer, accountName, secretAsByteArray, 3, false);
            var setupCodeFromBase32 = subject.GenerateSetupCode(issuer, accountName, secretAsBase32, true);

            setupCodeFromString.ManualEntryKey.ShouldBe(expected);
            setupCodeFromByteArray.ManualEntryKey.ShouldBe(expected);
            setupCodeFromBase32.ManualEntryKey.ShouldBe(expected);
        }
    }
}