using System.Text;
using CityAuthority.Court;
using CityAuthority.Data;
using CityAuthority.Emergency;

namespace CityAuthority.Media
{
    // 06 §8's structured-event-to-article pattern, filled with prewritten
    // templates per 11 §3 / 06 §11 rather than an LLM call. No outlet
    // personality drift (08 §8) — one canonical template per story kind.
    public static class NewsArticleGenerator
    {
        public static NewsArticle GenerateEmergencyResponseArticle(
            EmergencyIncidentDefinition incident,
            DispatchResult dispatchResult)
        {
            var district = incident.TargetDistrict.DisplayName;
            var department = incident.RespondingDepartment.DisplayName;

            if (dispatchResult == null)
            {
                var headline = $"{district} Fire Response Questioned";
                var body =
                    $"Despite a confirmed life-safety risk at the {district} structure fire, city records show no " +
                    $"{department} unit was ever dispatched to the scene. Residents have called for an explanation " +
                    "of the department's handling of the incident.";
                return new NewsArticle("EmergencyResponse", headline, body, incident.TargetDistrict);
            }

            var sb = new StringBuilder();
            sb.Append($"{department} units responded to a structure fire at a {district} residence, achieving ")
              .Append(dispatchResult.TargetDistrictCoverage)
              .Append(" coverage at the scene.");

            if (dispatchResult.SecondaryNotifications.Count > 0)
            {
                sb.Append(" The response came at a cost: ");
                for (var i = 0; i < dispatchResult.SecondaryNotifications.Count; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append(dispatchResult.SecondaryNotifications[i].RelatedDistrict.DisplayName);
                }
                sb.Append(" was left without adequate coverage while the department's unit was committed, according to city records.");
            }

            return new NewsArticle(
                "EmergencyResponse",
                $"{department} Responds to {district} Structure Fire",
                sb.ToString(),
                incident.TargetDistrict);
        }

        public static NewsArticle GenerateCourtRulingArticle(CondemnationCaseDefinition courtCase, CourtRulingRecord ruling)
        {
            var district = courtCase.TargetDistrict.DisplayName;
            var judge = courtCase.AssignedJudge.JudgeName;

            var headline = $"Court Rules on {district} Fire Condemnation Dispute";
            var body =
                $"{judge} ruled {ruling.SelectedOutcome} in the condemnation case over the {district} structure " +
                $"destroyed by fire, ordering the city to pay ${ruling.CityAmount:N0} and the property owner to pay " +
                $"${ruling.OwnerAmount:N0}. \"{ruling.Explanation}\"";

            return new NewsArticle("CourtRuling", headline, body, courtCase.TargetDistrict);
        }

        // 08 §9: only "if material" — the caller decides whether to publish this,
        // typically only when both proposals were actually rejected.
        public static NewsArticle GenerateDevelopmentRejectionArticle(DevelopmentListingDefinition listing, float interestAfterRejection)
        {
            var district = listing.District.DisplayName;
            var developerNames = new StringBuilder();
            for (var i = 0; i < listing.Proposals.Count; i++)
            {
                if (i > 0) developerNames.Append(" and ");
                developerNames.Append(listing.Proposals[i].DeveloperName);
            }

            var headline = $"City Rejects Both Proposals for {district} Parcel";
            var body =
                $"The city rejected both development proposals for {listing.DisplayName} in {district}, including " +
                $"bids from {developerNames}. Developer interest in the property fell to {interestAfterRejection:N0} " +
                "following the decision, raising questions among observers about the city's approach to private development.";

            return new NewsArticle("DevelopmentRejection", headline, body, listing.District);
        }
    }
}
