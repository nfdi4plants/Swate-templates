module ReferenceObjects

open Utils
open AVPRIndex
open AVPRIndex.Domain

let testDate = System.DateTimeOffset.Parse("01/01/2024")

module SemVer =
    
    module Strings =
        
        let mandatory = "1.0.0"
        let prerelease = "1.0.0-alpha.1"
        let buildmetadata = "1.0.0+build.1"
        let prereleaseAndBuildmetadata = "1.0.0-alpha.1+build.1"
        let majorZero = "0.1.2"

        let fixtureFile = "2.0.0-alpha.1+build.1"

        let invalidLeadingZero = "01.0.0"
        let invalidLeadingZeroMinor = "1.01.0"
        let invalidLeadingZeroPatch = "1.0.01"
        let invalidLeadingZeroPrerelease = "1.0.0-01"

    module SemVers =
        
        let mandatory = SemVer(Major = 1, Minor = 0, Patch = 0)
        let prerelease = SemVer(Major = 1, Minor = 0, Patch = 0, PreRelease = "alpha.1")
        let buildmetadata = SemVer(Major = 1, Minor = 0, Patch = 0, BuildMetadata = "build.1")
        let prereleaseAndBuildmetadata = SemVer(Major = 1, Minor = 0, Patch = 0, PreRelease = "alpha.1",BuildMetadata = "build.1")
        let majorZero = SemVer(Major = 0, Minor = 1, Patch = 2)
        let fixtureFile = SemVer(Major = 2, Minor = 0, Patch = 0, PreRelease = "alpha.1", BuildMetadata = "build.1")

module Hash =
    
    module Input =
        
        let noLineEndings = "This is a test string with no line endings."

        let windowsLineEndings = "This is a test string with Windows line endings.\r\nanother one"

        let unixLineEndings = "This is a test string with Unix line endings.\nanother one"

        let mixedLineEndings = "This is a test string with mixed line endings.\r\nanother one\nand another one"

    module Hashes =
        
        // note that these hashes represent the input with unified line endings!

        let noLineEndings = "810F403210CD3D056F2DF13676D9385A"
        let windowsLineEndings = "259AA1C2F8EE8F0A12A6077E60176973"
        let unixLineEndings = "A85A58F412C6358B3FF1638876579FC6"
        let mixedLineEndings = "EC425DD2233B497BCD9B9FDDFD35FA84"

        module CommentFrontmatter =
            
            let validMandatoryFrontmatter = "2A29D85A29D908C7DE214D56119DE207"
            let validFullFrontmatter = "E2BE9000A07122842FC805530DDC9FDA"
            let invalidMissingMandatoryFrontmatter = "4331EE804414463D7E6DE9B8B6A3D49C"

        module BindingFrontmatter =
            
            let validMandatoryFrontmatter = "FC9558E6681A4114794BA912925FC283"
            let validFullFrontmatter = "93B48B7357D19EC9F78A6F578A47CBEC"
            let invalidMissingMandatoryFrontmatter = "94C704CFD2538A819CC2C0FFA406A355"

