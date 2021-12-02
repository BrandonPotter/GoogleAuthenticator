using Xunit;
using Shouldly;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

namespace Google.Authenticator.Tests
{
    public class ValidationTests
    {
        const string secret = "ggggjhG&^*&^jfSSSddd";
        private readonly static byte[] secretAsBytes = Encoding.UTF8.GetBytes(secret);
        private readonly static string secretAsBase32 = Base32Encoding.ToString(secretAsBytes);

        [Theory]
        [MemberData(nameof(GetPins))]
        public void ValidateWorksWithDifferentSecretTypes(string pin, int irrelevantNumberToAvoidDuplicatePinsBeingRemoved)
        {
            // We can't directly test that the different overloads for GetCurrentPIN creates the same result,
            // as the time difference may may cause different PINS to be created.
            // So instead we generate the PINs by each method and validate each one by each method.
            var subject = new TwoFactorAuthenticator();

            subject.ValidateTwoFactorPIN(secret, pin, false);
            subject.ValidateTwoFactorPIN(secret, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), false);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved));
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, true);
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, TimeSpan.FromMinutes(irrelevantNumberToAvoidDuplicatePinsBeingRemoved), true);
        }

        public static IEnumerable<object[]> GetPins()
        {
            var subject = new TwoFactorAuthenticator();

            yield return new object[] { subject.GetCurrentPIN(secret), 2 };
            yield return new object[] { subject.GetCurrentPIN(secret, DateTime.UtcNow), 3 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBytes), 4 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBytes, DateTime.UtcNow), 5 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBase32, true), 6 };
            yield return new object[] { subject.GetCurrentPIN(secretAsBase32, DateTime.UtcNow, true), 7 };
        }
    }
}