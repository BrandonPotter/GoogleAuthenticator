# GoogleAuthenticator
Simple, easy to use server-side two-factor authentication library for .NET that works with Google Authenticator

[![Build Status](https://dev.azure.com/brandon-potter/GoogleAuthenticator/_apis/build/status/BrandonPotter.GoogleAuthenticator?branchName=master)](https://dev.azure.com/brandon-potter/GoogleAuthenticator/_build/latest?definitionId=1&branchName=master)

[`Install-Package GoogleAuthenticator`](https://www.nuget.org/packages/GoogleAuthenticator)

See blog post for usage instructions *(1.x only - does not apply to 2.x, see [Wiki](https://github.com/BrandonPotter/GoogleAuthenticator/wiki) for 2.x)*:

https://csharprookie.wordpress.com/2015/03/17/implementing-free-two-factor-authentication-in-net-using-google-authenticator/

# Notes
On linux, you need to ensure `libgdiplus` is installed if you want to generate QR Codes. See [https://github.com/codebude/QRCoder/issues/227](https://github.com/codebude/QRCoder/issues/227).
