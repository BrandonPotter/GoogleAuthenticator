using System;
using Xunit;
using Shouldly;

namespace Google.Authenticator.Tests
{
    public class QRCodeTest
    {
        [Fact]
        public void CanGenerateQRCode()
        {
            var subject = new TwoFactorAuthenticator();
            var setupCodeInfo = subject.GenerateSetupCode("issuer","a@b.com","secret", false, 2);
            setupCodeInfo.QrCodeSetupImageUrl.ShouldNotBeNull();
        }
    }
}