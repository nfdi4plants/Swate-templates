using Microsoft.AspNetCore.Http.HttpResults;
using STRService.Models;
using Microsoft.EntityFrameworkCore;
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
            return TypedResults.Ok(metadata);
        }

        public static async Task<Results<Ok<SwateTemplateMetadata>, NotFound<string>, Conflict<string>>> GetLatestTemplateMetadataById(Guid id, SwateTemplateDb database)
        {
            var metadata = await database.Metadata
                .Where(p => p.Id == id && p.BuildMetadataVersionSuffix == "" && p.BuildMetadataVersionSuffix == "") // only serve stable template versions here
                .OrderByDescending(p => p.MajorVersion)
                .ThenByDescending(p => p.MinorVersion)
                .ThenByDescending(p => p.PatchVersion)
                .FirstOrDefaultAsync();

            if (metadata is null)
            {
                return TypedResults.NotFound($"No template '{id}' available.");
            }

            return TypedResults.Ok(metadata);
        }

        public static async Task<Results<BadRequest<string>, NotFound<string>, Conflict<string>, Ok<SwateTemplateMetadata>>> GetTemplateMetadataByIdAndVersion(Guid id, string version, SwateTemplateDb database)
        {
            var semVerOpt = SemVer.tryParse(version);
            if (semVerOpt is null)
            {
                return TypedResults.BadRequest($"{version} is not a valid semantic version.");
            }
            var semVer = semVerOpt.Value;

            var metadata = await database.Metadata.FindAsync(id, semVer.Major, semVer.Minor, semVer.Patch, semVer.PreRelease, semVer.BuildMetadata);

            if (metadata is null)
            {
                return TypedResults.NotFound($"No template '{id}' @ {version} available.");
            }

            return TypedResults.Ok(metadata);
        }
    }
}
