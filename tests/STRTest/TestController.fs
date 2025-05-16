namespace STRTest

open System.IO
open System.Text
open System.Text.RegularExpressions

open FsSpreadsheet
open FsSpreadsheet.Net

open STRCI
open STRService.Models
open STRClient

open ARCtrl

open Expecto

module Constants =

    let TagWhiteList : string [] [] = 
        [| 
            [|"RNASeq"; "mRNASeq"; "DNASeq"|]
            [|"MIAPE"; "MIAPPE"|]
        |]
    let TagSimiliarityThreshold = 0.8

    let TemplateSimilarityThershold = 0.3 // Minimum 30% difference

module Helper =

    type CompositeHeader with
      member this.ToContent() =
        match Microsoft.FSharp.Reflection.FSharpValue.GetUnionFields(this, typeof<CompositeHeader>) with
        | case, more -> [|
            case.Name; 
            for o in more do 
              match o with
              | :? OntologyAnnotation as oa -> yield! [|oa.NameText; Option.defaultValue "" oa.TermSourceREF; oa.TermAccessionShort|] 
              | :? IOType as io -> io.ToString()
              | anyElse -> string anyElse
          |]

module SorensenDice =
    let inline calculateDistance (x : Set<'T>) (y : Set<'T>) =
        match  (x.Count, y.Count) with
        | (0,0) -> 1.
        | (xCount,yCount) -> (2. * (Set.intersect x y |> Set.count |> float)) / ((xCount + yCount) |> float)
    
    let createBigrams (s:string) =
        s
            .ToUpperInvariant()
            .ToCharArray()
        |> Array.windowed 2
        |> Array.map (fun inner -> sprintf "%c%c" inner.[0] inner.[1])
        |> set

    let createDistanceScores (searchStr:string) (f: 'a -> string) (arrayToSort:'a []) =
        let searchSet = searchStr |> createBigrams
        arrayToSort
        |> Array.map (fun result ->
            let resultSet = f result |> createBigrams
            calculateDistance resultSet searchSet, result
        )

open Helper
open Constants

