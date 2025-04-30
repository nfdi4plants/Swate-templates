using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STRService.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Templates",
                table: "Templates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Metadata",
                table: "Metadata");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Downloads",
                table: "Downloads");

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateId",
                table: "Templates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Metadata",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateId",
                table: "Downloads",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Templates",
                table: "Templates",
                columns: new[] { "TemplateId", "TemplateMajorVersion", "TemplateMinorVersion", "TemplatePatchVersion", "TemplatePreReleaseVersionSuffix", "TemplateBuildMetadataVersionSuffix" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Metadata",
                table: "Metadata",
                columns: new[] { "Id", "MajorVersion", "MinorVersion", "PatchVersion", "PreReleaseVersionSuffix", "BuildMetadataVersionSuffix" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Downloads",
                table: "Downloads",
                columns: new[] { "TemplateId", "TemplateMajorVersion", "TemplateMinorVersion", "TemplatePatchVersion", "TemplatePreReleaseVersionSuffix", "TemplateBuildMetadataVersionSuffix" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Templates",
                table: "Templates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Metadata",
                table: "Metadata");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Downloads",
                table: "Downloads");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Metadata");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "Downloads");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Templates",
                table: "Templates",
                columns: new[] { "TemplateName", "TemplateMajorVersion", "TemplateMinorVersion", "TemplatePatchVersion", "TemplatePreReleaseVersionSuffix", "TemplateBuildMetadataVersionSuffix" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Metadata",
                table: "Metadata",
                columns: new[] { "Name", "MajorVersion", "MinorVersion", "PatchVersion", "PreReleaseVersionSuffix", "BuildMetadataVersionSuffix" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Downloads",
                table: "Downloads",
                columns: new[] { "TemplateName", "TemplateMajorVersion", "TemplateMinorVersion", "TemplatePatchVersion", "TemplatePreReleaseVersionSuffix", "TemplateBuildMetadataVersionSuffix" });
        }
    }
}
