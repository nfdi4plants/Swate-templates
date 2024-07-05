namespace PackageContentHashTests

open System
open System.IO
open System.Text
open Xunit
open AVPRIndex
open AVPRIndex.Domain
open ReferenceObjects


module hashString =

    [<Fact>]
    let ``String with no line endings`` () =
        let actual = Hash.hashString Hash.Input.noLineEndings
        Assert.Equal(Hash.Hashes.noLineEndings, actual)

    [<Fact>]
    let ``String with CLRF`` () =
        let actual = Hash.hashString Hash.Input.windowsLineEndings
        Assert.Equal(Hash.Hashes.windowsLineEndings, actual)

    [<Fact>]
    let ``String with LF`` () =
        let actual = Hash.hashString Hash.Input.unixLineEndings
        Assert.Equal(Hash.Hashes.unixLineEndings, actual)

    [<Fact>]
    let ``String with mixed line endings`` () =
        let actual = Hash.hashString Hash.Input.mixedLineEndings
        Assert.Equal(Hash.Hashes.mixedLineEndings, actual)

module hashContent =
        
    [<Fact>]
    let ``mandatory binding frontmatter file`` () =
        let actual = 
            "fixtures/Frontmatter/Binding/valid@1.0.0.fsx"
            |> File.ReadAllText
            |> fun s -> s.ReplaceLineEndings("\n")
            |> Encoding.UTF8.GetBytes
            |> Hash.hashContent
        Assert.Equal(Hash.Hashes.BindingFrontmatter.validMandatoryFrontmatter, actual)

    [<Fact>]
    let ``full binding frontmatter file`` () =
        let actual = 
            "fixtures/Frontmatter/Binding/valid@2.0.0.fsx" 
            |> File.ReadAllText
            |> fun s -> s.ReplaceLineEndings("\n")
            |> Encoding.UTF8.GetBytes
            |> Hash.hashContent

        Assert.Equal(Hash.Hashes.BindingFrontmatter.validFullFrontmatter, actual)

    [<Fact>]
    let ``invalid binding frontmatter file`` () =
        let actual = 
            "fixtures/Frontmatter/Binding/invalid@0.0.fsx"
            |> File.ReadAllText
            |> fun s -> s.ReplaceLineEndings("\n")
            |> Encoding.UTF8.GetBytes
            |> Hash.hashContent
        Assert.Equal(Hash.Hashes.BindingFrontmatter.invalidMissingMandatoryFrontmatter, actual)

    [<Fact>]
    let ``mandatory comment frontmatter file`` () =
        let actual = 
            "fixtures/Frontmatter/Comment/valid@1.0.0.fsx"
            |> File.ReadAllText
            |> fun s -> s.ReplaceLineEndings("\n")
            |> Encoding.UTF8.GetBytes
            |> Hash.hashContent

        Assert.Equal(Hash.Hashes.CommentFrontmatter.validMandatoryFrontmatter, actual)

    [<Fact>]
    let ``full comment frontmatter file`` () =
        let actual = 
            "fixtures/Frontmatter/Comment/valid@2.0.0.fsx"
            |> File.ReadAllText
            |> fun s -> s.ReplaceLineEndings("\n")
            |> Encoding.UTF8.GetBytes
            |> Hash.hashContent
        Assert.Equal(Hash.Hashes.CommentFrontmatter.validFullFrontmatter, actual)

    [<Fact>]
    let ``invalid comment frontmatter file`` () =
        let actual = 
            "fixtures/Frontmatter/Comment/invalid@0.0.fsx"
            |> File.ReadAllText
            |> fun s -> s.ReplaceLineEndings("\n")
            |> Encoding.UTF8.GetBytes
            |> Hash.hashContent
        Assert.Equal(Hash.Hashes.CommentFrontmatter.invalidMissingMandatoryFrontmatter, actual)

module hashFile =
        
    [<Fact>]
    let ``mandatory binding frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Binding/valid@1.0.0.fsx" |> Hash.hashFile
        Assert.Equal(Hash.Hashes.BindingFrontmatter.validMandatoryFrontmatter, actual)

    [<Fact>]
    let ``full binding frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Binding/valid@2.0.0.fsx" |> Hash.hashFile
        Assert.Equal(Hash.Hashes.BindingFrontmatter.validFullFrontmatter, actual)

    [<Fact>]
    let ``invalid binding frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Binding/invalid@0.0.fsx" |> Hash.hashFile
        Assert.Equal(Hash.Hashes.BindingFrontmatter.invalidMissingMandatoryFrontmatter, actual)

    [<Fact>]
    let ``mandatory comment frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Comment/valid@1.0.0.fsx" |> Hash.hashFile
        Assert.Equal(Hash.Hashes.CommentFrontmatter.validMandatoryFrontmatter, actual)

    [<Fact>]
    let ``full comment frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Comment/valid@2.0.0.fsx" |> Hash.hashFile
        Assert.Equal(Hash.Hashes.CommentFrontmatter.validFullFrontmatter, actual)

    [<Fact>]
    let ``invalid comment frontmatter file`` () =
        let actual = "fixtures/Frontmatter/Comment/invalid@0.0.fsx" |> Hash.hashFile
        Assert.Equal(Hash.Hashes.CommentFrontmatter.invalidMissingMandatoryFrontmatter, actual)