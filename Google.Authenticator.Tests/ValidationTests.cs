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
            subject.ValidateTwoFactorPIN(secret, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, false);
            subject.ValidateTwoFactorPIN(secretAsBase32, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2, true);
            subject.ValidateTwoFactorPIN(secretAsBytes, pin, irrelevantNumberToAvoidDuplicatePinsBeingRemoved * 2);
        }

        [Fact]
        public void GetCurrentPinsHandles15SecondInterval()
        {
            // This is nonsensical, really, as anything less than 30 == 0 in practice.
            var subject = new TwoFactorAuthenticator();

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(15)).Length.ShouldBe(1);
        }


        [Fact]
        public void GetCurrentPinsHandles30SecondInterval()
        {
            var subject = new TwoFactorAuthenticator();

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(30)).Length.ShouldBe(3);
        }

        [Fact]
        public void GetCurrentPinsHandles60SecondInterval()
        {
            var subject = new TwoFactorAuthenticator();

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(60)).Length.ShouldBe(5);
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