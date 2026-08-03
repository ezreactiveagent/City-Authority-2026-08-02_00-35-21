using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Data;
using CityAuthority.Development;
using CityAuthority.Emergency;
using CityAuthority.Media;
using NUnit.Framework;
using UnityEditor;

namespace CityAuthority.Tests.Editor
{
    public class NewspaperCoverageTests
    {
        private const string SliceConfigAssetPath = "Assets/Data/Slice/SliceConfig_Default.asset";

        private static SliceConfig LoadSlice() => AssetDatabase.LoadAssetAtPath<SliceConfig>(SliceConfigAssetPath);

        [Test]
        public void SliceConfig_HasNewspaperOutlet()
        {
            var slice = LoadSlice();
            Assert.IsNotNull(slice.Newspaper, "08 §8: the slice needs its one newspaper outlet");
            Assert.IsFalse(string.IsNullOrWhiteSpace(slice.Newspaper.DisplayName));
        }

        [Test]
        public void GenerateEmergencyResponseArticle_DispatchedBranch_MentionsCoverageAndSecondaryDistrict()
        {
            var slice = LoadSlice();
            var incident = slice.EmergencyScenario.Incident;
            var fireState = new DepartmentCoverageState(incident.RespondingDepartment);
            var log = new CityLog();
            var runtime = new EmergencyIncidentRuntime(incident, fireState, slice.EmergencyScenario.Districts, slice.CitywideTravelTimeBands, log);
            runtime.RaiseWarning();
            runtime.EscalateToCritical();
            var dispatch = runtime.RecordActAndDispatch(NotificationLevel.Critical);

            var article = NewsArticleGenerator.GenerateEmergencyResponseArticle(incident, dispatch);

            Assert.AreEqual("EmergencyResponse", article.SourceEventType);
            Assert.IsFalse(string.IsNullOrWhiteSpace(article.Headline));
            StringAssert.Contains(dispatch.TargetDistrictCoverage.ToString(), article.Body);
            Assert.AreEqual(incident.TargetDistrict, article.RelatedDistrict);
        }

        [Test]
        public void GenerateEmergencyResponseArticle_NotDispatchedBranch_ReportsNoResponse()
        {
            var slice = LoadSlice();
            var incident = slice.EmergencyScenario.Incident;

            var article = NewsArticleGenerator.GenerateEmergencyResponseArticle(incident, null);

            Assert.AreEqual("EmergencyResponse", article.SourceEventType);
            StringAssert.Contains("no", article.Body.ToLowerInvariant());
        }

        [Test]
        public void GenerateCourtRulingArticle_QuotesTheRulingExplanation()
        {
            var slice = LoadSlice();
            var log = new CityLog();
            var courtRuntime = new CondemnationCaseRuntime(slice.CourtCase, log);
            var ruling = courtRuntime.IssueRuling();

            var article = NewsArticleGenerator.GenerateCourtRulingArticle(slice.CourtCase, ruling);

            Assert.AreEqual("CourtRuling", article.SourceEventType);
            StringAssert.Contains(ruling.Explanation, article.Body);
            StringAssert.Contains(slice.CourtCase.AssignedJudge.JudgeName, article.Body);
        }

        [Test]
        public void GenerateDevelopmentRejectionArticle_MentionsBothDeveloperNames()
        {
            var slice = LoadSlice();
            var listing = slice.DevelopmentListing;

            var article = NewsArticleGenerator.GenerateDevelopmentRejectionArticle(listing, 60f);

            Assert.AreEqual("DevelopmentRejection", article.SourceEventType);
            foreach (var proposal in listing.Proposals)
            {
                StringAssert.Contains(proposal.DeveloperName, article.Body);
            }
        }

        [Test]
        public void NewspaperCoverageRuntime_PublishingTwiceReturnsSameArticle_AndLogsOnce()
        {
            var slice = LoadSlice();
            var log = new CityLog();
            var newspaper = new NewspaperCoverageRuntime(slice.Newspaper, log);

            var first = newspaper.PublishEmergencyResponseStory(slice.EmergencyScenario.Incident, null);
            var second = newspaper.PublishEmergencyResponseStory(slice.EmergencyScenario.Incident, null);

            Assert.AreSame(first, second);

            var publishEvents = 0;
            foreach (var evt in log.Events)
            {
                if (evt.EventType == "NewsArticlePublished") publishEvents++;
            }
            Assert.AreEqual(1, publishEvents);
        }

        [Test]
        public void NewspaperCoverageRuntime_PublishesAllThreeStoryKindsIndependently()
        {
            var slice = LoadSlice();
            var log = new CityLog();
            var newspaper = new NewspaperCoverageRuntime(slice.Newspaper, log);

            var courtRuntime = new CondemnationCaseRuntime(slice.CourtCase, log);
            var ruling = courtRuntime.IssueRuling();

            newspaper.PublishEmergencyResponseStory(slice.EmergencyScenario.Incident, null);
            newspaper.PublishCourtRulingStory(slice.CourtCase, ruling);
            newspaper.PublishDevelopmentRejectionStory(slice.DevelopmentListing, 60f);

            Assert.AreEqual(3, newspaper.PublishedArticles.Count);
        }
    }
}
