# GoogleAuthenticator
Simple, easy to use server-side two-factor authentication library for .NET that works with Google Authenticator

[![Build Status](https://dev.azure.com/brandon-potter/GoogleAuthenticator/_apis/build/status/BrandonPotter.GoogleAuthenticator?branchName=master)](https://dev.azure.com/brandon-potter/GoogleAuthenticator/_build/latest?definitionId=1&branchName=master)
[![NuGet Status](https://buildstats.info/nuget/GoogleAuthenticator)](https://www.nuget.org/packages/GoogleAuthenticator/)

[`Install-Package GoogleAuthenticator`](https://www.nuget.org/packages/GoogleAuthenticator)

## 1.x Usage
See blog post for usage instructions *(1.x only)*:

https://csharprookie.wordpress.com/2015/03/17/implementing-free-two-factor-authentication-in-net-using-google-authenticator/

## 2.x Usage

*Additional examples at [Google.Authenticator.WinTest](https://github.com/BrandonPotter/GoogleAuthenticator/tree/master/Google.Authenticator.WinTest) and [Google.Authenticator.WebSample](https://github.com/BrandonPotter/GoogleAuthenticator/tree/master/Google.Authenticator.WebSample)*

```csharp
using Google.Authenticator;

string key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
SetupCode setupInfo = tfa.GenerateSetupCode("Test Two Factor", "user@example.com", key, false, 3);

string qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;
string manualEntrySetupCode = setupInfo.ManualEntryKey;

imgQrCode.ImageUrl = qrCodeImageUrl;
lblManualSetupCode.Text = manualEntrySetupCode;

// verify
TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
bool result = tfa.ValidateTwoFactorPIN(key, txtCode.Text)
```

## Common Pitfalls

* Old documentation indicated specifying width and height for the QR code, but changes in QR generation now uses pixels per module (QR "pixel") so using a value too high will result in a huge image that can overrun memory allocations
* Don't use the secret key and `ManualEntryKey` interchangeably. `ManualEntryKey` is used to enter into the authenticator app when scanning a QR code is impossible and is derived from the secret key ([discussion example](https://github.com/BrandonPotter/GoogleAuthenticator/issues/54))

# Notes
On linux, you need to ensure `libgdiplus` is installed if you want to generate QR Codes. See [https://github.com/codebude/QRCoder/issues/227](https://github.com/codebude/QRCoder/issues/227).
