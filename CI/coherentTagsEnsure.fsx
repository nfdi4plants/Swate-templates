#r "nuget: ARCtrl.NET, 1.0.4"
#r "nuget: Expecto, 10.1.0"
#load "templateRead.fsx"

open System.IO
open FsSpreadsheet
open FsSpreadsheet.ExcelIO
open ARCtrl.Template
open ARCtrl.NET

open Expecto

let SimiliarityThreshold = 0.8
let WhiteList : string [] [] = 
  [| 
    [|"RNASeq"; "mRNASeq"; "DNASeq"|]
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

let WhiteListMap =
  [|
    for set in WhiteList do
      for item in set do
        item, set
  |]
  |> fun x -> x
  |> Array.groupBy fst
  |> Array.map (fun (name,set) -> name, Array.collect snd set |> Set.ofArray)
  |> Map.ofArray

let getSimiliarTags (tag: ARCtrl.ISA.OntologyAnnotation) (tags: ARCtrl.ISA.OntologyAnnotation []) = 
  let scores = SorensenDice.createDistanceScores tag.NameText (fun (t: ARCtrl.ISA.OntologyAnnotation) -> t.NameText) tags
  let scoresFiltered = 
    scores 
    |> Array.filter (fun (_,oa) -> oa.NameText <> tag.NameText)
    |> Array.filter (fun (s,oa) -> s >= SimiliarityThreshold)
    |> Array.filter (fun (_,oa) -> // verify that the similiar tags are not on the white list
      let set = WhiteListMap |> Map.tryFind tag.NameText
      match set with
      | Some set -> set.Contains oa.NameText |> not
      | None -> true
    )
  if scoresFiltered.Length <> 0 then
    Some (tag, scoresFiltered)
  else
    None

let templates = TemplateRead.templates

let distinctTags = ARCtrl.Template.Templates.getDistinctOntologyAnnotations (templates)
printfn "Distinct Tags %i" distinctTags.Length

let testTagForSimiliarity (tag: ARCtrl.ISA.OntologyAnnotation) (tags: ARCtrl.ISA.OntologyAnnotation []) (id:int) =
  testCase $"{tag.NameText}_{id}" <| fun _ ->
    let similiarTags = getSimiliarTags tag tags
    let msg = 
      if similiarTags.IsNone then "" else
        let tag, tags = similiarTags.Value
        let tags = tags |> Array.distinctBy snd
        let sb = System.Text.StringBuilder()
        sb.AppendLine(sprintf "## Found similiar tags for `%s` in:" tag.NameText) |> ignore
        
        for (score,tag) in tags do
          let temps = ARCtrl.Template.Templates.filterByOntologyAnnotation ([|tag|]) templates
          for template in temps do 
            let authors = 
              template.Authors 
              |> Array.map (fun a -> 
                let names = [|a.FirstName; a.MidInitials; a.LastName|] |> Array.map (fun n -> Option.defaultValue "" n)
                sprintf "%s %s %s" names.[0] names.[1] names.[2]
              ) 
              |> String.concat ", "
            sb.AppendLine (sprintf "- `%s` [%f] **%s** by (*%s*)" tag.NameText score (template.Name.Trim()) (authors.Trim())) |> ignore
        sb.ToString()
    Expect.isNone similiarTags msg

Tests.runTestsWithCLIArgs [] [||] (testTagForSimiliarity distinctTags.[1] distinctTags 0)

let testTagForAmbiguous (name: string) (tags: ARCtrl.ISA.OntologyAnnotation []) (id:int) =
  testCase $"{name}_{id}" <| fun _ ->
    let msg =
      let sb = System.Text.StringBuilder()
      let temps = ARCtrl.Template.Templates.filterByOntologyAnnotation (tags) templates
      sb.AppendLine(sprintf "## Found ambiguous tag `%s` in:" name) |> ignore
      for template in temps do 
        let authors = 
          template.Authors 
          |> Array.map (fun a -> 
            let names = [|a.FirstName; a.MidInitials; a.LastName|] |> Array.map (fun n -> Option.defaultValue "" n)
            sprintf "%s %s %s" names.[0] names.[1] names.[2]
          ) 
          |> String.concat ", "
        sb.AppendLine (sprintf "- **%s** by (*%s*)" (template.Name.Trim()) (authors.Trim())) |> ignore
      sb.ToString()
    Expect.hasLength tags 1 msg // This runs on distinct tags, which means we assume that after grouping by name we get exactly one tag per name.

let tests = testList "Ensure Coherent Tags" [
  testList "Identical Names" [
    let mutable id = 0
    let groupedByNameTags = distinctTags |> Array.groupBy (fun oa -> oa.NameText)
    for (name, tags) in groupedByNameTags do
      testTagForAmbiguous name tags id
      id <- id + 1
  ]
  testList "Similiar Names" [
    let mutable id = 0
    let distinctByNamesTags = distinctTags |> Array.distinctBy (fun t -> t.NameText)
    for tag in distinctByNamesTags do
      testTagForSimiliarity tag distinctTags id
      id <- id + 1
  ]
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