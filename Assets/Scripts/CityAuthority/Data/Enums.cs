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
}
