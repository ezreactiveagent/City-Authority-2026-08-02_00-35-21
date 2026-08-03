using System.Collections.Generic;
using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Data;
using CityAuthority.Development;
using CityAuthority.Emergency;
using CityAuthority.Media;

namespace CityAuthority.SaveLoad
{
    // 08 §13 item 8, 06 §9: converts the slice's live runtime state to/from a
    // JsonUtility-serializable ScenarioSaveData. Restore never re-runs anything
    // that produces a decision (the court ruling, published articles, log
    // entries) — those come back as stored data. Design-time data (districts,
    // proposals, the region/department/court/newspaper definitions) is not
    // duplicated into the save file; it's re-resolved from the same SliceConfig
    // asset the original session used.
    public static class ScenarioSaveService
    {
        public static ScenarioSaveData Capture(
            CityLog log,
            EmergencyIncidentRuntime emergencyRuntime,
            DepartmentCoverageState respondingDepartmentState,
            bool warningResponded,
            bool criticalResponded,
            bool structureCondemned,
            CondemnationCaseRuntime courtCase,
            DevelopmentProposalCycleRuntime developmentCycle,
            NewspaperCoverageRuntime newspaper,
            bool finalReportGenerated)
        {
            var data = new ScenarioSaveData
            {
                structureCondemned = structureCondemned,
                finalReportGenerated = finalReportGenerated
            };

            foreach (var evt in log.Events)
            {
                data.logEvents.Add(ToData(evt));
            }

            var dispatchResult = emergencyRuntime.DispatchResultIfAny;
            data.emergency = new EmergencyStateData
            {
                warningRaised = emergencyRuntime.WarningRaised,
                criticalRaised = emergencyRuntime.CriticalRaised,
                warningResponded = warningResponded,
                criticalResponded = criticalResponded,
                committedUnitCount = respondingDepartmentState.TotalUnitCount - respondingDepartmentState.UncommittedUnitCount,
                hasDispatchResult = dispatchResult != null,
                dispatchResult = dispatchResult == null ? null : ToData(dispatchResult)
            };

            if (courtCase != null && courtCase.HasRuling)
            {
                data.hasCourtRuling = true;
                data.courtRuling = ToData(courtCase.Ruling);
            }

            if (developmentCycle != null)
            {
                data.development = new DevelopmentStateData
                {
                    developerInterest = developmentCycle.DeveloperInterestScore,
                    rejected = developmentCycle.WasRejected,
                    approvedProposalId = developmentCycle.ApprovedProposal != null ? developmentCycle.ApprovedProposal.Id : ""
                };
            }

            if (newspaper != null)
            {
                foreach (var article in newspaper.PublishedArticles)
                {
                    data.articles.Add(ToData(article));
                }
            }

            return data;
        }

        public static RestoredScenario Restore(ScenarioSaveData data, SliceConfig sliceConfig)
        {
            var districts = sliceConfig.EmergencyScenario.Districts;

            var events = new List<AccountabilityEvent>();
            foreach (var eventData in data.logEvents)
            {
                events.Add(FromData(eventData, districts));
            }
            var cityLog = CityLog.FromEvents(events);

            var incident = sliceConfig.EmergencyScenario.Incident;
            var respondingDepartmentState = new DepartmentCoverageState(incident.RespondingDepartment);
            respondingDepartmentState.RestoreCommittedUnitCount(data.emergency.committedUnitCount);

            DispatchResult dispatchResult = null;
            if (data.emergency.hasDispatchResult)
            {
                dispatchResult = FromData(data.emergency.dispatchResult, districts);
            }

            var emergencyRuntime = EmergencyIncidentRuntime.Restore(
                incident,
                respondingDepartmentState,
                districts,
                sliceConfig.CitywideTravelTimeBands,
                cityLog,
                data.emergency.warningRaised,
                data.emergency.criticalRaised,
                dispatchResult);

            CondemnationCaseRuntime courtCase = null;
            if (sliceConfig.CourtCase != null)
            {
                CourtOutcomeOption? selectedOutcome = null;
                string explanation = null;
                float cityAmount = 0f;
                float ownerAmount = 0f;
                var judgeTag = sliceConfig.CourtCase.AssignedJudge != null
                    ? sliceConfig.CourtCase.AssignedJudge.PersonalityTag
                    : default;

                if (data.hasCourtRuling)
                {
                    var rulingData = data.courtRuling;
                    selectedOutcome = new CourtOutcomeOption(
                        rulingData.outcomeType,
                        rulingData.hasSplitIncrement ? rulingData.splitIncrement : null);
                    explanation = rulingData.explanation;
                    cityAmount = rulingData.cityAmount;
                    ownerAmount = rulingData.ownerAmount;
                    judgeTag = rulingData.judgePersonalityTag;
                }

                courtCase = CondemnationCaseRuntime.Restore(
                    sliceConfig.CourtCase, cityLog, selectedOutcome, explanation, cityAmount, ownerAmount, judgeTag);
            }

            DevelopmentProposalCycleRuntime developmentCycle = null;
            if (sliceConfig.DevelopmentListing != null)
            {
                DevelopmentProposal approvedProposal = null;
                if (!string.IsNullOrEmpty(data.development.approvedProposalId))
                {
                    approvedProposal = FindProposal(sliceConfig.DevelopmentListing, data.development.approvedProposalId);
                }

                developmentCycle = DevelopmentProposalCycleRuntime.Restore(
                    sliceConfig.DevelopmentListing, cityLog, data.development.developerInterest, approvedProposal, data.development.rejected);
            }

            NewspaperCoverageRuntime newspaper = null;
            if (sliceConfig.Newspaper != null)
            {
                var articles = new List<NewsArticle>();
                foreach (var articleData in data.articles)
                {
                    articles.Add(FromData(articleData, districts));
                }
                newspaper = NewspaperCoverageRuntime.Restore(sliceConfig.Newspaper, cityLog, articles);
            }

            return new RestoredScenario(
                cityLog,
                respondingDepartmentState,
                emergencyRuntime,
                data.emergency.warningResponded,
                data.emergency.criticalResponded,
                data.structureCondemned,
                courtCase,
                developmentCycle,
                newspaper,
                data.finalReportGenerated);
        }

