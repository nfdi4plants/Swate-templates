namespace TypeExtensionsTests

open System
open System.IO
open System.Text
open Xunit
open AVPRClient

open System.Security.Cryptography

module ValidationPackage =
    
    [<Fact>]
    let ``IdentityEquals returns true for the same package (no suffixes)`` () =
        Assert.True(
            ReferenceObjects.ValidationPackage.allFields_cqcHookAddition.IdentityEquals(
                ReferenceObjects.ValidationPackage.allFields_cqcHookAddition
            )
        )
        
    [<Fact>]
    let ``IdentityEquals returns true for the same package with suffixes`` () =
        Assert.True(
            ReferenceObjects.ValidationPackage.allFields_semVerAddition.IdentityEquals(
                ReferenceObjects.ValidationPackage.allFields_semVerAddition
            )
        )

    [<Fact>]
    let ``IdentityEquals returns false for the different packages`` () =
        Assert.False(
            ReferenceObjects.ValidationPackage.allFields_cqcHookAddition.IdentityEquals(
                ReferenceObjects.ValidationPackage.allFields_semVerAddition
            )
        )

    [<Fact>]
    let ``IdentityEquals returns true for indexed package with same version (no suffixes)`` () =
        Assert.True(
            ReferenceObjects.ValidationPackage.allFields_cqcHookAddition.IdentityEquals(
                ReferenceObjects.ValidationPackageIndex.allFields_cqcHookAddition
            )
        )
        
    [<Fact>]
    let ``IdentityEquals returns true for indexed package with same version with suffixes`` () =
        Assert.True(
            ReferenceObjects.ValidationPackage.allFields_semVerAddition.IdentityEquals(
                ReferenceObjects.ValidationPackageIndex.allFields_semVerAddition
            )
        )

    [<Fact>]
    let ``IdentityEquals returns false for a different indexed package`` () =
        Assert.False(
            ReferenceObjects.ValidationPackage.allFields_cqcHookAddition.IdentityEquals(
                ReferenceObjects.ValidationPackageIndex.allFields_semVerAddition
            )
        )

module ValidationPackageIndex =

    let allFields_cqcHookAddition = AVPRIndex.Domain.ValidationPackageIndex.create(
        repoPath = "fixtures/allFields_cqcHookAddition.fsx",
        fileName = "allFields_cqcHookAddition.fsx",
        lastUpdated = ReferenceObjects.date,
        contentHash = ReferenceObjects.Hash.expected_hash_cqcHookAddition,
        metadata = ReferenceObjects.ValidationPackageMetadata.allFields_cqcHookAddition
    )

    [<Fact>]
    let ``CQCHook Addition - toValidationPackage with release date`` () =
        let actual = allFields_cqcHookAddition.toValidationPackage(ReferenceObjects.date)
        Assert.Equivalent(actual, ReferenceObjects.ValidationPackage.allFields_cqcHookAddition)

    [<Fact>]
    let ``CQCHook Addition - toPackageContentHash without direct file hash`` () =
        let actual = allFields_cqcHookAddition.toPackageContentHash()
        Assert.Equivalent(ReferenceObjects.Hash.allFields_cqcHookAddition, actual)

    [<Fact>]
    let ``CQCHook Addition - toPackageContentHash with direct file hash`` () =
        let actual = allFields_cqcHookAddition.toPackageContentHash(HashFileDirectly = true)
        Assert.Equivalent(ReferenceObjects.Hash.allFields_cqcHookAddition, actual)

    let allFields_semVerAddition = AVPRIndex.Domain.ValidationPackageIndex.create(
        repoPath = "fixtures/allFields_semVerAddition.fsx",
        fileName = "allFields_semVerAddition.fsx",
        lastUpdated = ReferenceObjects.date,
        contentHash = ReferenceObjects.Hash.expected_hash_semVerAddition,
        metadata = ReferenceObjects.ValidationPackageMetadata.allFields_semVerAddition
    )

    [<Fact>]
    let ``SemVer Addition - toValidationPackage with release date`` () =
        let actual = allFields_semVerAddition.toValidationPackage(ReferenceObjects.date)
        Assert.Equivalent(actual, ReferenceObjects.ValidationPackage.allFields_semVerAddition)

    [<Fact>]
    let ``SemVer Addition - toPackageContentHash without direct file hash`` () =
        let actual = allFields_semVerAddition.toPackageContentHash()
        Assert.Equivalent(ReferenceObjects.Hash.allFields_semVerAddition, actual)

    [<Fact>]
    let ``SemVer Addition - toPackageContentHash with direct file hash`` () =
        let actual = allFields_semVerAddition.toPackageContentHash(HashFileDirectly = true)
        Assert.Equivalent(ReferenceObjects.Hash.allFields_semVerAddition, actual)

    [<Fact>]
    let ``IdentityEquals returns true for package with same version (no suffixes)`` () =
        Assert.True(
            ReferenceObjects.ValidationPackageIndex.allFields_cqcHookAddition.IdentityEquals(
                ReferenceObjects.ValidationPackage.allFields_cqcHookAddition
            )
        )
        
    [<Fact>]
    let ``IdentityEquals returns true for package with same version with suffixes`` () =
        Assert.True(
            ReferenceObjects.ValidationPackageIndex.allFields_semVerAddition.IdentityEquals(
                ReferenceObjects.ValidationPackage.allFields_semVerAddition
            )
        )

    [<Fact>]
    let ``IdentityEquals returns false for a different package`` () =
        Assert.False(
            ReferenceObjects.ValidationPackageIndex.allFields_cqcHookAddition.IdentityEquals(
                ReferenceObjects.ValidationPackage.allFields_semVerAddition
            )
        )
