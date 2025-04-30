namespace DomainTests

open System
open System.IO
open Xunit
open AVPRIndex
open AVPRIndex.Domain
open ReferenceObjects

module SemVer =

    [<Fact>]
    let ``create function for mandatory fields``() =
        let actual = SemVer.create(major=1, minor=0, patch=0)
        Assert.Equivalent(SemVer.SemVers.mandatory, actual)
        Assert.Equal(SemVer.SemVers.mandatory, actual)

    [<Fact>]
    let ``create function for all fields``() =
        let actual = SemVer.create(major=1, minor=0, patch=0, PreRelease="alpha.1", BuildMetadata="build.1")
        Assert.Equivalent(SemVer.SemVers.prereleaseAndBuildmetadata, actual)
        Assert.Equal(SemVer.SemVers.prereleaseAndBuildmetadata, actual)

    [<Fact>]
    let ``Can parse mandatory``() =
        let actual = SemVer.Strings.mandatory |> SemVer.tryParse
        Assert.True(actual.IsSome)
        Assert.Equivalent(SemVer.SemVers.mandatory, actual.Value)
        Assert.Equal(SemVer.SemVers.mandatory, actual.Value)

    [<Fact>]
    let ``Can parse prerelease``() =
        let actual = SemVer.Strings.prerelease |> SemVer.tryParse
        Assert.True(actual.IsSome)
        Assert.Equivalent(SemVer.SemVers.prerelease, actual.Value)
        Assert.Equal(SemVer.SemVers.prerelease, actual.Value)

    [<Fact>]
    let ``Can parse buildmetadata``() =
        let actual = SemVer.Strings.buildmetadata |> SemVer.tryParse
        Assert.True(actual.IsSome)
        Assert.Equivalent(SemVer.SemVers.buildmetadata, actual.Value)
        Assert.Equal(SemVer.SemVers.buildmetadata, actual.Value)

    [<Fact>]
    let ``Can parse prereleaseAndBuildmetadata``() =
        let actual = SemVer.Strings.prereleaseAndBuildmetadata |> SemVer.tryParse
        Assert.True(actual.IsSome)
        Assert.Equivalent(SemVer.SemVers.prereleaseAndBuildmetadata, actual.Value)
        Assert.Equal(SemVer.SemVers.prereleaseAndBuildmetadata, actual.Value)

    [<Fact>]
    let ``Can parse major version 0``() =
        let actual = SemVer.Strings.majorZero |> SemVer.tryParse
        Assert.True(actual.IsSome)
        Assert.Equivalent(SemVer.SemVers.majorZero, actual.Value)
        Assert.Equal(SemVer.SemVers.majorZero, actual.Value)

    [<Fact>]
    let ``Cannot parse leading 0 in major``() =
        let actual = SemVer.Strings.invalidLeadingZero |> SemVer.tryParse
        Assert.True(actual.IsNone)

    [<Fact>]
    let ``Cannot parse leading 0 in minor``() =
        let actual = SemVer.Strings.invalidLeadingZeroMinor |> SemVer.tryParse
        Assert.True(actual.IsNone)

    [<Fact>]
    let ``Cannot parse leading 0 in patch``() =
        let actual = SemVer.Strings.invalidLeadingZeroPatch |> SemVer.tryParse
        Assert.True(actual.IsNone)

    [<Fact>]
    let ``Cannot parse leading 0 in prerelease``() =
        let actual = SemVer.Strings.invalidLeadingZeroPrerelease |> SemVer.tryParse
        Assert.True(actual.IsNone)

    [<Fact>]
    let ``Can write mandatory``() =
        let actual = SemVer.SemVers.mandatory |> SemVer.toString
        Assert.Equivalent(SemVer.Strings.mandatory, actual)
        Assert.Equal(SemVer.Strings.mandatory, actual)

    [<Fact>]
    let ``Can write prerelease``() =
        let actual = SemVer.SemVers.prerelease |> SemVer.toString
        Assert.Equivalent(SemVer.Strings.prerelease, actual)
        Assert.Equal(SemVer.Strings.prerelease, actual)

    [<Fact>]
    let ``Can write buildmetadata``() =
        let actual = SemVer.SemVers.buildmetadata |> SemVer.toString
        Assert.Equivalent(SemVer.Strings.buildmetadata, actual)
        Assert.Equal(SemVer.Strings.buildmetadata, actual)

    [<Fact>]
    let ``Can write prereleaseAndBuildmetadata``() =
        let actual = SemVer.SemVers.prereleaseAndBuildmetadata |> SemVer.toString
        Assert.Equivalent(SemVer.Strings.prereleaseAndBuildmetadata, actual)
        Assert.Equal(SemVer.Strings.prereleaseAndBuildmetadata, actual)


module Author =
    
    [<Fact>]
    let ``create function for mandatory fields``() =
        let actual = Author.create(fullName = "test")
        Assert.Equivalent(Author.mandatoryFields, actual)

    [<Fact>]
    let ``create function for all fields``() =
        let actual = Author.create(
            fullName = "test", 
            Email = "test@test.test",
            Affiliation = "testaffiliation",
            AffiliationLink = "test.com"
        )
        Assert.Equivalent(Author.allFields, actual)