module BinaryContent =
    open System.IO
    open System.Text

    module StringInput =
        
        let noLineEndings = "This is a test string with no line endings."
         
        let windowsLineEndings = "This is a test string with Windows line endings.\r\nanother one"

        let unixLineEndings = "This is a test string with Unix line endings.\nanother one"

        let mixedLineEndings = "This is a test string with mixed line endings.\r\nanother one\nand another one"


    module ArrayInput =
        
        let noLineEndings = StringInput.noLineEndings |> Encoding.UTF8.GetBytes
         
        let windowsLineEndings = StringInput.windowsLineEndings |> Encoding.UTF8.GetBytes

        let unixLineEndings = StringInput.unixLineEndings |> Encoding.UTF8.GetBytes

        let mixedLineEndings = StringInput.mixedLineEndings |> Encoding.UTF8.GetBytes


    module Content =

        // note that these hashes represent the input with unified line endings!

        let noLineEndings = StringInput.noLineEndings.ReplaceLineEndings("\n") |> Encoding.UTF8.GetBytes
        let windowsLineEndings = StringInput.windowsLineEndings.ReplaceLineEndings("\n") |> Encoding.UTF8.GetBytes
        let unixLineEndings = StringInput.unixLineEndings.ReplaceLineEndings("\n") |> Encoding.UTF8.GetBytes
        let mixedLineEndings = StringInput.mixedLineEndings.ReplaceLineEndings("\n") |> Encoding.UTF8.GetBytes

        module CommentFrontmatter =
            
            let validMandatoryFrontmatter =
                "fixtures/Frontmatter/Comment/valid@1.0.0.fsx"
                |> File.ReadAllText
                |> fun f -> f.ReplaceLineEndings("\n") 
                |> Encoding.UTF8.GetBytes

            let validFullFrontmatter =
                "fixtures/Frontmatter/Comment/valid@2.0.0.fsx"
                |> File.ReadAllText
                |> fun f -> f.ReplaceLineEndings("\n") 
                |> Encoding.UTF8.GetBytes

            let invalidMissingMandatoryFrontmatter = 
                "fixtures/Frontmatter/Comment/invalid@0.0.fsx"
                |> File.ReadAllText
                |> fun f -> f.ReplaceLineEndings("\n") 
                |> Encoding.UTF8.GetBytes

        module BindingFrontmatter =
            
            let validMandatoryFrontmatter =
                "fixtures/Frontmatter/Binding/valid@1.0.0.fsx"
                |> File.ReadAllText
                |> fun f -> f.ReplaceLineEndings("\n") 
                |> Encoding.UTF8.GetBytes

            let validFullFrontmatter =
                "fixtures/Frontmatter/Binding/valid@2.0.0.fsx"
                |> File.ReadAllText
                |> fun f -> f.ReplaceLineEndings("\n") 
                |> Encoding.UTF8.GetBytes

            let invalidMissingMandatoryFrontmatter = 
                "fixtures/Frontmatter/Binding/invalid@0.0.fsx"
                |> File.ReadAllText
                |> fun f -> f.ReplaceLineEndings("\n") 
                |> Encoding.UTF8.GetBytes

module Author = 
    
    let mandatoryFields = Author(FullName = "test")

    let allFields =
        Author(
            FullName = "test",
            Email = "test@test.test",
            Affiliation = "testaffiliation",
            AffiliationLink = "test.com"
        )

