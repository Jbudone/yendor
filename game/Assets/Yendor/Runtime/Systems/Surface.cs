using UnityEngine;

namespace Yendor
{
    public sealed class Surface : MonoBehaviour
    {
        public bool Water;
        public float Radius = 4;
        float burningUntil;

        public void Apply(Element element)
        {
            if (!Water && element == Element.Ember) burningUntil = Time.time + 8;
        }

        void Update()
        {
            bool burning = Time.time < burningUntil;
            var renderer = GetComponent<Renderer>();
            renderer.material.color = Water ? new Color(.08f, .55f, .65f) : burning ? new Color(1, .27f, .04f) : new Color(.23f, .16f, .1f);
            foreach (var body in FindObjectsByType<Health>(FindObjectsSortMode.None))
            {
                Vector3 delta = body.transform.position - transform.position;
                if (new Vector2(delta.x, delta.z).magnitude > Radius || Mathf.Abs(delta.y) > 3) continue;
                if (Water) body.Wet = true;
                if (burning) body.Damage(16 * Time.deltaTime, Element.Ember);
            }
        }
    }
}
