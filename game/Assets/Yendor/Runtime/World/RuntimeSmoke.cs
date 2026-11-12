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
            try { Check(); Debug.Log("YENDOR_RUNTIME_SMOKE_OK: spawns, ground, elemental damage, mixed roles, feedback, death, relic and extraction."); Exit(0); }
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
            run.Interact();
            Require(!run.HasRelic && !run.Ended, "Distant interaction collected the relic.");
            Teleport(run.Player, run.RelicPosition);
            run.Interact();
            Require(run.HasRelic && !run.Ended, "Relic pickup failed.");
            Teleport(run.Player, run.ExitPosition);
            run.Interact();
            Require(run.Ended, "Extraction failed.");
            float playerLife = run.Player.GetComponent<Health>().Current;
            foreach (var enemy in enemies) enemy.TickCombat(Time.time + 100, 0, run.Player, run.Ended);
            Require(run.Player.GetComponent<Health>().Current == playerLife, "Combat damaged player after extraction.");
            Require(run.Player.DashReadyAt <= Time.time && run.Player.StormReadyAt <= Time.time, "Initial abilities must be ready.");
            var diagnostics = FindFirstObjectByType<RunDiagnostics>();
            Require(diagnostics, "Missing run diagnostics.");
            string report = diagnostics.WriteReport();
            Require(File.Exists(Path.Combine(report, "run.json")), "Diagnostic report was not written.");
        }
    }
}
