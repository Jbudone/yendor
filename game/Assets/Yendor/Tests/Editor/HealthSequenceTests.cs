using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Yendor.Tests
{
    // Synchronous rules only: elapsed burning/wet expiry still needs frame-driven tests.
    public sealed class HealthSequenceTests
    {
        enum Kind { Kinetic, Ember, Storm, Heal, Wet, Dry }
        struct Command
        {
            public Kind Kind;
            public int Amount;
            public override string ToString() => Kind + "(" + Amount + ")";
        }
        static Command C(Kind kind, int amount = 0) => new Command { Kind = kind, Amount = amount };

        static string Replay(IList<Command> commands, int mutant = 0)
        {
            var go = new GameObject("Health sequence fixture");
            try
            {
                var health = go.AddComponent<Health>();
                // EditMode does not dispatch MonoBehaviour lifecycle normally.
                // Runtime smoke separately verifies the real lifecycle adapter.
                typeof(Health).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(health, null);
                int deaths = 0, expectedDeaths = 0;
                float expected = 100;
                bool wet = false;
                health.Died += () => deaths++;
                for (int index = 0; index < commands.Count; index++)
                {
                    var command = commands[index];
                    bool wasDead = expected <= 0;
                    if (command.Kind == Kind.Wet) { wet = true; health.Wet = true; }
                    else if (command.Kind == Kind.Dry) { wet = false; health.Wet = false; }
                    else if (command.Kind == Kind.Heal)
                    {
                        if (!wasDead) expected = Math.Min(100, expected + command.Amount);
                        health.Heal(command.Amount);
                        if (mutant == 3 && wasDead)
                            typeof(Health).GetProperty(nameof(Health.Current)).SetValue(health, (float)command.Amount);
                    }
                    else
                    {
                        if (!wasDead)
                        {
                            float damage = command.Amount * (wet && command.Kind == Kind.Storm ? 2 : 1);
                            expected = Math.Max(0, expected - damage);
                            if (expected <= 0) expectedDeaths++;
                        }
                        if (mutant == 1 && command.Kind == Kind.Storm) health.Wet = false;
                        health.Damage(command.Amount, command.Kind == Kind.Ember ? Element.Ember : command.Kind == Kind.Storm ? Element.Storm : Element.Kinetic);
                        if (mutant == 1) health.Wet = wet;
                        if (mutant == 2 && wasDead) health.Died?.Invoke();
                    }
                    if (!Mathf.Approximately(expected, health.Current) || deaths != expectedDeaths || health.Dead != (expected <= 0))
                        return "step=" + index + " expectedHealth=" + expected + " actualHealth=" + health.Current + " expectedDeaths=" + expectedDeaths + " actualDeaths=" + deaths;
                }
                return null;
            }
            finally { UnityEngine.Object.DestroyImmediate(go); }
        }

        static List<Command> Reduce(List<Command> sequence, int mutant)
        {
            var result = new List<Command>(sequence);
            bool changed;
            do
            {
                changed = false;
                for (int index = 0; index < result.Count; index++)
                {
                    var candidate = new List<Command>(result);
                    candidate.RemoveAt(index);
                    if (Replay(candidate, mutant) != null) { result = candidate; changed = true; break; }
                }
            } while (changed);
            return result;
        }

        [TestCase(17)] [TestCase(171)] [TestCase(1717)]
        public void GeneratedLegalHistoriesPreserveHealthAndDeathRules(int seed)
        {
            var random = new System.Random(seed);
            for (int example = 0; example < 500; example++)
            {
                var commands = new List<Command>();
                for (int index = 0; index < 30; index++) commands.Add(C((Kind)random.Next(6), random.Next(1, 61)));
                string failure = Replay(commands);
                if (failure != null)
                {
                    var reduced = Reduce(commands, 0);
                    Assert.Fail("seed=" + seed + " example=" + example + " full=" + string.Join(",", commands) + " reduced=" + string.Join(",", reduced) + " " + failure);
                }
            }
        }

        [TestCase(1)] [TestCase(2)] [TestCase(3)]
        public void IndependentOracleDetectsAndReducesDeliberatelyFaultyAdapters(int mutant)
        {
            // Negative controls emulate three faults at the adapter boundary. They do not
            // claim whole-project mutation coverage or modify production source.
            var commands = mutant == 1
                ? new List<Command> { C(Kind.Heal, 10), C(Kind.Wet), C(Kind.Storm, 10), C(Kind.Dry) }
                : mutant == 2
                    ? new List<Command> { C(Kind.Dry), C(Kind.Kinetic, 200), C(Kind.Kinetic, 1), C(Kind.Wet) }
                    : new List<Command> { C(Kind.Wet), C(Kind.Kinetic, 200), C(Kind.Heal, 7), C(Kind.Dry) };
            Assert.That(Replay(commands, mutant), Is.Not.Null, "Fault must be detected");
            var reduced = Reduce(commands, mutant);
            Assert.That(reduced.Count, Is.LessThanOrEqualTo(5));
            Assert.That(Replay(reduced, mutant), Is.Not.Null);
            Assert.That(Replay(reduced), Is.Null, "Real implementation must pass the reduced case");
            TestContext.WriteLine("mutant=" + mutant + " reduced=" + string.Join(",", reduced));
        }
    }
}
