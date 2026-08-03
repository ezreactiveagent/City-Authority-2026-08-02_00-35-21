namespace CityAuthority.Data
{
    public enum ZoningCategory
    {
        SingleFamily,
        MixedUse,
        HighDensity,
        Apartments
    }

    public enum DepartmentType
    {
        Fire,
        Police,
        Inspection,
        Education,
        Court
    }

    public enum OperatingPolicyLevel
    {
        Limited,
        Standard,
        Expanded
    }

    public enum EmergencyGenerationMode
    {
        ScriptedOnly,
        FrequencyModel
    }

    public enum MayorOutcomeType
    {
        ProposalApproved,
        ProposalRejected,
        EmergencyContained,
        EmergencyUncontained,
        CourtRulingCityPays,
        CourtRulingOwnerPays,
        CourtRulingSplit,
        CourtRulingUpheld
    }

    // 09 §3-4, 03 §14
    public enum CoverageState
    {
        Adequate,
        Reduced,
        Uncovered
    }

    // 03 §13
    public enum UnitCommitmentState
    {
        Uncommitted,
        CommittedReassignable,
        CommittedUnavailable
    }

    // 03 §5
    public enum NotificationLevel
    {
        Informational,
        Warning,
        Critical
    }

    // 03 §6
    public enum PlayerResponseType
    {
        Act,
        Acknowledge,
        Ignore
    }

    // 03 §6, 05 §13: the categories the final incident/accountability record distinguishes.
    public enum AccountabilityCategory
    {
        IgnoredWarning,
        AcknowledgedUnresolved,
        ActionAttempted,
        ActionCompleted,
        RecommendationFollowed,
        RecommendationRejected
    }

    // 04 §1 (via 11 §4): determines whether a resident is a claimant in the
    // condemnation court case (02 §13) — owners are, renters are affected but not a party.
    public enum HousingStatus
    {
        Owner,
        Renter
    }

    // 10 §3: the four kinds of ruling for the emergency condemnation dispute.
    // Split is parameterized by SplitIncrement rather than being three separate kinds.
    public enum CourtOutcomeType
    {
        CityPays,
        OwnerPays,
        Split,
        InitialAssignmentUpheld
    }

    // 10 §3: "three fixed increments only" — the City's share of the assessed value C.
    public enum SplitIncrement
    {
        TwentyFive,
        Fifty,
        SeventyFive
    }

    // 06 §5: persistent judicial tendencies. Exact decision weights stay hidden
    // from the player; this tag is what the deterministic ruling stub keys off of.
    public enum JudgePersonalityTag
    {
        PublicSafetyFocused,
        BusinessFriendly,
        StrictProceduralist,
        LiabilityCautious,
        CityFriendly,
        UnpredictableWithinLimits
    }
}
