using UnityEngine;

namespace Yendor
{
    public enum EnemyRole { Brute, Spitter, Flanker }

    [RequireComponent(typeof(CharacterController), typeof(Health))]
    public sealed class Enemy : MonoBehaviour
    {
        public EnemyRole Role { get; private set; }
        public bool WindingUp { get; private set; }
        public Vector3 AimPoint { get; private set; }
        public string RoleName => Role == EnemyRole.Brute ? "BRUTE" : Role == EnemyRole.Spitter ? "SPITTER" : "FLANKER";
        public Color RoleColor => Role == EnemyRole.Brute ? new Color(.8f, .23f, .28f) : Role == EnemyRole.Spitter ? new Color(.8f, .56f, .13f) : new Color(.65f, .35f, .9f);
        public float LastDamage { get; private set; }
        public float DamageVisibleUntil { get; private set; }
        float attackAt, releaseAt, vertical;
        int flankSide = 1;
        CharacterController motor;
        Health health;
        Renderer shell;
        LineRenderer telegraph;
        Material lineMaterial;

        void Awake()
        {
            motor = GetComponent<CharacterController>();
            health = GetComponent<Health>();
            shell = GetComponentInChildren<Renderer>();
            health.Damaged += (amount, element) => {
                // Burn ticks retain their orange status; direct hits get a brief white flash.
                if (amount >= 1) { LastDamage = amount; DamageVisibleUntil = Time.time + .3f; }
            };
            health.Died += () => {
                var run = Expedition.Instance;
                if (run && !run.Ended) { run.Kills++; if (run.Player) run.Player.ConfirmKill(RoleName); }
                CancelAttack();
                Destroy(gameObject);
            };
        }

        public void Configure(EnemyRole role, int ordinal)
        {
            Role = role;
            flankSide = ordinal % 2 == 0 ? 1 : -1;
            gameObject.name = RoleName + " / Briar revenant";
            if (shell) shell.sharedMaterial.color = RoleColor;
        }

        void Update()
        {
            var run = Expedition.Instance;
            TickCombat(Time.time, Time.deltaTime, run ? run.Player : null, !run || run.Ended);
            if (shell) shell.sharedMaterial.color = Time.time < DamageVisibleUntil ? Color.white : health.Wet ? Color.cyan : Time.time < health.BurningUntil ? new Color(1, .3f, .06f) : RoleColor;
            DrawTelegraph();
        }

        // Explicit time and target also let deterministic encounter checks exercise real colliders.
        public void TickCombat(float now, float dt, PlayerMotor target, bool ended)
        {
            if (ended || health.Dead || !target || target.GetComponent<Health>().Dead) { CancelAttack(); return; }
            Vector3 delta = target.transform.position - transform.position;
            delta.y = 0;
            float distance = delta.magnitude;
            if (WindingUp)
            {
                if (now < releaseAt) return;
                WindingUp = false;
                attackAt = now + (Role == EnemyRole.Spitter ? 1.8f : Role == EnemyRole.Flanker ? 1.1f : 1.25f);
                if (Role == EnemyRole.Spitter)
                {
                    Vector3 from = transform.position + Vector3.up * 1.3f;
                    Expedition.Beam(from, AimPoint, new Color(1, .55f, .12f));
                    // Aim is committed at windup start; moving or solid cover defeats the shot.
                    if (Vector3.Distance(target.transform.position + Vector3.up, AimPoint) < .95f && HasClearShot(target, AimPoint))
                        target.GetComponent<Health>().Damage(15, Element.Kinetic);
                }
                else if (distance < 2.8f && Vector3.Distance(target.transform.position + Vector3.up, AimPoint) < 1.4f && HasClearShot(target, target.transform.position + Vector3.up))
                    target.GetComponent<Health>().Damage(Role == EnemyRole.Brute ? 18 : 11, Element.Kinetic);
                return;
            }
            if (distance > 27) return;
            if (delta.sqrMagnitude > .01f) transform.rotation = Quaternion.LookRotation(delta);
            float attackRange = Role == EnemyRole.Spitter ? 19 : 2.5f;
            if (distance < attackRange && now >= attackAt && HasClearShot(target, target.transform.position + Vector3.up))
            {
                WindingUp = true;
                AimPoint = target.transform.position + Vector3.up;
                releaseAt = now + (Role == EnemyRole.Brute ? .85f : Role == EnemyRole.Spitter ? 1.05f : .55f);
                return;
            }
            Vector3 move = Vector3.zero;
            if (Role == EnemyRole.Spitter)
            {
                if (distance > 13) move = delta.normalized * 2.7f;
                else if (distance < 8) move = -delta.normalized * 2.7f;
            }
            else if (distance > 1.9f)
            {
                move = delta.normalized;
                if (Role == EnemyRole.Flanker && distance > 4)
                    move = (move + Vector3.Cross(Vector3.up, move) * flankSide * 1.15f).normalized;
                move *= Role == EnemyRole.Flanker ? 5.6f : 3.3f;
            }
            vertical = motor.isGrounded ? -2 : vertical - 20 * dt;
            if (motor.enabled && dt > 0) motor.Move((move + Vector3.up * vertical) * dt);
        }

        public bool HasClearShot(PlayerMotor target, Vector3 point)
        {
            Vector3 from = transform.position + Vector3.up * 1.3f;
            Vector3 delta = point - from;
            foreach (var hit in Physics.RaycastAll(from, delta.normalized, delta.magnitude, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.IsChildOf(transform) || hit.transform.IsChildOf(target.transform)) continue;
                return false;
            }
            return true;
        }

        void CancelAttack() { WindingUp = false; if (telegraph) telegraph.enabled = false; }
        void DrawTelegraph()
        {
            if (!WindingUp) { if (telegraph) telegraph.enabled = false; return; }
            if (!telegraph)
            {
                telegraph = new GameObject("Committed attack warning").AddComponent<LineRenderer>();
                telegraph.transform.SetParent(transform);
                lineMaterial = new Material(Shader.Find("Sprites/Default"));
                telegraph.sharedMaterial = lineMaterial;
                telegraph.positionCount = 2;
                telegraph.startWidth = telegraph.endWidth = Role == EnemyRole.Spitter ? .08f : .25f;
            }
            telegraph.enabled = true;
            telegraph.startColor = telegraph.endColor = Color.Lerp(RoleColor, Color.white, .4f + Mathf.Sin(Time.time * 20) * .3f);
            telegraph.SetPosition(0, transform.position + Vector3.up * (Role == EnemyRole.Spitter ? 1.3f : .15f));
            telegraph.SetPosition(1, Role == EnemyRole.Spitter ? AimPoint : new Vector3(AimPoint.x, transform.position.y + .15f, AimPoint.z));
        }
        void OnDestroy() { if (lineMaterial) Destroy(lineMaterial); }
    }
}
