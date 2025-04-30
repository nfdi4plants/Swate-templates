namespace FrontmatterTests

open System
open Xunit
open AVPRIndex
open ReferenceObjects

module Comment =

    module InMemory =

        [<Fact>]
        let ``valid frontmatter capture guides lead to results`` () =
            Assert.All(
                [Frontmatter.Comment.validMandatoryFrontmatter; Frontmatter.Comment.validFullFrontmatter; Frontmatter.Comment.invalidMissingMandatoryFrontmatter],
                (fun fm ->
                    Assert.True ((Frontmatter.tryExtractFromString fm).IsSome)
                )
            )

        [<Fact>]
        let ``valid frontmatter capture guides lead to correctly extracted substrings`` () =
            Assert.All(
                [
                    Frontmatter.Comment.validMandatoryFrontmatter, Frontmatter.Comment.validMandatoryFrontmatterExtracted 
                    Frontmatter.Comment.validFullFrontmatter, Frontmatter.Comment.validFullFrontmatterExtracted
                    Frontmatter.Comment.invalidMissingMandatoryFrontmatter, Frontmatter.Comment.invalidMissingMandatoryFrontmatterExtracted
                ],
                (fun (fm, expected) ->
                    let actual = Frontmatter.extractFromString fm
                    Assert.Equal(expected, actual)
                )
            )

        [<Fact>]
        let ``invalid frontmatter capture substrings are leading to None`` () =
            Assert.All(
                [Frontmatter.Comment.invalidEndFrontmatter; Frontmatter.Comment.invalidStartFrontmatter],
                (fun fm ->
                    Assert.True ((Frontmatter.tryExtractFromString fm).IsNone)
                )
            )

    module IO =

        open System.IO

        [<Fact>]
        let ``valid frontmatter substring is extracted from valid mandatory field test file`` () =

            let actual = File.ReadAllText("fixtures/Frontmatter/Comment/valid@1.0.0.fsx") |> Frontmatter.tryExtractFromString

            Assert.True actual.IsSome
            Assert.Equal (Frontmatter.Comment.validMandatoryFrontmatterExtracted, actual.Value)

        [<Fact>]
        let ``valid frontmatter substring is correctly from all fields test file`` () =

            let actual = File.ReadAllText("fixtures/Frontmatter/Comment//valid@2.0.0.fsx") |> Frontmatter.tryExtractFromString

            Assert.True actual.IsSome
            Assert.Equal (Frontmatter.Comment.validFullFrontmatterExtracted, actual.Value)

        [<Fact>]
        let ``frontmatter substring is extracted although metadata is missing fields`` () =

            let actual = File.ReadAllText("fixtures/Frontmatter/Comment//invalid@0.0.fsx") |> Frontmatter.tryExtractFromString

            Assert.True actual.IsSome
            Assert.Equal (Frontmatter.Comment.invalidMissingMandatoryFrontmatterExtracted, actual.Value)

module Binding =

    module InMemory =

        [<Fact>]
        let ``valid frontmatter capture guides lead to results`` () =
            Assert.All(
                [Frontmatter.Binding.validMandatoryFrontmatter; Frontmatter.Binding.validFullFrontmatter; Frontmatter.Binding.invalidMissingMandatoryFrontmatter],
                (fun fm ->
                    Assert.True ((Frontmatter.tryExtractFromString fm).IsSome)
                )
            )

        [<Fact>]
        let ``valid frontmatter capture guides lead to correctly extracted substrings`` () =
            Assert.All(
                [
                    Frontmatter.Binding.validMandatoryFrontmatter, Frontmatter.Binding.validMandatoryFrontmatterExtracted 
                    Frontmatter.Binding.validFullFrontmatter, Frontmatter.Binding.validFullFrontmatterExtracted
                    Frontmatter.Binding.invalidMissingMandatoryFrontmatter, Frontmatter.Binding.invalidMissingMandatoryFrontmatterExtracted
                ],
                (fun (fm, expected) ->
                    let actual = Frontmatter.extractFromString fm
                    Assert.Equal(expected, actual)
                )
            )

        [<Fact>]
        let ``invalid frontmatter capture substrings are leading to None`` () =
            Assert.All(
                [Frontmatter.Binding.invalidEndFrontmatter; Frontmatter.Binding.invalidStartFrontmatter],
                (fun fm ->
                    Assert.True ((Frontmatter.tryExtractFromString fm).IsNone)
                )
            )

    module IO =

        open System.IO

        [<Fact>]
        let ``valid frontmatter substring is extracted from valid mandatory field test file`` () =

            let actual = File.ReadAllText("fixtures/Frontmatter/Binding/valid@1.0.0.fsx") |> Frontmatter.tryExtractFromString

            Assert.True actual.IsSome
            Assert.Equal (Frontmatter.Binding.validMandatoryFrontmatterExtracted, actual.Value)

        [<Fact>]
        let ``valid frontmatter substring is correctly from all fields test file`` () =

            let actual = File.ReadAllText("fixtures/Frontmatter/Binding/valid@2.0.0.fsx") |> Frontmatter.tryExtractFromString

            Assert.True actual.IsSome
            Assert.Equal (Frontmatter.Binding.validFullFrontmatterExtracted, actual.Value)

        [<Fact>]
        let ``frontmatter substring is extracted although metadata is missing fields`` () =

            let actual = File.ReadAllText("fixtures/Frontmatter/Binding/invalid@0.0.fsx") |> Frontmatter.tryExtractFromString

            Assert.True actual.IsSome
            Assert.Equal (Frontmatter.Binding.invalidMissingMandatoryFrontmatterExtracted, actual.Value)