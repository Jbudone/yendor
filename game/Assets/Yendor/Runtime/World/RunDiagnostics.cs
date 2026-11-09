using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Yendor
{
    public sealed class RunDiagnostics : MonoBehaviour
    {
        readonly Queue<string> recent = new Queue<string>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install() => new GameObject("Run diagnostics (F8)").AddComponent<RunDiagnostics>();
        void OnEnable() => Application.logMessageReceived += Record;
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
            public int seed, kills;
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
