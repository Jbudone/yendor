using UnityEngine;

namespace Yendor
{
    [RequireComponent(typeof(CharacterController), typeof(Health))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        public Camera View;
        public float Speed = 9;
        public float DashReadyAt { get; private set; }
        public float StormReadyAt { get; private set; }
        float yaw, pitch = 13, vertical, shotAt, emberAt, dashUntil;
        int jumps;
        Vector3 dashDirection;
        CharacterController motor;
        Health health;

        void Awake() { motor = GetComponent<CharacterController>(); health = GetComponent<Health>(); }
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
                        enemy.GetComponent<Health>().Damage(28, Element.Storm);
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
            Vector3 target = View.transform.position + View.transform.forward * 100;
            if (Physics.Raycast(View.ViewportPointToRay(new Vector3(.5f, .5f)), out var aim, 100, ~(1 << 2))) target = aim.point;
            Vector3 muzzle = transform.position + Vector3.up * 1.4f + transform.forward * .6f;
            if (Physics.Raycast(muzzle, (target - muzzle).normalized, out var hit, 100, ~(1 << 2)))
            {
                target = hit.point;
                hit.collider.GetComponentInParent<Health>()?.Damage(damage, element);
                hit.collider.GetComponentInParent<Surface>()?.Apply(element);
            }
            Expedition.Beam(muzzle, target, element == Element.Ember ? new Color(1, .4f, .08f) : new Color(.7f, 1, .85f));
        }
    }
}
