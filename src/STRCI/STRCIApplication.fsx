
#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/Newtonsoft.Json.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/DocumentFormat.OpenXml.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/FsSpreadsheet.dll"
#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/FsSpreadsheet.Net.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/System.IO.Packaging.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/STRIndex.dll"
#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/STRService.dll"
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
open STRService.Data
open STRService.Models

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

fileInfos
|> Array.map (fun item -> STRCIController.CreateDirectoryForTemplate item)

let updatedNewDicetories = DirectoryInfo(templatesPath).GetDirectories()

let newFileInfos =
    updatedNewDicetories
    |> Array.collect(fun directory ->
        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

newFileInfos
|> Array.map (fun item -> STRCIController.UpdateFileName item)

//let newDirectories = DirectoryInfo(newTemplatesPath).GetDirectories()

//let newFileInfos =
//    newDirectories
//    |> Array.collect(fun directory ->
//        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

//newFileInfos
//|> Array.map (fun item -> STRCIController.UpdateFileName item)

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

//let client = STRCIController.Client("TestToken")

////Set client baseURL for testing
////client.BaseUrl <- (createTestURL 62835)
//printfn "client.BaseUrl: %s" client.BaseUrl

//let dbTemplates = client.GetAllTemplatesAsync().Result |> Array.ofSeq
//printfn "templates: %i" (Array.length dbTemplates)

//let testGuid = Guid.NewGuid()
//let arcTable = ArcTable.create("TestPlapla", ResizeArray [||], dictionary)
//let emptyOntologyAnnotations = new List<STRIndex.Domain.OntologyAnnotation>() :> ICollection<STRIndex.Domain.OntologyAnnotation>
//let clientEmptyOntologyAnnotations = new List<STRClient.OntologyAnnotation>() :> ICollection<STRClient.OntologyAnnotation>
//let emptyAuthors = new List<STRIndex.Domain.Author>() :> ICollection<STRIndex.Domain.Author>
//let clientEmptyAuthors = new List<STRClient.Author>() :> ICollection<STRClient.Author>
//let templateContent = Template.create(testGuid, arcTable, "Test_Plapla", "Description of Plapla").toJsonString()

//let swateTemplate = SwateTemplate.Create(testGuid, "Test_Plapla", 0, 0, 1, "Suffix", "MoreSuffix", templateContent)
//let swateTemplateMetadata = SwateTemplateMetadata.Create(testGuid, "Test_Plapla", "Description of Plapla", 0, 0, 1, "Suffix", "MoreSuffix", "RPTU", emptyOntologyAnnotations, DateOnly(), emptyOntologyAnnotations, emptyAuthors)

//let clientTemplate = 
//    let x = new STRClient.SwateTemplate()
//    x.TemplateId <- swateTemplate.TemplateId
//    x.TemplateName <- swateTemplate.TemplateName
//    x.TemplateMajorVersion <- swateTemplate.TemplateMajorVersion
//    x.TemplateMinorVersion <- swateTemplate.TemplateMinorVersion
//    x.TemplatePatchVersion <- swateTemplate.TemplatePatchVersion
//    x.TemplatePreReleaseVersionSuffix <- swateTemplate.TemplatePreReleaseVersionSuffix
//    x.TemplateBuildMetadataVersionSuffix <- swateTemplate.TemplateBuildMetadataVersionSuffix
//    x.TemplateContent <- swateTemplate.TemplateContent
//    x

//let clientMetadata = 
//    let x = new STRClient.SwateTemplateMetadata()
//    x.Id <- swateTemplateMetadata.Id
//    x.Name <- swateTemplateMetadata.Name
//    x.Description <- swateTemplateMetadata.Description
//    x.MajorVersion <- swateTemplateMetadata.MajorVersion
//    x.MinorVersion <- swateTemplateMetadata.MinorVersion
//    x.PatchVersion <- swateTemplateMetadata.PatchVersion
//    x.PreReleaseVersionSuffix <- swateTemplateMetadata.PreReleaseVersionSuffix
//    x.BuildMetadataVersionSuffix <- swateTemplateMetadata.BuildMetadataVersionSuffix
//    x.Organisation <- swateTemplateMetadata.BuildMetadataVersionSuffix
//    x.EndpointRepositories <- clientEmptyOntologyAnnotations
//    x.ReleaseDate <- DateTimeOffset()
//    x.Tags <- clientEmptyOntologyAnnotations
//    x.Authors <- clientEmptyAuthors
//    x

//let swateTemplateDto = SwateTemplateDto.Create(swateTemplate, swateTemplateMetadata)

//let swateClientDto = 
//    let x = new STRClient.SwateTemplateDto()
//    x.Content <- clientTemplate
//    x.Metadata <- clientMetadata
//    x

//let result = client.CreateTemplateAsync(swateClientDto).Result

//let newTemplates = client.GetAllTemplatesAsync().Result |> Array.ofSeq
//printfn "newTemplates: %i" (Array.length newTemplates)

//let directories = DirectoryInfo(templatesPath).GetDirectories()

//let fileInfos =
//    directories
//    |> Array.collect(fun directory ->
//        directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

//let localTemplates =
//    fileInfos
//    |> Array.map (fun item -> STRCIController.CreateTemplateFromXlsx(item))

//let result =
//    localTemplates
//    |> Array.map (fun item -> STRCIController.IsTemplateInDB(item, dbTemplates))

//result
//|> Array.iter (fun item -> printfn "Template exists in db: %s" (item.ToString()))

//STRCIController.TemplatesToJsonV2()