        private static AccountabilityEventData ToData(AccountabilityEvent evt) => new()
        {
            eventType = evt.EventType,
            summary = evt.Summary,
            districtId = evt.RelatedDistrict != null ? evt.RelatedDistrict.Id : "",
            hasLevel = evt.RelatedLevel.HasValue,
            level = evt.RelatedLevel ?? default,
            hasCategory = evt.Category.HasValue,
            category = evt.Category ?? default
        };

        private static AccountabilityEvent FromData(AccountabilityEventData data, IReadOnlyList<District> districts) => new(
            data.eventType,
            data.summary,
            FindDistrict(districts, data.districtId),
            data.hasLevel ? data.level : null,
            data.hasCategory ? data.category : null);

        private static NotificationData ToData(Notification notification) => new()
        {
            level = notification.Level,
            message = notification.Message,
            districtId = notification.RelatedDistrict != null ? notification.RelatedDistrict.Id : ""
        };

        private static Notification FromData(NotificationData data, IReadOnlyList<District> districts) =>
            new(data.level, data.message, FindDistrict(districts, data.districtId));

        private static DispatchResultData ToData(DispatchResult dispatch)
        {
            var data = new DispatchResultData
            {
                targetDistrictCoverage = dispatch.TargetDistrictCoverage,
                severityMultiplier = dispatch.SeverityMultiplier
            };
            foreach (var secondary in dispatch.SecondaryNotifications)
            {
                data.secondaryNotifications.Add(ToData(secondary));
            }
            return data;
        }

        private static DispatchResult FromData(DispatchResultData data, IReadOnlyList<District> districts)
        {
            var secondaries = new List<Notification>();
            foreach (var secondaryData in data.secondaryNotifications)
            {
                secondaries.Add(FromData(secondaryData, districts));
            }
            return new DispatchResult(data.targetDistrictCoverage, data.severityMultiplier, secondaries);
        }

        private static CourtRulingData ToData(CourtRulingRecord ruling) => new()
        {
            outcomeType = ruling.SelectedOutcome.OutcomeType,
            hasSplitIncrement = ruling.SelectedOutcome.SplitIncrement.HasValue,
            splitIncrement = ruling.SelectedOutcome.SplitIncrement ?? default,
            explanation = ruling.Explanation,
            cityAmount = ruling.CityAmount,
            ownerAmount = ruling.OwnerAmount,
            judgePersonalityTag = ruling.JudgePersonalityTag
        };

        private static NewsArticleData ToData(NewsArticle article) => new()
        {
            sourceEventType = article.SourceEventType,
            headline = article.Headline,
            body = article.Body,
            districtId = article.RelatedDistrict != null ? article.RelatedDistrict.Id : ""
        };

        private static NewsArticle FromData(NewsArticleData data, IReadOnlyList<District> districts) => new(
            data.sourceEventType, data.headline, data.body, FindDistrict(districts, data.districtId));

        private static District FindDistrict(IReadOnlyList<District> districts, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            foreach (var district in districts)
            {
                if (district.Id == id)
                {
                    return district;
                }
            }
            return null;
        }

        private static DevelopmentProposal FindProposal(DevelopmentListingDefinition listing, string id)
        {
            foreach (var proposal in listing.Proposals)
            {
                if (proposal.Id == id)
                {
                    return proposal;
                }
            }
            return null;
        }
    }
}
