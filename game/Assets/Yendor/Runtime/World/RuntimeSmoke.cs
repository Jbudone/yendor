using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Yendor
{
    public sealed class RuntimeSmoke : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void StartIfRequested()
        {
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--yendor-smoke") >= 0)
                new GameObject("Runtime smoke checks").AddComponent<RuntimeSmoke>();
        }

        IEnumerator Start()
        {
            yield return null;
            yield return null;
            try { Check(); Debug.Log("YENDOR_RUNTIME_SMOKE_OK: spawns, ground, elemental damage, mixed roles, feedback, pickups, build choices, overlook support/clearance, cache effects, bounded alarm, death, relic and extraction."); Exit(0); }
            catch (Exception error) { Debug.LogException(error); Exit(1); }
        }


        static void Exit(int code)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(code);
#else
            Application.Quit(code);
#endif
        }

        static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }

        static void Teleport(PlayerMotor player, Vector3 position)
        {
            var cc = player.GetComponent<CharacterController>();
            cc.enabled = false; player.transform.position = position; cc.enabled = true;
        }

        static void CheckOverlook()
        {
            // Physics sampling proves support and capsule space, not human traversal or feel.
            for (int z = -12; z <= 32; z += 2)
                foreach (float x in new[] { 18.5f, 20f, 21.5f })
                {
                    float expected = z < 4 ? Mathf.Lerp(.05f, 4, (z + 12) / 16f) : z <= 18 ? 4 : Mathf.Lerp(4, .05f, (z - 18) / 14f);
                    Require(Physics.Raycast(new Vector3(x, expected + .3f, z), Vector3.down, out var hit, .6f, ~0, QueryTriggerInteraction.Ignore), "Overlook route has a support gap.");
                    Require(Mathf.Abs(hit.point.y - expected) < .15f && Vector3.Angle(hit.normal, Vector3.up) < 25, "Overlook slope/support exceeds authored limits.");
                    foreach (var obstruction in Physics.OverlapCapsule(hit.point + Vector3.up * .48f, hit.point + Vector3.up * 1.55f, .4f, ~0, QueryTriggerInteraction.Ignore))
                        Require(obstruction.GetComponentInParent<Enemy>() || obstruction.GetComponentInParent<PlayerMotor>(), "Overlook capsule blocked by " + obstruction.name);
                }
            Debug.Log("YENDOR_SMOKE_OVERLOOK: 69 support/clearance samples passed; 4m deck, 6m-wide ramps; human traversal untested.");
        }

        static void Check()
        {
            var run = Expedition.Instance;
            Require(run && run.Player && run.Player.View, "Missing expedition, player or camera.");
            Require(!run.Ended && !run.HasRelic, "Incorrect initial run state.");
            var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            Require(enemies.Length == 7, "Expected seven revenants.");
            Require(Array.FindAll(enemies, enemy => enemy.Role == EnemyRole.Brute).Length == 3 &&
                Array.FindAll(enemies, enemy => enemy.Role == EnemyRole.Spitter).Length == 2 &&
                Array.FindAll(enemies, enemy => enemy.Role == EnemyRole.Flanker).Length == 2, "Mixed role roster must be 3 brutes, 2 spitters and 2 flankers.");
            Require(Physics.Raycast(run.ExitPosition + Vector3.up * 4, Vector3.down, 6), "No ground at extraction.");
            // Starting positions are distinct on this authored route. Root sibling
            // indices are not reliable ordering keys in the packaged player.
            Array.Sort(enemies, (left, right) => {
                Vector3 a = left.transform.position, b = right.transform.position;
                int order = a.z.CompareTo(b.z);
                if (order == 0) order = a.x.CompareTo(b.x);
                return order == 0 ? a.y.CompareTo(b.y) : order;
            });
            for (int i = 0; i < enemies.Length; i++) Require(enemies[i].Role == (EnemyRole)(i % 3), "Spawn role ordering changed.");
            Debug.Log($"YENDOR_SMOKE_WORLD: seed={run.Seed} enemies={string.Join(";", Array.ConvertAll(enemies, enemy => enemy.transform.position.ToString("F3")))}");
            // Prefer an ambient wet target to exercise the regression: water may
            // already have applied before this synchronous fixture runs.
            var target = Array.Find(enemies, enemy => enemy.GetComponent<Health>().Wet) ?? enemies[0];
            var health = target.GetComponent<Health>();
            Require(health && !health.Dead, "Elemental smoke target must be alive.");
            Debug.Log($"YENDOR_SMOKE_PRECONDITION: seed={run.Seed} position={target.transform.position.ToString("F3")} ambientWet={health.Wet} health={health.Current}");
            // No frame elapses between setup and assertions. Ambient surfaces
            // cannot reapply wet until after this synchronous check returns.
            health.Wet = false;
            Require(!health.Wet, "Dry smoke precondition was not established.");
            health.Damage(10, Element.Ember);
            Require(health.BurningUntil > Time.time, "Dry ember hit did not ignite.");
            Require(target.DamageVisibleUntil > Time.time && Mathf.Approximately(target.LastDamage, 10), "Direct hit feedback did not receive actual damage.");
            health.Wet = true;
            Require(health.Wet, "Wet smoke precondition was not established.");
            float before = health.Current;
            health.Damage(10, Element.Storm);
            Require(Mathf.Approximately(before - health.Current, 20), "Wet storm multiplier failed.");
            health.Damage(1000, Element.Kinetic);
            Require(run.Kills == 1 && health.Dead, "Enemy death did not update the run.");
            Require(run.Player.KillVisibleUntil > Time.time && run.Player.LastKill == target.RoleName, "Kill confirmation missing.");
            health.Damage(10, Element.Kinetic);
            Require(run.Kills == 1, "Death counted twice.");
            var pickups = FindObjectsByType<RunPickup>(FindObjectsSortMode.None);
            Require(pickups.Length == 7, "Expected three shrine cores, two road supplies and two overlook supplies.");
            CheckOverlook();
            var cacheWood = Array.Find(pickups, item => item.Id == "overlook-heartwood");
            var cacheSalve = Array.Find(pickups, item => item.Id == "overlook-salve");
            Require(cacheWood && cacheSalve, "Missing uniquely identified overlook cache.");
            foreach (var offering in pickups)
                if (offering.Definition.IsModifier)
                    foreach (var enemy in enemies)
                        Require(Vector3.Distance(offering.transform.position, enemy.transform.position) > 27, "Starting shrine must be outside initial enemy aggro.");
            Require(!run.Player.Build.Core.HasValue && run.Player.Build.Supplies == 0, "New run must have no powers.");
            Teleport(run.Player, cacheWood.transform.position);
            run.Interact();
            var playerHealth = run.Player.GetComponent<Health>();
            Require(playerHealth.Maximum == 125 && run.Player.Build.Supplies == 1, "Overlook Heartwood did not grant real maximum life.");
            playerHealth.Damage(70, Element.Kinetic);
            Teleport(run.Player, cacheSalve.transform.position);
            run.Interact();
            Require(playerHealth.Current == 95 && run.Player.Build.Supplies == 2, "Overlook salve did not heal 40 life.");
            var core = Array.Find(pickups, item => item.Definition.IsModifier);
            Require(!core.TryCollect(run.Player, false), "Distant core collected.");
            Teleport(run.Player, core.transform.position);
            run.Interact();
            Require(run.Player.Build.Core == core.Kind, "E interaction did not choose the nearest core.");
            Require(!core.TryCollect(run.Player, false), "Core collected twice.");
            var otherCore = Array.Find(pickups, item => item.Definition.IsModifier && item != core);
            Teleport(run.Player, otherCore.transform.position);
            Require(!otherCore.TryCollect(run.Player, false), "Mutually exclusive core collected.");
            Debug.Log($"YENDOR_SMOKE_BUILD: seed={run.Seed} core={run.Player.Build.Core} offerings={string.Join(",", PickupCatalog.Offerings(run.Seed))}");
            Teleport(run.Player, run.ExitPosition);
            run.Interact();
            Require(!run.HasRelic && !run.Ended, "Distant interaction collected the relic.");
            Teleport(run.Player, run.RelicPosition);
            run.SpawnPickup("smoke-relic-priority", PickupKind.Heartwood, run.RelicPosition);
            run.Interact();
            Require(run.HasRelic && !run.Ended, "Relic pickup failed.");
            Require(run.AlarmRaised && run.Phase == "Return" && run.ReturnEnemiesSpawned == 3 && run.TotalEnemies == 10, "Relic must raise one bounded mixed return encounter.");
            Physics.SyncTransforms();
            var returning = Array.FindAll(FindObjectsByType<Enemy>(FindObjectsSortMode.None), enemy => Array.IndexOf(enemies, enemy) < 0);
            Require(returning.Length == 3, "Expected exactly three new sentries.");
            foreach (EnemyRole role in Enum.GetValues(typeof(EnemyRole))) Require(Array.Exists(returning, enemy => enemy.Role == role), "Alarm missing mixed role.");
            foreach (var enemy in returning)
            {
                Require(Vector3.Distance(enemy.transform.position, run.Player.transform.position) >= 10, "Alarm spawned too close to the player.");
                Require(Vector3.Distance(enemy.transform.position, run.ExitPosition) > 8 && Vector3.Distance(enemy.transform.position, run.RelicPosition) > 8, "Alarm overlaps an objective.");
                Require(Physics.Raycast(enemy.transform.position + Vector3.up * .1f, Vector3.down, out var support, 1, ~0, QueryTriggerInteraction.Ignore) && support.collider.name == "Pilgrim path", "Return sentry must have authored floor support.");
            }
            Require(!run.Player.Build.HasCollected("smoke-relic-priority"), "Supply consumed instead of relic priority.");
            run.Interact();
            Require(run.ReturnEnemiesSpawned == 3 && run.TotalEnemies == 10, "Repeated E created another wave.");
            Require(run.Player.Build.HasCollected("smoke-relic-priority") && run.Player.Build.Supplies == 3, "Second E must collect the supply normally.");
            Teleport(run.Player, run.ExitPosition);
            run.SpawnPickup("smoke-exit-priority", PickupKind.Heartwood, run.ExitPosition);
            run.Interact();
            Require(run.Ended, "Extraction failed.");
            Require(!run.Player.Build.HasCollected("smoke-exit-priority"), "Supply consumed instead of extraction priority.");
            var supply = Array.Find(pickups, item => item.Kind == PickupKind.Heartwood);
            Teleport(run.Player, supply.transform.position);
            Require(!supply.TryCollect(run.Player, run.Ended), "Supply collected after run ended.");
            float playerLife = run.Player.GetComponent<Health>().Current;
            foreach (var enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None)) enemy.TickCombat(Time.time + 100, 0, run.Player, run.Ended);
            Require(run.Player.GetComponent<Health>().Current == playerLife, "Combat damaged player after extraction.");
            Require(run.Player.DashReadyAt <= Time.time && run.Player.StormReadyAt <= Time.time, "Initial abilities must be ready.");
            var diagnostics = FindFirstObjectByType<RunDiagnostics>();
            Require(diagnostics, "Missing run diagnostics.");
            string report = diagnostics.WriteReport();
            Require(File.Exists(Path.Combine(report, "run.json")), "Diagnostic report was not written.");
            string diagnostic = File.ReadAllText(Path.Combine(report, "run.json"));
            Require(diagnostic.Contains("\"phase\": \"Escaped\"") && diagnostic.Contains("\"alarmRaised\": true") && diagnostic.Contains("\"returnEnemiesSpawned\": 3") && diagnostic.Contains("\"supplies\": 3") && diagnostic.Contains("\"totalEnemies\": 10") && diagnostic.Contains("\"core\": \"" + run.Player.Build.Core + "\""), "F8 report missing expedition/build state.");
        }
    }
}
