// ref: https://pratikpokhrel51.medium.com/creating-data-seeder-in-ef-core-that-reads-from-json-file-in-dot-net-core-69004df7ad0a

using Microsoft.CodeAnalysis;
using STRService.Models;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using STRIndex;
using static STRIndex.Domain;
using System.Reflection;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using FsSpreadsheet.Net;
using NuGet.Protocol;
using ARCtrl.Json;
using static ARCtrl.Json.TemplateExtension;

namespace STRService.Data
    
{
    public class DataInitializer
    {
        public static STRIndex.Domain.OntologyAnnotation MapOntologyAnnotation (ARCtrl.OntologyAnnotation tag)
        {
            var name = "";
            var tan = "";
            var tsr = "";

            if (tag.Name is not null)
            {
                name = tag.Name.Value;
            }

            if (tag.TermAccessionNumber is not null)
            {
                tan = tag.TermAccessionNumber.Value;
            }

            if (tag.TermSourceREF is not null)
            {
                tsr = tag.TermSourceREF.Value;
            }

            return new STRIndex.Domain.OntologyAnnotation
            {
                Name = name,
                TermAccessionNumber = tan,
                TermSourceREF = tsr
            };
        }

        public static STRIndex.Domain.Author MapAuthor(ARCtrl.Person p)
        {
            var fullName = "";
            var aff = "";
            var email = "";

            if (p.FirstName is not null)
            {
                fullName = fullName + p.FirstName.Value;
            }

            if (p.MidInitials is not null)
            {
                fullName = fullName + " " + p.MidInitials.Value;
            }

            if (p.LastName is not null)
            {
                fullName = fullName + " " + p.LastName.Value;
            }

            if (p.EMail is not null)
            {
                email = p.EMail.Value;
            }

            if (p.Affiliation is not null)
            {
                aff = p.Affiliation.Value;
            }

            return new STRIndex.Domain.Author
            {
                FullName = fullName,
                Affiliation = aff,
                Email = email
            };
        }

        public static void SeedData(SwateTemplateDb context)
        {
            if (!context.Templates.Any() && !context.Metadata.Any())
            {
                var templatesPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                var templates = STRRepo.getStagedTemplates(templatesPath);
                
                var db_templates =
                    templates
                        .Select((t) =>
                            {
                                var semVer = SemVer.tryParse(t.Version);

                                if (semVer == null)
                                {
                                    throw new Exception($"Invalid SemVer version: {t.Version}");
                                }

                                return new SwateTemplateMetadata
                                {
                                    Id = t.Id,
                                    Name = t.Name,
                                    Description = t.Description,
                                    MajorVersion = semVer.Value.Major,
                                    MinorVersion = semVer.Value.Minor,
                                    PatchVersion = semVer.Value.Patch,
                                    PreReleaseVersionSuffix = semVer.Value.PreRelease,
                                    BuildMetadataVersionSuffix = semVer.Value.BuildMetadata,
                                    Organisation = t.Organisation.ToString(),
                                    EndpointRepositories = t.EndpointRepositories.Select(MapOntologyAnnotation).ToList(),
                                    ReleaseDate = new(t.LastUpdated.Year, t.LastUpdated.Month, t.LastUpdated.Day),
                                    Tags = t.Tags.Select(MapOntologyAnnotation).ToList(),
                                    Authors = t.Authors.Select(MapAuthor).ToList()
                                };
                            });

                context.AddRange(db_templates);

                var template_contents =
                    templates
                        .Select((t) =>
                        {
                            var semVer = SemVer.tryParse(t.Version);

                            if (semVer == null)
                            {
                                throw new Exception($"Invalid SemVer version: {t.Version}");
                            }

                            return new SwateTemplate
                            {
                                TemplateId = t.Id,
                                TemplateName = t.Name,
                                TemplateMajorVersion = semVer.Value.Major,
                                TemplateMinorVersion = semVer.Value.Minor,
                                TemplatePatchVersion = semVer.Value.Patch,
                                TemplatePreReleaseVersionSuffix = semVer.Value.PreRelease,
                                TemplateBuildMetadataVersionSuffix = semVer.Value.BuildMetadata,
                                TemplateContent = Wrapper.templateToJson(t)
                            };
                        });

                context.AddRange(template_contents);

                var downloads =
                     templates
                        .Select((t) =>
                        {
                            var semVer = SemVer.tryParse(t.Version);

                            if (semVer == null)
                            {
                                throw new Exception($"Invalid SemVer version: {t.Version}");
                            }

                            return new Downloads
                            {
                                TemplateId = t.Id,
                                TemplateName = t.Name,
                                TemplateMajorVersion = semVer.Value.Major,
                                TemplateMinorVersion = semVer.Value.Minor,
                                TemplatePatchVersion = semVer.Value.Patch,
                                TemplatePreReleaseVersionSuffix = semVer.Value.PreRelease,
                                TemplateBuildMetadataVersionSuffix = semVer.Value.BuildMetadata,
                                DownloadCount = 0
                            };
                        });

                context.AddRange(downloads);

                context.SaveChanges();
            }
        }
    }
}
