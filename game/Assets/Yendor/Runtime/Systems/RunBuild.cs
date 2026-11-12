using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yendor
{
    public enum PickupKind { Salve, Heartwood, NeedleCore, EmberCore, StormCore }

    public sealed class PickupDefinition
    {
        public readonly PickupKind Kind;
        public readonly string Name, Description;
        public readonly Color Color;
        public readonly float KineticDamage, EmberDamage, StormDamage, KineticInterval, EmberInterval, StormCooldown, StormRadius;
        public readonly float Healing, MaximumLife;
        public bool IsModifier => Kind >= PickupKind.NeedleCore;
        public PickupDefinition(PickupKind kind, string name, string description, Color color,
            float kineticDamage = 15, float emberDamage = 22, float stormDamage = 28,
            float kineticInterval = .14f, float emberInterval = .65f, float stormCooldown = 5, float stormRadius = 14,
            float healing = 0, float maximumLife = 0)
        {
            Kind = kind; Name = name; Description = description; Color = color;
            KineticDamage = kineticDamage; EmberDamage = emberDamage; StormDamage = stormDamage;
            KineticInterval = kineticInterval; EmberInterval = emberInterval; StormCooldown = stormCooldown; StormRadius = stormRadius;
            Healing = healing; MaximumLife = maximumLife;
        }
    }

    public static class PickupCatalog
    {
        static readonly PickupDefinition[] definitions = {
            new(PickupKind.Salve, "Rain salve", "Restore 40 life. Saved if already full.", new Color(.3f, 1, .5f), healing: 40),
            new(PickupKind.Heartwood, "Heartwood", "+25 maximum life and restore 25 life.", new Color(1, .65f, .3f), healing: 25, maximumLife: 25),
            new(PickupKind.NeedleCore, "Needle core", "Carbine: 27 damage, slower 0.22s shots. Storm cooldown 7s. Pick one core per run.", new Color(.8f, 1, .65f), kineticDamage: 27, kineticInterval: .22f, stormCooldown: 7),
            new(PickupKind.EmberCore, "Cinder core", "Ember: 32 damage every 0.35s. Carbine damage 10. Pick one core per run.", new Color(1, .35f, .1f), kineticDamage: 10, emberDamage: 32, emberInterval: .35f),
            new(PickupKind.StormCore, "Tempest core", "Storm: 38 damage, 20m radius, 3s cooldown. Ember damage 12. Pick one core per run.", new Color(.25f, .8f, 1), emberDamage: 12, stormDamage: 38, stormCooldown: 3, stormRadius: 20)
        };
        public static PickupDefinition Get(PickupKind kind) => definitions[(int)kind];
        // Deliberately independent of the decoration/enemy random stream.
        public static PickupKind[] Offerings(int seed, int shrine = 0)
        {
            var result = new[] { PickupKind.NeedleCore, PickupKind.EmberCore, PickupKind.StormCore };
            var random = new System.Random(unchecked(seed * 397 ^ shrine * 7919 ^ 0x51F15E));
            for (int i = result.Length - 1; i > 0; i--) { int j = random.Next(i + 1); (result[i], result[j]) = (result[j], result[i]); }
            return result;
        }
    }

    // One instance per Wayfarer. No static run powers or persistent inventory.
    public sealed class RunBuild
    {
        readonly HashSet<string> collected = new();
        public PickupKind? Core { get; private set; }
        public int Supplies { get; private set; }
        public bool HasCollected(string id) => collected.Contains(id);
        public bool CanCollect(string id, PickupKind kind) => !HasCollected(id) && (!PickupCatalog.Get(kind).IsModifier || !Core.HasValue);
        public bool Collect(string id, PickupKind kind, Health life)
        {
            if (life == null || life.Dead || !CanCollect(id, kind)) return false;
            if (kind == PickupKind.Salve && life.Current >= life.Maximum) return false;
            var definition = PickupCatalog.Get(kind);
            if (definition.IsModifier) Core = kind;
            else { life.Maximum += definition.MaximumLife; life.Heal(definition.Healing); }
            collected.Add(id); if (!PickupCatalog.Get(kind).IsModifier) Supplies++;
            return true;
        }
        PickupDefinition Stats => PickupCatalog.Get(Core ?? PickupKind.Salve);
        public float Damage(Element element) => element == Element.Kinetic ? Stats.KineticDamage : element == Element.Ember ? Stats.EmberDamage : Stats.StormDamage;
        public float ShotInterval(Element element) => element == Element.Ember ? Stats.EmberInterval : Stats.KineticInterval;
        public float StormCooldown => Stats.StormCooldown;
        public float StormRadius => Stats.StormRadius;
        public string Summary => Core.HasValue ? PickupCatalog.Get(Core.Value).Name + " — " + PickupCatalog.Get(Core.Value).Description : "No core — choose ONE at the three-light shrine ahead.";
    }
}
