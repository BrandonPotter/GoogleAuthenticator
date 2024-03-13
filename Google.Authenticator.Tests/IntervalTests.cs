using Xunit;
using Shouldly;
using System.Collections.Generic;
using System.Text;
using System;

namespace Google.Authenticator.Tests
{
    public class IntervalTests
    {
        const string secret = "ggggjhG&^*&^jfSSSddd";

        [Theory]
        [MemberData(nameof(GetTestValues))]
        public void GetCurrentPinsHandlesDifferentIntervals(int timeTolerance, int timeStep, int expectedCount)
        {
            var subject = new TwoFactorAuthenticator(timeStep);

            subject.GetCurrentPINs(secret, TimeSpan.FromSeconds(timeTolerance)).Length.ShouldBe(expectedCount);
        }

        public static IEnumerable<object[]> GetTestValues()
        {
            yield return new object[] { 15, 30, 1 };
            yield return new object[] { 30, 30, 3 };
            yield return new object[] { 60, 30, 5 };

            yield return new object[] { 15, 15, 3 };
            yield return new object[] { 30, 15, 5 };
            yield return new object[] { 60, 15, 9 };

            yield return new object[] { 15, 60, 1 };
            yield return new object[] { 30, 60, 1 };
            yield return new object[] { 60, 60, 3 };
            yield return new object[] { 300, 60, 11 };
        }
    }
}