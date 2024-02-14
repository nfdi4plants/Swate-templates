#r "nuget: ARCtrl.NET, 1.0.4"
#r "nuget: Expecto, 10.1.0"
#load "templateRead.fsx"

open System.IO
open FsSpreadsheet
open FsSpreadsheet.ExcelIO
open ARCtrl.Template
open ARCtrl.NET

open Expecto

let templates = TemplateRead.templates

let ER_Tags = ARCtrl.Template.Templates.getDistinctEndpointRepositories (templates)
let Tags = ARCtrl.Template.Templates.getDistinctTags (templates)
printfn "Found %i ER_Tags" ER_Tags.Length
printfn "Found %i Tags" Tags.Length

// These are ER_Tags also used as Tags
let ER_TagsAsTags = Tags |> Array.filter (fun tag -> ER_Tags |> Array.contains tag)

open Expecto    

let tests = testList "ER_Tags and Tags Split" [
    testCase "No tags in both lists" <| fun _ ->
      Expect.equal ER_TagsAsTags.Length 0 "Found tags in both lists!"
]

let result = Tests.runTestsWithCLIArgs [] [||] tests

for (tag) in ER_TagsAsTags do
  let temps = ARCtrl.Template.Templates.filterByOntologyAnnotation ([|tag|]) templates
  printfn "## Found ER_Tags as tag `%s` in:" tag.NameText
  for template in temps do 
    let authors = 
      template.Authors 
      |> Array.map (fun a -> 
        let names = [|a.FirstName; a.MidInitials; a.LastName|] |> Array.map (fun n -> Option.defaultValue "" n)
        sprintf "%s %s %s" names.[0] names.[1] names.[2]
      ) 
      |> String.concat ", "
    printfn "- **%s** by (*%s*)" (template.Name.Trim()) (authors.Trim())

match result with
| 0 -> 
  printfn "All checks successfull! ✅"
  System.Environment.ExitCode <- 0
| 1 -> 
  System.Environment.ExitCode <- 1
  printfn "Error! Tests failed!"
| 2 -> 
  System.Environment.ExitCode <- 2
  printfn "Error! Tests errored!"
| anyElse -> failwithf "Error! Unknown exit condition! %i" anyElse 