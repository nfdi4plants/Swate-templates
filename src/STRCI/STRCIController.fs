namespace STRCI

open System
open System.IO
open System.Text.RegularExpressions

open ARCtrl

open FsSpreadsheet
open FsSpreadsheet.Net

open Newtonsoft.Json

open STRService.Models

open FSharp.Data

open STRClient

type STRCIController =        

    static member Client () =
        let httpClient = new System.Net.Http.HttpClient()
        httpClient.DefaultRequestHeaders.Add("X-API-KEY", "")
        new STRClient.Client(httpClient)

    static member FindSolutionRoot (dir: DirectoryInfo) =
        let rec findSolutionRoot (dir: DirectoryInfo) =
            if dir = null then failwith "Solution file not found"
            elif dir.GetFiles("*.sln").Length > 0 then dir.FullName
            else findSolutionRoot dir.Parent
        findSolutionRoot dir

    static member CleanFileNameFromInfo (file: FileInfo) =
        let nameWithoutExt = Path.GetFileNameWithoutExtension(file.Name)
        let pattern = @"_v\d+\.\d+\.\d+$"
        Regex.Replace(nameWithoutExt, pattern, "")

    static member CopyDirectory (sourceDir: string, targetDir: string) =
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

    static member CreateDirectoryForTemplate (file: FileInfo) =        
        let fileName = STRCIController.CleanFileNameFromInfo(file)
        let fileDirectory = file.Directory.FullName
        if fileDirectory.ToLower().Contains(fileName.ToLower()) then
            file.MoveTo($"{fileDirectory}/{file.Name}", false)
        else
            let newFileDirectory = DirectoryInfo($"{fileDirectory}/{fileName}")
            if not newFileDirectory.Exists then
                newFileDirectory.Create()
            file.MoveTo($"{newFileDirectory}/{file.Name}", false)

    static member FindExistingParentDirectory (directory: DirectoryInfo, sourceDirectoryNames: DirectoryInfo []) =
        let rec findParentDirectory (dir: DirectoryInfo) (parentdirectories: string) =
            let potDir =
                sourceDirectoryNames
                |> Array.tryFind (fun item -> (item.Name.ToLower()) = (dir.Name.ToLower()))
            if potDir.IsSome then $"{potDir.Value.FullName}/{parentdirectories}"
            else findParentDirectory dir.Parent $"{dir.Name}/{parentdirectories}"
        findParentDirectory directory.Parent directory.Name

    static member CreateDirectoryForExternalTemplate (file: FileInfo) =        
        let fileName = STRCIController.CleanFileNameFromInfo(file)

        let sourceDirectories = 
            let solutionRoot = STRCIController.FindSolutionRoot (DirectoryInfo(System.Environment.CurrentDirectory))
            let templatesPath = Path.Combine(solutionRoot, "templates")
            Directory.GetDirectories(templatesPath, "*", SearchOption.AllDirectories)
            |> Array.map (fun item -> new DirectoryInfo(item))

        let sourceDirectoryNames = 
            sourceDirectories
            |> Array.map (fun item -> item.Name.ToLower())

        match sourceDirectoryNames with
        //Check whether a directory with the name of the file exists or not
        | directoryNames when Array.contains (fileName.ToLower()) directoryNames ->
            let newFileDirectory = 
                let directory =
                    sourceDirectories
                    |> Array.find (fun item -> (item.Name.ToLower()).EndsWith(fileName.ToLower()))
                DirectoryInfo($"{directory}")
            let path = Regex.Replace(newFileDirectory.FullName, @"\\", "/")
            let newFile = new FileInfo($"{path}/{file.Name}")

            if not newFile.Exists then 
                file.CopyTo($"{path}/{file.Name}", false) |> ignore
                printfn "copied file: %s" file.Name

        //Check whether a directory with the name of the files parent directory exists or not
        | directoryNames when Array.contains (file.Directory.Name.ToLower()) directoryNames ->
            let newFileDirectory = 
                let directory =
                    sourceDirectories
                    |> Array.find (fun item -> (item.Name.ToLower()).EndsWith(file.Directory.Name.ToLower()))
                DirectoryInfo($"{directory}/{fileName}")

            let path = Regex.Replace(newFileDirectory.FullName, @"\\", "/")

            if not newFileDirectory.Exists then
                newFileDirectory.Create()
                printfn "created directory: %s" file.Name

            let newFile = new FileInfo($"{path}/{file.Name}")

            if not newFile.Exists then 
                file.CopyTo($"{path}/{file.Name}", false) |> ignore
                printfn "copied file: %s" file.Name

        | _ ->
            let newFileDirectory = 
                let path = STRCIController.FindExistingParentDirectory(file.Directory, sourceDirectories)
                new DirectoryInfo(path)
            let path = Regex.Replace(newFileDirectory.FullName, @"\\", "/")

            printfn "path: %s" path

            if not newFileDirectory.Exists then
                newFileDirectory.Create()
                printfn "created directory: %s" file.Name

            let newFile = new FileInfo($"{path}/{file.Name}")

            if not newFile.Exists then 
                file.CopyTo($"{path}/{file.Name}", false) |> ignore
                printfn "copied file: %s" file.Name

    static member HasRightParentDirectory (fileInfo: FileInfo) =
        let parentDirectory = fileInfo.Directory
        let folderName = STRCIController.CleanFileNameFromInfo fileInfo
        parentDirectory.FullName.ToLower().EndsWith(folderName.ToLower())

    static member UpdateFileName (fileInfo: FileInfo) =
        let template = fileInfo.FullName |> (FsWorkbook.fromXlsxFile >> Spreadsheet.Template.fromFsWorkbook)
        let fileName = STRCIController.CleanFileNameFromInfo fileInfo
        let newFileName = $"{fileName}_v{template.Version}{fileInfo.Extension}"
        let newPath = Path.Combine(fileInfo.DirectoryName, newFileName)
        fileInfo.MoveTo(newPath)
        FileInfo(newPath)

    static member CreateTemplateFromXlsx (fileInfo: FileInfo) =
        FsWorkbook.fromXlsxFile fileInfo.FullName
        |> Spreadsheet.Template.fromFsWorkbook
