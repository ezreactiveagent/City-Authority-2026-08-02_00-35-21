namespace CityAuthority.Development
{
    // 10 §2: property-specific developer interest. Pure functions — the slice
    // has no game-clock/tick system yet, so RecoverOverWeeks exists to be
    // correct and testable per spec without a live time-passage system to
    // drive it (10 §2 notes recovery "only matters if the slice is replayed
    // or extended past the single scenario" in its Definition of Done).
    public static class DeveloperInterest
    {
        public const float Starting = 100f;
        public const float RejectionPenalty = 40f;
        public const float RecoveryPerWeek = 5f;
        public const float Floor = 10f;
        public const float Ceiling = 100f;

        public static float ApplyRejectionPenalty(float currentInterest)
        {
            var reduced = currentInterest - RejectionPenalty;
            return reduced < Floor ? Floor : reduced;
        }

        public static float RecoverOverWeeks(float currentInterest, float weeksElapsed)
        {
            var recovered = currentInterest + RecoveryPerWeek * weeksElapsed;
            return recovered > Ceiling ? Ceiling : recovered;
        }
    }
}
