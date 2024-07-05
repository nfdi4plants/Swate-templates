namespace ValidationPackageIndexTests

open System
open System.IO
open Xunit
open AVPRIndex
open AVPRIndex.Domain
open AVPRIndex.Frontmatter
open Utils
open ReferenceObjects

module StaticMethods = 

    [<Fact>]
    let ``create function for mandatory fields``() =
        let actual =
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Comment/valid@1.0.0.fsx",
                fileName = "valid@1.0.0.fsx",
                lastUpdated = testDate,
                contentHash = "2A29D85A29D908C7DE214D56119DE207",
                metadata = ValidationPackageMetadata.create(
                    name = "valid",
                    majorVersion = 1,
                    minorVersion = 0,
                    patchVersion = 0,
                    summary = "My package does the thing.",
                    description = """My package does the thing.
It does it very good, it does it very well.
It does it very fast, it does it very swell.
""".ReplaceLineEndings("\n"))
            )
        Assert.Equivalent(ValidationPackageIndex.CommentFrontmatter.validMandatoryFrontmatter, actual)
        Assert.Equal(ValidationPackageIndex.CommentFrontmatter.validMandatoryFrontmatter, actual)

    [<Fact>]
    let ``create function for all fields``() =
        let actual =
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Comment/valid@2.0.0.fsx",
                fileName = "valid@2.0.0.fsx",
                lastUpdated = testDate,
                contentHash = "E2BE9000A07122842FC805530DDC9FDA",
                metadata = ValidationPackageMetadata.create(
                    name = "valid",
                    majorVersion = 2,
                    minorVersion = 0,
                    patchVersion = 0,
                    summary = "My package does the thing.",
                    description = """My package does the thing.
It does it very good, it does it very well.
It does it very fast, it does it very swell.
""".ReplaceLineEndings("\n"),
                    PreReleaseVersionSuffix = "alpha.1",
                    BuildMetadataVersionSuffix = "build.1",
                    Publish = true,
                    Authors = [|
                        Author(
                            FullName = "John Doe",
                            Email = "j@d.com",
                            Affiliation = "University of Nowhere",
                            AffiliationLink = "https://nowhere.edu"
                        )
                        Author(
                            FullName = "Jane Doe",
                            Email = "jj@d.com",
                            Affiliation = "University of Somewhere",
                            AffiliationLink = "https://somewhere.edu"
                        )
                    |],
                    Tags = [|
                        OntologyAnnotation(Name = "validation")
                        OntologyAnnotation(Name = "my-tag", TermSourceREF = "my-ontology", TermAccessionNumber = "MO:12345")
                    |],
                    ReleaseNotes = """- initial release
  - does the thing
  - does it well
""".ReplaceLineEndings("\n"),
                    CQCHookEndpoint = "https://hook.com"
                )
            )
        Assert.Equivalent(ValidationPackageIndex.CommentFrontmatter.validFullFrontmatter, actual)
        Assert.Equal(ValidationPackageIndex.CommentFrontmatter.validFullFrontmatter, actual)

    [<Fact>]
    let ``tryGetSemanticVersion from valid package with mandatory fields``() =
        let actual = ValidationPackageIndex.CommentFrontmatter.validMandatoryFrontmatter |> ValidationPackageIndex.tryGetSemanticVersion
        Assert.True(actual.IsSome)
        Assert.Equivalent(SemVer.SemVers.mandatory, actual.Value)
        Assert.Equal(SemVer.SemVers.mandatory, actual.Value)

    [<Fact>]
    let ``getSemanticVersion from valid package with mandatory fields``() =
        let actual = ValidationPackageIndex.CommentFrontmatter.validMandatoryFrontmatter |> ValidationPackageIndex.getSemanticVersion
        Assert.Equivalent(SemVer.SemVers.mandatory, actual)
        Assert.Equal(SemVer.SemVers.mandatory, actual)

    [<Fact>]
    let ``tryGetSemanticVersionString from valid package with mandatory fields``() =
        let actual = ValidationPackageIndex.CommentFrontmatter.validMandatoryFrontmatter |> ValidationPackageIndex.tryGetSemanticVersionString
        Assert.True(actual.IsSome)
        Assert.Equal(SemVer.Strings.mandatory, actual.Value)

    [<Fact>]
    let ``getSemanticVersionString from valid package with mandatory fields``() =
        let actual = ValidationPackageIndex.CommentFrontmatter.validMandatoryFrontmatter |> ValidationPackageIndex.getSemanticVersionString
        Assert.Equal(SemVer.Strings.mandatory, actual)

    [<Fact>]
    let ``tryGetSemanticVersion from valid package with all fields``() =
        let actual = ValidationPackageIndex.CommentFrontmatter.validFullFrontmatter |> ValidationPackageIndex.tryGetSemanticVersion
        Assert.True(actual.IsSome)
        Assert.Equivalent(SemVer.SemVers.fixtureFile, actual.Value)
        Assert.Equal(SemVer.SemVers.fixtureFile, actual.Value)

    [<Fact>]
    let ``getSemanticVersion from valid package with all fields``() =
        let actual = ValidationPackageIndex.CommentFrontmatter.validFullFrontmatter |> ValidationPackageIndex.getSemanticVersion
        Assert.Equivalent(SemVer.SemVers.fixtureFile, actual)
        Assert.Equal(SemVer.SemVers.fixtureFile, actual)

    [<Fact>]
    let ``tryGetSemanticVersionString from valid package with all fields``() =
        let actual = ValidationPackageIndex.CommentFrontmatter.validFullFrontmatter |> ValidationPackageIndex.tryGetSemanticVersionString
        Assert.True(actual.IsSome)
        Assert.Equal(SemVer.Strings.fixtureFile, actual.Value)

    [<Fact>]
    let ``getSemanticVersionString from valid package with all fields``() =
        let actual = ValidationPackageIndex.CommentFrontmatter.validFullFrontmatter |> ValidationPackageIndex.getSemanticVersionString
        Assert.Equal(SemVer.Strings.fixtureFile, actual)

    [<Fact>]
    let ``identityEquals returns true for identical versions``() =
        Assert.True(
            ValidationPackageIndex.identityEquals
                ValidationPackageIndex.CommentFrontmatter.validMandatoryFrontmatter
                ValidationPackageIndex.CommentFrontmatter.validMandatoryFrontmatter
        )

    [<Fact>]
    let ``identityEquals returns false for non-identical versions``() =
        Assert.False(
            ValidationPackageIndex.identityEquals
                ValidationPackageIndex.CommentFrontmatter.validMandatoryFrontmatter
                ValidationPackageIndex.CommentFrontmatter.validFullFrontmatter
        )

    [<Fact>]
    let ``identityEquals returns false for same version with suffixes``() =
        let a = ValidationPackageIndex.create(
            repoPath = "",
            fileName = "",
            lastUpdated = testDate,
            contentHash = "",
            metadata = ValidationPackageMetadata.create(
                name = "",
                majorVersion = 1,
                minorVersion = 0,
                patchVersion = 0,
                summary = "",
                description = "",
                PreReleaseVersionSuffix = "some",
                BuildMetadataVersionSuffix = "suffix"
            )
        )
        let b = ValidationPackageIndex.create(
            repoPath = "",
            fileName = "",
            lastUpdated = testDate,
            contentHash = "",
            metadata = ValidationPackageMetadata.create(
                name = "",
                majorVersion = 1,
                minorVersion = 0,
                patchVersion = 0,
                summary = "",
                description = ""
            )
        )
        Assert.False(ValidationPackageIndex.identityEquals a b)

    [<Fact>]
    let ``identityEquals returns true for identical version with suffixes``() =
        let a = ValidationPackageIndex.create(
            repoPath = "",
            fileName = "",
            lastUpdated = testDate,
            contentHash = "",
            metadata = ValidationPackageMetadata.create(
                name = "",
                majorVersion = 1,
                minorVersion = 0,
                patchVersion = 0,
                summary = "",
                description = "",
                PreReleaseVersionSuffix = "some",
                BuildMetadataVersionSuffix = "suffix"
            )
        )
        let b = ValidationPackageIndex.create(
            repoPath = "",
            fileName = "",
            lastUpdated = testDate,
            contentHash = "",
            metadata = ValidationPackageMetadata.create(
                name = "",
                majorVersion = 1,
                minorVersion = 0,
                patchVersion = 0,
                summary = "",
                description = "",
                PreReleaseVersionSuffix = "some",
                BuildMetadataVersionSuffix = "suffix"
            )
        )
        Assert.True(ValidationPackageIndex.identityEquals a b)

