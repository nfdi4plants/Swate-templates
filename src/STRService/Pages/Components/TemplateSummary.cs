using YamlDotNet.Core.Tokens;

namespace STRService.Pages.Components
{
    public record TemplateSummary(string Name, string [] Tags, string LatestVersion, DateOnly ReleaseDate, int TotalDownloads)
    {
        public static string Render(TemplateSummary summary)
        {
            return $@"<tr>
<th scope=""row""><a href=""/template/{summary.Name}"">{summary.Name}</a></th>
<td><a href=""/template/{summary.Name}/{summary.LatestVersion}"">{summary.LatestVersion}</a></td>
<td>{summary.ReleaseDate}</td>
<td>{string.Join("; ", summary.Tags.Select(TemplateTag.RenderLink))}</td>
<td>{summary.TotalDownloads}</td>
</tr>";
        }

        public static string RenderList(IEnumerable<TemplateSummary> summaries)
        {
            var content = @$"<h1>All available Swate templates</h1><br>
<div class=""overflow-auto"">
<table class=""striped"">
<thead>
<tr>
<th scope=""col"">Name</th>
<th scope=""col"">Summary</th>
<th scope=""col"">Latest stable version</th>
<th scope=""col"">Release date</th>
<th scope=""col"">Tags</th>
<th scope=""col"">Total Downloads</th>
</tr>
</thead>
{string.Join(System.Environment.NewLine, summaries.Select(TemplateSummary.Render))}
</table>
</div>";
            return content;
        }
    }
}
