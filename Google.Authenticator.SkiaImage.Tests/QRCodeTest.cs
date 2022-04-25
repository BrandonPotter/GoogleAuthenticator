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
        [InlineData("issuer", "otpauth://totp/issuer:a@b.com?secret=ONSWG4TFOQ&issuer=issuer")]
        [InlineData("Foo & Bar", "otpauth://totp/Foo%20%26%20Bar:a@b.com?secret=ONSWG4TFOQ&issuer=Foo%20%26%20Bar")]
        [InlineData("个", "otpauth://totp/%E4%B8%AA:a@b.com?secret=ONSWG4TFOQ&issuer=%E4%B8%AA")]
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
            var headerLength = "data:image/jpeg;base64,".Length;
            var rawImageData = qrCodeSetupImageUrl.Substring(headerLength, qrCodeSetupImageUrl.Length - headerLength);
            var imageData = Convert.FromBase64String(rawImageData);

            var bw = new BinaryWriter(new FileInfo("C:\\temp\\skia_raw.jpg").OpenWrite());
            bw.Write(imageData);
            bw.Flush();
            bw.Close();

            //var reader = new BarcodeReaderGeneric();
            //reader.Options.PossibleFormats = new List<BarcodeFormat> {
            //    BarcodeFormat.QR_CODE
            //};

#if NETFRAMEWORK
            var reader = new BarcodeReader();
            reader.Options.PossibleFormats = new List<BarcodeFormat> {
                BarcodeFormat.QR_CODE
            };
            using (var ms = new MemoryStream(imageData))
            {
                var image = new System.Drawing.Bitmap(ms);
                return reader.Decode(image).Text;
            }
#else
            var reader = new BarcodeReaderGeneric();
            reader.Options.PossibleFormats = new List<BarcodeFormat> {
                BarcodeFormat.QR_CODE
            };
            reader.Options.TryHarder=true;
            reader.Options.TryInverted=true;
            reader.AutoRotate=true;
            var image = new ImageMagick.MagickImage(imageData);
            image.Write(new FileInfo("C:\\temp\\skia_qrcode.jpg"));
            image.Resize(image.Width*3, image.Height*3);
            image.Sharpen(3.0f, 3.0f);
            image.Grayscale();
            image.Write(new FileInfo("C:\\temp\\skia_qrcode_extended.jpg"));
            var wrappedImage = new ZXing.Magick.MagickImageLuminanceSource(image);
            var res = reader.Decode(wrappedImage);
            return res.Text;
#endif
        }
    }
}