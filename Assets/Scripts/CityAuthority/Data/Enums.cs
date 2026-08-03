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
}
