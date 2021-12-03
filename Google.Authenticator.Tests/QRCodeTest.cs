using Xunit;
using Shouldly;

namespace Google.Authenticator.Tests
{
    public class QRCodeTest
    {
        [Fact]
        public void CanGenerateQRCode()
        {
            // If anything changes in the image generation code, this unit test is going to fail.
            // There are legitimate reasons for this to happen, but when this unit test fails, it means
            // we need to do a manual check to see if the QR Image is still correct before updating this test.
            var expected = "data:image/png;base64,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";

            var subject = new TwoFactorAuthenticator();
            var setupCodeInfo = subject.GenerateSetupCode(
                "issuer",
                "a@b.com",
                "secret", 
                false, 
                2);

            setupCodeInfo.QrCodeSetupImageUrl.ShouldBe(expected);
        }
    }
}