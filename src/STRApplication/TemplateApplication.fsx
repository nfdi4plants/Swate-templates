
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

#load @"../STRApplication/TemplateController.fs"

open System
open System.IO
open System.Text.RegularExpressions

open ARCtrl

open FsSpreadsheet
open FsSpreadsheet.Net

open STRIndex
open STRApplication
open STRService.Data
open STRService.Models

let url = "https://localhost:60539/api/v1"
let templateController = new TemplateController("url")

let solutionRoot = templateController.FindSolutionRoot (DirectoryInfo(System.Environment.CurrentDirectory))
let templatesPath = Path.Combine(solutionRoot, "templates")

///Use these lines to create a local copy of the templates in the test folder
//let testTemplatesPath = Path.Combine(solutionRoot, "templates/test")
//let directories =
//    DirectoryInfo(templatesPath).GetDirectories()
//    |> Array.filter (fun directory -> not (directory.Name.ToLower() = "test"))
//directories
//|> Array.map (fun directory ->
//    templateController.CopyDirectory(directory.FullName, $"{testTemplatesPath}/{directory.Name}"))

let newDirectories = DirectoryInfo(templatesPath).GetDirectories()

let fileInfos =
    newDirectories
    |> Array.collect(fun directory ->
        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

fileInfos
|> Array.map (fun item -> templateController.CreateDirectoryForTemplate item)

let updatedNewDicetories = DirectoryInfo(templatesPath).GetDirectories()

let newFileInfos =
    updatedNewDicetories
    |> Array.collect(fun directory ->
        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

newFileInfos
|> Array.map (fun item -> templateController.UpdateFileName item)
