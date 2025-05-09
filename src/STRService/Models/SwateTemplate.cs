using ARCtrl;
using Microsoft.EntityFrameworkCore;

namespace STRService.Models
{
    [PrimaryKey(nameof(TemplateId), nameof(TemplateMajorVersion), nameof(TemplateMinorVersion), nameof(TemplatePatchVersion), nameof(TemplatePreReleaseVersionSuffix), nameof(TemplateBuildMetadataVersionSuffix))]
    public class SwateTemplate
    {
        public static SwateTemplate Create(
            Guid id,
            string name, 
            int majorVersion, 
            int minorVersion,
            int patchVersion, 
            string preReleaseVersionSuffix,
            string buildMetadataVersionSuffix,
            string content) =>
                new SwateTemplate
                {
                    TemplateId = id,
                    TemplateName = name,
                    TemplateMajorVersion = majorVersion,
                    TemplateMinorVersion = minorVersion,
                    TemplatePatchVersion = patchVersion,
                    TemplatePreReleaseVersionSuffix = preReleaseVersionSuffix,
                    TemplateBuildMetadataVersionSuffix = buildMetadataVersionSuffix,
                    TemplateContent = content
                };

        /// <summary>
        /// The unique identifier of the Swate template.
        /// </summary>
        /// <example>XXXXXXXX-XXXX-XXXX-XXX-XXXXXXXXXXXX</example>
        public required Guid TemplateId { get; set; }
        /// <summary>
        /// The name of the Swate template.
        /// </summary>
        /// <example>MyTemplate</example>
        public required string TemplateName { get; set; }

        /// <summary>
        /// SemVer major version of the Swate template.
        /// </summary>
        /// <example>1</example>
        public required int TemplateMajorVersion { get; set; }

        /// <summary>
        /// SemVer minor version of the Swate template.
        /// </summary>
        /// <example>0</example>
        public required int TemplateMinorVersion { get; set; }

        /// <summary>
        /// SemVer patch version of the SwateTemplate.
        /// </summary>
        /// <example>0</example>

        public required int TemplatePatchVersion { get; set; }

        /// <summary>
        /// SemVer prerelease version of the SwateTemplate.
        /// </summary>
        /// <example>alpha.1</example>
        public required string TemplatePreReleaseVersionSuffix { get; set; }

        /// <summary>
        /// SemVer buildmetadata of the SwateTemplate.
        /// </summary>
        /// <example>0</example>
        public required string TemplateBuildMetadataVersionSuffix { get; set; }

        /// <summary>
        /// Number of downloads for the template.
        /// </summary>
        /// <example>420691337</example>
        public required string TemplateContent { get; set; }
    }
}
