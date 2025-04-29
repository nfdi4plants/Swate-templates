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

        public static void IncrementDownloadCount(SwateTemplate template, SwateTemplateDb database)
        {
            var result = database.Downloads.SingleOrDefault(d => d.TemplateId == template.TemplateId && d.TemplateMajorVersion == template.TemplateMajorVersion && d.TemplateMinorVersion == template.TemplateMinorVersion && d.TemplatePatchVersion == template.TemplatePatchVersion && d.TemplatePreReleaseVersionSuffix == template.TemplatePreReleaseVersionSuffix && d.TemplateBuildMetadataVersionSuffix == template.TemplateBuildMetadataVersionSuffix);

            if (result != null)
            {
                result.DownloadCount += 1; // increment download count for each template
            }
            else
            {
                var d = new Downloads
                {
                    TemplateId = template.TemplateId,
                    TemplateName = template.TemplateName,
                    TemplateMajorVersion = template.TemplateMajorVersion,
                    TemplateMinorVersion = template.TemplateMinorVersion,
                    TemplatePatchVersion = template.TemplatePatchVersion,
                    TemplatePreReleaseVersionSuffix = template.TemplatePreReleaseVersionSuffix,
                    TemplateBuildMetadataVersionSuffix = template.TemplateBuildMetadataVersionSuffix,
                    DownloadCount = 1
                };
                database.Downloads.Add(d);
            }
        }
    }
}
