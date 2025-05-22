
#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/Newtonsoft.Json.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/DocumentFormat.OpenXml.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/FsSpreadsheet.dll"
#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/FsSpreadsheet.Net.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/System.IO.Packaging.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/STRIndex.dll"
#r @"C:/Users/Patri/source/repos/Swate-templates/src/STRCI/bin/Debug/net8.0/STRClient.dll"

#r "nuget: Fable.Core"
#r "nuget: ARCtrl"
#r "nuget: FSharp.Data"

#load @"../STRCI/STRCIController.fs"

open System
open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions

open ARCtrl
open ARCtrl.Json

open FsSpreadsheet
open FsSpreadsheet.Net

open STRIndex
open STRCI
open STRClient

let createTestURL (port: int) = $"https://localhost:{port}/"

let solutionRoot = STRCIController.FindSolutionRoot (DirectoryInfo(System.Environment.CurrentDirectory))
let templatesPath = Path.Combine(solutionRoot, "templates")
let newTemplatesPath = "C:/Users/Patri/Downloads/templates"

///Use these lines to create a local copy of the templates in the test folder
//let testTemplatesPath = Path.Combine(solutionRoot, "templates/test")
//let directories =
//    DirectoryInfo(templatesPath).GetDirectories()
//    |> Array.filter (fun directory -> not (directory.Name.ToLower() = "test"))
//directories
//|> Array.map (fun directory ->
//    templateController.CopyDirectory(directory.FullName, $"{testTemplatesPath}/{directory.Name}"))

let directories = DirectoryInfo(templatesPath).GetDirectories()

let fileInfos =
    directories
    |> Array.collect(fun directory ->
        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

//fileInfos
//|> Array.map (fun item -> STRCIController.CreateDirectoryForTemplate item)

//let updatedNewDicetories = DirectoryInfo(templatesPath).GetDirectories()

//let newFileInfos =
//    updatedNewDicetories
//    |> Array.collect(fun directory ->
//        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

//newFileInfos
//|> Array.map (fun item -> STRCIController.UpdateFileName item)

//let client = STRCIController.Client("token")

////Set client baseURL for testing
//client.BaseUrl <- (createTestURL 50794)

//printfn "client.BaseUrl: %s" client.BaseUrl

//let localTemplates =
//    fileInfos
//    |> Array.map (fun fileInfo -> STRCIController.CreateTemplateFromXlsx(fileInfo))

//let dbTemplates = client.GetAllTemplatesAsync().Result |> Array.ofSeq

//let createTemplatesInDB () =
//    async  {
//        for item in localTemplates do
//            if not (STRCIController.IsTemplateInDB(item, dbTemplates)) then
//                let swateTemplate = STRCIController.CreateSwateClientTemplate(item)
//                let metaData = STRCIController.CreateSwateClientMetadata(item)
//                let swateTemplateDto = STRClient.SwateTemplateDto()
//                swateTemplateDto.Content <- swateTemplate
//                swateTemplateDto.Metadata <- metaData

//                try
//                    let! _ = client.CreateTemplateAsync(swateTemplateDto) |> Async.AwaitTask
//                    printfn "created template: %s" swateTemplate.TemplateName
//                with ex ->
//                    return raise (Exception("Error during template creation in DB", ex))
//    }

//createTemplatesInDB()
//|> Async.RunSynchronously
