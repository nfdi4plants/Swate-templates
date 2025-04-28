namespace STRApplication

open System.IO
open System.Text.RegularExpressions

open ARCtrl

open FsSpreadsheet
open FsSpreadsheet.Net

open Newtonsoft.Json

open STRService.Models

open FSharp.Data

type TemplateController (baseUrl: string) =        

    member this.BaseUrl = baseUrl //"https://str.nfdi4plants.org/api/v1"

    member this.GetAllMetadata () : SwateTemplateMetadata [] =
        let url = $"{this.BaseUrl}/metadata"
        let response =
            Http.Request(
                url,
                httpMethod = "GET",
                headers = [
                    "Content-Type", "application/json"
                ]
            )
        let result = 
            match response.Body with
            | Text bodyText ->
                JsonConvert.DeserializeObject<SwateTemplateMetadata []>(bodyText)
            | Binary _ ->
                failwith "Expected text response but got binary"
        result

    member this.GetMetadataByName (name: string) : SwateTemplateMetadata =
        let url = $"{this.BaseUrl}/metadata/{name}"
        let response =
            Http.Request(
                url,
                httpMethod = "GET",
                headers = [
                    "Content-Type", "application/json"
                ]
            )
        let result = 
            match response.Body with
            | Text bodyText ->
                JsonConvert.DeserializeObject<SwateTemplateMetadata>(bodyText)
            | Binary _ ->
                failwith "Expected text response but got binary"
        result

    member this.GetMetadataByKeys (name: string, version: string) : SwateTemplateMetadata =
        let url = $"{this.BaseUrl}/metadata/{name}/{version}"
        let response =
            Http.Request(
                url,
                httpMethod = "GET",
                headers = [
                    "Content-Type", "application/json"
                ]
            )
        let result = 
            match response.Body with
            | Text bodyText ->
                JsonConvert.DeserializeObject<SwateTemplateMetadata>(bodyText)
            | Binary _ ->
                failwith "Expected text response but got binary"
        result

    member this.GetAllTemplates () : SwateTemplate [] =
        let url = $"{this.BaseUrl}/templates"
        let response =
            Http.Request(
                url,
                httpMethod = "GET",
                headers = [
                    "Content-Type", "application/json"
                ]
            )
        let result = 
            match response.Body with
            | Text bodyText ->
                JsonConvert.DeserializeObject<SwateTemplate []>(bodyText)
            | Binary _ ->
                failwith "Expected text response but got binary"
        result

    member this.GetTemplateByName (name: string) : SwateTemplate =
        let url = $"{this.BaseUrl}/templates/{name}"
        let response =
            Http.Request(
                url,
                httpMethod = "GET",
                headers = [
                    "Content-Type", "application/json"
                ]
            )
        let result = 
            match response.Body with
            | Text bodyText ->
                JsonConvert.DeserializeObject<SwateTemplate>(bodyText)
            | Binary _ ->
                failwith "Expected text response but got binary"
        result

    member this.GetTemplateByKeys (name: string, version: string) : SwateTemplate =
        let url = $"{this.BaseUrl}/templates/{name}/{version}"
        let response =
            Http.Request(
                url,
                httpMethod = "GET",
                headers = [
                    "Content-Type", "application/json"
                ]
            )
        let result = 
            match response.Body with
            | Text bodyText ->
                JsonConvert.DeserializeObject<SwateTemplate>(bodyText)
            | Binary _ ->
                failwith "Expected text response but got binary"
        result

    member this.PostAsync (dto: SwateTemplateDto, authenticatioToken) : HttpResponse =
        let url = $"{this.BaseUrl}/templates"

        let json = SwateTemplateDto.ToJsonString(dto)
        let requestBody = TextRequest json

        Http.Request(
            url,
            httpMethod = "POST",
            headers = [
                "X-API-KEY", authenticatioToken
                "Content-Type", "application/json"
            ],
            body = requestBody
        )

    member this.FindSolutionRoot (dir: DirectoryInfo) =
        let rec findSolutionRoot (dir: DirectoryInfo) =
            if dir = null then failwith "Solution file not found"
            elif dir.GetFiles("*.sln").Length > 0 then dir.FullName
            else findSolutionRoot dir.Parent
        findSolutionRoot dir

    member this.CleanFileNameFromInfo (file: FileInfo) =
        let nameWithoutExt = Path.GetFileNameWithoutExtension(file.Name)
        let pattern = @"_v\d+\.\d+\.\d+$"
        Regex.Replace(nameWithoutExt, pattern, "")

    member this.CopyDirectory (sourceDir: string, targetDir: string) =
        let rec copyDirectory (sourceDir: string) (targetDir: string) =
            let source = DirectoryInfo(sourceDir)
            let target = DirectoryInfo(targetDir)

            // Create target directory if it doesn't exist
            if not target.Exists then
                target.Create() |> ignore

            // Copy all files 
            source.GetFiles()
            |> Array.iter (fun file ->
                let targetFilePath = Path.Combine(target.FullName, file.Name)
                file.CopyTo(targetFilePath, true) |> ignore)

            // Recursively copy subdirectories
            source.GetDirectories()
            |> Array.iter (fun subDir ->
                let nextTargetSubDir = Path.Combine(target.FullName, subDir.Name)
                copyDirectory subDir.FullName nextTargetSubDir)
        copyDirectory sourceDir targetDir

    member this.CreateDirectoryForTemplate (file: FileInfo) =        
        let fileName = this.CleanFileNameFromInfo(file)
        let fileDirectory = file.Directory.FullName
        if fileDirectory.ToLower().Contains(fileName.ToLower()) then
            file.MoveTo($"{fileDirectory}/{file.Name}", false)
        else
            let newFileDirectory = DirectoryInfo($"{fileDirectory}/{fileName}")
            if not newFileDirectory.Exists then
                newFileDirectory.Create()
            file.MoveTo($"{newFileDirectory}/{file.Name}", false)

    member this.HasRightParentDirectory (fileInfo: FileInfo) =
        let parentDirectory = fileInfo.Directory
        let folderName = this.CleanFileNameFromInfo fileInfo
        parentDirectory.FullName.ToLower().EndsWith(folderName.ToLower())

    member this.UpdateFileName (fileInfo: FileInfo) =
        let template = fileInfo.FullName |> (FsWorkbook.fromXlsxFile >> Spreadsheet.Template.fromFsWorkbook)
        let fileName = this.CleanFileNameFromInfo fileInfo
        let newFileName = $"{fileName}_v{template.Version}{fileInfo.Extension}"
        let newPath = Path.Combine(fileInfo.DirectoryName, newFileName)
        fileInfo.MoveTo(newPath)
        FileInfo(newPath)
