using System.Collections.Generic;
using CityAuthority.Accountability;
using CityAuthority.Court;
using CityAuthority.Data;
using CityAuthority.Emergency;

namespace CityAuthority.Media
{
    // 08 §8: the slice's one newspaper outlet. Each story kind publishes once —
    // republishing returns the already-published article rather than
    // regenerating it, matching the same "computed once, then stable" pattern
    // as the court ruling.
    public sealed class NewspaperCoverageRuntime
    {
        private readonly NewspaperOutlet outlet;
        private readonly IAccountabilityRecorder recorder;
        private readonly List<NewsArticle> published = new();

        public NewspaperCoverageRuntime(NewspaperOutlet outlet, IAccountabilityRecorder recorder)
        {
            this.outlet = outlet;
            this.recorder = recorder;
        }

        public IReadOnlyList<NewsArticle> PublishedArticles => published;

        public NewsArticle PublishEmergencyResponseStory(EmergencyIncidentDefinition incident, DispatchResult dispatchResult)
        {
            var existing = FindBySourceType("EmergencyResponse");
            if (existing != null)
            {
                return existing;
            }

            var article = NewsArticleGenerator.GenerateEmergencyResponseArticle(incident, dispatchResult);
            Publish(article);
            return article;
        }

        public NewsArticle PublishCourtRulingStory(CondemnationCaseDefinition courtCase, CourtRulingRecord ruling)
        {
            var existing = FindBySourceType("CourtRuling");
            if (existing != null)
            {
                return existing;
            }

            var article = NewsArticleGenerator.GenerateCourtRulingArticle(courtCase, ruling);
            Publish(article);
            return article;
        }

        // 08 §9: only publish this "if material" — typically only called when
        // both proposals were actually rejected.
        public NewsArticle PublishDevelopmentRejectionStory(DevelopmentListingDefinition listing, float interestAfterRejection)
        {
            var existing = FindBySourceType("DevelopmentRejection");
            if (existing != null)
            {
                return existing;
            }

            var article = NewsArticleGenerator.GenerateDevelopmentRejectionArticle(listing, interestAfterRejection);
            Publish(article);
            return article;
        }

        // Save/load (08 §13 item 8): restores previously published articles
        // verbatim rather than regenerating them from templates, so headline/
        // body text can't drift between a save and a reload.
        public static NewspaperCoverageRuntime Restore(
            NewspaperOutlet outlet,
            IAccountabilityRecorder recorder,
            IEnumerable<NewsArticle> existingArticles)
        {
            var runtime = new NewspaperCoverageRuntime(outlet, recorder);
            runtime.published.AddRange(existingArticles);
            return runtime;
        }

        private void Publish(NewsArticle article)
        {
            published.Add(article);
            recorder.Record(new AccountabilityEvent(
                "NewsArticlePublished",
                $"{outlet.DisplayName}: \"{article.Headline}\"",
                article.RelatedDistrict));
        }

        private NewsArticle FindBySourceType(string sourceEventType)
        {
            foreach (var article in published)
            {
                if (article.SourceEventType == sourceEventType)
                {
                    return article;
                }
            }
            return null;
        }
    }
}