module CommentFrontmatterIO =

    open System.IO

    [<Fact>]
    let ``valid indexed package is extracted from valid mandatory field test file`` () =

        let actual = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Comment/valid@1.0.0.fsx",
                lastUpdated = testDate
            )
        Assert.Equivalent(ValidationPackageIndex.CommentFrontmatter.validMandatoryFrontmatter, actual)


    [<Fact>]
    let ``valid indexed package is extracted from all fields test file`` () =

        let actual = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Comment/valid@2.0.0.fsx",
                lastUpdated = testDate
            )
        Assert.Equivalent(ValidationPackageIndex.CommentFrontmatter.validFullFrontmatter, actual)

    [<Fact>]
    let ``invalid indexed package is extracted from testfile with missing fields`` () =

        let actual = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Comment/invalid@0.0.fsx",
                lastUpdated = testDate
            )
        Assert.Equivalent(ValidationPackageIndex.CommentFrontmatter.invalidMissingMandatoryFrontmatter, actual)

    [<Fact>]
    let ``CRLF: correct content hash (with line endings replaced) is extracted from valid mandatory field test file`` () =
        let tmp_path = Path.GetTempFileName()
        File.WriteAllText(
            tmp_path,
            Frontmatter.Comment.validMandatoryFrontmatter.ReplaceLineEndings("\r\n")
        )
        let actual = 
            ValidationPackageIndex.create(
                repoPath = tmp_path,
                lastUpdated = testDate
            )
        let expected = {
            ValidationPackageIndex.CommentFrontmatter.validMandatoryFrontmatter with 
                RepoPath = tmp_path
                FileName = Path.GetFileName(tmp_path)
        }
        Assert.Equivalent(expected, actual)


    [<Fact>]
    let ``CRLF: correct content hash (with line endings replaced) is extracted from all fields test file`` () =
        let tmp_path = Path.GetTempFileName()
        File.WriteAllText(
            tmp_path,
            Frontmatter.Comment.validFullFrontmatter.ReplaceLineEndings("\r\n")
        )
        let actual = 
            ValidationPackageIndex.create(
                repoPath = tmp_path,
                lastUpdated = testDate
            )
        let expected = {
            ValidationPackageIndex.CommentFrontmatter.validFullFrontmatter with 
                RepoPath = tmp_path
                FileName = Path.GetFileName(tmp_path)
        }
        Assert.Equivalent(expected, actual)

    [<Fact>]
    let ``CRLF: correct content hash (with line endings replaced) is extracted from testfile with missing fields`` () =
        let tmp_path = Path.GetTempFileName()
        File.WriteAllText(
            tmp_path,
            Frontmatter.Comment.invalidMissingMandatoryFrontmatter.ReplaceLineEndings("\r\n")
        )
        let actual = 
            ValidationPackageIndex.create(
                repoPath = tmp_path,
                lastUpdated = testDate
            )
        let expected = {
            ValidationPackageIndex.CommentFrontmatter.invalidMissingMandatoryFrontmatter with 
                RepoPath = tmp_path
                FileName = Path.GetFileName(tmp_path)
        }
        Assert.Equivalent(expected, actual)


