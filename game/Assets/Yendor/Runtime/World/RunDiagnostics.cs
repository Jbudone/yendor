using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Yendor
{
    public sealed class RunDiagnostics : MonoBehaviour
    {
        static RunDiagnostics instance;
        readonly Queue<string> recent = new Queue<string>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install()
        {
            if (!instance) new GameObject("Run diagnostics (F8)").AddComponent<RunDiagnostics>();
        }
        void Awake()
        {
            if (instance && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        void OnEnable() { if (instance == this) Application.logMessageReceived += Record; }
        void OnDestroy() { if (instance == this) instance = null; }
        void OnDisable() => Application.logMessageReceived -= Record;
        void Record(string message, string stack, LogType type)
        {
            recent.Enqueue(type + ": " + message.Substring(0, Math.Min(message.Length, 1000)));
            while (recent.Count > 64) recent.Dequeue();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                try { WriteReport(); }
                catch (Exception error) { Debug.LogException(error); }
            }
        }

        [Serializable]
        sealed class Report
        {
            public string capturedAt, buildGuid, unityVersion, platform, cpu, gpu;
            public int seed, kills, totalEnemies, returnEnemiesSpawned, supplies;
            public string phase, core;
            public bool alarmRaised;
            public bool ended, hasRelic, wet;
            public float health, burningUntil;
            public Vector3 position;
            public string[] recentEvents;
        }

        public string WriteReport()
        {
            var run = Expedition.Instance;
            var health = run && run.Player ? run.Player.GetComponent<Health>() : null;
            string dir = Path.Combine(Application.persistentDataPath, "Reports", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff"));
            Directory.CreateDirectory(dir);
            var report = new Report {
                capturedAt = DateTime.UtcNow.ToString("o"), buildGuid = Application.buildGUID,
                unityVersion = Application.unityVersion, platform = Application.platform.ToString(),
                cpu = SystemInfo.processorType, gpu = SystemInfo.graphicsDeviceName,
                seed = run ? run.Seed : 0, kills = run ? run.Kills : 0,
                ended = run && run.Ended, hasRelic = run && run.HasRelic,
                totalEnemies = run ? run.TotalEnemies : 0, returnEnemiesSpawned = run ? run.ReturnEnemiesSpawned : 0,
                phase = run ? run.Phase : "Unavailable", alarmRaised = run && run.AlarmRaised,
                core = run && run.Player ? run.Player.Build.Core?.ToString() ?? "None" : "None",
                supplies = run && run.Player ? run.Player.Build.Supplies : 0,
                position = run && run.Player ? run.Player.transform.position : Vector3.zero,
                health = health ? health.Current : 0, wet = health && health.Wet,
                burningUntil = health ? health.BurningUntil : 0, recentEvents = recent.ToArray()
            };
            File.WriteAllText(Path.Combine(dir, "run.json"), JsonUtility.ToJson(report, true));
            if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                ScreenCapture.CaptureScreenshot(Path.Combine(dir, "screenshot.png"));
            Debug.Log("YENDOR_REPORT_WRITTEN: " + dir);
            return dir;
        }
    }
}
