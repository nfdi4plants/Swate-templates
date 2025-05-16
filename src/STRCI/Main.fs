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
    localTemplates
    |> Array.map (fun item ->
        let isTemplateInDB = STRCIController.IsTemplateInDB(item, dbTemplates)
        if not isTemplateInDB then
            let swateTemplate = STRCIController.CreateSwateClientTemplate(item)
            let metaData = STRCIController.CreateSwateClientMetadata(item)
            let swateTemplateDto = new STRClient.SwateTemplateDto()
            swateTemplateDto.Content <- swateTemplate
            swateTemplateDto.Metadata <- metaData
            let result = client.CreateTemplateAsync(swateTemplateDto)

            match result with
            | success when success.IsCompletedSuccessfully -> success.Result
            | failure when failure.IsFaulted -> raise failure.Exception
            | _  -> raise (Exception("Unexpected task state during template creation in db"))
        else
            null
    )

[<EntryPoint>]
let main argv =
    match argv |> Array.toList with
    | ["CreateTemplatesInDB"] ->
        createTemplatesInDB()
        0
    | _ ->
        printfn "Not the right Usage given"
        1
