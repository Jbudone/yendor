using System;
using System.Collections;
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
            try { Check(); Debug.Log("YENDOR_RUNTIME_SMOKE_OK: spawns, ground, elemental damage, death, relic and extraction."); Exit(0); }
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
            Require(Physics.Raycast(run.ExitPosition + Vector3.up * 4, Vector3.down, 6), "No ground at extraction.");
            var health = enemies[0].GetComponent<Health>();
            health.Damage(10, Element.Ember);
            Require(health.BurningUntil > Time.time, "Dry ember hit did not ignite.");
            health.Wet = true;
            float before = health.Current;
            health.Damage(10, Element.Storm);
            Require(Mathf.Approximately(before - health.Current, 20), "Wet storm multiplier failed.");
            health.Damage(1000, Element.Kinetic);
            Require(run.Kills == 1 && health.Dead, "Enemy death did not update the run.");
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
        }
    }
}