module OntologyAnnotation = 
    
    let mandatoryFields = OntologyAnnotation(Name = "test")

    let allFields = OntologyAnnotation(
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

    let allFields = ValidationPackageMetadata(
        Name = "name",
        Summary = "summary" ,
        Description = "description" ,
        MajorVersion = 1,
        MinorVersion = 0,
        PatchVersion = 0,
        PreReleaseVersionSuffix = "alpha.1",
        BuildMetadataVersionSuffix = "build.1",
        Publish = true,
        Authors = [|Author.allFields|],
        Tags = [|OntologyAnnotation.allFields|],
        ReleaseNotes = "releasenotes",
        CQCHookEndpoint = "hookendpoint"
    )

module Frontmatter = 

    module Comment =

        let validMandatoryFrontmatter = """(*
---
Name: valid
MajorVersion: 1
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
---
*)"""                                                                         .ReplaceLineEndings("\n")

        let validMandatoryFrontmatterExtracted = """
Name: valid
MajorVersion: 1
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
"""                                                                         .ReplaceLineEndings("\n")


        let validFullFrontmatter = """(*
---
Name: valid
MajorVersion: 2
MinorVersion: 0
PatchVersion: 0
PreReleaseVersionSuffix: alpha.1
BuildMetadataVersionSuffix: build.1
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
Publish: true
Authors:
  - FullName: John Doe
    Email: j@d.com
    Affiliation: University of Nowhere
    AffiliationLink: https://nowhere.edu
  - FullName: Jane Doe
    Email: jj@d.com
    Affiliation: University of Somewhere
    AffiliationLink: https://somewhere.edu
Tags:
  - Name: validation
  - Name: my-tag
    TermSourceREF: my-ontology
    TermAccessionNumber: MO:12345
ReleaseNotes: |
  - initial release
    - does the thing
    - does it well
CQCHookEndpoint: https://hook.com
---
*)"""                                                                         .ReplaceLineEndings("\n")

        let validFullFrontmatterExtracted = """
Name: valid
MajorVersion: 2
MinorVersion: 0
PatchVersion: 0
PreReleaseVersionSuffix: alpha.1
BuildMetadataVersionSuffix: build.1
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
Publish: true
Authors:
  - FullName: John Doe
    Email: j@d.com
    Affiliation: University of Nowhere
    AffiliationLink: https://nowhere.edu
  - FullName: Jane Doe
    Email: jj@d.com
    Affiliation: University of Somewhere
    AffiliationLink: https://somewhere.edu
Tags:
  - Name: validation
  - Name: my-tag
    TermSourceREF: my-ontology
    TermAccessionNumber: MO:12345
ReleaseNotes: |
  - initial release
    - does the thing
    - does it well
CQCHookEndpoint: https://hook.com
"""                                                                         .ReplaceLineEndings("\n")

        let invalidStartFrontmatter = """(
---
Name: invalid
MajorVersion: 1
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
---
*)"""                                                                         .ReplaceLineEndings("\n")

        let invalidEndFrontmatter = """(*
---
Name: invalid
MajorVersion: 1
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.---*)""".ReplaceLineEndings("\n")

        let invalidMissingMandatoryFrontmatter = """(*
---
Name: invalid
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
---
*)"""                                                                         .ReplaceLineEndings("\n")

        let invalidMissingMandatoryFrontmatterExtracted = """
Name: invalid
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
"""                                                                         .ReplaceLineEndings("\n")

    module Binding = 

        let validMandatoryFrontmatter = "let [<Literal>]PACKAGE_METADATA = \"\"\"(*
---
Name: valid
MajorVersion: 1
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
---
*)\"\"\""                                                                         .ReplaceLineEndings("\n")

        let validMandatoryFrontmatterExtracted = """
Name: valid
MajorVersion: 1
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
"""                                                                         .ReplaceLineEndings("\n")


        let validFullFrontmatter = "let [<Literal>]PACKAGE_METADATA = \"\"\"(*
---
Name: valid
MajorVersion: 2
MinorVersion: 0
PatchVersion: 0
PreReleaseVersionSuffix: alpha.1
BuildMetadataVersionSuffix: build.1
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
Publish: true
Authors:
  - FullName: John Doe
    Email: j@d.com
    Affiliation: University of Nowhere
    AffiliationLink: https://nowhere.edu
  - FullName: Jane Doe
    Email: jj@d.com
    Affiliation: University of Somewhere
    AffiliationLink: https://somewhere.edu
Tags:
  - Name: validation
  - Name: my-tag
    TermSourceREF: my-ontology
    TermAccessionNumber: MO:12345
ReleaseNotes: |
  - initial release
    - does the thing
    - does it well
CQCHookEndpoint: https://hook.com
---
*)\"\"\""                                                                         .ReplaceLineEndings("\n")

        let validFullFrontmatterExtracted = """
Name: valid
MajorVersion: 2
MinorVersion: 0
PatchVersion: 0
PreReleaseVersionSuffix: alpha.1
BuildMetadataVersionSuffix: build.1
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
Publish: true
Authors:
  - FullName: John Doe
    Email: j@d.com
    Affiliation: University of Nowhere
    AffiliationLink: https://nowhere.edu
  - FullName: Jane Doe
    Email: jj@d.com
    Affiliation: University of Somewhere
    AffiliationLink: https://somewhere.edu
Tags:
  - Name: validation
  - Name: my-tag
    TermSourceREF: my-ontology
    TermAccessionNumber: MO:12345
ReleaseNotes: |
  - initial release
    - does the thing
    - does it well
CQCHookEndpoint: https://hook.com
"""                                                                         .ReplaceLineEndings("\n")

        let invalidStartFrontmatter = "let [<Literal>]PACKAGE_METADATA = \"\"\"
---
Name: invalid
MajorVersion: 1
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
---
*)\"\"\""                                                                         .ReplaceLineEndings("\n")

        let invalidEndFrontmatter = "let [<Literal>]PACKAGE_METADATA = \"\"\"(*
