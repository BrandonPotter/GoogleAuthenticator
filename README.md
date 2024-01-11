# GoogleAuthenticator
Simple, easy to use server-side two-factor authentication library for .NET that works with Google Authenticator

[![Build Status](https://dev.azure.com/brandon-potter/GoogleAuthenticator/_apis/build/status/BrandonPotter.GoogleAuthenticator?branchName=master)](https://dev.azure.com/brandon-potter/GoogleAuthenticator/_build/latest?definitionId=1&branchName=master)
[![NuGet Status](https://buildstats.info/nuget/GoogleAuthenticator)](https://www.nuget.org/packages/GoogleAuthenticator/)

[`Install-Package GoogleAuthenticator`](https://www.nuget.org/packages/GoogleAuthenticator)

## Usage

*Additional examples at [Google.Authenticator.WinTest](https://github.com/BrandonPotter/GoogleAuthenticator/tree/master/Google.Authenticator.WinTest) and [Google.Authenticator.WebSample](https://github.com/BrandonPotter/GoogleAuthenticator/tree/master/Google.Authenticator.WebSample)*

`key` should be stored by your application for future authentication and shouldn't be regenerated for each request. The process of storing the private key is outside the scope of this library and is the responsibility of the application.

```csharp
using Google.Authenticator;

string key;

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

## Update history

### 3.2.0

Added support for HMACSHA256 and HMACSHA512 as per the [RFC spec](https://datatracker.ietf.org/doc/html/rfc6238#section-1.2). In testing it was found that several popular apps (such as Authy and Microsoft Authenticator) may not have support for these algorithms so care should be taken by the developer to ensure compatible apps are used.

### 3.1.1

Fixed an edge case where specifying an interval of 30 seconds to the Validate function would be treated as if you had passed in 0.

### 3.1.0

- Removed .NET 5 and added .NET 7 to test frameworks
- Updated dependencies for test runs
- Support ValidateTwoFactorPIN with iterationOffset as parameter

### 3.0.0

- Removed support for legacy .Net Framework. Lowest supported versions are now netstandard2.0 and .Net 4.6.2.  
- All use of System.Drawing has been removed. In 2.5, only Net 6.0 avoided System.Drawing.
- Linux installations no longer need to ensure `libgdiplus` is installed as it is no longer used.
- Changed from using `EscapeUriString` to `EscapeDataString` to encode the "account title" as the former is [obsolete in .Net 6](https://docs.microsoft.com/en-us/dotnet/fundamentals/syslib-diagnostics/syslib0013). This changes the value in the generated data string from `a@b.com` to `a%40b.com`. We have tested this with Google Authenticator, Lastpass Authenticator and Microsoft Authenticator. All three of them handle it correctly and all three recognise that it is still the same account so this should be safe in most cases.

### 2.5.0

Now runs on .Net 6.0.  
Technically the QR Coder library we rely on still does not fully support .Net 6.0 so it is possible there will be other niggling issues, but for now all tests pass for .Net 6.0 on both Windows and Linux.

## Common Pitfalls

* Old documentation indicated specifying width and height for the QR code, but changes in QR generation now uses pixels per module (QR "pixel") so using a value too high will result in a huge image that can overrun memory allocations
* Don't use the secret key and `ManualEntryKey` interchangeably. `ManualEntryKey` is used to enter into the authenticator app when scanning a QR code is impossible and is derived from the secret key ([discussion example](https://github.com/BrandonPotter/GoogleAuthenticator/issues/54))
* *With versions prior to 3.0 only*, on linux, you need to ensure `libgdiplus` is installed if you want to generate QR Codes. See [https://github.com/codebude/QRCoder/issues/227](https://github.com/codebude/QRCoder/issues/227).
