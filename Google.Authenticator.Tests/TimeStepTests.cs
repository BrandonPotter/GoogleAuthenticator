using Xunit;
using Shouldly;
using System;

namespace Google.Authenticator.Tests
{
    public class TimeStepTests 
    {
        [Fact]
        public void DefaultPINHasNotBeenChangedByAddingTimeStepConfig()
        {
            var now = new DateTime(2024,1,2,3,4,5,DateTimeKind.Utc);
            var secret = "12314241234342342";
            var defaultPin = new TwoFactorAuthenticator().GetCurrentPIN(secret, now);
            
            defaultPin.ShouldBe("668182"); // This pin was created with the code from before the timestep config was added
        }
        
        [Fact]
        public void DifferentTimeStepsReturnsDifferentPINs()
        {
            var now = new DateTime(2024,1,2,3,4,5,DateTimeKind.Utc);
            var secret = "12314241234342342";
            var defaultPin = new TwoFactorAuthenticator().GetCurrentPIN(secret, now);
            var pinWith15SecondTimeStep = new TwoFactorAuthenticator(15).GetCurrentPIN(secret, now);
            var pinWith60SecondTimeStep = new TwoFactorAuthenticator(60).GetCurrentPIN(secret, now);

            defaultPin.ShouldNotBe(pinWith15SecondTimeStep);
            defaultPin.ShouldNotBe(pinWith60SecondTimeStep);
            pinWith15SecondTimeStep.ShouldNotBe(pinWith60SecondTimeStep);
        }

        [Fact]
        public void DefaultTimeStepGivesSamePinAs30()
        {
            var now = new DateTime(2024,1,2,3,4,5,DateTimeKind.Utc);
            var secret = "12314241234342342";
            var defaultPin = new TwoFactorAuthenticator().GetCurrentPIN(secret, now);
            var pinWith30SecondTimeStep = new TwoFactorAuthenticator(30).GetCurrentPIN(secret, now);

            defaultPin.ShouldBe(pinWith30SecondTimeStep);

        }
    }
}