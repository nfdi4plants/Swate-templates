module ReferenceObjects

open Utils
open AVPRIndex
open AVPRClient

let date = System.DateTime.ParseExact("2021-01-01", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)

let testDate = System.DateTimeOffset.Parse("01/01/2024")

module Author = 
    
    let mandatoryFieldsClient = AVPRClient.Author(FullName = "test")

    let allFieldsClient =
        AVPRClient.Author(
            FullName = "test",
            Email = "test@test.test",
            Affiliation = "testaffiliation",
            AffiliationLink = "test.com"
        )

    let mandatoryFieldsIndex = AVPRIndex.Domain.Author(FullName = "test")

    let allFieldsIndex =
        AVPRIndex.Domain.Author(
            FullName = "test",
            Email = "test@test.test",
            Affiliation = "testaffiliation",
            AffiliationLink = "test.com"
        )

module OntologyAnnotation = 
    
    let mandatoryFieldsClient = AVPRClient.OntologyAnnotation(Name = "test")

    let allFieldsClient = AVPRClient.OntologyAnnotation(
        Name = "test",
        TermSourceREF = "REF",
        TermAccessionNumber = "TAN"
    )

    let mandatoryFieldsIndex = AVPRIndex.Domain.OntologyAnnotation(Name = "test")

    let allFieldsIndex = AVPRIndex.Domain.OntologyAnnotation(
        Name = "test",
        TermSourceREF = "REF",
        TermAccessionNumber = "TAN"
    )

module ValidationPackageMetadata = 
    
    let mandatoryFields = ValidationPackageMetadata(
        Name = "name",
        Summary = "summary" ,
        Description = "description" ,
        MajorVersion = 1,
        MinorVersion = 0,
        PatchVersion = 0
    )

    let allFields_cqcHookAddition = ValidationPackageMetadata(
        Name = "name",
        Summary = "summary" ,
        Description = "description" ,
        MajorVersion = 1,
        MinorVersion = 0,
        PatchVersion = 0,
        Publish = true,
        Authors = [|Author.allFieldsIndex|],
        Tags = [|OntologyAnnotation.allFieldsIndex|],
        ReleaseNotes = "releasenotes",
        CQCHookEndpoint = "hookendpoint"
    )

    let allFields_semVerAddition = ValidationPackageMetadata(
        Name = "name",
        Summary = "summary" ,
        Description = "description" ,
        MajorVersion = 1,
        MinorVersion = 0,
        PatchVersion = 0,
        PreReleaseVersionSuffix = "use",
        BuildMetadataVersionSuffix = "suffixes",
        Publish = true,
        Authors = [|Author.allFieldsIndex|],
        Tags = [|OntologyAnnotation.allFieldsIndex|],
        ReleaseNotes = "releasenotes",
        CQCHookEndpoint = "hookendpoint"
    )

module Hash =

    let expected_hash_cqcHookAddition = "C5BD4262301D27CF667106D9024BD721"

    let allFields_cqcHookAddition = AVPRClient.PackageContentHash(
        PackageName = "name",
        PackageMajorVersion = 1,
        PackageMinorVersion = 0,
        PackagePatchVersion = 0,
        PackagePreReleaseVersionSuffix = "",
        PackageBuildMetadataVersionSuffix = "",
        Hash = expected_hash_cqcHookAddition
    )

    let expected_hash_semVerAddition = "E3D3C259C4B3F54783283735B20F8C23"

    let allFields_semVerAddition = AVPRClient.PackageContentHash(
        PackageName = "name",
        PackageMajorVersion = 1,
        PackageMinorVersion = 0,
        PackagePatchVersion = 0,
        PackagePreReleaseVersionSuffix = "use",
        PackageBuildMetadataVersionSuffix = "suffixes",
        Hash = expected_hash_semVerAddition
    )

module BinaryContent =

    open System.IO

    let expected_content_cqcHookAddition = "(*
