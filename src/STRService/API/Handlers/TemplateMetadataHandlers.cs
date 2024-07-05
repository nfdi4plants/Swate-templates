using Microsoft.AspNetCore.Http.HttpResults;
using STRService.Models;
using Microsoft.EntityFrameworkCore;
using STRService.Pages.Components;
using STRIndex;
using static STRIndex.Domain;

namespace STRService.API.Handlers
{
    public class TemplateMetadataHandlers
    {
        // get all Swate templates
        public static async Task<Results<Ok<SwateTemplateMetadata[]>, Conflict<string>>> GetAllTemplateMetadata(SwateTemplateDb database)
        {
            var metadata = await database.Metadata.ToArrayAsync();

            Array.ForEach(metadata, (p => SwateTemplateDb.IncrementDownloadCount(p, database)));
            await database.SaveChangesAsync();

            return TypedResults.Ok(metadata);
        }

        public static async Task<Results<Ok<SwateTemplateMetadata>, NotFound<string>, Conflict<string>>> GetLatestTemplateMetadataByName(string name, SwateTemplateDb database)
        {
            var metadata = await database.Metadata
                .Where(p => p.Name == name && p.BuildMetadataVersionSuffix == "" && p.BuildMetadataVersionSuffix == "") // only serve stable template versions here
                .OrderByDescending(p => p.MajorVersion)
                .ThenByDescending(p => p.MinorVersion)
                .ThenByDescending(p => p.PatchVersion)
                .FirstOrDefaultAsync();

            if (metadata is null)
            {
                return TypedResults.NotFound($"No template '{name}' available.");
            }

            SwateTemplateDb.IncrementDownloadCount(metadata, database);
            await database.SaveChangesAsync();

            return TypedResults.Ok(metadata);
        }

        public static async Task<Results<BadRequest<string>, NotFound<string>, Conflict<string>, Ok<SwateTemplateMetadata>>> GetTemplateMetadataByNameAndVersion(string name, string version, SwateTemplateDb database)
        {
            var semVerOpt = SemVer.tryParse(version);
            if (semVerOpt is null)
            {
                return TypedResults.BadRequest($"{version} is not a valid semantic version.");
            }
            var semVer = semVerOpt.Value;

            var metadata = await database.Metadata.FindAsync(name, semVer.Major, semVer.Minor, semVer.Patch, semVer.PreRelease, semVer.BuildMetadata);

            if (metadata is null)
            {
                return TypedResults.NotFound($"No template '{name}' @ {version} available.");
            }

            SwateTemplateDb.IncrementDownloadCount(metadata, database);
            await database.SaveChangesAsync();

            return TypedResults.Ok(metadata);
        }

        public static async Task<Results<Ok<SwateTemplateMetadata>, Conflict, UnauthorizedHttpResult, UnprocessableEntity<string>>> CreateTemplateMetadata(SwateTemplateMetadata metadata, SwateTemplateDb database)
        {
            var existing = await database.Metadata.FindAsync(metadata.Name, metadata.MajorVersion, metadata.MinorVersion, metadata.PatchVersion, metadata.PreReleaseVersionSuffix, metadata.BuildMetadataVersionSuffix);
            if (existing != null)
            {
                return TypedResults.Conflict();
            }

            database.Metadata.Add(metadata);
            await database.SaveChangesAsync();

            return TypedResults.Ok(metadata);

        }
    }
}