module BindingFrontmatterIO =

    open System.IO

    [<Fact>]
    let ``valid indexed package is extracted from valid mandatory field test file`` () =

        let actual = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Binding/valid@1.0.0.fsx",
                lastUpdated = testDate
            )
        Assert.Equivalent(ValidationPackageIndex.BindingFrontmatter.validMandatoryFrontmatter, actual)


    [<Fact>]
    let ``valid indexed package is extracted from all fields test file`` () =

        let actual = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Binding/valid@2.0.0.fsx",
                lastUpdated = testDate
            )
        Assert.Equivalent(ValidationPackageIndex.BindingFrontmatter.validFullFrontmatter, actual)

    [<Fact>]
    let ``invalid indexed package is extracted from testfile with missing fields`` () =

        let actual = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Binding/invalid@0.0.fsx",
                lastUpdated = testDate
            )
        Assert.Equivalent(ValidationPackageIndex.BindingFrontmatter.invalidMissingMandatoryFrontmatter, actual)

    [<Fact>]
    let ``CRLF: correct content hash (with line endings replaced) is extracted from valid mandatory field test file`` () =
        let tmp_path = Path.GetTempFileName()
        File.WriteAllText(
            tmp_path,
            Frontmatter.Binding.validMandatoryFrontmatter.ReplaceLineEndings("\r\n")
        )
        let actual = 
            ValidationPackageIndex.create(
                repoPath = tmp_path,
                lastUpdated = testDate
            )
        let expected = {
            ValidationPackageIndex.BindingFrontmatter.validMandatoryFrontmatter with 
                RepoPath = tmp_path
                FileName = Path.GetFileName(tmp_path)
        }
        Assert.Equivalent(expected, actual)


    [<Fact>]
    let ``CRLF: correct content hash (with line endings replaced) is extracted from all fields test file`` () =
        let tmp_path = Path.GetTempFileName()
        File.WriteAllText(
            tmp_path,
            Frontmatter.Binding.validFullFrontmatter.ReplaceLineEndings("\r\n")
        )
        let actual = 
            ValidationPackageIndex.create(
                repoPath = tmp_path,
                lastUpdated = testDate
            )
        let expected = {
            ValidationPackageIndex.BindingFrontmatter.validFullFrontmatter with 
                RepoPath = tmp_path
                FileName = Path.GetFileName(tmp_path)
        }
        Assert.Equivalent(expected, actual)

    [<Fact>]
    let ``CRLF: correct content hash (with line endings replaced) is extracted from testfile with missing fields`` () =
        let tmp_path = Path.GetTempFileName()
        File.WriteAllText(
            tmp_path,
            Frontmatter.Binding.invalidMissingMandatoryFrontmatter.ReplaceLineEndings("\r\n")
        )
        let actual = 
            ValidationPackageIndex.create(
                repoPath = tmp_path,
                lastUpdated = testDate
            )
        let expected = {
            ValidationPackageIndex.BindingFrontmatter.invalidMissingMandatoryFrontmatter with 
                RepoPath = tmp_path
                FileName = Path.GetFileName(tmp_path)
        }
        Assert.Equivalent(expected, actual)