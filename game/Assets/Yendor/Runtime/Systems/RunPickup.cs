using UnityEngine;

namespace Yendor
{
    public sealed class RunPickup : MonoBehaviour
    {
        public string Id { get; private set; }
        public PickupKind Kind { get; private set; }
        public PickupDefinition Definition => PickupCatalog.Get(Kind);
        public const float Reach = 2.6f;
        public void Configure(string id, PickupKind kind) { Id = id; Kind = kind; }
        public bool Available(PlayerMotor player) => player && player.Build.CanCollect(Id, Kind);
        public bool TryCollect(PlayerMotor player, bool runEnded)
        {
            if (runEnded || !Available(player) || player.GetComponent<Health>().Dead || Vector3.Distance(transform.position, player.transform.position) > Reach) return false;
            if (!player.Build.Collect(Id, Kind, player.GetComponent<Health>())) return false;
            gameObject.SetActive(false);
            return true;
        }
    }
}