module Author =

    open System.Collections
    open System.Collections.Generic

    [<Fact>]
    let ``AsIndexType with mandatory fields`` () =
        let actual = ReferenceObjects.Author.mandatoryFieldsClient.AsIndexType()
        Assert.Equivalent(actual, ReferenceObjects.Author.mandatoryFieldsClient)

    [<Fact>]
    let ``AsIndexType with all fields`` () =
        let actual = ReferenceObjects.Author.allFieldsClient.AsIndexType()
        Assert.Equivalent(actual, ReferenceObjects.Author.allFieldsClient)

    [<Fact>]
    let ``AsIndexType with mandatory fields on collection`` () =
        let actual = [|ReferenceObjects.Author.mandatoryFieldsClient|].AsIndexType()
        Assert.Equivalent(actual, [|ReferenceObjects.Author.mandatoryFieldsClient|])

    [<Fact>]
    let ``AsIndexType with all fields on collection`` () =
        let actual = [|ReferenceObjects.Author.allFieldsClient|].AsIndexType()
        Assert.Equivalent(actual, [|ReferenceObjects.Author.allFieldsClient|])

module OntologyAnnotation =
    
    [<Fact>]
    let ``AsIndexType with mandatory fields`` () =
        let actual = ReferenceObjects.OntologyAnnotation.mandatoryFieldsClient.AsIndexType()
        Assert.Equivalent(actual, ReferenceObjects.OntologyAnnotation.mandatoryFieldsClient)

    [<Fact>]
    let ``AsIndexType with all fields`` () =
        let actual = ReferenceObjects.OntologyAnnotation.allFieldsClient.AsIndexType()
        Assert.Equivalent(actual, ReferenceObjects.OntologyAnnotation.allFieldsClient)

    [<Fact>]
    let ``AsIndexType with mandatory fields on collection`` () =
        let actual = [|ReferenceObjects.OntologyAnnotation.mandatoryFieldsClient|].AsIndexType()
        Assert.Equivalent(actual, [|ReferenceObjects.OntologyAnnotation.mandatoryFieldsClient|])

    [<Fact>]
    let ``AsIndexType with all fields on collection`` () =
        let actual = [|ReferenceObjects.OntologyAnnotation.allFieldsClient|].AsIndexType()
        Assert.Equivalent(actual, [|ReferenceObjects.OntologyAnnotation.allFieldsClient|])