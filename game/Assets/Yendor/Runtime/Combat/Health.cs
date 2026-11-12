using UnityEngine;

namespace Yendor
{
    public enum Element { Kinetic, Ember, Storm }

    public sealed class Health : MonoBehaviour
    {
        public float Maximum = 100;
        public float Current { get; private set; }
        float wetUntil;
        public bool Wet { get => Time.time < wetUntil; set => wetUntil = value ? Time.time + .3f : 0; }
        public bool Dead => Current <= 0;
        public float BurningUntil { get; private set; }
        public System.Action Died;
        public System.Action<Element> Hit;
        public System.Action<float, Element> Damaged;

        void Awake() => Current = Maximum;

        public void Damage(float amount, Element element)
        {
            if (Dead) return;
            if (element == Element.Ember && !Wet) BurningUntil = Time.time + 3;
            if (element == Element.Storm && Wet) amount *= 2;
            float previous = Current;
            Current = Mathf.Max(0, Current - amount);
            Damaged?.Invoke(previous - Current, element);
            Hit?.Invoke(element);
            if (Dead) Died?.Invoke();
        }

        public void Heal(float amount) { if (!Dead) Current = Mathf.Min(Maximum, Current + amount); }

        void Update()
        {
            if (Wet) BurningUntil = 0;
            if (Time.time < BurningUntil) Damage(7 * Time.deltaTime, Element.Kinetic);
        }
    }
}