---
Name: invalid
MajorVersion: 1
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.---*)".ReplaceLineEndings("\n")

        let invalidMissingMandatoryFrontmatter = "let [<Literal>]PACKAGE_METADATA = \"\"\"(*
---
Name: invalid
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
---
*)\"\"\""                                                                        .ReplaceLineEndings("\n")

        let invalidMissingMandatoryFrontmatterExtracted = """
Name: invalid
MinorVersion: 0
PatchVersion: 0
Summary: My package does the thing.
Description: |
  My package does the thing.
  It does it very good, it does it very well.
  It does it very fast, it does it very swell.
"""                                                                         .ReplaceLineEndings("\n")

module Metadata =
    
    let validMandatoryFrontmatter = 
    
        ValidationPackageMetadata(
            Name = "valid",
            MajorVersion = 1,
            MinorVersion = 0,
            PatchVersion = 0,
            Summary = "My package does the thing.",
            Description = """My package does the thing.
It does it very good, it does it very well.
It does it very fast, it does it very swell.
""".ReplaceLineEndings("\n")
        )
       
    let validFullFrontmatter = 
        ValidationPackageMetadata(
            Name = "valid",
            MajorVersion = 2,
            MinorVersion = 0,
            PatchVersion = 0,
            PreReleaseVersionSuffix = "alpha.1",
            BuildMetadataVersionSuffix = "build.1",
            Summary = "My package does the thing.",
            Publish = true,
            Description = """My package does the thing.
It does it very good, it does it very well.
It does it very fast, it does it very swell.
""".ReplaceLineEndings("\n"),
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

    let invalidMissingMandatoryFrontmatter = 
            ValidationPackageMetadata(
                Name = "invalid",
                MajorVersion = -1,
                MinorVersion = 0,
                PatchVersion = 0,
                Summary = "My package does the thing.",
                Description = """My package does the thing.
It does it very good, it does it very well.
It does it very fast, it does it very swell.
""".ReplaceLineEndings("\n")
            )

module ValidationPackageIndex =
    
    module CommentFrontmatter =

        let validMandatoryFrontmatter = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Comment/valid@1.0.0.fsx",
                fileName = "valid@1.0.0.fsx",
                lastUpdated = testDate,
                contentHash = (Frontmatter.Comment.validMandatoryFrontmatter |> md5hashNoReplace),
                metadata = Metadata.validMandatoryFrontmatter
            )

        let validFullFrontmatter = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Comment/valid@2.0.0.fsx",
                fileName = "valid@2.0.0.fsx",
                lastUpdated = testDate,
                contentHash = (Frontmatter.Comment.validFullFrontmatter |> md5hashNoReplace),
                metadata = Metadata.validFullFrontmatter
            )

        let invalidMissingMandatoryFrontmatter = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Comment/invalid@0.0.fsx",
                fileName = "invalid@0.0.fsx",
                lastUpdated = testDate,
                contentHash = (Frontmatter.Comment.invalidMissingMandatoryFrontmatter |> md5hashNoReplace),
                metadata = Metadata.invalidMissingMandatoryFrontmatter
            )

    module BindingFrontmatter =

        let validMandatoryFrontmatter = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Binding/valid@1.0.0.fsx",
                fileName = "valid@1.0.0.fsx",
                lastUpdated = testDate,
                contentHash = (Frontmatter.Binding.validMandatoryFrontmatter |> md5hashNoReplace),
                metadata = Metadata.validMandatoryFrontmatter
            )

        let validFullFrontmatter = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Binding/valid@2.0.0.fsx",
                fileName = "valid@2.0.0.fsx",
                lastUpdated = testDate,
                contentHash = (Frontmatter.Binding.validFullFrontmatter |> md5hashNoReplace),
                metadata = Metadata.validFullFrontmatter
            )

        let invalidMissingMandatoryFrontmatter = 
            ValidationPackageIndex.create(
                repoPath = "fixtures/Frontmatter/Binding/invalid@0.0.fsx",
                fileName = "invalid@0.0.fsx",
                lastUpdated = testDate,
                contentHash = (Frontmatter.Binding.invalidMissingMandatoryFrontmatter |> md5hashNoReplace),
                metadata = Metadata.invalidMissingMandatoryFrontmatter
            )