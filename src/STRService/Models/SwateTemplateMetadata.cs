using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Security.Cryptography;
using STRIndex;
using static STRIndex.Domain;

namespace STRService.Models
{
    /// <summary>
    /// 
    /// </summary>
    [PrimaryKey(nameof(Id), nameof(MajorVersion), nameof(MinorVersion), nameof(PatchVersion), nameof(PreReleaseVersionSuffix), nameof(BuildMetadataVersionSuffix))]
    public class SwateTemplateMetadata
    {
        public static SwateTemplateMetadata Create(
            Guid id,
            string name, 
            string description,
            int majorVersion, 
            int minorVersion,
            int patchVersion, 
            string preReleaseVersionSuffix,
            string buildMetadataVersionSuffix,
            string organisation, 
            ICollection<OntologyAnnotation> endpointRepositories,
            DateOnly releaseDate, 
            ICollection<OntologyAnnotation> tags,
            ICollection<Author> authors) =>
                new SwateTemplateMetadata
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    MajorVersion = majorVersion,
                    MinorVersion = minorVersion,
                    PatchVersion = patchVersion,
                    PreReleaseVersionSuffix = preReleaseVersionSuffix,
                    BuildMetadataVersionSuffix = buildMetadataVersionSuffix,
                    Organisation = organisation,
                    EndpointRepositories = endpointRepositories,
                    ReleaseDate = releaseDate,
                    Tags = tags,
                    Authors = authors
                };

        /// <summary>
        /// The unique identifier of the Swate template.
        /// </summary>
        /// <example>XXXXXXXX-XXXX-XXXX-XXX-XXXXXXXXXXXX</example>
        public required Guid Id { get; set; }

        /// <summary>
        /// The name of the Swate template.
        /// </summary>
        /// <example>MyTemplate</example>
        public required string Name { get; set; }

        ///// <summary>
        ///// Single sentence Swate template description.
        ///// </summary>
        ///// <example>MyTemplate does the thing</example>
        //public required string Summary { get; set; }

        /// <summary>
        /// Free text Swate template description.
        /// </summary>
        /// <example>
        /// MyTemplate does the thing.
        /// It does it very good, it does it very well.
        /// It does it very fast, it does it very swell.
        /// </example>
        public required string Description { get; set; }

        /// <summary>
        /// SemVer major version of the Swate template.
        /// </summary>
        /// <example>1</example>
        public required int MajorVersion { get; set; }

        /// <summary>
        /// SemVer minor version of the Swate template.
        /// </summary>
        /// <example>0</example>
        public required int MinorVersion { get; set; }

        /// <summary>
        /// SemVer patch version of the Swate template.
        /// </summary>
        /// <example>0</example>
        public required int PatchVersion { get; set; }


        /// <summary>
        /// SemVer prerelease version of the Swate template.
        /// </summary>
        /// <example>alpha.1</example>
        public string PreReleaseVersionSuffix { get; set; } = "";

        /// <summary>
        /// </summary>
        /// <example></example>
        public required string Organisation { get; set; } = "";

        /// <summary>
        /// SemVer buildmetadata of the Swate template.
        /// </summary>
        /// <example>0</example>
        public string BuildMetadataVersionSuffix { get; set; } = "";

        /// <summary>
        ///
        /// </summary>
        public required DateOnly ReleaseDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<STRIndex.Domain.OntologyAnnotation> Tags { get; set; } = Array.Empty<STRIndex.Domain.OntologyAnnotation>().ToList();

        /// <summary>
        /// 
        /// </summary>
        public ICollection<STRIndex.Domain.OntologyAnnotation> EndpointRepositories { get; set; } = Array.Empty<STRIndex.Domain.OntologyAnnotation>().ToList();

        /// <summary>
        /// 
        /// </summary>
        public string ReleaseNotes { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public ICollection<STRIndex.Domain.Author> Authors { get; set; } = Array.Empty<STRIndex.Domain.Author>().ToList();// https://www.learnentityframeworkcore.com/relationships#navigation-properties

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A string containing the semantic version of the Swate template</returns>
        public string GetSemanticVersionString()
        {
            SemVer semVer = new SemVer {
                Major = MajorVersion,
                Minor = MinorVersion,
                Patch = PatchVersion,
                PreRelease = PreReleaseVersionSuffix,
                BuildMetadata = BuildMetadataVersionSuffix
            };
            return SemVer.toString(semVer);
        }
    }
}
