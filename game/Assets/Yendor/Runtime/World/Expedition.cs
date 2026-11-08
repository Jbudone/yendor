using UnityEngine;
using UnityEngine.SceneManagement;

namespace Yendor
{
    public sealed class Expedition : MonoBehaviour
    {
        public static Expedition Instance { get; private set; }
        public PlayerMotor Player { get; private set; }
        public int Seed = 43117;
        public int Kills;
        public bool Ended { get; private set; }
        public bool HasRelic { get; private set; }
        public Vector3 RelicPosition { get; private set; }
        public Vector3 ExitPosition => new Vector3(0, 0, -28);
        GameObject relic;
        bool escaped;
        float startedAt;
        float finishedAt;
        static int? nextSeed;
        readonly System.Collections.Generic.List<Material> materials = new();

        void Awake() { Instance = this; if (nextSeed.HasValue) Seed = nextSeed.Value; }
        void Start()
        {
            startedAt = Time.time;
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(.15f, .26f, .28f);
            RenderSettings.fogDensity = .012f;
            RenderSettings.ambientLight = new Color(.42f, .54f, .55f);
            var sun = new GameObject("Late afternoon sun").AddComponent<Light>();
            sun.type = LightType.Directional; sun.intensity = 1.3f; sun.color = new Color(1, .82f, .6f);
            sun.transform.rotation = Quaternion.Euler(35, -35, 0);
            sun.shadows = LightShadows.Soft;
            BuildWorld();
            var hero = new GameObject("Wayfarer");
            hero.layer = 2;
            hero.transform.position = ExitPosition + Vector3.up * 2;
            var cc = hero.AddComponent<CharacterController>(); cc.height = 2; cc.center = Vector3.up; cc.radius = .4f;
            var health = hero.AddComponent<Health>();
            health.Died += () => Finish(false);
            var body = Shape("Wayfarer cloak", PrimitiveType.Capsule, hero.transform.position + Vector3.up, new Vector3(.7f, 1, .7f), new Color(.8f, .65f, .32f));
            Destroy(body.GetComponent<Collider>()); body.layer = 2; body.transform.SetParent(hero.transform, true);
            Player = hero.AddComponent<PlayerMotor>();
            var camera = new GameObject("Expedition camera").AddComponent<Camera>();
            camera.tag = "MainCamera"; camera.fieldOfView = 78; camera.farClipPlane = 220;
            camera.backgroundColor = RenderSettings.fogColor;
            camera.gameObject.AddComponent<AudioListener>();
            Player.View = camera;
        }

        void BuildWorld()
        {
            var random = new System.Random(Seed);
            Shape("Moss plateau", PrimitiveType.Cube, new Vector3(0, -1, 5), new Vector3(100, 2, 110), new Color(.21f, .32f, .22f));
            Shape("Pilgrim path", PrimitiveType.Cube, new Vector3(0, .015f, 0), new Vector3(8, .03f, 66), new Color(.45f, .4f, .28f));
            for (int i = 0; i < 90; i++)
            {
                float x = (float)random.NextDouble() * 90 - 45;
                float z = (float)random.NextDouble() * 96 - 42;
                if (Mathf.Abs(x) < 8 || (new Vector2(x, z - 30)).magnitude < 17) continue;
                float height = 5 + (float)random.NextDouble() * 7;
                Shape("Copperpine trunk", PrimitiveType.Cylinder, new Vector3(x, height / 2, z), new Vector3(.7f, height / 2, .7f), new Color(.27f, .22f, .17f));
                var crown = Shape("Copperpine crown", PrimitiveType.Sphere, new Vector3(x, height, z), new Vector3(5, 6, 5), new Color(.18f + (float)random.NextDouble() * .12f, .4f, .3f));
                Destroy(crown.GetComponent<Collider>());
            }
            for (int side = -1; side <= 1; side += 2)
                for (int i = 0; i < 5; i++)
                {
                    Shape("Ruin pillar", PrimitiveType.Cube, new Vector3(side * 12, 4, 16 + i * 7), new Vector3(2, 8, 2), new Color(.48f, .49f, .42f));
                    Shape("Ruin lintel", PrimitiveType.Cube, new Vector3(side * 12, 8.5f, 19 + i * 7), new Vector3(3, 1, 8), new Color(.42f, .44f, .39f));
                }
            Shape("Relic dais", PrimitiveType.Cylinder, new Vector3(0, .65f, 38), new Vector3(5, .65f, 5), new Color(.48f, .49f, .42f));
            RelicPosition = new Vector3(0, 2.5f, 38);
            relic = Shape("Starheart", PrimitiveType.Sphere, RelicPosition, Vector3.one, new Color(1, .75f, .2f));
            Destroy(relic.GetComponent<Collider>());
            var glow = relic.AddComponent<Light>(); glow.color = new Color(1, .7f, .25f); glow.range = 12; glow.intensity = 4;
            var exit = Shape("Waystone", PrimitiveType.Cylinder, ExitPosition + Vector3.up * .15f, new Vector3(5, .15f, 5), new Color(.2f, .75f, .7f));
            for (int i = 0; i < 2; i++)
            {
                var pool = Shape(i == 0 ? "Rain pool" : "Oil seep", PrimitiveType.Cylinder, new Vector3(i == 0 ? -4 : 4, .09f, 14 + i * 12), new Vector3(8, .07f, 8), Color.cyan);
                var surface = pool.AddComponent<Surface>(); surface.Water = i == 0;
            }
            for (int i = 0; i < 7; i++) SpawnEnemy(new Vector3((i % 2 == 0 ? -1 : 1) * (3 + random.Next(5)), 1, 4 + i * 5));
        }

