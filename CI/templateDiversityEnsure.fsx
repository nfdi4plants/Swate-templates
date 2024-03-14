#r "nuget: ARCtrl.NET, 1.0.4"
#r "nuget: Expecto, 10.1.0"
#load "templateRead.fsx"

open System.IO
open FsSpreadsheet
open FsSpreadsheet.ExcelIO
open ARCtrl.Template
open ARCtrl.NET
open ARCtrl.ISA
open Microsoft.FSharp.Reflection
open System.Text
open Expecto

let threshhold = 0.3 // Minimum 30% difference

let templates = TemplateRead.templates

type CompositeHeader with
  member this.ToContent() =
    match Microsoft.FSharp.Reflection.FSharpValue.GetUnionFields(this, typeof<CompositeHeader>) with
    | case, more -> [|
        case.Name; 
        for o in more do 
          match o with
          | :? OntologyAnnotation as oa -> yield! [|oa.NameText; Option.defaultValue "" oa.TermSourceREF ; oa.TermAccessionShort|] 
          | :? IOType as io -> io.ToString()
          | anyElse -> string anyElse
      |]

/// Assumption: Similiarity is defined by headers
let ensureTemplateDiverse (template:Template) (all: Template []) =
  let headers = template.Table.Headers |> Seq.map (fun h -> h.ToContent()) |> Set
  let allHeaders = all |> Array.map (fun x -> x.Id, x.Name, x.Table.Headers |> Seq.map (fun h -> h.ToContent()) |> Set)
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

let testForDiversity (template:Template) (templates:Template []) = 
  testCase $"{template.Name}_{template.Id}" <| fun _ ->
    let r = ensureTemplateDiverse template templates
    let msg = 
      let sb = StringBuilder()
      sb.AppendLine(sprintf "## Found similiar templates for `%s` in:" template.Name) |> ignore
      for sId, sName, sScore in r do
        let fullTemplate = templates |> Seq.find (fun x -> x.Id = sId)
        let authors = 
              fullTemplate.Authors 
              |> Array.map (fun a -> 
                let names = [|a.FirstName; a.MidInitials; a.LastName|] |> Array.map (fun n -> Option.defaultValue "" n)
                sprintf "%s %s %s" names.[0] names.[1] names.[2]
              ) 
              |> String.concat ", "
        sb.AppendLine (sprintf "- `%s` [%f] **%s** by (*%s*)" sName sScore (sId.ToString()) authors) |> ignore
      sb.ToString()
    Expect.hasLength r 0 msg

let tests = testList "Ensure Template Diversity" [
  for template in templates do
    testForDiversity template templates
]

let result = Tests.runTestsWithCLIArgs [] [||] tests

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