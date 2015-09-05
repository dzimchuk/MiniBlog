namespace MiniBlog.Contracts.Model.Search
{
    public class SearchResult
    {
        public double Score { get; set; }
        public HitHighlights Highlights { get; set; }
        public Document Document { get; set; }
    }
}