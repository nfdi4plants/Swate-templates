namespace STRService.Models
{

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using System.Reflection.Metadata;

    public class SwateTemplateDb : DbContext
    {
        public SwateTemplateDb(DbContextOptions<SwateTemplateDb> options) : base(options) { }

        public DbSet<SwateTemplate> Templates => Set<SwateTemplate>();
        public DbSet<SwateTemplateMetadata> Metadata => Set<SwateTemplateMetadata>();
        public DbSet<Downloads> Downloads => Set<Downloads>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SwateTemplateMetadata>()
            .OwnsMany(v => v.Authors, a =>
            {
                a.ToJson();
            })
            .OwnsMany(v => v.Tags, t =>
            {
                t.ToJson();
            })
            .OwnsMany(v => v.EndpointRepositories, t =>
            {
                t.ToJson();
            });

            modelBuilder.Entity<SwateTemplate>()
            .Property("TemplateContent")
            .HasColumnType("jsonb");
        }

        public static void IncrementDownloadCount(SwateTemplateMetadata templateMetadata, SwateTemplateDb database)
        {
            var result = database.Downloads.SingleOrDefault(d => d.TemplateName == templateMetadata.Name && d.TemplateMajorVersion == templateMetadata.MajorVersion && d.TemplateMinorVersion == templateMetadata.MinorVersion && d.TemplatePatchVersion == templateMetadata.PatchVersion && d.TemplatePreReleaseVersionSuffix == templateMetadata.PreReleaseVersionSuffix && d.TemplateBuildMetadataVersionSuffix == templateMetadata.BuildMetadataVersionSuffix);

            if (result != null)
            {
                result.DownloadCount += 1; // increment download count for each template
            }
            else
            {
                var d = new Downloads
                {
                    TemplateName = templateMetadata.Name,
                    TemplateMajorVersion = templateMetadata.MajorVersion,
                    TemplateMinorVersion = templateMetadata.MinorVersion,
                    TemplatePatchVersion = templateMetadata.PatchVersion,
                    TemplatePreReleaseVersionSuffix = templateMetadata.PreReleaseVersionSuffix,
                    TemplateBuildMetadataVersionSuffix = templateMetadata.BuildMetadataVersionSuffix,
                    DownloadCount = 1
                };
                database.Downloads.Add(d);
            }
        }
    }
}
