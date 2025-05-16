module STRTest.TestData

let TemplateResults = ReadTemplates.tryReadTemplates()

let getTemplates() =
    TemplateResults
    |> Array.choose (fun (_, result) ->
        match result with
        | Error _ -> None
        | Ok template -> Some (template)
    )

let getLatestTemplates() =
    getTemplates()
    |> Array.groupBy (fun t -> t.Id)
    |> Array.map (fun (_, templates) ->
        templates
        |> Array.sortBy (fun t -> t.Version)
        |> Array.last
    )

let getLatestDistinctTags() =
    getLatestTemplates()
    |> ARCtrl.Templates.getDistinctOntologyAnnotations
