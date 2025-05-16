module STRTest.TagAmbiguity

open Expecto


let private mkTestForAmbiguity (name: string) (tags: ARCtrl.OntologyAnnotation []) (templates: ARCtrl.Template []) =
  testCase $"{name}" <| fun _ ->
    let msg =
      let sb = System.Text.StringBuilder()
      let temps = ARCtrl.Templates.filterByOntologyAnnotation (ResizeArray tags) (ResizeArray templates)
      sb.AppendLine(sprintf "## Found ambiguous tag `%s` in:" name) |> ignore
      for template in temps do
        let authors =
          template.Authors
          |> Seq.map (fun a ->
            [|a.FirstName; a.MidInitials; a.LastName|]
            |> Array.choose id
            |> String.concat " "
          )
          |> String.concat ", "
        sb.AppendLine (sprintf "- **%s** by (*%s*)" (template.Name.Trim()) (authors.Trim())) |> ignore
      sb.ToString()
    Expect.hasLength tags 1 msg


/// <summary>
/// Ensure that all tags with the same name have the same ontology
/// This is used to avoid:
///
/// - `Protein` `None` `None`
/// - `Protein` `BSP` `BSP:0001`
///
/// ```
/// </summary>
let Main = testList "Tag Ambiguity" [

    let templates = TestData.getLatestTemplates()
    let latestDistinctTags = TestData.getLatestDistinctTags()
    let groupedAndDistinctByNameTags =
        latestDistinctTags
        |> Array.groupBy (fun t -> t.NameText)
        |> Array.distinctBy fst

    for (name, tags) in groupedAndDistinctByNameTags do
        mkTestForAmbiguity name tags templates
]
