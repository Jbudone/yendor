using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Yendor.Tests
{
    public sealed class RunPickupTests
    {
        GameObject playerObject, pickupObject, targetObject, cameraObject;
        PlayerMotor player;
        Health life;
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return new EnterPlayMode();
            playerObject = new GameObject("Pickup fixture"); playerObject.layer = 2;
            playerObject.transform.position = new Vector3(0, 1000, 0);
            player = playerObject.AddComponent<PlayerMotor>(); player.enabled = false;
            life = player.GetComponent<Health>();
            pickupObject = new GameObject("Offering fixture"); pickupObject.transform.position = player.transform.position;
            cameraObject = new GameObject("Aim fixture");
            cameraObject.transform.position = player.transform.position + Vector3.up * 1.4f;
            player.View = cameraObject.AddComponent<Camera>();
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (var item in new[] { playerObject, pickupObject, targetObject, cameraObject }) if (item) Object.DestroyImmediate(item);
            yield return new ExitPlayMode();
        }
        RunPickup Offering(PickupKind kind, string id = "test")
        {
            var result = pickupObject.GetComponent<RunPickup>() ?? pickupObject.AddComponent<RunPickup>();
            result.Configure(id, kind); return result;
        }
        Health Target(float distance, bool enemy = false)
        {
            targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            targetObject.transform.position = player.transform.position + new Vector3(0, 1.4f, distance);
            var health = targetObject.AddComponent<Health>();
            if (enemy) { var actor = targetObject.AddComponent<Enemy>(); actor.enabled = false; }
            Physics.SyncTransforms(); return health;
        }
        [Test]
        public void OfferingsReplayIndependentlyAndContainAllThreeChoices()
        {
            var expected = PickupCatalog.Offerings(43117);
            var unrelatedWorldRandom = new System.Random(43117);
            for (int i = 0; i < 1000; i++) unrelatedWorldRandom.Next();
            Assert.That(PickupCatalog.Offerings(43117), Is.EqualTo(expected));
            Assert.That(expected, Is.EquivalentTo(new[] { PickupKind.NeedleCore, PickupKind.EmberCore, PickupKind.StormCore }));
            bool anotherOrder = false;
            for (int seed = 43118; seed < 43130; seed++) if (string.Join(",", PickupCatalog.Offerings(seed)) != string.Join(",", expected)) anotherOrder = true;
            Assert.That(anotherOrder, Is.True, "Seed must affect offering slots");
        }
        [UnityTest]
        public IEnumerator SuppliesApplyOnceAndFullHealthSalveIsSaved()
        {
            var salve = Offering(PickupKind.Salve);
            Assert.That(salve.TryCollect(player, false), Is.False);
            life.Damage(60, Element.Kinetic);
            Assert.That(salve.TryCollect(player, false), Is.True);
            Assert.That(life.Current, Is.EqualTo(80));
            Assert.That(salve.TryCollect(player, false), Is.False);
            Assert.That(player.Build.Collect("wood", PickupKind.Heartwood, life), Is.True);
            Assert.That(life.Maximum, Is.EqualTo(125));
            Assert.That(life.Current, Is.EqualTo(105));
            Assert.That(player.Build.Collect("wood", PickupKind.Heartwood, life), Is.False);
            yield break;
        }
        [UnityTest]
        public IEnumerator DistanceEndDeathAndCoreExclusionAreEnforced()
        {
            var core = Offering(PickupKind.NeedleCore);
            pickupObject.transform.position += Vector3.right * 10;
            Assert.That(core.TryCollect(player, false), Is.False);
            pickupObject.transform.position = player.transform.position;
            Assert.That(core.TryCollect(player, true), Is.False);
            Assert.That(core.TryCollect(player, false), Is.True);
            Assert.That(player.Build.Collect("second-core", PickupKind.StormCore, life), Is.False);
            Assert.That(player.Build.Core, Is.EqualTo(PickupKind.NeedleCore));
            life.Damage(1000, Element.Kinetic);
            Assert.That(player.Build.Collect("dead-salve", PickupKind.Salve, life), Is.False);
            Assert.That(Offering(PickupKind.Heartwood, "dead-wood").TryCollect(player, false), Is.False);
            var fresh = new GameObject("New run Wayfarer");
            var freshPlayer = fresh.AddComponent<PlayerMotor>(); freshPlayer.enabled = false;
            Assert.That(freshPlayer.Build.Core, Is.Null);
            Assert.That(freshPlayer.Build.Supplies, Is.Zero);
            Assert.That(freshPlayer.GetComponent<Health>().Maximum, Is.EqualTo(100));
            Object.DestroyImmediate(fresh);
            yield break;
        }
        [UnityTest]
        public IEnumerator NeedleCarbineUsesDamageAndCadenceOnActualRaycast()
        {
            var target = Target(8);
            Assert.That(Offering(PickupKind.NeedleCore).TryCollect(player, false), Is.True);
            Assert.That(player.TryFire(Element.Kinetic, 10, false), Is.True);
            Assert.That(target.Current, Is.EqualTo(73));
            Assert.That(player.TryFire(Element.Kinetic, 10.15f, false), Is.False);
            Assert.That(target.Current, Is.EqualTo(73));
            Assert.That(player.TryFire(Element.Kinetic, 10.23f, false), Is.True);
            Assert.That(target.Current, Is.EqualTo(46));
            Assert.That(player.TryFire(Element.Kinetic, 11, true), Is.False);
            yield break;
        }
        [UnityTest]
        public IEnumerator CinderUsesFastEmberButWeakensCarbineAndPreservesWetRule()
        {
            var target = Target(8); target.Wet = true;
            Assert.That(Offering(PickupKind.EmberCore).TryCollect(player, false), Is.True);
            Assert.That(player.TryFire(Element.Ember, 10, false), Is.True);
            Assert.That(target.Current, Is.EqualTo(68));
            Assert.That(target.BurningUntil, Is.Zero, "Wet still prevents ignition");
            Assert.That(player.TryFire(Element.Ember, 10.36f, false), Is.True);
            Assert.That(target.Current, Is.EqualTo(36));
            Assert.That(player.TryFire(Element.Kinetic, 10.4f, false), Is.True);
            Assert.That(target.Current, Is.EqualTo(26));
            yield break;
        }
        [UnityTest]
        public IEnumerator TempestReachesDistantWetEnemyWithShortCooldown()
        {
            var target = Target(18, true); target.Wet = true;
            Assert.That(player.TryStorm(10, false), Is.True);
            Assert.That(target.Current, Is.EqualTo(100), "Base storm cannot reach 18m");
            Assert.That(Offering(PickupKind.StormCore).TryCollect(player, false), Is.True);
            Assert.That(player.TryStorm(16, false), Is.True);
            Assert.That(target.Current, Is.EqualTo(24), "Tempest damage must retain wet doubling");
            Assert.That(player.StormReadyAt, Is.EqualTo(19));
            Assert.That(player.TryStorm(18, false), Is.False);
            Assert.That(player.TryStorm(20, true), Is.False);
            life.Damage(1000, Element.Kinetic);
            Assert.That(player.TryStorm(20, false), Is.False);
            Assert.That(player.TryFire(Element.Ember, 20, false), Is.False);
            yield break;
        }
    }
}
