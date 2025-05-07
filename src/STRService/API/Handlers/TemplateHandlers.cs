using Microsoft.AspNetCore.Http.HttpResults;
using STRService.Models;
using Microsoft.EntityFrameworkCore;
using STRIndex;
using ARCtrl;
using static STRIndex.Domain;

namespace STRService.API.Handlers
{
    public class TemplateHandlers
    {
        // get all Swate templates
        public static async Task<Results<Ok<SwateTemplate[]>, Conflict<string>>> GetAllTemplates(SwateTemplateDb database)
        {
            var templates = await database.Templates.ToArrayAsync();

            Array.ForEach(templates, (p => SwateTemplateDb.IncrementDownloadCount(p, database)));
            await database.SaveChangesAsync();

            return TypedResults.Ok(templates);
        }

        public static async Task<Results<Ok<SwateTemplate>, NotFound<string>, Conflict<string>>> GetLatestTemplateById(Guid id, SwateTemplateDb database)
        {
            var template = await database.Templates
                .Where(p => p.TemplateId == id && p.TemplateBuildMetadataVersionSuffix == "" && p.TemplateBuildMetadataVersionSuffix == "") // only serve stable template versions here
                .OrderByDescending(p => p.TemplateMajorVersion)
                .ThenByDescending(p => p.TemplateMinorVersion)
                .ThenByDescending(p => p.TemplatePatchVersion)
                .FirstOrDefaultAsync();

            if (template is null)
            {
                return TypedResults.NotFound($"No template '{id}' available.");
            }

            SwateTemplateDb.IncrementDownloadCount(template, database);
            await database.SaveChangesAsync();

            return TypedResults.Ok(template);
        }

        public static async Task<Results<BadRequest<string>, NotFound<string>, Conflict<string>, Ok<SwateTemplate>>> GetTemplateByIdAndVersion(Guid id, string version, SwateTemplateDb database)
        {
            var semVerOpt = SemVer.tryParse(version);
            if (semVerOpt is null)
            {
                return TypedResults.BadRequest($"{version} is not a valid semantic version.");
            }
            var semVer = semVerOpt.Value;

            var template = await database.Templates.FindAsync(id, semVer.Major, semVer.Minor, semVer.Patch, semVer.PreRelease, semVer.BuildMetadata);

            if (template is null)
            {
                return TypedResults.NotFound($"No template '{id}' @ {version} available.");
            }

            SwateTemplateDb.IncrementDownloadCount(template, database);
            await database.SaveChangesAsync();

            return TypedResults.Ok(template);
        }

        public static async Task<Results<Ok<SwateTemplate>, Conflict, UnauthorizedHttpResult, UnprocessableEntity<string>>> CreateTemplate(SwateTemplateDto swateTemplateDto, SwateTemplateDb database)
        {
            var metadata = swateTemplateDto.Metadata;
            var templateContent = swateTemplateDto.Content.TemplateContent;

            var existingTemplate = await database.Templates.FindAsync(metadata.Id, metadata.MajorVersion, metadata.MinorVersion, metadata.PatchVersion, metadata.PreReleaseVersionSuffix, metadata.BuildMetadataVersionSuffix);
            var existingMetadata = await database.Templates.FindAsync(metadata.Id, metadata.MajorVersion, metadata.MinorVersion, metadata.PatchVersion, metadata.PreReleaseVersionSuffix, metadata.BuildMetadataVersionSuffix);
            if (existingTemplate != null || existingMetadata != null)
            {
                return TypedResults.Conflict();
            }

            database.Metadata.Add(metadata);

            var template = new SwateTemplate
            {
                TemplateId = metadata.Id,
                TemplateName = metadata.Name,
                TemplateMajorVersion = metadata.MajorVersion,
                TemplateMinorVersion = metadata.MinorVersion,
                TemplatePatchVersion = metadata.PatchVersion,
                TemplatePreReleaseVersionSuffix = metadata.PreReleaseVersionSuffix,
                TemplateBuildMetadataVersionSuffix = metadata.BuildMetadataVersionSuffix,
                TemplateContent = templateContent
            };

            database.Templates.Add(template);

            await database.SaveChangesAsync();

            return TypedResults.Ok(template);
        }
    }
}