        void SpawnEnemy(Vector3 position)
        {
            var enemy = new GameObject("Briar revenant"); enemy.transform.position = position;
            var cc = enemy.AddComponent<CharacterController>(); cc.center = Vector3.up; cc.height = 2; cc.radius = .55f;
            var body = Shape("Revenant shell", PrimitiveType.Capsule, position + Vector3.up, new Vector3(1, 1, 1), new Color(.65f, .2f, .32f));
            Destroy(body.GetComponent<Collider>()); body.transform.SetParent(enemy.transform, true);
            enemy.AddComponent<Health>(); enemy.AddComponent<Enemy>();
        }

        GameObject Shape(string label, PrimitiveType kind, Vector3 position, Vector3 size, Color color)
        {
            var item = GameObject.CreatePrimitive(kind); item.name = label;
            item.transform.position = position; item.transform.localScale = size;
            var material = new Material(Shader.Find("Standard")); material.color = color;
            materials.Add(material); item.GetComponent<Renderer>().sharedMaterial = material;
            return item;
        }

        void Update()
        {
            if (!Player) return;
            if (relic) relic.transform.Rotate(0, 65 * Time.deltaTime, 0);
            if (Ended && Input.GetKeyDown(KeyCode.R)) Restart(false);
            if (Ended && Input.GetKeyDown(KeyCode.N)) Restart(true);
        }

        public void Interact()
        {
            if (!HasRelic && Vector3.Distance(Player.transform.position, RelicPosition) < 4)
            { HasRelic = true; Destroy(relic); Player.GetComponent<Health>().Heal(35); }
            if (HasRelic && Vector3.Distance(Player.transform.position, ExitPosition) < 4) Finish(true);
        }

        void Finish(bool success) { if (Ended) return; Ended = true; escaped = success; finishedAt = Time.time; Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        void Restart(bool fresh) { nextSeed = fresh ? Seed + 1 : Seed; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

        void OnGUI()
        {
            if (!Player) return;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * (Screen.height / 900f));
            var title = new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold };
            var label = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
            GUI.Label(new Rect(30, 24, 800, 45), "YENDOR / THE COPPER WILD", title);
            GUI.Label(new Rect(30, 70, 900, 35), $"LIFE {Mathf.CeilToInt(Player.GetComponent<Health>().Current)}     REVENANTS {Kills}/7     SEED {Seed}", label);
            GUI.Label(new Rect(30, 110, 750, 60), HasRelic ? "Return the Starheart to the turquoise waystone. Press E to escape." : "Follow the old road. Recover the Starheart from the ruins [E].", label);
            GUI.Label(new Rect(30, 775, 1000, 100), "WASD move · Mouse aim · Space double jump · Shift dash\nLMB carbine · RMB ember · Q storm · E interact · Esc release cursor\nEmber ignites oil. Water extinguishes fire and doubles storm damage.", label);
            GUI.Label(new Rect(Screen.width / (Screen.height / 900f) / 2 - 6, 440, 30, 30), "+", label);
            if (Ended)
            {
                GUI.Box(new Rect(240, 270, 700, 180), "");
                GUI.Label(new Rect(270, 290, 650, 50), escaped ? "THE STARHEART RETURNS" : "THE WILD KEEPS ITS DUE", title);
                GUI.Label(new Rect(270, 350, 650, 80), $"{Kills} revenants defeated · {Mathf.FloorToInt(finishedAt - startedAt)} seconds\nR replay this seed · N explore the next seed", label);
            }
        }

        void OnDestroy() { foreach (var material in materials) if (material) Destroy(material); if (Instance == this) Instance = null; }

        public static void Beam(Vector3 from, Vector3 to, Color color)
        {
            var beam = new GameObject("Spell tracer").AddComponent<LineRenderer>();
            var material = new Material(Shader.Find("Sprites/Default"));
            beam.sharedMaterial = material; beam.startColor = beam.endColor = color;
            beam.startWidth = .06f; beam.endWidth = .02f; beam.positionCount = 2;
            beam.SetPosition(0, from); beam.SetPosition(1, to);
            Destroy(material, .1f); Destroy(beam.gameObject, .1f);
        }
    }
}
