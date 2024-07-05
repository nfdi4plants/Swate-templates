using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using STRService.Models;
using STRService.Pages.Components;
using System.Text;
using System.Xml.Linq;
using static STRIndex.Domain;

namespace STRService.Pages.Handlers
{
    public static class TemplateHandlers
    {
        public static async Task<Results<ContentHttpResult, NotFound, BadRequest<string>>> Render(string templateName, string version, SwateTemplateDb database)
        {
            var semVerOpt = SemVer.tryParse(version);
            if (semVerOpt is null)
            {
                return TypedResults.BadRequest($"{version} is not a valid semantic version.");
            }
            var semVer = semVerOpt.Value;

            var metadata = await database.Metadata.FindAsync(templateName, semVer.Major, semVer.Minor, semVer.Patch, semVer.PreRelease, semVer.BuildMetadata);
            var template = await database.Templates.FindAsync(templateName, semVer.Major, semVer.Minor, semVer.Patch, semVer.PreRelease, semVer.BuildMetadata);
            var downloads = await database.Downloads.FindAsync(templateName, semVer.Major, semVer.Minor, semVer.Patch, semVer.PreRelease, semVer.BuildMetadata);

            if (template == null)
            {
                return TypedResults.NotFound();
            }

            var allMetadata = await 
                database.Metadata
                .Where(p => p.Name == metadata.Name)
                .ToArrayAsync();

            var page = Layout.Render(
                activeNavbarItem: "",
                title: $"Template {templateName} - ARC Swate template registry API",
                content: Components.Template.Render(
                    templateName: templateName,
                    templateVersion: metadata.GetSemanticVersionString(),
                    templateContent: template.TemplateContent.ToJson(),
                    templateReleaseDate: metadata.ReleaseDate,
                    templateTags: (metadata.Tags ?? []).Select(t => t.Name).ToArray(),
                    templateDescription: metadata.Description,
                    templateReleaseNotes: metadata.ReleaseNotes ?? "",
                    templateAuthors: (metadata.Authors ?? []).ToArray(),
                    versionTable: Components.TemplateAvailableVersion.RenderVersionTable(allMetadata),
                    downloads: downloads?.DownloadCount ?? 0
                )
            );

            return TypedResults.Text(content: page, contentType: "text/html");
        }
        public static async Task<Results<ContentHttpResult, NotFound>> RenderLatest(string templateName, SwateTemplateDb database)
        {

            var metadata = await
                database.Metadata
                    .Where(p => p.Name == templateName)
                    .ToArrayAsync();

            var latestMetadata =
                metadata
                .Where(p => p.BuildMetadataVersionSuffix == "" && p.BuildMetadataVersionSuffix == "")
                .OrderByDescending(p => p.MajorVersion)
                .ThenByDescending(p => p.MinorVersion)
                .ThenByDescending(p => p.PatchVersion)
                .FirstOrDefault();

            var latestTemplate =
                database.Templates
                .Where(p => p.TemplateName == latestMetadata.Name && p.TemplateMajorVersion == latestMetadata.MajorVersion && p.TemplateMinorVersion == latestMetadata.MinorVersion && p.TemplatePatchVersion == latestMetadata.PatchVersion && p.TemplatePreReleaseVersionSuffix == latestMetadata.PreReleaseVersionSuffix && p.TemplateBuildMetadataVersionSuffix == latestMetadata.BuildMetadataVersionSuffix)
                .FirstOrDefault();

            if (latestMetadata == null)
            {
                return TypedResults.NotFound();
            }

            var downloads = await database.Downloads.FindAsync(latestMetadata.Name, latestMetadata.MajorVersion, latestMetadata.MinorVersion, latestMetadata.PatchVersion, latestMetadata.PreReleaseVersionSuffix, latestMetadata.BuildMetadataVersionSuffix);

            var page = Layout.Render(
                activeNavbarItem: "",
                title: $"Template {templateName} - ARC Swate template registry API",
                content: Components.Template.Render(
                    templateName: templateName,
                    templateVersion: latestMetadata.GetSemanticVersionString(),
                    templateContent: latestTemplate.TemplateContent.ToJson(),
                    templateReleaseDate: latestMetadata.ReleaseDate,
                    templateTags: (latestMetadata.Tags ?? []).Select(t => t.Name).ToArray(),
                    templateDescription: latestMetadata.Description,
                    templateReleaseNotes: latestMetadata.ReleaseNotes ?? "",
                    templateAuthors: (latestMetadata.Authors ?? []).ToArray(),
                    versionTable: Components.TemplateAvailableVersion.RenderVersionTable(metadata),
                    downloads: downloads?.DownloadCount ?? 0
                )
            );

            return TypedResults.Text(content: page, contentType: "text/html");
        }
    }
}
