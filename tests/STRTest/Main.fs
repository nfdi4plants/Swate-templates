module STRTest.Tests

open System
open System.IO

open Expecto

// let testController = new TestController()

// let allTests2 =
//     let localTemplates = testController.ReadAllTemplates() |> Array.filter (fun template -> template.Organisation.IsOfficial())
//     let distinctTags = testController.DistinctTags(ResizeArray localTemplates)
//     let directories =
//         DirectoryInfo(testController.TemplatesPath).GetDirectories()
//         |> Array.filter (fun directory -> not (directory.Name.ToLower() = "test"))
//     let fileInfos =
//         directories
//         |> Array.collect(fun directory ->
//             directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
//     let dbTemplates = testController.Client.GetAllTemplatesAsync().Result |> Array.ofSeq

//     let convertibleTests = testController.TestConvertibleTemplateFiles() |> testList "Template Parsing"
//     let templateDiversityTests =
//         localTemplates
//         |> Array.map (fun template -> testController.TestForTemplateDiversity(template, localTemplates))
//     let distinctTagsTests = testController.TestDistinctTags(ResizeArray localTemplates)
//     let ambiguousTagsTests =
//         let groupedByNameTags = distinctTags |> Array.groupBy (fun oa -> oa.NameText)
//         groupedByNameTags
//         |> List.ofArray
//         |> List.mapi (fun id (name, tags) -> testController.TestTagForAmbiguous(name, tags, id, ResizeArray localTemplates))
//         |> testList "Ambiguous tags tests"
//     let similarityTagsTests =
//         let distinctByNamesTags = distinctTags |> Array.distinctBy (fun t -> t.NameText)
//         distinctByNamesTags
//         |> List.ofArray
//         |> List.mapi (fun id tag -> testController.TestTagForSimiliarity(tag, distinctTags, id, ResizeArray localTemplates))
//         |> testList "Similarity tags tests"
//     // let parentFolderTests =
//     //     fileInfos
//     //     |> Array.mapi (fun i fileInfo -> testController.TestCheckParentFolder(fileInfo, i))
//     let runAreAllDBTemplatesAvailableTests =
//         dbTemplates
//         |> List.ofArray
//         |> List.map (fun dbTemplate -> testController.TestAreAllDBTemplatesAvailable(dbTemplate, localTemplates))
//         |> testList "Are all DB templates available tests"

//     let allTest =
//         [
//             convertibleTests
//             // templateDiversityTests
//             ambiguousTagsTests
//             similarityTagsTests
//             // parentFolderTests
//             runAreAllDBTemplatesAvailableTests
//             distinctTagsTests
//         ]

//     testList "All tests" (allTest)

let allTests = testList "all" [
    TemplatesParse.Main
    TagCategory.Main
    TagSimilarity.Main
    TagAmbiguity.Main
    FileNameVersion.Main
    //TemplateAvailability.Main
]

[<EntryPoint>]
let main argv = Tests.runTestsWithCLIArgs [] argv allTests
