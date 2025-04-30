namespace STRService.Components.Models
{
    public class TemplateSummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public string LatestVersion { get; set; }
        public int TotalDownloads { get; set; }
    }
}