---
Name: name
Summary: summary
Description = description
MajorVersion: 1
MinorVersion: 0
PatchVersion: 0
Publish: true
Authors:
  - FullName: test
    Email: test@test.test
    Affiliation: testaffiliation
    AffiliationLink: test.com
Tags:
  - Name: test
    TermSourceREF: REF
    TermAccessionNumber: TAN
ReleaseNotes: releasenotes
CQCHookEndpoint: hookendpoint
---
*)

printfn \"yes\""                                    .ReplaceLineEndings("\n")

    let expected_binary_content_cqcHookAddition = expected_content_cqcHookAddition |> System.Text.Encoding.UTF8.GetBytes

    let expected_content_semVerAddition = "(*
---
Name: name
Summary: summary
Description = description
MajorVersion: 1
MinorVersion: 0
PatchVersion: 0
PreReleaseVersionSuffix: use
BuildMetadataVersionSuffix: suffixes
Publish: true
Authors:
  - FullName: test
    Email: test@test.test
    Affiliation: testaffiliation
    AffiliationLink: test.com
Tags:
  - Name: test
    TermSourceREF: REF
    TermAccessionNumber: TAN
ReleaseNotes: releasenotes
CQCHookEndpoint: hookendpoint
---
*)

printfn \"yes\""                                    .ReplaceLineEndings("\n")

    let expected_binary_content_semVerAddition = expected_content_semVerAddition |> System.Text.Encoding.UTF8.GetBytes

module ValidationPackage =

    open System.IO

    let allFields_cqcHookAddition = AVPRClient.ValidationPackage(
        Name = "name",
        Summary = "summary" ,
        Description = "description" ,
        MajorVersion = 1,
        MinorVersion = 0,
        PatchVersion = 0,
        PreReleaseVersionSuffix = "",
        BuildMetadataVersionSuffix = "",
        PackageContent = BinaryContent.expected_binary_content_cqcHookAddition,
        ReleaseDate = date,
        Authors = [|Author.allFieldsClient|],
        Tags = [|OntologyAnnotation.allFieldsClient|],
        ReleaseNotes = "releasenotes",
        CQCHookEndpoint = "hookendpoint"
    )

    let allFields_semVerAddition = AVPRClient.ValidationPackage(
        Name = "name",
        Summary = "summary" ,
        Description = "description" ,
        MajorVersion = 1,
        MinorVersion = 0,
        PatchVersion = 0,
        PreReleaseVersionSuffix = "use",
        BuildMetadataVersionSuffix = "suffixes",
        PackageContent = BinaryContent.expected_binary_content_semVerAddition,
        ReleaseDate = date,
        Authors = [|Author.allFieldsClient|],
        Tags = [|OntologyAnnotation.allFieldsClient|],
        ReleaseNotes = "releasenotes",
        CQCHookEndpoint = "hookendpoint"
    )

module ValidationPackageIndex =

    open System.IO

    let allFields_cqcHookAddition = AVPRIndex.Domain.ValidationPackageIndex.create(
        repoPath = "",
        fileName = "",
        lastUpdated = System.DateTime.Now,
        contentHash = "",
        metadata = AVPRIndex.Domain.ValidationPackageMetadata.create(
            name = "name",
            majorVersion = 1,
            minorVersion = 0,
            patchVersion = 0,
            summary = "summary",
            description = "description"
        )
    )

    let allFields_semVerAddition = AVPRIndex.Domain.ValidationPackageIndex.create(
        repoPath = "",
        fileName = "",
        lastUpdated = System.DateTime.Now,
        contentHash = "",
        metadata = AVPRIndex.Domain.ValidationPackageMetadata.create(
            name = "name",
            majorVersion = 1,
            minorVersion = 0,
            patchVersion = 0,
            summary = "summary",
            description = "description",
            PreReleaseVersionSuffix = "use",
            BuildMetadataVersionSuffix = "suffixes"
        )
    )