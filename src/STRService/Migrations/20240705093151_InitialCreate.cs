using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STRService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Downloads",
                columns: table => new
                {
                    TemplateName = table.Column<string>(type: "text", nullable: false),
                    TemplateMajorVersion = table.Column<int>(type: "integer", nullable: false),
                    TemplateMinorVersion = table.Column<int>(type: "integer", nullable: false),
                    TemplatePatchVersion = table.Column<int>(type: "integer", nullable: false),
                    TemplatePreReleaseVersionSuffix = table.Column<string>(type: "text", nullable: false),
                    TemplateBuildMetadataVersionSuffix = table.Column<string>(type: "text", nullable: false),
                    DownloadCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Downloads", x => new { x.TemplateName, x.TemplateMajorVersion, x.TemplateMinorVersion, x.TemplatePatchVersion, x.TemplatePreReleaseVersionSuffix, x.TemplateBuildMetadataVersionSuffix });
                });

            migrationBuilder.CreateTable(
                name: "Metadata",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    MajorVersion = table.Column<int>(type: "integer", nullable: false),
                    MinorVersion = table.Column<int>(type: "integer", nullable: false),
                    PatchVersion = table.Column<int>(type: "integer", nullable: false),
                    PreReleaseVersionSuffix = table.Column<string>(type: "text", nullable: false),
                    BuildMetadataVersionSuffix = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Organisation = table.Column<string>(type: "text", nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReleaseNotes = table.Column<string>(type: "text", nullable: false),
                    Authors = table.Column<string>(type: "jsonb", nullable: true),
                    EndpointRepositories = table.Column<string>(type: "jsonb", nullable: true),
                    Tags = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadata", x => new { x.Name, x.MajorVersion, x.MinorVersion, x.PatchVersion, x.PreReleaseVersionSuffix, x.BuildMetadataVersionSuffix });
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    TemplateName = table.Column<string>(type: "text", nullable: false),
                    TemplateMajorVersion = table.Column<int>(type: "integer", nullable: false),
                    TemplateMinorVersion = table.Column<int>(type: "integer", nullable: false),
                    TemplatePatchVersion = table.Column<int>(type: "integer", nullable: false),
                    TemplatePreReleaseVersionSuffix = table.Column<string>(type: "text", nullable: false),
                    TemplateBuildMetadataVersionSuffix = table.Column<string>(type: "text", nullable: false),
                    TemplateContent = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => new { x.TemplateName, x.TemplateMajorVersion, x.TemplateMinorVersion, x.TemplatePatchVersion, x.TemplatePreReleaseVersionSuffix, x.TemplateBuildMetadataVersionSuffix });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Downloads");

            migrationBuilder.DropTable(
                name: "Metadata");

            migrationBuilder.DropTable(
                name: "Templates");
        }
    }
}
