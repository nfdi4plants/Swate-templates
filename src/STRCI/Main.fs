module STRCI.Tests

open System
open System.IO
open System.Diagnostics

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
    |> Array.iter (fun item -> 
        let isTemplateInDB = STRCIController.IsTemplateInDB(item, dbTemplates)
        if not isTemplateInDB then
            let swateTemplate = STRCIController.CreateSwateClientTemplate(item)
            let metaData = STRCIController.CreateSwateClientMetadata(item)
            let swateTemplateDto = new STRClient.SwateTemplateDto()
            swateTemplateDto.Content <- swateTemplate
            swateTemplateDto.Metadata <- metaData
            client.CreateTemplateAsync(swateTemplateDto).Result |> ignore
    )

[<EntryPoint>]
let main argv = 
    match argv |> Array.toList with
    | ["Release_1.0.0"] ->
        //let path = @"../STRCI/templates-to-json_v1.0.0.fsx"
        let relativePath = "../src/STRCI/Test.fsx"
        let fullPath = Path.GetFullPath(relativePath, Directory.GetCurrentDirectory())

        let fullPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Test.fsx")

        //System.Environment.CurrentDirectory 

        if not (File.Exists(fullPath)) then
            failwithf "Script file not found: %s" fullPath

        printfn "Running script at: %s" fullPath

        let psi = ProcessStartInfo("dotnet", $"fsi {fullPath}")
        psi.RedirectStandardOutput <- true
        psi.UseShellExecute <- false
        psi.WorkingDirectory <- Directory.GetCurrentDirectory()

        let proc = Process.Start(psi)
        let output = proc.StandardOutput.ReadToEnd()
        proc.WaitForExit()
        0
    //| ["Release_2.0.0"] ->
    //    STRCIController.TemplatesToJsonV2()
    //    1
    //| ["CreateTemplatesInDB"] ->
    //    createTemplatesInDB()
    //    2
    | _ ->
        printfn "Not the right Usage given"
        3
