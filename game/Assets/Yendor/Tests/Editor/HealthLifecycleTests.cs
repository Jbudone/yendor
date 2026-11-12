using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Yendor.Tests
{
    // EnterPlayMode keeps these discoverable by the existing EditMode runner while
    // exercising Unity's real Awake/Update dispatch, without invoking private methods.
    public sealed class HealthLifecycleTests
    {
        const float FrameSeconds = 1f / 60;
        GameObject fixture;
        Health health;
        float previousTimeScale;
        float previousCaptureDeltaTime;
        bool capturedTimeSettings;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return new EnterPlayMode();
            previousTimeScale = Time.timeScale;
            previousCaptureDeltaTime = Time.captureDeltaTime;
            capturedTimeSettings = true;
            Time.timeScale = 1;
            // Advance engine time consistently on actual player-loop frames. This
            // avoids batch-machine frame-rate jitter; Health still uses Time itself.
            Time.captureDeltaTime = FrameSeconds;
            fixture = new GameObject("Health lifecycle fixture");
            health = fixture.AddComponent<Health>();
            Assert.That(health.Current, Is.EqualTo(100), "Real Awake initializes health");
            Assert.That(health.Dead, Is.False);
            yield return AdvanceTo(Time.time + FrameSeconds * 2);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (fixture != null) Object.DestroyImmediate(fixture);
            if (capturedTimeSettings)
            {
                Time.captureDeltaTime = previousCaptureDeltaTime;
                Time.timeScale = previousTimeScale;
                capturedTimeSettings = false;
            }
            if (Application.isPlaying) yield return new ExitPlayMode();
        }

        static IEnumerator AdvanceTo(float deadline)
        {
            float started = Time.realtimeSinceStartup;
            int startingFrame = Time.frameCount;
            while (Time.time < deadline)
            {
                Assert.That(Time.realtimeSinceStartup - started, Is.LessThan(15),
                    "Player loop failed to reach simulation deadline within 15 real seconds");
                Assert.That(Time.frameCount - startingFrame, Is.LessThan(600),
                    "Simulation deadline exceeded its bounded frame budget");
                yield return null;
            }
            TestContext.WriteLine("advanced simulation to=" + Time.time + " deadline=" + deadline
                + " frames=" + (Time.frameCount - startingFrame)
                + " realSeconds=" + (Time.realtimeSinceStartup - started));
            Assert.That(Time.frameCount, Is.GreaterThan(startingFrame), "Must advance real Unity frames");
        }

        [UnityTest]
        public IEnumerator BurningDealsSevenDamagePerSecondThenExpires()
        {
            float ignition = Time.time;
            health.Damage(1, Element.Ember);
            Assert.That(health.BurningUntil, Is.EqualTo(ignition + 3).Within(.001f));
            yield return AdvanceTo(ignition + 1);
            float elapsed = Time.time - ignition;
            Assert.That(99 - health.Current, Is.EqualTo(7 * elapsed).Within(.35f),
                "Burn must deal damage at the accepted rate through real Update frames");

            yield return AdvanceTo(ignition + 3.1f);
            Assert.That(99 - health.Current, Is.EqualTo(21).Within(.35f),
                "Three seconds of burning must stop at about 21 damage");
            float afterExpiry = health.Current;
            yield return AdvanceTo(Time.time + .5f);
            Assert.That(health.Current, Is.EqualTo(afterExpiry), "Expired burn must stop damaging");
        }

        [UnityTest]
        public IEnumerator WetExtinguishesOnUpdateAndBurnDoesNotResumeAfterDrying()
        {
            health.Damage(1, Element.Ember);
            yield return AdvanceTo(Time.time + .15f);
            Assert.That(health.Current, Is.LessThan(99), "Precondition: burn was active");
            float beforeWater = health.Current;
            float wetStarted = Time.time;
            health.Wet = true;
            yield return AdvanceTo(wetStarted + FrameSeconds * 2);
            Assert.That(health.Wet, Is.True);
            Assert.That(health.BurningUntil, Is.Zero, "Update must extinguish the existing burn");
            Assert.That(health.Current, Is.EqualTo(beforeWater), "Water must prevent subsequent burn ticks");

            yield return AdvanceTo(wetStarted + .4f);
            Assert.That(health.Wet, Is.False, "Wet status expires without another water application");
            yield return AdvanceTo(Time.time + .4f);
            Assert.That(health.BurningUntil, Is.Zero);
            Assert.That(health.Current, Is.EqualTo(beforeWater), "Extinguished burn must not resume when dry");
        }

        [UnityTest]
        public IEnumerator WetExpiryRestoresNormalStormDamageAndAllowsIgnition()
        {
            float wetStarted = Time.time;
            health.Wet = true;
            health.Damage(4, Element.Storm);
            health.Damage(1, Element.Ember);
            Assert.That(health.Current, Is.EqualTo(91), "Wet doubles Storm's four damage");
            Assert.That(health.BurningUntil, Is.Zero, "Wet prevents Ember ignition");
            yield return AdvanceTo(wetStarted + .15f);
            Assert.That(health.Wet, Is.True, "Wet must persist before its 0.3-second expiry");
            yield return AdvanceTo(wetStarted + .35f);
            Assert.That(health.Wet, Is.False);
            Assert.That(health.Current, Is.EqualTo(91));

            health.Damage(4, Element.Storm);
            Assert.That(health.Current, Is.EqualTo(87), "Expired wet must no longer amplify Storm");
            health.Damage(1, Element.Ember);
            Assert.That(health.BurningUntil, Is.EqualTo(Time.time + 3).Within(.001f));
            yield return AdvanceTo(Time.time + .15f);
            Assert.That(health.Current, Is.LessThan(86), "Dry Ember must ignite a real ticking burn");
        }

        [UnityTest]
        public IEnumerator LethalBurnEmitsOneDeathAcrossLaterFramesAndHits()
        {
            int deaths = 0;
            health.Died += () => deaths++;
            health.Damage(98, Element.Kinetic);
            health.Damage(1, Element.Ember);
            Assert.That(health.Current, Is.EqualTo(1));
            yield return AdvanceTo(Time.time + .5f);
            Assert.That(health.Current, Is.Zero, "Actual burn frames must kill at zero");
            Assert.That(health.Dead, Is.True);
            Assert.That(deaths, Is.EqualTo(1));

            yield return AdvanceTo(Time.time + .5f);
            health.Damage(10, Element.Ember);
            health.Damage(10, Element.Storm);
            health.Heal(100);
            Assert.That(health.Current, Is.Zero);
            Assert.That(deaths, Is.EqualTo(1), "Later burn frames and hits must not repeat death");
        }
    }
}
