namespace STRCI

open System
open System.IO
open System.Text.RegularExpressions

open ARCtrl
open ARCtrl.Json
open ARCtrl.Helper

open FsSpreadsheet
open FsSpreadsheet.Net

module Helper =

    type Logger(path : string) =

        let fileWriter = new StreamWriter(path)

        member this.Info(s) =
            printfn "INFO: %s" s
            fileWriter.WriteLine(sprintf "INFO: %s" s)
            fileWriter.Flush()

        member this.Error(s) =
            printfn "ERROR: %s" s
            fileWriter.WriteLine(sprintf "ERROR: %s" s)
            fileWriter.Flush()

    let mapOntologyAnnotation (tag: ARCtrl.OntologyAnnotation) =
        let name = defaultArg tag.Name ""
        let tan = defaultArg tag.TermAccessionNumber ""
        let tsr = defaultArg tag.TermSourceREF ""

        let result = new STRIndex.Domain.OntologyAnnotation()
        result.Name <- name
        result.TermAccessionNumber <- tan
        result.TermSourceREF <- tsr

        result

    let mapAuthor(p: ARCtrl.Person) =
        let fullName =
            let fst = defaultArg p.FirstName ""
            let mid = if p.MidInitials.IsSome then $"{fst} {p.MidInitials.Value}" else fst
            let last = if p.LastName.IsSome then $"{mid} {p.LastName.Value}" else mid
            last
        let aff = defaultArg p.Affiliation ""
        let email = defaultArg p.EMail ""

        let result = new STRIndex.Domain.Author()
        result.FullName <- fullName
        result.Affiliation <- aff
        result.Email <- email

        result
open Helper

