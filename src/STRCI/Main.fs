module STRCI.Tests

open System
open System.IO

open STRCI

let token =
    match Environment.GetEnvironmentVariable("STR_PAT") with
    | null | "" -> failwith "STR_PAT environment variable is not set!"
    | t -> t

let client = STRCIController.Client(token)

let allDbTemplates = client.GetAllTemplateMetadataAsync().Result

let solutionRoot = STRCIController.FindSolutionRoot (DirectoryInfo(System.Environment.CurrentDirectory))
let templatesPath = Path.Combine(solutionRoot, "templates")

let directories = DirectoryInfo(templatesPath).GetDirectories()

let fileInfos =
    directories
    |> Array.collect(fun directory ->
        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

let localTemplates =
    fileInfos
    |> Array.map (fun fileInfo -> STRCIController.CreateTemplateFromXlsx(fileInfo))

let dbTemplates = client.GetAllTemplatesAsync().Result |> Array.ofSeq

let createTemplatesInDB () =
    async  {
        for item in localTemplates do
            if not (STRCIController.IsTemplateInDB(item, dbTemplates)) then
                let swateTemplate = STRCIController.CreateSwateClientTemplate(item)
                let metaData = STRCIController.CreateSwateClientMetadata(item)
                let swateTemplateDto = new STRClient.SwateTemplateDto()
                swateTemplateDto.Content <- swateTemplate
                swateTemplateDto.Metadata <- metaData

                try
                    let! _ = client.CreateTemplateAsync(swateTemplateDto) |> Async.AwaitTask
                    printfn "created template: %s" swateTemplate.TemplateName
                with ex ->
                    return raise (Exception("Error during template creation in DB", ex))
    }

[<EntryPoint>]
let main argv =
    match argv |> Array.toList with
    | ["Release_2.0.0"] ->
        STRCIController.TemplatesToJsonArtifact()
        0
    | ["CreateTemplatesInDB"] ->
        createTemplatesInDB()
        |> Async.RunSynchronously
        0
    | _ ->
        printfn "Not the right Usage given"
        1
