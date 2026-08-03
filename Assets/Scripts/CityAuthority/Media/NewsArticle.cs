using CityAuthority.Data;

namespace CityAuthority.Media
{
    // 06 §8: the article produced from a structured event. Headline/Body are
    // templated text (11 §3's fallback), not LLM-generated, but the shape
    // matches what a future model call would also return.
    public sealed class NewsArticle
    {
        public string SourceEventType { get; }
        public string Headline { get; }
        public string Body { get; }
        public District RelatedDistrict { get; }

        public NewsArticle(string sourceEventType, string headline, string body, District relatedDistrict)
        {
            SourceEventType = sourceEventType;
            Headline = headline;
            Body = body;
            RelatedDistrict = relatedDistrict;
        }
    }
}
