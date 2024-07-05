namespace BinaryContentTests

open System
open System.IO
open System.Text
open Xunit
open AVPRIndex
open AVPRIndex.Domain
open ReferenceObjects

module fromString =

    [<Fact>]
    let ``String with no line endings`` () =
        let actual = BinaryContent.fromString BinaryContent.StringInput.noLineEndings
        Assert.Equal<byte array>(BinaryContent.Content.noLineEndings, actual)

    [<Fact>]
    let ``String with CLRF`` () =
        let actual = BinaryContent.fromString BinaryContent.StringInput.windowsLineEndings
        Assert.Equal<byte array>(BinaryContent.Content.windowsLineEndings, actual)

    [<Fact>]
    let ``String with LF`` () =
        let actual = BinaryContent.fromString BinaryContent.StringInput.unixLineEndings
        Assert.Equal<byte array>(BinaryContent.Content.unixLineEndings, actual)

    [<Fact>]
    let ``String with mixed line endings`` () =
        let actual = BinaryContent.fromString BinaryContent.StringInput.mixedLineEndings
        Assert.Equal<byte array>(BinaryContent.Content.mixedLineEndings, actual)

module fromFile =
        
    [<Fact>]
    let ``mandatory binding frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Binding/valid@1.0.0.fsx" |> BinaryContent.fromFile
        Assert.Equal<byte array>(BinaryContent.Content.BindingFrontmatter.validMandatoryFrontmatter, actual)

    [<Fact>]
    let ``full binding frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Binding/valid@2.0.0.fsx" |> BinaryContent.fromFile
        Assert.Equal<byte array>(BinaryContent.Content.BindingFrontmatter.validFullFrontmatter, actual)

    [<Fact>]
    let ``invalid binding frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Binding/invalid@0.0.fsx" |> BinaryContent.fromFile
        Assert.Equal<byte array>(BinaryContent.Content.BindingFrontmatter.invalidMissingMandatoryFrontmatter, actual)

    [<Fact>]
    let ``mandatory comment frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Comment/valid@1.0.0.fsx" |> BinaryContent.fromFile
        Assert.Equal<byte array>(BinaryContent.Content.CommentFrontmatter.validMandatoryFrontmatter, actual)

    [<Fact>]
    let ``full comment frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Comment/valid@2.0.0.fsx" |> BinaryContent.fromFile
        Assert.Equal<byte array>(BinaryContent.Content.CommentFrontmatter.validFullFrontmatter, actual)

    [<Fact>]
    let ``invalid comment frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Comment/invalid@0.0.fsx" |> BinaryContent.fromFile
        Assert.Equal<byte array>(BinaryContent.Content.CommentFrontmatter.invalidMissingMandatoryFrontmatter, actual)
