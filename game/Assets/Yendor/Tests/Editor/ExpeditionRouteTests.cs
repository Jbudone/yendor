using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Yendor.Tests
{
    public sealed class ExpeditionRouteTests
    {
        Expedition run;
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return new EnterPlayMode();
            run = Expedition.Instance;
            if (!run) run = new GameObject("Route test expedition").AddComponent<Expedition>();
            yield return null;
            run.Player.enabled = false;
            foreach (var enemy in UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            { enemy.enabled = false; enemy.GetComponent<CharacterController>().enabled = false; }
            Physics.SyncTransforms();
        }
        [UnityTearDown]
        public IEnumerator TearDown() { yield return new ExitPlayMode(); }
        void MovePlayer(Vector3 position)
        {
            var motor = run.Player.GetComponent<CharacterController>();
            motor.enabled = false; run.Player.transform.position = position; motor.enabled = true;
            Physics.SyncTransforms();
        }
        [UnityTest]
        public IEnumerator BroadOverlookAndConnectorsHaveSupportedCapsuleSpace()
        {
            for (int z = -12; z <= 32; z += 2)
                foreach (float x in new[] { 18.5f, 20f, 21.5f })
                {
                    float height = z < 4 ? Mathf.Lerp(.05f, 4, (z + 12) / 16f) : z <= 18 ? 4 : Mathf.Lerp(4, .05f, (z - 18) / 14f);
                    CheckSpace(new Vector3(x, height, z));
                }
            foreach (float z in new[] { -13f, -12f, -11f, 32f, 33f, 34f })
                for (int x = 0; x <= 20; x++)
                {
                    // At the inner approach edge, the broad ascent overlays the connector.
                    float height = x >= 17 && z == -11 ? Mathf.Lerp(.05f, 4, 1f / 16) : .05f;
                    CheckSpace(new Vector3(x, height, z));
                }
            yield break;
        }
        void CheckSpace(Vector3 expected)
        {
            Assert.That(Physics.Raycast(expected + Vector3.up * .3f, Vector3.down, out var hit, .6f, ~0, QueryTriggerInteraction.Ignore), Is.True, "Support missing: " + expected);
            Assert.That(hit.point.y, Is.EqualTo(expected.y).Within(.15f), "Wrong support height: " + expected);
            Assert.That(Vector3.Angle(hit.normal, Vector3.up), Is.LessThan(25), "Slope exceeds controller limit");
            foreach (var collider in Physics.OverlapCapsule(hit.point + Vector3.up * .48f, hit.point + Vector3.up * 1.55f, .4f, ~0, QueryTriggerInteraction.Ignore))
                Assert.That(collider.GetComponentInParent<PlayerMotor>(), Is.Not.Null, "Blocked capsule at " + expected + ": " + collider.name);
        }
        [UnityTest]
        public IEnumerator CharacterControllerWalksBothDirectionsAcrossBothDeckJoins()
        {
            var walker = new GameObject("Automated traversal capsule"); walker.layer = 2;
            walker.transform.position = new Vector3(0, .25f, -12);
            var motor = walker.AddComponent<CharacterController>();
            motor.height = 2; motor.center = Vector3.up; motor.radius = .4f;
            try
            {
                foreach (var goal in new[] { new Vector3(20, 0, -12), new Vector3(20, 0, 4), new Vector3(20, 0, 18), new Vector3(20, 0, 32), new Vector3(20, 0, 33), new Vector3(0, 0, 33), new Vector3(20, 0, 33), new Vector3(20, 0, 32), new Vector3(20, 0, 18), new Vector3(20, 0, 4), new Vector3(20, 0, -12), new Vector3(0, 0, -12) })
                {
                    int steps = 0;
                    while (true)
                    {
                        Vector3 delta = goal - walker.transform.position; delta.y = 0;
                        if (delta.magnitude < .12f) break;
                        Assert.That(++steps, Is.LessThan(700), "Controller stuck approaching " + goal + " at " + walker.transform.position);
                        motor.Move(delta.normalized * .08f + Vector3.down * .06f);
                        Assert.That(Physics.Raycast(walker.transform.position + Vector3.up * .3f, Vector3.down, out var support, .8f, ~0, QueryTriggerInteraction.Ignore), Is.True, "Controller lost route support");
                        Assert.That(walker.transform.position.y - support.point.y, Is.InRange(-.05f, .3f), "Controller left the supported surface");
                    }
                }
                Assert.That(walker.transform.position.y, Is.InRange(0, .3f), "Descent must return to ground level");
            }
            finally { UnityEngine.Object.DestroyImmediate(walker); }
            yield break;
        }
        [UnityTest]
        public IEnumerator AuthoredCacheGrantsLifeAndHealingOnceWithDistinctRoadIds()
        {
            var pickups = UnityEngine.Object.FindObjectsByType<RunPickup>(FindObjectsSortMode.None);
            Assert.That(pickups.Length, Is.EqualTo(7));
            Assert.That(Array.ConvertAll(pickups, item => item.Id), Is.Unique);
            var wood = Array.Find(pickups, item => item.Id == "overlook-heartwood");
            var salve = Array.Find(pickups, item => item.Id == "overlook-salve");
            var life = run.Player.GetComponent<Health>();
            MovePlayer(wood.transform.position); run.Interact();
            Assert.That(life.Maximum, Is.EqualTo(125));
            Assert.That(life.Current, Is.EqualTo(125));
            run.Interact(); Assert.That(life.Maximum, Is.EqualTo(125));
            MovePlayer(salve.transform.position); run.Interact();
            Assert.That(run.Player.Build.HasCollected(salve.Id), Is.False, "Save full-life salve");
            life.Damage(60, Element.Kinetic); run.Interact();
            Assert.That(life.Current, Is.EqualTo(105));
            Assert.That(run.Player.Build.Supplies, Is.EqualTo(2));
            run.Interact(); Assert.That(life.Current, Is.EqualTo(105));
            yield break;
        }
        [UnityTest]
        public IEnumerator StarheartAddsOneSafeMixedEncounterAndExtractionRemainsOpen()
        {
            Assert.That(run.Phase, Is.EqualTo("Outbound"));
            Assert.That(run.AlarmRaised, Is.False);
            var initial = UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            Array.Sort(initial, (a, b) => a.transform.position.z.CompareTo(b.transform.position.z));
            var originalX = new[] { -3f, 5f, -5f, 4f, -6f, 6f, -5f };
            for (int i = 0; i < 7; i++)
            {
                Assert.That(initial[i].transform.position.x, Is.EqualTo(originalX[i]).Within(.001f), "Seed 43117 initial enemy X changed");
                Assert.That(initial[i].transform.position.z, Is.EqualTo(4 + 5 * i).Within(.001f), "Seed 43117 initial enemy Z changed");
            }
            MovePlayer(run.RelicPosition + Vector3.right * 3.9f); run.Interact();
            Assert.That(run.Phase, Is.EqualTo("Return"));
            Assert.That(run.AlarmRaised, Is.True);
            Assert.That(run.TotalEnemies, Is.EqualTo(10));
            var added = Array.FindAll(UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None), enemy => Array.IndexOf(initial, enemy) < 0);
            Assert.That(added.Length, Is.EqualTo(3));
            Assert.That(Array.ConvertAll(added, enemy => enemy.Role), Is.EquivalentTo(new[] { EnemyRole.Brute, EnemyRole.Spitter, EnemyRole.Flanker }));
            foreach (var enemy in added)
            {
                Assert.That(Vector3.Distance(enemy.transform.position, run.Player.transform.position), Is.GreaterThan(10));
                Assert.That(Vector3.Distance(enemy.transform.position, run.ExitPosition), Is.GreaterThan(8));
                Assert.That(Vector3.Distance(enemy.transform.position, run.RelicPosition), Is.GreaterThan(8));
                Assert.That(Physics.Raycast(enemy.transform.position + Vector3.up * .1f, Vector3.down, out var hit, 1), Is.True);
                Assert.That(hit.collider.name, Is.EqualTo("Pilgrim path"));
            }
            for (int i = 0; i < 5; i++) run.Interact();
            Assert.That(run.ReturnEnemiesSpawned, Is.EqualTo(3));
            Assert.That(run.TotalEnemies, Is.EqualTo(10));
            var killed = added[0].GetComponent<Health>();
            killed.Damage(1000, Element.Kinetic); killed.Damage(1000, Element.Kinetic);
            Assert.That(run.Kills, Is.EqualTo(1));
            MovePlayer(run.ExitPosition); run.Interact();
            Assert.That(run.Phase, Is.EqualTo("Escaped"));
            var life = run.Player.GetComponent<Health>(); float before = life.Current;
            foreach (var enemy in added) enemy.TickCombat(Time.time + 100, 0, run.Player, run.Ended);
            Assert.That(life.Current, Is.EqualTo(before));
            MovePlayer(run.RelicPosition); run.Interact();
            Assert.That(run.ReturnEnemiesSpawned, Is.EqualTo(3));
            yield break;
        }
        void PauseActors()
        {
            run.Player.enabled = false;
            foreach (var enemy in UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None)) enemy.enabled = false;
        }
        Vector3[] SpawnPositions()
        {
            var enemies = UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            Array.Sort(enemies, (a, b) => a.transform.position.z.CompareTo(b.transform.position.z));
            return Array.ConvertAll(enemies, enemy => enemy.transform.position);
        }
        PickupKind[] PhysicalOfferings()
        {
            var offerings = Array.FindAll(UnityEngine.Object.FindObjectsByType<RunPickup>(FindObjectsSortMode.None), item => item.Definition.IsModifier);
            Array.Sort(offerings, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
            return Array.ConvertAll(offerings, item => item.Kind);
        }
        void AssertFreshRun(int seed)
        {
            Assert.That(run.Seed, Is.EqualTo(seed));
            Assert.That(run.Ended || run.HasRelic || run.AlarmRaised, Is.False);
            Assert.That(run.Phase, Is.EqualTo("Outbound"));
            Assert.That(run.Player.Build.Core, Is.Null);
            Assert.That(run.Player.Build.Supplies, Is.Zero);
            Assert.That(run.Player.Build.HasCollected("road-heartwood"), Is.False);
            Assert.That(run.Player.GetComponent<Health>().Maximum, Is.EqualTo(100));
            Assert.That(run.Player.GetComponent<Health>().Current, Is.EqualTo(100));
            Assert.That(run.TotalEnemies, Is.EqualTo(7));
            Assert.That(run.Kills, Is.Zero);
            Assert.That(run.ReturnEnemiesSpawned, Is.Zero);
            Assert.That(UnityEngine.Object.FindObjectsByType<RunDiagnostics>(FindObjectsSortMode.None).Length, Is.EqualTo(1), "Exactly one F8 reporter must survive replay");
        }
        [UnityTest]
        public IEnumerator ActualSceneReplayRestoresSeedBuildAndAlarmThenNextSeedStartsClean()
        {
            // The test runner's transient scene has no build index. Exercise the real shipped scene.
            SceneManager.LoadScene(0);
            yield return null; yield return null;
            run = Expedition.Instance; PauseActors();
            Assert.That(SceneManager.GetActiveScene().buildIndex, Is.Zero);
            int seed = run.Seed;
            var positions = SpawnPositions(); var offerings = PhysicalOfferings();
            var active = run; run.Restart(false);
            Assert.That(Expedition.Instance, Is.SameAs(active), "Restart must be guarded until the run ends");
            var pickups = UnityEngine.Object.FindObjectsByType<RunPickup>(FindObjectsSortMode.None);
            var core = Array.Find(pickups, item => item.Definition.IsModifier);
            MovePlayer(core.transform.position); run.Interact();
            var wood = Array.Find(pickups, item => item.Id == "road-heartwood");
            MovePlayer(wood.transform.position); run.Interact();
            Assert.That(run.Player.Build.Core, Is.EqualTo(core.Kind));
            Assert.That(run.Player.GetComponent<Health>().Maximum, Is.EqualTo(125));
            MovePlayer(run.RelicPosition); run.Interact();
            Assert.That(run.AlarmRaised, Is.True);
            MovePlayer(run.ExitPosition); run.Interact();
            Assert.That(run.Ended, Is.True);
            run.Restart(false);
            yield return null; yield return null;
            run = Expedition.Instance; PauseActors();
            AssertFreshRun(seed);
            Assert.That(SpawnPositions(), Is.EqualTo(positions), "Same-seed scene replay must reproduce original enemy positions");
            Assert.That(PhysicalOfferings(), Is.EqualTo(offerings), "Same-seed scene replay must reproduce physical core slots");
            run.Player.GetComponent<Health>().Damage(1000, Element.Kinetic);
            Assert.That(run.Ended, Is.True);
            run.Restart(true);
            yield return null; yield return null;
            run = Expedition.Instance; PauseActors();
            AssertFreshRun(seed + 1);
            Assert.That(PhysicalOfferings(), Is.EqualTo(PickupCatalog.Offerings(seed + 1)));
        }

        [UnityTest]
        public IEnumerator DeathPreventsRelicAndAlarm()
        {
            MovePlayer(run.RelicPosition);
            run.Player.GetComponent<Health>().Damage(1000, Element.Kinetic);
            run.Interact();
            Assert.That(run.Phase, Is.EqualTo("Lost"));
            Assert.That(run.HasRelic, Is.False);
            Assert.That(run.AlarmRaised, Is.False);
            Assert.That(run.ReturnEnemiesSpawned, Is.Zero);
            Assert.That(run.TotalEnemies, Is.EqualTo(7));
            yield break;
        }
    }
}
