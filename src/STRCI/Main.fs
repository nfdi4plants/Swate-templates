module STRCI.Tests

open System
open System.IO
open STRCI


let token =
    match Environment.GetEnvironmentVariable("STRC_PAT") with
    | null | "" -> failwith "STRC_PAT environment variable is not set!"
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

[<EntryPoint>]
let main argv = 
    localTemplates
    |> Array.iter (fun item -> 
        let isTemplateInDB = STRCIController.IsTemplateInDB(item, dbTemplates)
        if not isTemplateInDB then
            let swateTemplate = STRCIController.createSwateClientTemplate(item)
            let metaData = STRCIController.createSwateClientMetadata(item)
            let swateTemplateDto = new STRClient.SwateTemplateDto()
            swateTemplateDto.Content <- swateTemplate
            swateTemplateDto.Metadata <- metaData
            client.CreateTemplateAsync(swateTemplateDto).Result |> ignore
    )

    0
