
#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/Newtonsoft.Json.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/DocumentFormat.OpenXml.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/FsSpreadsheet.dll"
#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/FsSpreadsheet.Net.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/System.IO.Packaging.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/STRIndex.dll"
#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/STRService.dll"

#r "nuget: Fable.Core"
#r "nuget: ARCtrl"
#r "nuget: FSharp.Data"

#load @"../STRApplication/STRCIController.fs"

open System
open System.IO
open System.Text.RegularExpressions

open ARCtrl

open FsSpreadsheet
open FsSpreadsheet.Net

open STRIndex
open STRCI
open STRService.Data
open STRService.Models

let url = "https://localhost:60539/api/v1"
let strciController = new STRCIController()

let solutionRoot = strciController.FindSolutionRoot (DirectoryInfo(System.Environment.CurrentDirectory))
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

//let directories = DirectoryInfo(templatesPath).GetDirectories()

//let fileInfos =
//    directories
//    |> Array.collect(fun directory ->
//        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

//fileInfos
//|> Array.map (fun item -> templateController.CreateDirectoryForTemplate item)

//let updatedNewDicetories = DirectoryInfo(templatesPath).GetDirectories()

//let newFileInfos =
//    updatedNewDicetories
//    |> Array.collect(fun directory ->
//        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

//newFileInfos
//|> Array.map (fun item -> templateController.UpdateFileName item)

//let newDirectories = DirectoryInfo(newTemplatesPath).GetDirectories()

//let newFileInfos =
//    newDirectories
//    |> Array.collect(fun directory ->
//        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

//newFileInfos
//|> Array.map (fun item -> templateController.UpdateFileName item)

//let updatedNewDicetories = DirectoryInfo(newTemplatesPath).GetDirectories()

//let updatedNewFileInfos =
//    updatedNewDicetories
//    |> Array.collect(fun directory ->
//        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

//updatedNewFileInfos
//|> Array.map (fun item -> templateController.CreateDirectoryForExternalTemplate item)

//let directories = DirectoryInfo(templatesPath).GetDirectories()

//let fileInfos =
//    directories
//    |> Array.collect(fun directory ->
//        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

//let templates =
//    fileInfos
//    |> Array.map (fun item -> templateController.CreateTemplateFromXlsx(item))

//let filteredTemplates =
//    templates
//    |> Array.groupBy (fun item -> item.Id, item.Version)
//    |> Array.filter (fun (_, item) -> item.Length > 1)
//    |> Array.collect (fun (_, item) -> item)
//    |> Array.map (fun item -> item.Id, item.Version, item.Name)

//filteredTemplates
//|> Array.iter (fun (key, version, name) -> printfn "id: %s, version: %s, name: %s" (key.ToString()) version name)
