using UnityEngine;

namespace CityAuthority.Data
{
    [CreateAssetMenu(fileName = "TravelTimeBands_", menuName = "City Authority/Data/Travel Time Bands")]
    public sealed class TravelTimeBandsConfig : ScriptableObject
    {
        [Min(0)] [SerializeField] private float adequateMaxMinutes = 6f;
        [Min(0)] [SerializeField] private float reducedMaxMinutes = 12f;

        public float AdequateMaxMinutes => adequateMaxMinutes;
        public float ReducedMaxMinutes => reducedMaxMinutes;
    }
}