type STRCIController =

    static member Client (authenticationToken: string) =
        let httpClient = new System.Net.Http.HttpClient()
        httpClient.DefaultRequestHeaders.Add("X-API-KEY", authenticationToken)
        new STRClient.Client(httpClient)

    static member FindSolutionRoot (dir: DirectoryInfo) =
        let rec findSolutionRoot (dir: DirectoryInfo) =
            if dir = null then failwith "Solution file not found"
            elif dir.GetFiles("*.sln").Length > 0 then dir.FullName
            else findSolutionRoot dir.Parent
        findSolutionRoot dir

    static member GetAllTemplateFiles (dir: DirectoryInfo) =
        let solutionRoot = STRCIController.FindSolutionRoot (dir)

        let templatesPath = Path.Combine(solutionRoot, "templates")

        let directories = DirectoryInfo(templatesPath).GetDirectories()

        directories
        |> Array.collect(fun directory ->
            directory.GetFiles("*.xlsx", SearchOption.AllDirectories))

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

    static member IsTemplateInDB (template: Template, dbTemplates: STRClient.SwateTemplate []) =
        let potTemplate = Array.tryFind (fun (item: STRClient.SwateTemplate) ->
            let dbVersion = SemVer.SemVer.create(item.TemplateMajorVersion, item.TemplateMinorVersion, item.TemplatePatchVersion).AsString()
            item.TemplateId = template.Id && dbVersion = template.Version) dbTemplates
        potTemplate.IsSome

    static member GetVersionInformation (template: ARCtrl.Template) =
        if template.SemVer.IsSome then
            let semVer = template.SemVer.Value
            let preReleaseVersionSuffix = defaultArg semVer.PreRelease ""
            let buildMetadataVersionSuffix = defaultArg semVer.Metadata ""
            semVer.Major, semVer.Minor, semVer.Patch, preReleaseVersionSuffix, buildMetadataVersionSuffix
        else
            0, 0, 0, "", ""

    static member CreateSwateClientTemplate (template: ARCtrl.Template) =
        let swateTemplate = new STRClient.SwateTemplate()

        let content = template.toJsonString()
        let majorVersion, minorVersion, patchVersion, preReleaseVersionSuffix, buildMetadataVersionSuffix = STRCIController.GetVersionInformation(template)

        swateTemplate.TemplateId <- template.Id
        swateTemplate.TemplateName <- template.Name
        swateTemplate.TemplateContent <- content
        swateTemplate.TemplateMajorVersion <- majorVersion
        swateTemplate.TemplateMinorVersion <- minorVersion
        swateTemplate.TemplatePatchVersion <- patchVersion
        swateTemplate.TemplatePreReleaseVersionSuffix <- preReleaseVersionSuffix
        swateTemplate.TemplateBuildMetadataVersionSuffix <- buildMetadataVersionSuffix
        swateTemplate

    static member CreateClientOntologyAnnotation (ontologyAnnotation: ARCtrl.OntologyAnnotation) =
        let swateOntologyAnnotation = mapOntologyAnnotation(ontologyAnnotation)
        let clientOntologyAnnotation = new STRClient.OntologyAnnotation()
        clientOntologyAnnotation.Name <- swateOntologyAnnotation.Name
        clientOntologyAnnotation.TermAccessionNumber <- swateOntologyAnnotation.TermAccessionNumber
        clientOntologyAnnotation.TermSourceREF <- swateOntologyAnnotation.TermSourceREF
        clientOntologyAnnotation

    static member createClientAuthor (person: ARCtrl.Person) =
        let author = mapAuthor(person)
        let clientAuthor = new STRClient.Author()
        clientAuthor.FullName <- author.FullName
        clientAuthor.Email <- author.Email
        clientAuthor.Affiliation <- author.Affiliation
        clientAuthor.AffiliationLink <- author.AffiliationLink
        clientAuthor

    static member CreateSwateClientMetadata (template: ARCtrl.Template) =

        let endpointRepositories =
            template.EndpointRepositories
            |> Array.ofSeq
            |> Array.map (fun ontologyAnnotation -> STRCIController.CreateClientOntologyAnnotation(ontologyAnnotation))
            |> ResizeArray :> System.Collections.Generic.ICollection<STRClient.OntologyAnnotation>

        let tags =
            template.Tags
            |> Array.ofSeq
            |> Array.map (fun ontologyAnnotation -> STRCIController.CreateClientOntologyAnnotation(ontologyAnnotation))
            |> ResizeArray :> System.Collections.Generic.ICollection<STRClient.OntologyAnnotation>

        let authors =
            template.Authors
            |> Array.ofSeq
            |> Array.map (fun person -> STRCIController.createClientAuthor(person))
            |> ResizeArray :> System.Collections.Generic.ICollection<STRClient.Author>

        let majorVersion, minorVersion, patchVersion, preReleaseVersionSuffix, buildMetadataVersionSuffix = STRCIController.GetVersionInformation(template)

        let swateMetadata = new STRClient.SwateTemplateMetadata()

        swateMetadata.Id <- template.Id
        swateMetadata.Name <- template.Name
        swateMetadata.Description <- template.Description
        swateMetadata.Organisation <- template.Organisation.ToString()
        swateMetadata.EndpointRepositories <- endpointRepositories
        swateMetadata.ReleaseDate <- DateTimeOffset.Now
        swateMetadata.Tags <- tags
        swateMetadata.Authors <- authors
        swateMetadata.MajorVersion <- majorVersion
        swateMetadata.MinorVersion <- minorVersion
        swateMetadata.PatchVersion <- patchVersion
        swateMetadata.PreReleaseVersionSuffix <- preReleaseVersionSuffix
        swateMetadata.BuildMetadataVersionSuffix <- buildMetadataVersionSuffix
        swateMetadata

    static member CreateFileNames(version: string) =
        let currentDirectory = DirectoryInfo(System.Environment.CurrentDirectory)
        let solutionRoot = STRCIController.FindSolutionRoot(currentDirectory)

        let outputPath = Path.Combine(solutionRoot, "templates-to-json")
        let outputFileName = Path.Combine(outputPath, $"templates_{version}.json")
        let reportFileName = Path.Combine(outputPath, $"report_{version}.txt")
        currentDirectory, outputPath, outputFileName, reportFileName

    static member GetLatestTemplates(currentDirectory, log: Logger) =
        let files = STRCIController.GetAllTemplateFiles(currentDirectory)

        let templates =
            files
            |> Array.choose (fun f ->
                try
                    Some (STRCIController.CreateTemplateFromXlsx f)
                with
                | ex ->
                    log.Error(sprintf "Error loading template %s: %s" f.Name ex.Message)
                    None
            )

        log.Info(sprintf "Success! Read %d templates" templates.Length)

        let getLatestTemplate (templates: Template []) =
            templates
            |> Array.sortByDescending (fun template -> template.Version)
            //enables checking, whether the templates are
            //|> Array.map (fun template ->
            //    printfn "template.Name: %s template.Version: %s" template.Name template.Version
            //    template)
            |> Array.head

        let latestTemplates =
            let groupedTemplates =
                templates
                |> Array.groupBy (fun template -> template.Id)
            groupedTemplates
            |> Array.map (fun (_, templates) ->
                getLatestTemplate templates
            )
        latestTemplates

    static member TemplatesToJsonArtifact () =

        let version = "latest"

        let currentDirectory, outputPath, outputFileName, reportFileName = STRCIController.CreateFileNames(version)

        let ensureDirectory (dirPath : string) =
            if not (Directory.Exists (dirPath)) then
                Directory.CreateDirectory (dirPath) |> ignore

        ensureDirectory outputPath

        let log = Logger(reportFileName)

        log.Info("Starting templates-to-json.fsx")

        let latestTemplates = STRCIController.GetLatestTemplates(currentDirectory, log)

        let json =
            latestTemplates
            |> Templates.toJsonString 0

        log.Info("Write json")

        File.WriteAllText(outputFileName, json)

        log.Info("Finished templates-to-json.fsx")
