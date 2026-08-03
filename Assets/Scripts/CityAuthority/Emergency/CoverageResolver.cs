using CityAuthority.Data;

namespace CityAuthority.Emergency
{
    // 09 §3-4: travel-time band resolution plus the concurrent-demand downgrade.
    public static class CoverageResolver
    {
        public static CoverageState ResolveBand(float travelMinutes, TravelTimeBandsConfig bands)
        {
            if (travelMinutes <= bands.AdequateMaxMinutes)
            {
                return CoverageState.Adequate;
            }

            if (travelMinutes <= bands.ReducedMaxMinutes)
            {
                return CoverageState.Reduced;
            }

            return CoverageState.Uncovered;
        }

        // Coverage state for a district is the worse of the travel-time band from the
        // nearest uncommitted unit and a concurrent-demand downgrade (09 §4).
        public static CoverageState ResolveDistrictCoverage(
            DepartmentCoverageState departmentState,
            District district,
            int concurrentOpenCalls,
            TravelTimeBandsConfig bands)
        {
            if (departmentState.UncommittedUnitCount <= 0)
            {
                return CoverageState.Uncovered;
            }

            var travelMinutes = FindBaseTravelTime(departmentState.Department, district);
            if (travelMinutes == null)
            {
                return CoverageState.Uncovered;
            }

            var band = ResolveBand(travelMinutes.Value, bands);

            if (concurrentOpenCalls > departmentState.UncommittedUnitCount)
            {
                band = Downgrade(band);
            }

            return band;
        }

        private static CoverageState Downgrade(CoverageState state)
        {
            return state == CoverageState.Adequate ? CoverageState.Reduced : CoverageState.Uncovered;
        }

        // The department's base (uncommitted, no concurrent demand) travel time to a
        // district, regardless of current commitment state — this is the raw drive time
        // used both for coverage queries and for resolving an actively dispatched unit's
        // arrival at its target.
        public static float? FindBaseTravelTime(DepartmentDefinition department, District district)
        {
            foreach (var area in department.CoverageAreas)
            {
                if (area.District == district)
                {
                    return area.BaseTravelTimeMinutes;
                }
            }

            return null;
        }
    }
}
