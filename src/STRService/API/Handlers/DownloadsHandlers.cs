using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using STRService.Models;
using static STRIndex.Domain;

namespace STRService.API.Handlers
{
    public class DownloadsHandlers
    {
        // get all download stats
        public static async Task<Ok<Downloads[]>> GetAllDownloads(SwateTemplateDb database)
        {
            var downloads = await database.Downloads.ToArrayAsync();
            return TypedResults.Ok(downloads);
        }

        public static async Task<Results<Ok<Downloads[]>, NotFound<string>>> GetAllDownloadsByName(string name, SwateTemplateDb database)
        {
            var downloads =
                await database.Downloads
                    .Where(p => p.TemplateName == name)
                    .ToArrayAsync();

            return downloads is null || downloads.Length == 0
                ? TypedResults.NotFound($"No download stats for template '{name}' available.")
                : TypedResults.Ok(downloads);
        }

        public static async Task<Results<BadRequest<string>, NotFound<string>, Ok<Downloads>>> GetDownloadsByNameAndVersion(string name, string version, SwateTemplateDb database)
        {
            var semVerOpt = SemVer.tryParse(version);
            if (semVerOpt is null)
            {
                return TypedResults.BadRequest($"{version} is not a valid semantic version.");
            }
            var semVer = semVerOpt.Value;

            var downloads = await database.Downloads.FindAsync(name, semVer.Major, semVer.Minor, semVer.Patch, semVer.PreRelease, semVer.BuildMetadata);

            return downloads is null
                ? TypedResults.NotFound($"No download stats for template '{name}' version '{version}' available.")
                : TypedResults.Ok(downloads);
        }
    }
}
