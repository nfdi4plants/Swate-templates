module STRTest.TagSimilarity

open Expecto
open ARCtrl

module TagSimilarityHelper =

    let tryGetSimiliarTags (tag: ARCtrl.OntologyAnnotation) (tags: ARCtrl.OntologyAnnotation []) =
        let scores = Util.SorensenDice.createDistanceScores tag.NameText (fun (t: ARCtrl.OntologyAnnotation) -> t.NameText) tags
        let scoresFiltered =
            scores
            |> Array.filter (fun (_,oa) -> oa.NameText <> tag.NameText)
            |> Array.filter (fun (s,oa) -> s >= Constants.TagCoherence.TagSimiliarityThreshold)
            |> Array.filter (fun (_,oa) -> // verify that the similiar tags are not on the white list
                let set = Constants.TagCoherence.WhiteListMap |> Map.tryFind tag.NameText
                match set with
                | Some set -> set.Contains oa.NameText |> not
                | None -> true
            )
        if scoresFiltered.Length <> 0 then
            Some (tag, scoresFiltered)
        else
            None


open TagSimilarityHelper

let private mkTestForSimilarity (tag: ARCtrl.OntologyAnnotation) (tags: ARCtrl.OntologyAnnotation []) (templates: Template []) =
    testCase $"{tag.NameText}_{tag.GetHashCode()}" <| fun _ ->
        let similiarTags = tryGetSimiliarTags tag tags
        let msg = // create calculation heavy message only on fail case
            if similiarTags.IsNone then
                ""
            else
                let tag, tags = similiarTags.Value
                let tags = tags |> Array.distinctBy snd
                let sb = System.Text.StringBuilder()
                sb.AppendLine(sprintf "## Found similiar tags for `%s` in:" tag.NameText) |> ignore

                for (score,tag) in tags do
                    let offendingTemplates = ARCtrl.Templates.filterByOntologyAnnotation (ResizeArray [|tag|]) (ResizeArray templates)
                    for template in offendingTemplates do
                        let authors =
                            template.Authors
                            |> Seq.map (fun a ->
                                [|a.FirstName; a.MidInitials; a.LastName|]
                                |> Array.choose (fun n -> n)
                                |> String.concat " "
                            )
                            |> String.concat ", "
                        sb.AppendLine (sprintf "- `%s` [%f] **%s** by (*%s*)" tag.NameText score (template.Name.Trim()) (authors.Trim())) |> ignore
                sb.ToString()
        Expect.isNone similiarTags msg


/// Ensure that all tags are distinct and do not have similar names
/// This is used to avoid "Protein", "protein", "Proteins", etc.
let Main = testList "Tag Similarity" [
    let templates = TestData.getLatestTemplates()
    let latestDistinctTags = TestData.getLatestDistinctTags()
    for tag in latestDistinctTags do
        mkTestForSimilarity tag latestDistinctTags templates

]
