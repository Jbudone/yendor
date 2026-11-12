using UnityEngine;

namespace Yendor
{
    [RequireComponent(typeof(CharacterController), typeof(Health))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        public Camera View;
        public float Speed = 9;
        public float HitVisibleUntil { get; private set; }
        public float HurtVisibleUntil { get; private set; }
        public float KillVisibleUntil { get; private set; }
        public string LastKill { get; private set; }
        public bool LastHitKilled { get; private set; }
        AudioSource voice;
        AudioClip shotSound, hitSound, killSound, hurtSound;
        public float DashReadyAt { get; private set; }
        public float StormReadyAt { get; private set; }
        float yaw, pitch = 13, vertical, shotAt, emberAt, dashUntil, nextHurtCueAt;
        int jumps;
        Vector3 dashDirection;
        CharacterController motor;
        Health health;

        void Awake()
        {
            motor = GetComponent<CharacterController>(); health = GetComponent<Health>();
            voice = gameObject.AddComponent<AudioSource>(); voice.spatialBlend = 0; voice.volume = .22f;
            shotSound = Tone(180, .055f); hitSound = Tone(650, .065f);
            killSound = Tone(960, .16f); hurtSound = Tone(90, .15f);
            health.Damaged += (amount, element) => {
                HurtVisibleUntil = Time.time + .4f;
                if (Time.time >= nextHurtCueAt) { voice.PlayOneShot(hurtSound); nextHurtCueAt = Time.time + .3f; }
            };
        }
        static AudioClip Tone(float frequency, float duration)
        {
            const int rate = 22050;
            var data = new float[Mathf.CeilToInt(rate * duration)];
            for (int i = 0; i < data.Length; i++)
            {
                float envelope = 1f - i / (float)data.Length;
                data[i] = Mathf.Sin(i * frequency * 2 * Mathf.PI / rate) * envelope * envelope * Mathf.Min(1, i / 40f);
            }
            var clip = AudioClip.Create("Procedural combat cue", data.Length, 1, rate, false);
            clip.SetData(data, 0); return clip;
        }
        public void ConfirmKill(string role)
        {
            LastKill = role; KillVisibleUntil = Time.time + 1.3f;
            voice.PlayOneShot(killSound);
        }
        void ConfirmHit(Health target, float damage, Element element)
        {
            bool alive = !target.Dead;
            target.Damage(damage, element);
            if (!alive || target.GetComponent<Enemy>() == null) return;
            HitVisibleUntil = Time.time + .16f; LastHitKilled = target.Dead;
            if (!target.Dead) voice.PlayOneShot(hitSound);
        }
        void OnDestroy()
        {
            foreach (var clip in new[] { shotSound, hitSound, killSound, hurtSound }) if (clip) Destroy(clip);
        }
        void Start() { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }

        void Update()
        {
            if (Expedition.Instance.Ended) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                if (Input.GetMouseButtonDown(0)) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
                return;
            }
            yaw += Input.GetAxisRaw("Mouse X") * 2.2f;
            pitch = Mathf.Clamp(pitch - Input.GetAxisRaw("Mouse Y") * 1.8f, -45, 70);
            transform.rotation = Quaternion.Euler(0, yaw, 0);
            Vector3 move = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
            move = Vector3.ClampMagnitude(move, 1);
            if (motor.isGrounded && vertical < 0) { vertical = -2; jumps = 0; }
            if (Input.GetKeyDown(KeyCode.Space) && jumps < 2) { vertical = 10; jumps++; }
            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= DashReadyAt)
            {
                dashDirection = move.sqrMagnitude > 0 ? move : transform.forward;
                dashUntil = Time.time + .18f;
                DashReadyAt = Time.time + 1.3f;
            }
            vertical -= 26 * Time.deltaTime;
            motor.Move(((Time.time < dashUntil ? dashDirection * 29 : move * Speed) + Vector3.up * vertical) * Time.deltaTime);
            if (transform.position.y < -12) health.Damage(1000, Element.Kinetic);
            if (Input.GetMouseButton(0) && Time.time >= shotAt) { shotAt = Time.time + .14f; Fire(Element.Kinetic, 15); }
            if (Input.GetMouseButton(1) && Time.time >= emberAt) { emberAt = Time.time + .65f; Fire(Element.Ember, 22); }
            if (Input.GetKeyDown(KeyCode.Q) && Time.time >= StormReadyAt)
            {
                StormReadyAt = Time.time + 5;
                foreach (var enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
                    if (Vector3.Distance(transform.position, enemy.transform.position) < 14)
                    {
                        ConfirmHit(enemy.GetComponent<Health>(), 28, Element.Storm);
                        Expedition.Beam(transform.position + Vector3.up, enemy.transform.position + Vector3.up, Color.cyan);
                    }
            }
            if (Input.GetKeyDown(KeyCode.E)) Expedition.Instance.Interact();
        }

        void LateUpdate()
        {
            if (!View) return;
            Quaternion look = Quaternion.Euler(pitch, yaw, 0);
            Vector3 pivot = transform.position + Vector3.up * 1.65f;
            Vector3 offset = look * new Vector3(.7f, .15f, -4.8f);
            float distance = offset.magnitude;
            if (Physics.SphereCast(pivot, .2f, offset.normalized, out var hit, distance, ~(1 << 2), QueryTriggerInteraction.Ignore))
                distance = Mathf.Max(.1f, hit.distance - .1f);
            View.transform.SetPositionAndRotation(pivot + offset.normalized * distance, look);
        }

        void Fire(Element element, float damage)
        {
            voice.PlayOneShot(shotSound, element == Element.Ember ? .7f : .45f);
            Vector3 target = View.transform.position + View.transform.forward * 100;
            if (Physics.Raycast(View.ViewportPointToRay(new Vector3(.5f, .5f)), out var aim, 100, ~(1 << 2))) target = aim.point;
            Vector3 muzzle = transform.position + Vector3.up * 1.4f + transform.forward * .6f;
            if (Physics.Raycast(muzzle, (target - muzzle).normalized, out var hit, 100, ~(1 << 2)))
            {
                target = hit.point;
                var targetHealth = hit.collider.GetComponentInParent<Health>();
                if (targetHealth) ConfirmHit(targetHealth, damage, element);
                hit.collider.GetComponentInParent<Surface>()?.Apply(element);
            }
            Expedition.Beam(muzzle, target, element == Element.Ember ? new Color(1, .4f, .08f) : new Color(.7f, 1, .85f));
        }
    }
}
