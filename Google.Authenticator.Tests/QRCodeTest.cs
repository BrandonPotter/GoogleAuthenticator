using Xunit;
using Shouldly;
using System.Diagnostics;
using System;
using ZXing;
using System.Collections.Generic;
using System.IO;

namespace Google.Authenticator.Tests
{
    public class QRCodeTest
    {
        [Theory]
        [InlineData("issuer", "otpauth://totp/issuer:a%40b.com?secret=ONSWG4TFOQ&issuer=issuer")]
        [InlineData("Foo & Bar", "otpauth://totp/Foo%20%26%20Bar:a%40b.com?secret=ONSWG4TFOQ&issuer=Foo%20%26%20Bar")]
        [InlineData("个", "otpauth://totp/%E4%B8%AA:a%40b.com?secret=ONSWG4TFOQ&issuer=%E4%B8%AA")]
        public void CanGenerateQRCode(string issuer, string expectedUrl)
        {
            var subject = new TwoFactorAuthenticator();
            var setupCodeInfo = subject.GenerateSetupCode(
                issuer,
                "a@b.com",
                "secret", 
                false, 
                2);

            var actualUrl = ExtractUrlFromQRImage(setupCodeInfo.QrCodeSetupImageUrl);

            actualUrl.ShouldBe(expectedUrl);
        }

        private static string ExtractUrlFromQRImage(string qrCodeSetupImageUrl)
        {
            var headerLength = "data:image/png;base64,".Length;
            var rawImageData = qrCodeSetupImageUrl.Substring(headerLength, qrCodeSetupImageUrl.Length - headerLength);
            var imageData = Convert.FromBase64String(rawImageData);

            var reader = new BarcodeReaderGeneric();
            reader.Options.PossibleFormats = new List<BarcodeFormat> {
                BarcodeFormat.QR_CODE
            };
            var image = new ImageMagick.MagickImage(imageData);
            var wrappedImage = new ZXing.Magick.MagickImageLuminanceSource(image);
            return reader.Decode(wrappedImage).Text;
        }
    }
}