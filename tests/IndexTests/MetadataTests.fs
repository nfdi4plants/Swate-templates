namespace MetadataTests

open System
open Xunit
open AVPRIndex
open AVPRIndex.Domain
open AVPRIndex.Frontmatter
open ReferenceObjects
open Utils

module InMemory =

    [<Fact>]
    let ``valid metadata is extracted from valid comment frontmatter`` () =
        Assert.All(
            [
                Frontmatter.Comment.validMandatoryFrontmatter, Metadata.validMandatoryFrontmatter
                Frontmatter.Comment.validFullFrontmatter, Metadata.validFullFrontmatter
            ],
            (fun (fm, expected) ->
                let actual = ValidationPackageMetadata.extractFromString fm
                Assert.Equivalent(expected, actual)
            )
        )

    [<Fact>]
    let ``valid metadata is extracted from valid binding frontmatter`` () =
        Assert.All(
            [
                Frontmatter.Binding.validMandatoryFrontmatter, Metadata.validMandatoryFrontmatter
                Frontmatter.Binding.validFullFrontmatter, Metadata.validFullFrontmatter
            ],
            (fun (fm, expected) ->
                let actual = ValidationPackageMetadata.extractFromString fm
                Assert.Equivalent(expected, actual)
            )
        )

module IO =

    open System.IO

    [<Fact>]
    let ``valid metadata is extracted from valid mandatory field test file with comment frontmatter`` () =

        let actual = File.ReadAllText("fixtures/Frontmatter/Comment/valid@1.0.0.fsx") |> ValidationPackageMetadata.extractFromString

        Assert.MetadataValid(actual)
        Assert.Equivalent(Metadata.validMandatoryFrontmatter, actual)

    [<Fact>]
    let ``valid metadata is extracted from all fields test file with comment frontmatter`` () =

        let actual = File.ReadAllText("fixtures/Frontmatter/Comment/valid@2.0.0.fsx") |> ValidationPackageMetadata.extractFromString

        Assert.MetadataValid(actual)
        Assert.Equivalent(Metadata.validFullFrontmatter, actual)

    [<Fact>]
    let ``invalid metadata is extracted from testfile with missing fields with comment frontmatter`` () =

        let actual = File.ReadAllText("fixtures/Frontmatter/Comment/invalid@0.0.fsx") |> ValidationPackageMetadata.extractFromString

        Assert.ThrowsAny(fun () -> Assert.MetadataValid(actual)) |> ignore
        Assert.Equivalent(Metadata.invalidMissingMandatoryFrontmatter, actual)

    [<Fact>]
    let ``valid metadata is extracted from valid mandatory field test file with binding frontmatter`` () =

        let actual = File.ReadAllText("fixtures/Frontmatter/Binding/valid@1.0.0.fsx") |> ValidationPackageMetadata.extractFromString

        Assert.MetadataValid(actual)
        Assert.Equivalent(Metadata.validMandatoryFrontmatter, actual)

    [<Fact>]
    let ``valid metadata is extracted from all fields test file with binding frontmatter`` () =

        let actual = File.ReadAllText("fixtures/Frontmatter/Binding/valid@2.0.0.fsx") |> ValidationPackageMetadata.extractFromString

        Assert.MetadataValid(actual)
        Assert.Equivalent(Metadata.validFullFrontmatter, actual)

    [<Fact>]
    let ``invalid metadata is extracted from testfile with missing fields with binding frontmatter`` () =

        let actual = File.ReadAllText("fixtures/Frontmatter/Binding/invalid@0.0.fsx") |> ValidationPackageMetadata.extractFromString

        Assert.ThrowsAny(fun () -> Assert.MetadataValid(actual)) |> ignore
        Assert.Equivalent(Metadata.invalidMissingMandatoryFrontmatter, actual)