using Xunit;
using Shouldly;
using System.Diagnostics;
using System;
using ZXing;
using System.Collections.Generic;
using System.IO;

namespace Google.Authenticator.Tests
{
    public class QRCodeAndSetupUrlTests
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
            var rawUrl = setupCodeInfo.SetupUrl;

            Assert.Multiple(() =>
            {
                actualUrl.ShouldBe(expectedUrl, "QR Code Url is not as expected");
                rawUrl.ShouldBe(expectedUrl, "SetupUrl is not as expected");
            });
        }

        [Theory]
        [InlineData("issuer", "otpauth://totp/issuer:a%40b.com?secret=ONSWG4TFOQ&issuer=issuer&algorithm=SHA256")]
        [InlineData("Foo & Bar", "otpauth://totp/Foo%20%26%20Bar:a%40b.com?secret=ONSWG4TFOQ&issuer=Foo%20%26%20Bar&algorithm=SHA256")]
        [InlineData("个", "otpauth://totp/%E4%B8%AA:a%40b.com?secret=ONSWG4TFOQ&issuer=%E4%B8%AA&algorithm=SHA256")]
        public void CanGenerateSHA256QRCode(string issuer, string expectedUrl)
        {
            var subject = new TwoFactorAuthenticator(HashType.SHA256);
            var setupCodeInfo = subject.GenerateSetupCode(
                issuer,
                "a@b.com",
                "secret", 
                false, 
                2);

            var actualUrl = ExtractUrlFromQRImage(setupCodeInfo.QrCodeSetupImageUrl);
            var rawUrl = setupCodeInfo.SetupUrl;

            Assert.Multiple(() =>
            {
                actualUrl.ShouldBe(expectedUrl, "QR Code Url is not as expected");
                rawUrl.ShouldBe(expectedUrl, "SetupUrl is not as expected");
            });
        }

        [Theory]
        [InlineData("issuer", "otpauth://totp/issuer:a%40b.com?secret=ONSWG4TFOQ&issuer=issuer&algorithm=SHA512")]
        [InlineData("Foo & Bar", "otpauth://totp/Foo%20%26%20Bar:a%40b.com?secret=ONSWG4TFOQ&issuer=Foo%20%26%20Bar&algorithm=SHA512")]
        [InlineData("个", "otpauth://totp/%E4%B8%AA:a%40b.com?secret=ONSWG4TFOQ&issuer=%E4%B8%AA&algorithm=SHA512")]
        public void CanGenerateSHA512QRCode(string issuer, string expectedUrl)
        {
            var subject = new TwoFactorAuthenticator(HashType.SHA512);
            var setupCodeInfo = subject.GenerateSetupCode(
                issuer,
                "a@b.com",
                "secret", 
                false, 
                2);

            var actualUrl = ExtractUrlFromQRImage(setupCodeInfo.QrCodeSetupImageUrl);
            var rawUrl = setupCodeInfo.SetupUrl;

            Assert.Multiple(() =>
            {
                actualUrl.ShouldBe(expectedUrl, "QR Code Url is not as expected");
                rawUrl.ShouldBe(expectedUrl, "SetupUrl is not as expected");
            });
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