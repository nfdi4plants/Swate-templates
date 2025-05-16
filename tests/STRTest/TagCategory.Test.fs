module STRTest.TagCategory

open Expecto

let Main =
    let templates = STRTest.TestData.getLatestTemplates()

    let ER_Tags = ARCtrl.Templates.getDistinctEndpointRepositories(ResizeArray templates) |> Array.ofSeq
    let Tags = ARCtrl.Templates.getDistinctTags(ResizeArray templates) |> Array.ofSeq

    testList "Tag Category" [

        for erTag in ER_Tags do
            testCase (sprintf "ER_Tag `%s`" erTag.NameText) <| fun _ ->
                let isAlsoTag = Tags |> Array.contains erTag
                match isAlsoTag with
                | false ->
                    Expect.isFalse isAlsoTag "This can never hit -- Redundant code is used to improve error message in 'Error' case"
                | true -> // This is a bit redundant, but I decided to use this syntax to improve error message
                    let temps = ARCtrl.Templates.filterByOntologyAnnotation(ResizeArray[|erTag|]) (ResizeArray templates)
                    printfn "## Found ER_Tags as tag `%s` in:" erTag.NameText
                    let msg =
                        temps
                        |> Seq.map (fun template ->
                            let authors =
                                template.Authors
                                |> Array.ofSeq
                                |> Array.map (fun a ->
                                let names = [|a.FirstName; a.MidInitials; a.LastName|] |> Array.map (fun n -> Option.defaultValue "" n)
                                sprintf "%s %s %s" names.[0] names.[1] names.[2]
                                )
                                |> String.concat ", "
                            sprintf "- **%s** by (*%s*)" (template.Name.Trim()) (authors.Trim())
                        )
                        |> String.concat "\n"
                    Expect.isFalse true msg
    ]
