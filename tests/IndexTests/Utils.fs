module Utils

open AVPRIndex
open AVPRIndex.Domain
open Xunit
open System 
open System.Text
open System.IO
open System.Security.Cryptography

let md5hashNoReplace (content: string) =
    let md5 = MD5.Create()
    content
    |> Encoding.UTF8.GetBytes
    |> md5.ComputeHash
    |> Convert.ToHexString

type Assert with
    static member MetadataValid(m: ValidationPackageMetadata) =
        //test wether all required fields are present
        Assert.NotNull(m)
        Assert.NotNull(m.Name)
        Assert.NotEqual<string>(m.Name, "")
        Assert.NotNull(m.Summary)
        Assert.NotEqual<string>(m.Summary, "")
        Assert.NotNull(m.Description)
        Assert.NotEqual<string>(m.Description, "")
        Assert.NotNull(m.MajorVersion)
        Assert.True(m.MajorVersion >= 0)
        Assert.NotNull(m.MinorVersion)
        Assert.True(m.MinorVersion >= 0)
        Assert.NotNull(m.PatchVersion)
        Assert.True(m.PatchVersion >= 0)

    static member MetadataEqual(expected: ValidationPackageMetadata, actual: ValidationPackageMetadata) =
        Assert.Equal(expected.Name, actual.Name)
        Assert.Equal(expected.Summary, actual.Summary)
        Assert.Equal(expected.MajorVersion, actual.MajorVersion)
        Assert.Equal(expected.MinorVersion, actual.MinorVersion)
        Assert.Equal(expected.PatchVersion, actual.PatchVersion)
        Assert.Equal(expected.Publish, actual.Publish)
        Assert.Equivalent(expected.Authors, actual.Authors, strict = true)
        Assert.Equivalent(expected.Tags, actual.Tags, strict = true)
        Assert.Equal(expected.ReleaseNotes, actual.ReleaseNotes)
        Assert.Equal(expected.CQCHookEndpoint, actual.CQCHookEndpoint)
        Assert.Equivalent(expected, actual, strict = true)