type TestController (?templatesPath) = 

    member this.VersionPattern = @"_v\d+\.\d+\.\d+"

    member this.Client =
        let httpClient = new System.Net.Http.HttpClient()
        httpClient.DefaultRequestHeaders.Add("X-API-KEY", "")
        new STRClient.Client(httpClient)

    member this.TemplatesPath =
        if templatesPath.IsSome then templatesPath.Value
        else
            let solutionRoot = STRCIController.FindSolutionRoot (DirectoryInfo(System.Environment.CurrentDirectory))
            Path.Combine(solutionRoot, "templates")

    member this.MatchResult result =
        match result with
        | 0 -> 
            printfn "All checks successfull! ✅"
            System.Environment.ExitCode <- 0
            System.Environment.Exit(0)
            0
        | 1 -> 
            System.Environment.ExitCode <- 1
            printfn "Error! Tests failed!"
            System.Environment.Exit(1)
            1
        | 2 -> 
            System.Environment.ExitCode <- 2
            printfn "Error! Tests errored!"
            System.Environment.Exit(2)
            2
        | anyElse -> failwithf "Error! Unknown exit condition! %i" anyElse

    member this.TestConvertibleTemplateFiles () = 
        let directories =
            DirectoryInfo(this.TemplatesPath).GetDirectories()
        let filePaths =
            directories
            |> Array.collect(fun directory ->
                directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
            |> Array.map (fun file -> file.FullName)

        filePaths
        |> Array.map (fun templatePath ->
            testCase templatePath <| fun _ ->
                let p = templatePath
                let template =
                    try
                        FsWorkbook.fromXlsxFile p
                        |> Spreadsheet.Template.fromFsWorkbook
                        |> Ok
                    with
                        | e -> Error(templatePath, e)
                match template with
                | Error (p, e) -> failwith $"Unable to read template: {p}. {e.Message}"
                | Ok _ ->
                    // a bit redundant, i decided to use this syntax to improve error message in 'Error' case
                    Expect.isOk template $"Should be Ok for: '{templatePath}'"
        )

    member this.RunTestConvertibleTemplateFiles () = 
        let tests = 
            testList "TemplateConversion" 
                (
                    this.TestConvertibleTemplateFiles ()
                    |> List.ofArray
                )
        let result = Tests.runTestsWithCLIArgs [] [||] tests

        this.MatchResult(result)
        

    member this.ReadAllTemplates () =

        let directories = DirectoryInfo(this.TemplatesPath).GetDirectories()

        let filePaths =
            directories
            |> Array.collect(fun directory ->
                directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
            |> Array.map (fun file -> file.FullName)

        filePaths 
        |> Array.map (FsWorkbook.fromXlsxFile >> Spreadsheet.Template.fromFsWorkbook)


    /// Assumption: Similiarity is defined by headers
    member this.EnsureTemplateDiverse (template: Template, templates: Template [], ?threshhold) =
        let threshhold = defaultArg threshhold TemplateSimilarityThershold // Minimum difference
        let headers = template.Table.Headers |> Seq.map (fun h -> h.ToContent()) |> Set
        let allHeaders = templates |> Array.map (fun x -> x.Id, x.Name, x.Table.Headers |> Seq.map (fun h -> h.ToContent()) |> Set)
        allHeaders 
        |> Array.choose (fun (cId, cName, ct) ->
            if cId = template.Id then
                None
            else
                let diff = Set.difference headers ct
                let diversity = float (diff |> Set.count)/float headers.Count
                if diversity < threshhold then
                    Some (cId, cName, diversity)
                else
                    None
        )

    member this.TestForDiversity (template: Template, templates: Template [], ?threshhold) = 
        let threshhold = defaultArg threshhold TemplateSimilarityThershold // Minimum difference
        testCase $"Diversity-{template.Name}_{template.Version}_{template.Id}" <| fun _ ->
            let fileterdTemplates =
                templates
                |> Array.filter (fun item -> item.Id <> template.Id)
            let r = this.EnsureTemplateDiverse(template, fileterdTemplates, threshhold)
            let msg = 
                let sb = StringBuilder()
                sb.AppendLine(sprintf "## Found similiar templates for `%s` in:" template.Name) |> ignore
                for sId, sName, sScore in r do

                    let fullTemplate = templates |> Seq.find (fun x -> x.Id = sId)
                    let authors = 
                        fullTemplate.Authors
                        |> Array.ofSeq
                        |> Array.map (fun a -> 
                            let names = [|a.FirstName; a.MidInitials; a.LastName|] |> Array.map (fun n -> Option.defaultValue "" n)
                            sprintf "%s %s %s" names.[0] names.[1] names.[2]
                        ) 
                        |> String.concat ", "
                    sb.AppendLine (sprintf "- `%s` [%f] **%s** by (*%s*)" sName sScore (sId.ToString()) authors) |> ignore
                sb.ToString()
            Expect.hasLength r 0 msg

    member this.RunTestForDiversity(templates) =
        let tests = 
            testList "Ensure Template Diversity"
                (
                    templates
                    |> Array.map(fun template ->
                        this.TestForDiversity(template, templates))
                    |> List.ofArray
                )
        let result = Tests.runTestsWithCLIArgs [] [||] tests

        this.MatchResult(result)


    member this.DistinctTags(templates) =
        let ER_Tags = ARCtrl.Templates.getDistinctEndpointRepositories(templates) |> Array.ofSeq
        let Tags = ARCtrl.Templates.getDistinctTags(templates) |> Array.ofSeq

        // These are ER_Tags also used as Tags
        let ER_TagsAsTags = Tags |> Array.filter (fun tag -> ER_Tags |> Array.contains tag)

        if ER_TagsAsTags.Length > 0 then
            for tag in ER_TagsAsTags do
                let temps = ARCtrl.Templates.filterByOntologyAnnotation(ResizeArray[|tag|]) templates
                printfn "## Found ER_Tags as tag `%s` in:" tag.NameText
                for template in temps do 
                let authors = 
                    template.Authors
                    |> Array.ofSeq
                    |> Array.map (fun a -> 
                    let names = [|a.FirstName; a.MidInitials; a.LastName|] |> Array.map (fun n -> Option.defaultValue "" n)
                    sprintf "%s %s %s" names.[0] names.[1] names.[2]
                    ) 
                    |> String.concat ", "
                printfn "- **%s** by (*%s*)" (template.Name.Trim()) (authors.Trim())

        ER_TagsAsTags

    member this.TestDistinctTags(templates) =
        testList "ER_Tags and Tags Split" [
            testCase "No tags in both lists" <| fun _ ->
                let er_TagsAsTags = this.DistinctTags(templates)
                Expect.equal er_TagsAsTags.Length 0 "Found tags in both lists!"
        ]

    member this.RunTestDistinctTags(templates) =
        let tests = 
            testList "ER_Tags and Tags Split" [
                testCase "No tags in both lists" <| fun _ ->
                    let er_TagsAsTags = this.DistinctTags(templates)
                    Expect.equal er_TagsAsTags.Length 0 "Found tags in both lists!"
            ]
        let result = Tests.runTestsWithCLIArgs [] [||] tests

        this.MatchResult(result)

    member this.GetSimiliarTags(tag: ARCtrl.OntologyAnnotation, tags: ARCtrl.OntologyAnnotation [], ?similiarityThreshold) = 
        let similiarityThreshold = defaultArg similiarityThreshold TagSimiliarityThreshold
        let WhiteListMap =
            [|
                for set in TagWhiteList do
                    for item in set do
                    item, set
            |]
            |> fun x -> x
            |> Array.groupBy fst
            |> Array.map (fun (name,set) -> name, Array.collect snd set |> Set.ofArray)
            |> Map.ofArray
        let scores = SorensenDice.createDistanceScores tag.NameText (fun (t: ARCtrl.OntologyAnnotation) -> t.NameText) tags
        let scoresFiltered = 
            scores 
            |> Array.filter (fun (_, oa) -> oa.NameText <> tag.NameText)
            |> Array.filter (fun (s, _) -> s >= similiarityThreshold)
            |> Array.filter (fun (_, oa) -> // verify that the similiar tags are not on the white list
                let set = WhiteListMap |> Map.tryFind tag.NameText
                match set with
                | Some set -> set.Contains oa.NameText |> not
                | None -> true
            )
        if scoresFiltered.Length <> 0 then
            Some (tag, scoresFiltered)
        else
            None

    member this.TestTagForSimiliarity (tag: ARCtrl.OntologyAnnotation, tags: ARCtrl.OntologyAnnotation [], id: int, templates, ?similiarityThreshold) =
        let similiarityThreshold = defaultArg similiarityThreshold 0.8
        testCase $"Similiarity_{tag.NameText}_{id}" <| fun _ ->
            let similiarTags = this.GetSimiliarTags(tag, tags, similiarityThreshold)
            let msg = 
                if similiarTags.IsNone then "" else
                let tag, tags = similiarTags.Value
                let tags = tags |> Array.distinctBy snd
                let sb = System.Text.StringBuilder()
                sb.AppendLine(sprintf "## Found similiar tags for `%s` in:" tag.NameText) |> ignore
        
                for (score,tag) in tags do
                    let temps = ARCtrl.Templates.filterByOntologyAnnotation (ResizeArray [|tag|]) templates
                    for template in temps do 
                    let authors = 
                        template.Authors
                        |> Array.ofSeq
                        |> Array.map (fun a -> 
                        let names = [|a.FirstName; a.MidInitials; a.LastName|] |> Array.map (fun n -> Option.defaultValue "" n)
                        sprintf "%s %s %s" names.[0] names.[1] names.[2]
                        ) 
                        |> String.concat ", "
                    sb.AppendLine (sprintf "- `%s` [%f] **%s** by (*%s*)" tag.NameText score (template.Name.Trim()) (authors.Trim())) |> ignore
                sb.ToString()
            Expect.isNone similiarTags msg

    member this.TestTagForAmbiguous (name: string, tags: ARCtrl.OntologyAnnotation [], id: int, templates) =
        testCase $"Ambiguous_{name}_{id}" <| fun _ ->
            let msg =
                let sb = System.Text.StringBuilder()
                let temps = ARCtrl.Templates.filterByOntologyAnnotation (ResizeArray tags) templates
                sb.AppendLine(sprintf "## Found ambiguous tag `%s` in:" name) |> ignore
                for template in temps do 
                    let authors = 
                        template.Authors
                        |> Array.ofSeq
                        |> Array.map (fun a -> 
                        let names = [|a.FirstName; a.MidInitials; a.LastName|] |> Array.map (fun n -> Option.defaultValue "" n)
                        sprintf "%s %s %s" names.[0] names.[1] names.[2]
                        ) 
                        |> String.concat ", "
                    sb.AppendLine (sprintf "- **%s** by (*%s*)" (template.Name.Trim()) (authors.Trim())) |> ignore
                sb.ToString()
            Expect.hasLength tags 1 msg // This runs on distinct tags, which means we assume that after grouping by name we get exactly one tag per name.

    member this.RunTestAmbiguousAndSimilarityTag (distinctTags: OntologyAnnotation [], templates) =
        let tests = testList "Ensure Coherent Tags" [
            testList "Identical Names" [
                let mutable id = 0
                let groupedByNameTags = distinctTags |> Array.groupBy (fun oa -> oa.NameText)
                for (name, tags) in groupedByNameTags do
                    this.TestTagForAmbiguous(name, tags, id, templates)
                    id <- id + 1
                ]
            testList "Similiar Names" [
                let mutable id = 0
                let distinctByNamesTags = distinctTags |> Array.distinctBy (fun t -> t.NameText)
                for tag in distinctByNamesTags do
                    this.TestTagForSimiliarity(tag, distinctTags, id, templates)
                    id <- id + 1
                ]
        ]
        let result = Tests.runTestsWithCLIArgs [] [||] tests

        this.MatchResult(result)

    member this.TestCheckParentFolder(file: FileInfo, index) =

        let folderName = STRCIController.CleanFileNameFromInfo file

        testCase $"{folderName}_{index}" <| fun _ ->
            let parentDirectory = file.Directory
            let folderName = STRCIController.CleanFileNameFromInfo file
            Expect.equal (parentDirectory.Name.ToLower()) (folderName.ToLower()) $"Expected parent folder {folderName} but got {parentDirectory.Name}"

    member this.RunTestCheckParentFolder(templatePath) =
        let directories =
            DirectoryInfo(templatePath).GetDirectories()
            |> Array.filter (fun directory -> not (directory.Name.ToLower() = "test"))
        let fileInfos =
            directories
            |> Array.collect(fun directory ->
                directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
        let testSetup =
            fileInfos
            |> Array.mapi (fun i fileInfo -> this.TestCheckParentFolder(fileInfo, i))
            |> List.ofArray
        let tests = testList "Check template parent folders" (testSetup)
        let result = Tests.runTestsWithCLIArgs [] [||] tests

        this.MatchResult(result)

    member this.TestCheckFileNameVersioning(file: FileInfo, i) =
        testCase $"{file.Name}_{i}" <| fun _ ->
            let fileName = Path.GetFileNameWithoutExtension(file.Name)
            Expect.isTrue (Regex.IsMatch(fileName, this.VersionPattern)) $"The file {file.Name} contains no version information"

    member this.RunTestCheckFileNameVersioning(templatePath) =
        let directories =
            DirectoryInfo(templatePath).GetDirectories()
            |> Array.filter (fun directory -> not (directory.Name.ToLower() = "test"))
        let fileInfos =
            directories
            |> Array.collect(fun directory ->
                directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
        let testSetup =
            fileInfos
            |> Array.mapi (fun i fileInfo -> this.TestCheckFileNameVersioning(fileInfo, i))
            |> List.ofArray
        let tests = testList "Check template file name versioning" (testSetup)
        let result = Tests.runTestsWithCLIArgs [] [||] tests

        this.MatchResult(result)

    member this.CheckFileVersioning(file: FileInfo) =
        let regex = new Regex(this.VersionPattern)
        let potMatch = regex.Match(file.Name)
        let fileVersion = if System.String.IsNullOrWhiteSpace(potMatch.Value) then "" else potMatch.Value.Substring(2)
        let template = STRCIController.CreateTemplateFromXlsx file
        fileVersion = template.Version && not (System.String.IsNullOrWhiteSpace fileVersion)

    member this.TestCheckFileVersioning(file: FileInfo) =
        testCase $"{file.Name}" <| fun _ ->
            let rightVersion = this.CheckFileVersioning(file)
            Expect.isTrue rightVersion $"The file {file.Name} contains no version information"

     member this.RunTestCheckCheckFileVersioning(templatePath) =
        let directories =
            DirectoryInfo(templatePath).GetDirectories()
            |> Array.filter (fun directory -> not (directory.Name.ToLower() = "test"))
        let fileInfos =
            directories
            |> Array.collect(fun directory ->
                directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
        let testSetup =
            fileInfos
            |> Array.map (fun fileInfo -> this.TestCheckFileVersioning(fileInfo))
            |> List.ofArray
        let tests = testList "Check template file versioning" (testSetup)
        let result = Tests.runTestsWithCLIArgs [] [||] tests

        this.MatchResult(result)

    member this.TestAreAllDBTemplatesAvailable(dbTemplate: STRClient.SwateTemplate, localTemplates: Template []) =
        testCase $"DBEnsure_{dbTemplate.TemplateName}_{dbTemplate.TemplateId}_{dbTemplate.TemplateMajorVersion}.{dbTemplate.TemplateMinorVersion}.{dbTemplate.TemplatePatchVersion}" <| fun _ ->
            let dbVersion = SemVer.SemVer.create(dbTemplate.TemplateMajorVersion, dbTemplate.TemplateMinorVersion, dbTemplate.TemplatePatchVersion).AsString()
            let test = localTemplates |> Array.tryFind (fun localTemplate -> localTemplate.Id = dbTemplate.TemplateId && localTemplate.Version = dbVersion)
            Expect.isTrue (test.IsSome) $"The template {dbTemplate.TemplateName} with Id {dbTemplate.TemplateId} is locally not available"

    member this.RunAreAllDBTemplatesAvailable () =
        let dbTemplates = this.Client.GetAllTemplatesAsync().Result |> Array.ofSeq
        let localTemplates = this.ReadAllTemplates()
        let testSetup =
            dbTemplates
            |> Array.map (fun dbTemplate -> this.TestAreAllDBTemplatesAvailable(dbTemplate, localTemplates))
            |> List.ofArray
        let tests = testList "Check template file versioning" (testSetup)
        let result = Tests.runTestsWithCLIArgs [] [||] tests

        this.MatchResult(result)

    member this.RunAllTests() = 
        let localTemplates = this.ReadAllTemplates()
        let officialTemplates = this.ReadAllTemplates() |> Array.filter (fun template -> template.Organisation.IsOfficial())

        let distinctTags = this.DistinctTags(ResizeArray localTemplates)
        let directories =
            DirectoryInfo(this.TemplatesPath).GetDirectories()
            |> Array.filter (fun directory -> not (directory.Name.ToLower() = "test"))
        let fileInfos =
            directories
            |> Array.collect(fun directory ->
                directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
        let dbTemplates = this.Client.GetAllTemplatesAsync().Result |> Array.ofSeq

        let convertibleTests = this.TestConvertibleTemplateFiles()
        let diversityTests = 
            localTemplates
            |> Array.map (fun template -> this.TestForDiversity(template, officialTemplates))
        let distinctTests = this.TestDistinctTags(ResizeArray localTemplates)
        let ambiguousTests =
            let groupedByNameTags = distinctTags |> Array.groupBy (fun oa -> oa.NameText)
            groupedByNameTags
            |> Array.mapi (fun id (name, tags) -> this.TestTagForAmbiguous(name, tags, id, ResizeArray localTemplates))
        let similarityTests =
            let distinctByNamesTags = distinctTags |> Array.distinctBy (fun t -> t.NameText)
            distinctByNamesTags
            |> Array.mapi (fun id tag -> this.TestTagForSimiliarity(tag, distinctTags, id, ResizeArray localTemplates))
        let parentFolderTests =
            fileInfos
            |> Array.mapi (fun i fileInfo -> this.TestCheckParentFolder(fileInfo, i))
        let fileNameVersioningTests =
            fileInfos
            |> Array.mapi (fun i fileInfo -> this.TestCheckFileNameVersioning(fileInfo, i))
        let versioningTests =
            fileInfos
            |> Array.map (fun fileInfo -> this.TestCheckFileVersioning(fileInfo))
        let runAreAllDBTemplatesAvailableTests =
            dbTemplates
            |> Array.map (fun dbTemplate -> this.TestAreAllDBTemplatesAvailable(dbTemplate, localTemplates))

        let allTest =
            [|
                convertibleTests
                //diversityTests
                ambiguousTests
                similarityTests
                parentFolderTests
                fileNameVersioningTests
                versioningTests
                runAreAllDBTemplatesAvailableTests
            |]
            |> Array.concat
            |> List.ofArray
            |> (fun tests -> distinctTests :: tests)

        let tests = testList "All tests" (allTest)
        let result = Tests.runTestsWithCLIArgs [] [||] tests
        this.MatchResult(result)
