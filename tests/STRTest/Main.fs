module STRTest.Tests

open System
open System.IO

open Expecto

let testController = new TestController()

let allTests =
    let localTemplates = testController.ReadAllTemplates() |> Array.filter (fun template -> template.Organisation.IsOfficial())
    let distinctTags = testController.DistinctTags(ResizeArray localTemplates)
    let directories =
        DirectoryInfo(testController.TemplatesPath).GetDirectories()
        |> Array.filter (fun directory -> not (directory.Name.ToLower() = "test"))
    let fileInfos =
        directories
        |> Array.collect(fun directory ->
            directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
    let dbTemplates = testController.Client.GetAllTemplatesAsync().Result |> Array.ofSeq

    let convertibleTests = testController.TestConvertibleTemplateFiles()
    let diversityTests = 
        localTemplates
        |> Array.map (fun template -> testController.TestForDiversity(template, localTemplates))
    let distinctTests = testController.TestDistinctTags(ResizeArray localTemplates)
    let ambiguousTests =
        let groupedByNameTags = distinctTags |> Array.groupBy (fun oa -> oa.NameText)
        groupedByNameTags
        |> Array.mapi (fun id (name, tags) -> testController.TestTagForAmbiguous(name, tags, id, ResizeArray localTemplates))
    let similarityTests =
        let distinctByNamesTags = distinctTags |> Array.distinctBy (fun t -> t.NameText)
        distinctByNamesTags
        |> Array.mapi (fun id tag -> testController.TestTagForSimiliarity(tag, distinctTags, id, ResizeArray localTemplates))
    // let parentFolderTests =
    //     fileInfos
    //     |> Array.mapi (fun i fileInfo -> testController.TestCheckParentFolder(fileInfo, i))
    let runAreAllDBTemplatesAvailableTests =
        dbTemplates
        |> Array.map (fun dbTemplate -> testController.TestAreAllDBTemplatesAvailable(dbTemplate, localTemplates))

    let allTest =
        [|
            convertibleTests
            diversityTests
            ambiguousTests
            similarityTests
            // parentFolderTests
            runAreAllDBTemplatesAvailableTests
        |]
        |> Array.concat
        |> List.ofArray
        |> (fun tests -> distinctTests :: tests)

    testList "All tests" (allTest)

[<EntryPoint>]
let main argv = Tests.runTestsWithCLIArgs [] argv allTests
