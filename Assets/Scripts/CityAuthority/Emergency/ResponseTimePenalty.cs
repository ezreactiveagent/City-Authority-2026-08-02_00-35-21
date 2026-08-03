using CityAuthority.Data;

namespace CityAuthority.Emergency
{
    // 09 §5: severity multiplier applied to whatever downstream loss calculation
    // an incident type defines. This report only computes the multiplier itself.
    public static class ResponseTimePenalty
    {
        public const float BaselineMultiplier = 1f;
        public const float ReducedBandMultiplier = 1.25f;
        public const float UncoveredBandMultiplier = 1.6f;

        public static float SeverityMultiplier(float resolvedTravelMinutes, TravelTimeBandsConfig bands)
        {
            if (resolvedTravelMinutes <= bands.AdequateMaxMinutes)
            {
                return BaselineMultiplier;
            }

            if (resolvedTravelMinutes <= bands.ReducedMaxMinutes)
            {
                return ReducedBandMultiplier;
            }

            return UncoveredBandMultiplier;
        }
    }
}
