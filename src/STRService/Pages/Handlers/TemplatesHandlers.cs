using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;
using STRService.Models;
using STRService.Pages.Components;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace STRService.Pages.Handlers
{
    public static class TemplatesHandlers
    {
        public static async Task<ContentHttpResult> Render(SwateTemplateDb database)
        {
            var metadata = await database.Metadata.ToArrayAsync();
            var templates = await database.Templates.ToArrayAsync();

            var latestTemplates =
                metadata
                .OrderByDescending(p => p.MajorVersion)
                .ThenByDescending(p => p.MinorVersion)
                .ThenByDescending(p => p.PatchVersion)
                .FirstOrDefault();

            var templateSummaries =
                metadata
                    .GroupBy(p => p.Name)
                    .ToList()
                    .Select(group =>
                        {
                            var downloads =
                                database.Downloads
                                .Where(p => p.TemplateName == group.Key)
                                .Sum(d => d.DownloadCount);

                            var latestTemplate =
                                group
                                    .Where(p => p.BuildMetadataVersionSuffix == "" && p.BuildMetadataVersionSuffix == "")
                                    .OrderByDescending(p => p.MajorVersion)
                                    .ThenByDescending(p => p.MinorVersion)
                                    .ThenByDescending(p => p.PatchVersion)
                                    .FirstOrDefault();

                            return
                            new TemplateSummary(
                                Name: group.Key,
                                Tags: (latestTemplate.Tags ?? []).Select(t => t.Name).ToArray(),
                                ReleaseDate: latestTemplate.ReleaseDate,
                                LatestVersion: latestTemplate.GetSemanticVersionString(),
                                TotalDownloads: downloads
                            );
                        }
                    );

            var content = Layout.Render(
                activeNavbarItem: "Browse Templates",
                title: "ARC Swate template registry API",
                content: TemplateSummary.RenderList(templateSummaries)
            );

            return TypedResults.Text(content: content, contentType: "text/html");
        }
    }
}
