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
        public int TotalEnemies { get; private set; }
        public int ReturnEnemiesSpawned { get; private set; }
        public bool AlarmRaised { get; private set; }
        public string Phase => Ended ? (escaped ? "Escaped" : "Lost") : HasRelic ? "Return" : "Outbound";
        public Vector3 OverlookCachePosition => new Vector3(20, 4.85f, 11);
        public bool Ended { get; private set; }
        public bool HasRelic { get; private set; }
        public Vector3 RelicPosition { get; private set; }
        public Vector3 ExitPosition => new Vector3(0, 0, -28);
        public Vector3 ShrinePosition => new Vector3(0, .8f, -24);
        GameObject relic;
        AudioSource waystoneActivation;
        bool escaped;
        float startedAt;
        float finishedAt;
        static int? nextSeed;
        readonly System.Collections.Generic.List<Material> materials = new();
        Material materialTemplate;

        void Awake() { Instance = this; if (nextSeed.HasValue) Seed = nextSeed.Value; }
        void Start()
        {
            materialTemplate = Resources.Load<Material>("YendorGreybox");
            if (!materialTemplate) throw new System.InvalidOperationException("Missing YendorGreybox material resource. Create the greybox render resources in the Editor.");
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
            BuildPickups();
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
            var detourTrees = new System.Collections.Generic.List<GameObject>();
            Shape("Moss plateau", PrimitiveType.Cube, new Vector3(0, -1, 5), new Vector3(100, 2, 110), new Color(.21f, .32f, .22f));
            Shape("Pilgrim path", PrimitiveType.Cube, new Vector3(0, .015f, 0), new Vector3(8, .03f, 66), new Color(.45f, .4f, .28f));
            for (int i = 0; i < 90; i++)
            {
                float x = (float)random.NextDouble() * 90 - 45;
                float z = (float)random.NextDouble() * 96 - 42;
                if (Mathf.Abs(x) < 8 || (new Vector2(x, z - 30)).magnitude < 17) continue;
                float height = 5 + (float)random.NextDouble() * 7;
                var trunk = Shape("Copperpine trunk", PrimitiveType.Cylinder, new Vector3(x, height / 2, z), new Vector3(.7f, height / 2, .7f), new Color(.27f, .22f, .17f));
                var crown = Shape("Copperpine crown", PrimitiveType.Sphere, new Vector3(x, height, z), new Vector3(5, 6, 5), new Color(.18f + (float)random.NextDouble() * .12f, .4f, .3f));
                Destroy(crown.GetComponent<Collider>());
                // Consume the full original decoration stream before clearing the authored loop.
                if (x > 7 && x < 28 && z > -18 && z < 39) { detourTrees.Add(trunk); detourTrees.Add(crown); }
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
            var waystonePrefab = Resources.Load<GameObject>("YendorWaystone");
            if (!waystonePrefab) throw new System.InvalidOperationException("Missing YendorWaystone prefab resource. Author the reviewed waystone in the Editor.");
            var waystone = Instantiate(waystonePrefab, ExitPosition, Quaternion.identity);
            waystoneActivation = waystone.GetComponent<AudioSource>();
            for (int i = 0; i < 2; i++)
            {
                var pool = Shape(i == 0 ? "Rain pool" : "Oil seep", PrimitiveType.Cylinder, new Vector3(i == 0 ? -4 : 4, .09f, 14 + i * 12), new Vector3(8, .07f, 8), Color.cyan);
                var surface = pool.AddComponent<Surface>(); surface.Water = i == 0;
            }
            foreach (var tree in detourTrees) { tree.SetActive(false); Destroy(tree); }
            BuildOverlook();
            BuildLandmarks();
            for (int i = 0; i < 7; i++) SpawnEnemy(new Vector3((i % 2 == 0 ? -1 : 1) * (3 + random.Next(5)), 1, 4 + i * 5), i);
        }

        void BuildOverlook()
        {
            var stone = new Color(.49f, .42f, .31f);
            Shape("Overlook south approach", PrimitiveType.Cube, new Vector3(11.5f, -.2f, -12), new Vector3(23, .5f, 6), stone);
            Ramp("Overlook ascent", new Vector3(20, .05f, -12), new Vector3(20, 4, 4), stone);
            Shape("Heartwood overlook deck", PrimitiveType.Cube, new Vector3(20, 3.75f, 11), new Vector3(6, .5f, 14.4f), stone);
            Ramp("Overlook descent", new Vector3(20, 4, 18), new Vector3(20, .05f, 32), stone);
            Shape("Overlook north approach", PrimitiveType.Cube, new Vector3(11.5f, -.2f, 33), new Vector3(23, .5f, 4), stone);
            // Solid foundations and broad ramps keep this an authored route, not floating jump pads.
            Shape("Overlook foundation", PrimitiveType.Cube, new Vector3(20, 1.75f, 11), new Vector3(5.7f, 3.5f, 13.8f), stone);
            foreach (float z in new[] { 7f, 15f })
                Shape("Overlook firing cover", PrimitiveType.Cube, new Vector3(17.3f, 4.7f, z), new Vector3(.6f, 1.4f, 2.5f), stone);
            Shape("Overlook outer parapet", PrimitiveType.Cube, new Vector3(23.1f, 4.4f, 11), new Vector3(.4f, .8f, 14), stone);
            foreach (var point in new[] { new Vector3(5, 0, -15.6f), new Vector3(22.5f, 0, -12), new Vector3(22.5f, 4, 11), new Vector3(22.5f, 0, 33), new Vector3(5, 0, 35.6f) })
            {
                Shape("Amber overlook trail cairn", PrimitiveType.Cylinder, point + Vector3.up * .6f, new Vector3(.7f, .6f, .7f), stone);
                var cue = Shape("Amber overlook trail light", PrimitiveType.Sphere, point + Vector3.up * 1.4f, Vector3.one * .65f, new Color(1, .6f, .15f));
                Destroy(cue.GetComponent<Collider>());
            }
            SpawnPickup("overlook-heartwood", PickupKind.Heartwood, OverlookCachePosition + Vector3.back * 1.5f);
            SpawnPickup("overlook-salve", PickupKind.Salve, OverlookCachePosition + Vector3.forward * 1.5f);
        }

        void Ramp(string name, Vector3 start, Vector3 end, Color color)
        {
            Vector3 delta = end - start;
            float angle = -Mathf.Atan2(delta.y, delta.z) * Mathf.Rad2Deg;
            var ramp = Shape(name, PrimitiveType.Cube, (start + end) * .5f - Vector3.up * (.25f * Mathf.Cos(angle * Mathf.Deg2Rad)), new Vector3(6, .5f, delta.magnitude + .4f), color);
            ramp.transform.rotation = Quaternion.Euler(angle, 0, 0);
        }

        void BuildLandmarks()
        {
            Beacon("Starheart gold spire", new Vector3(0, 0, 42), new Color(1, .73f, .15f));
            Beacon("Waystone turquoise spire", new Vector3(0, 0, -31), new Color(.15f, 1, .9f));
        }

        void Beacon(string name, Vector3 position, Color color)
        {
            foreach (float side in new[] { -1f, 1f })
                Shape(name + " pylon", PrimitiveType.Cube, position + new Vector3(side * 3.6f, 3, 0), new Vector3(.8f, 6, .8f), color * .65f);
            var crown = Shape(name + " crown", PrimitiveType.Sphere, position + Vector3.up * 8, new Vector3(2, 2, 2), color);
            Destroy(crown.GetComponent<Collider>());
            var beam = Shape(name + " beacon", PrimitiveType.Cylinder, position + Vector3.up * 13, new Vector3(.3f, 6, .3f), color);
            Destroy(beam.GetComponent<Collider>());
        }

        void BuildPickups()
        {
            Shape("Shrine: choose one core", PrimitiveType.Cylinder, ShrinePosition + Vector3.down * .65f, new Vector3(6, .15f, 3), new Color(.35f, .35f, .45f));
            var offerings = PickupCatalog.Offerings(Seed);
            for (int i = 0; i < offerings.Length; i++) SpawnPickup("shrine-" + i, offerings[i], ShrinePosition + Vector3.right * ((i - 1) * 3.5f));
            SpawnPickup("road-heartwood", PickupKind.Heartwood, new Vector3(-3, .8f, -5));
            SpawnPickup("road-salve", PickupKind.Salve, new Vector3(3, .8f, 20));
        }

        public RunPickup SpawnPickup(string id, PickupKind kind, Vector3 position)
        {
            var definition = PickupCatalog.Get(kind);
            var item = Shape(definition.Name, definition.IsModifier ? PrimitiveType.Sphere : PrimitiveType.Cube, position, Vector3.one * .8f, definition.Color);
            Destroy(item.GetComponent<Collider>());
            var light = item.AddComponent<Light>(); light.color = definition.Color; light.range = 5; light.intensity = 1.6f;
            var pickup = item.AddComponent<RunPickup>(); pickup.Configure(id, kind); return pickup;
        }

        public RunPickup NearbyPickup()
        {
            RunPickup closest = null; float distance = RunPickup.Reach;
            foreach (var pickup in FindObjectsByType<RunPickup>(FindObjectsSortMode.None))
            {
                if (!pickup.Available(Player)) continue;
                float candidate = Vector3.Distance(Player.transform.position, pickup.transform.position);
                if (candidate < distance) { distance = candidate; closest = pickup; }
            }
            return closest;
        }

        Enemy SpawnEnemy(Vector3 position, int ordinal)
        {
            var enemy = new GameObject("Briar revenant"); enemy.transform.position = position;
            var cc = enemy.AddComponent<CharacterController>(); cc.center = Vector3.up; cc.height = 2; cc.radius = .55f;
            var role = (EnemyRole)(ordinal % 3);
            var size = role == EnemyRole.Brute ? new Vector3(1.35f, 1.1f, 1.35f) : role == EnemyRole.Spitter ? new Vector3(.85f, 1.3f, .85f) : new Vector3(.7f, .8f, .7f);
            var body = Shape("Revenant shell", role == EnemyRole.Spitter ? PrimitiveType.Cube : PrimitiveType.Capsule, position + Vector3.up, size, new Color(.65f, .2f, .32f));
            Destroy(body.GetComponent<Collider>()); body.transform.SetParent(enemy.transform, true);
            enemy.AddComponent<Health>(); var actor = enemy.AddComponent<Enemy>(); actor.Configure(role, ordinal);
            TotalEnemies++; return actor;
        }

        GameObject Shape(string label, PrimitiveType kind, Vector3 position, Vector3 size, Color color)
        {
            var item = GameObject.CreatePrimitive(kind); item.name = label;
            item.transform.position = position; item.transform.localScale = size;
            var material = new Material(materialTemplate); material.color = color;
            materials.Add(material); item.GetComponent<Renderer>().sharedMaterial = material;
            return item;
        }

        void Update()
        {
            if (!Player) return;
            foreach (var pickup in FindObjectsByType<RunPickup>(FindObjectsSortMode.None))
                if (!pickup.Available(Player)) pickup.gameObject.SetActive(false);
            if (relic) relic.transform.Rotate(0, 65 * Time.deltaTime, 0);
            if (Ended && Input.GetKeyDown(KeyCode.R)) Restart(false);
            if (Ended && Input.GetKeyDown(KeyCode.N)) Restart(true);
        }

        public void Interact()
        {
            if (!Player || Ended || Player.GetComponent<Health>().Dead) return;
            if (!HasRelic && Vector3.Distance(Player.transform.position, RelicPosition) < 4)
            { HasRelic = true; Destroy(relic); Player.GetComponent<Health>().Heal(35); RaiseAlarm(); return; }
            if (HasRelic && Vector3.Distance(Player.transform.position, ExitPosition) < 4) { Finish(true); return; }
            NearbyPickup()?.TryCollect(Player, Ended);
        }

        void RaiseAlarm()
        {
            if (AlarmRaised || Ended || !Player || Player.GetComponent<Health>().Dead) return;
            AlarmRaised = true;
            // Fixed, supported road positions remain over 10m from every legal relic interaction.
            // A single mixed encounter changes the return without closing either route or the exit.
            foreach (var position in new[] { new Vector3(0, .2f, 18), new Vector3(-3, .2f, 6), new Vector3(3, .2f, -4) })
            {
                SpawnEnemy(position, ReturnEnemiesSpawned);
                ReturnEnemiesSpawned++;
            }
            Debug.Log($"YENDOR_STARHEART_ALARM: seed={Seed} spawned={ReturnEnemiesSpawned} total={TotalEnemies}");
        }

        public string ObjectiveBearing()
        {
            if (!Player || !Player.View) return "";
            Vector3 target = HasRelic ? ExitPosition : RelicPosition;
            Vector3 delta = target - Player.transform.position; delta.y = 0;
            Vector3 forward = Player.View.transform.forward; forward.y = 0;
            float angle = Vector3.SignedAngle(forward, delta, Vector3.up);
            string direction = Mathf.Abs(angle) > 130 ? "BEHIND" : angle > 25 ? "RIGHT" : angle < -25 ? "LEFT" : "AHEAD";
            return $"{(HasRelic ? "TURQUOISE WAYSTONE" : "GOLD STARHEART")} · {delta.magnitude:F0}m · {direction}";
        }

        void Finish(bool success)
        {
            if (Ended) return;
            Ended = true; escaped = success; finishedAt = Time.time;
            if (success && waystoneActivation) waystoneActivation.Play();
            Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        }
        public void Restart(bool fresh) { if (!Ended) return; nextSeed = fresh ? Seed + 1 : Seed; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

        void OnGUI()
        {
            if (!Player) return;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * (Screen.height / 900f));
            var title = new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold };
            var label = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
            GUI.Label(new Rect(30, 24, 800, 45), "YENDOR / THE COPPER WILD", title);
            GUI.Label(new Rect(30, 70, 900, 35), $"LIFE {Mathf.CeilToInt(Player.GetComponent<Health>().Current)}/{Mathf.CeilToInt(Player.GetComponent<Health>().Maximum)}     REVENANTS {Kills}/{TotalEnemies}     SEED {Seed}     {Phase.ToUpperInvariant()}", label);
            GUI.Label(new Rect(30, 110, 1050, 60), ObjectiveBearing() + "\n" + (HasRelic ? "ALARM — three sentries on the return road. Escape [E]; overlook stays open." : "Recover the relic [E]. Amber trail on the right: overlook, cover and supplies."), label);
            if (!Ended && Vector3.Distance(Player.transform.position, OverlookCachePosition) < 14)
                GUI.Label(new Rect(30, 610, 900, 60), "AMBER OVERLOOK · Heartwood + salve cache · ramps join the road at both ends", label);
            GUI.Label(new Rect(30, 175, 920, 65), "BUILD: " + Player.Build.Summary, label);
            var nearby = NearbyPickup();
            bool comparingCores = !Ended && !Player.Build.Core.HasValue && Vector3.Distance(Player.transform.position, ShrinePosition) < 9;
            if (comparingCores)
            {
                GUI.Box(new Rect(25, 255, 790, 275), "");
                GUI.Label(new Rect(40, 262, 760, 30), "SHRINE — choose one core. Approach a light and press E.", label);
                var choices = PickupCatalog.Offerings(Seed);
                for (int i = 0; i < choices.Length; i++)
                {
                    var choice = PickupCatalog.Get(choices[i]);
                    GUI.color = choice.Color;
                    string side = i == 0 ? "LEFT" : i == 1 ? "MIDDLE" : "RIGHT";
                    GUI.Label(new Rect(40, 300 + i * 75, 755, 75), side + " · " + choice.Name + (nearby && nearby.Kind == choice.Kind ? " [E]" : "") + "\n" + choice.Description, label);
                }
                GUI.color = Color.white;
            }
            else if (!Ended && nearby)
            {
                GUI.Box(new Rect(25, 255, 750, 90), "");
                GUI.Label(new Rect(40, 265, 720, 80), "[E] " + nearby.Definition.Name + "\n" + nearby.Definition.Description, label);
            }
            float hudWidth = Screen.width / (Screen.height / 900f);
            float dash = Mathf.Max(0, Player.DashReadyAt - Time.time), storm = Mathf.Max(0, Player.StormReadyAt - Time.time);
            GUI.color = dash == 0 ? new Color(.5f, 1, .8f) : Color.white;
            GUI.Label(new Rect(30, 690, 330, 35), dash == 0 ? "[SHIFT] DASH READY" : $"[SHIFT] DASH  {dash:F1}s", label);
            GUI.color = storm == 0 ? Color.cyan : Color.white;
            GUI.Label(new Rect(30, 728, 330, 35), storm == 0 ? "[Q] STORM READY" : $"[Q] STORM  {storm:F1}s", label);
            GUI.color = Color.white;
            if (!Ended && Time.time < Player.HitVisibleUntil)
            {
                GUI.color = Player.LastHitKilled ? new Color(1, .75f, .2f) : Color.white;
                GUI.Label(new Rect(hudWidth / 2 - 8, 437, 40, 40), "×", title);
                GUI.color = Color.white;
            }
            if (Time.time < Player.KillVisibleUntil) GUI.Label(new Rect(hudWidth / 2 - 140, 505, 400, 40), Player.LastKill + " DEFEATED", label);
            if (!Ended && Time.time < Player.HurtVisibleUntil)
            {
                GUI.color = new Color(1, .12f, .1f, .65f);
                GUI.DrawTexture(new Rect(0, 0, hudWidth, 10), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, 890, hudWidth, 10), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(0, 0, 10, 900), Texture2D.whiteTexture);
                GUI.DrawTexture(new Rect(hudWidth - 10, 0, 10, 900), Texture2D.whiteTexture);
                GUI.color = Color.white;
            }
            if (!Ended) foreach (var enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            {
                if (Vector3.Distance(Player.transform.position, enemy.transform.position) > 30) continue;
                Vector3 point = Player.View.WorldToViewportPoint(enemy.transform.position + Vector3.up * 2.8f);
                if (point.z <= 0 || point.x < 0 || point.x > 1 || point.y < 0 || point.y > 1) continue;
                float x = point.x * hudWidth - 48, y = (1 - point.y) * 900;
                GUI.color = enemy.RoleColor;
                GUI.DrawTexture(new Rect(x, y, 96 * enemy.GetComponent<Health>().Current / enemy.GetComponent<Health>().Maximum, 5), Texture2D.whiteTexture);
                GUI.color = enemy.WindingUp ? Color.yellow : Color.white;
                GUI.Label(new Rect(x - 12, y - 27, 160, 30), enemy.RoleName + (enemy.WindingUp ? " !" : ""), label);
                if (Time.time < enemy.DamageVisibleUntil) GUI.Label(new Rect(x + 28, y - 55, 90, 30), Mathf.CeilToInt(enemy.LastDamage).ToString(), label);
                GUI.color = Color.white;
            }
            if (!Ended) foreach (var pickup in FindObjectsByType<RunPickup>(FindObjectsSortMode.None))
            {
                if (!pickup.Available(Player)) continue;
                if (Vector3.Distance(Player.transform.position, pickup.transform.position) > 24) continue;
                Vector3 point = Player.View.WorldToViewportPoint(pickup.transform.position + Vector3.up);
                if (point.z <= 0 || point.x < 0 || point.x > 1 || point.y < 0 || point.y > 1) continue;
                GUI.color = pickup.Definition.Color;
                GUI.Label(new Rect(point.x * hudWidth - 85, (1 - point.y) * 900, 220, 55), pickup.Definition.Name + (pickup == nearby ? " [E]" : ""), label);
                GUI.color = Color.white;
            }
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