module OntologyAnnotation =

    [<Fact>]
    let ``create function for mandatory fields``() =
        let actual = OntologyAnnotation.create(name = "test")
        Assert.Equivalent(OntologyAnnotation.mandatoryFields, actual)

    [<Fact>]
    let ``create function for all fields``() =
        let actual = OntologyAnnotation.create(
            name = "test",
            TermSourceREF = "REF",
            TermAccessionNumber = "TAN"
        )
        Assert.Equivalent(OntologyAnnotation.allFields, actual)

module ValidationPackageMetadata =
    
    [<Fact>]
    let ``create function for mandatory fields``() =
        let actual = ValidationPackageMetadata.create(
            name = "name",
            summary = "summary" ,
            description = "description" ,
            majorVersion = 1,
            minorVersion = 0,
            patchVersion = 0
        )
        Assert.Equivalent(ValidationPackageMetadata.mandatoryFields, actual)

    [<Fact>]
    let ``create function for all fields``() =
        let actual = ValidationPackageMetadata.create(
            name = "name",
            summary = "summary" ,
            description = "description" ,
            majorVersion = 1,
            minorVersion = 0,
            patchVersion = 0,
            PreReleaseVersionSuffix = "alpha.1",
            BuildMetadataVersionSuffix = "build.1",
            Publish = true,
            Authors = [|
                Author.create(
                    fullName = "test", 
                    Email = "test@test.test",
                    Affiliation = "testaffiliation",
                    AffiliationLink = "test.com"
                )
            |],
            Tags = [|
            OntologyAnnotation.create(
                    name = "test",
                    TermSourceREF = "REF",
                    TermAccessionNumber = "TAN"
                )
            |],
            ReleaseNotes = "releasenotes",
            CQCHookEndpoint = "hookendpoint"
        )
        Assert.Equivalent(ValidationPackageMetadata.allFields, actual)

    [<Fact>]
    let ``tryGetSemanticVersion from valid package metadata with mandatory fields``() =
        let actual = ValidationPackageMetadata.mandatoryFields |> ValidationPackageMetadata.tryGetSemanticVersion
        Assert.True(actual.IsSome)
        Assert.Equivalent(SemVer.SemVers.mandatory, actual.Value)
        Assert.Equal(SemVer.SemVers.mandatory, actual.Value)

    [<Fact>]
    let ``getSemanticVersion from valid package metadata with mandatory fields``() =
        let actual = ValidationPackageMetadata.mandatoryFields |> ValidationPackageMetadata.getSemanticVersion
        Assert.Equivalent(SemVer.SemVers.mandatory, actual)
        Assert.Equal(SemVer.SemVers.mandatory, actual)

    [<Fact>]
    let ``tryGetSemanticVersionString from valid package metadata with mandatory fields``() =
        let actual = ValidationPackageMetadata.mandatoryFields |> ValidationPackageMetadata.tryGetSemanticVersionString
        Assert.True(actual.IsSome)
        Assert.Equal(SemVer.Strings.mandatory, actual.Value)

    [<Fact>]
    let ``getSemanticVersionString from valid package metadata with mandatory fields``() =
        let actual = ValidationPackageMetadata.mandatoryFields |> ValidationPackageMetadata.getSemanticVersionString
        Assert.Equal(SemVer.Strings.mandatory, actual)

    [<Fact>]
    let ``tryGetSemanticVersion from valid package metadata with all fields``() =
        let actual = ValidationPackageMetadata.allFields |> ValidationPackageMetadata.tryGetSemanticVersion
        Assert.True(actual.IsSome)
        Assert.Equivalent(SemVer.SemVers.prereleaseAndBuildmetadata, actual.Value)
        Assert.Equal(SemVer.SemVers.prereleaseAndBuildmetadata, actual.Value)

    [<Fact>]
    let ``getSemanticVersion from valid package metadata with all fields``() =
        let actual = ValidationPackageMetadata.allFields |> ValidationPackageMetadata.getSemanticVersion
        Assert.Equivalent(SemVer.SemVers.prereleaseAndBuildmetadata, actual)
        Assert.Equal(SemVer.SemVers.prereleaseAndBuildmetadata, actual)

    [<Fact>]
    let ``tryGetSemanticVersionString from valid package metadata with all fields``() =
        let actual = ValidationPackageMetadata.allFields |> ValidationPackageMetadata.tryGetSemanticVersionString
        Assert.True(actual.IsSome)
        Assert.Equal(SemVer.Strings.prereleaseAndBuildmetadata, actual.Value)

    [<Fact>]
    let ``getSemanticVersionString from valid package metadata with all fields``() =
        let actual = ValidationPackageMetadata.allFields |> ValidationPackageMetadata.getSemanticVersionString
        Assert.Equal(SemVer.Strings.prereleaseAndBuildmetadata, actual)