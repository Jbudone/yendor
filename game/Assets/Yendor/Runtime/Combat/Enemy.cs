using UnityEngine;

namespace Yendor
{
    [RequireComponent(typeof(CharacterController), typeof(Health))]
    public sealed class Enemy : MonoBehaviour
    {
        float attackAt, vertical;
        CharacterController motor;
        Health health;
        Renderer shell;

        void Awake()
        {
            motor = GetComponent<CharacterController>();
            health = GetComponent<Health>();
            shell = GetComponentInChildren<Renderer>();
            health.Died += () => { Expedition.Instance.Kills++; Destroy(gameObject); };
        }

        void Update()
        {
            var run = Expedition.Instance;
            if (!run || run.Ended || !run.Player) return;
            Vector3 delta = run.Player.transform.position - transform.position;
            delta.y = 0;
            float distance = delta.magnitude;
            if (distance < 25 && distance > 1.8f)
            {
                transform.rotation = Quaternion.LookRotation(delta);
                vertical = motor.isGrounded ? -2 : vertical - 20 * Time.deltaTime;
                motor.Move((delta.normalized * 3.6f + Vector3.up * vertical) * Time.deltaTime);
            }
            if (distance < 2.2f && Time.time >= attackAt)
            {
                attackAt = Time.time + 1.1f;
                run.Player.GetComponent<Health>().Damage(12, Element.Kinetic);
            }
            if (shell) shell.material.color = health.Wet ? Color.cyan : Time.time < health.BurningUntil ? new Color(1, .3f, .06f) : new Color(.65f, .2f, .32f);
        }
    }
}
