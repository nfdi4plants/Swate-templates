#r "nuget: ARCtrl"

open System
open System.IO
open System.Collections.Generic

open ARCtrl
open FsSpreadsheet
open FsSpreadsheet.Net


let getAllXLSXFilePaths() =
    let directories = DirectoryInfo("templates").GetDirectories()

    let filePaths =
        directories
        |> Array.collect(fun directory ->
            directory.GetFiles("*.xlsx", SearchOption.AllDirectories))
        |> Array.map (fun file -> file.FullName)
    filePaths

let tryReadTemplate (templatePath: string) =
    let result =
        try
            templatePath
            |> FsSpreadsheet.FsWorkbook.fromXlsxFile
            |> Spreadsheet.Template.fromFsWorkbook
            |> Ok
        with
        | e -> Error(e)
    templatePath, result
let readTemplate (templatePath: string) =
    tryReadTemplate templatePath
    |> function
        | p, Error (e) -> failwith $"Unable to read template: {p}. {e.Message}"
        | p ,Ok template -> p, template

let readAllTemplates() =
    getAllXLSXFilePaths()
    |> Array.map readTemplate

let templates =
    readAllTemplates()
    |> Array.map (fun (path, template) ->
        let name = template.Name
        let path = System.IO.Path.GetRelativePath(__SOURCE_DIRECTORY__, path)
        let authors =
            template.Authors
            |> Seq.map (fun a -> [a.FirstName; a.MidInitials; a.LastName] |> Seq.choose id |> String.concat " ")
            |> String.concat ", "
        name, path, authors
    )

type Node =
    | Folder of name: string * children: Dictionary<string, Node>
    | File of name: string * templateName: string * authors: string


// Recursive insert into tree
let rec insertPath (root: Dictionary<string, Node>) (segments: string list) (templateName: string) (authors: string) =
    match segments with
    | [] -> ()
    | [ fileName ] ->
        root.[fileName] <- File(fileName, templateName, authors)
    | folderName :: rest ->
        match root.TryGetValue(folderName) with
        | true, Folder(name, children) ->
            insertPath children rest templateName authors
        | _ ->
            let newFolder = Dictionary<string, Node>()
            root.[folderName] <- Folder(folderName, newFolder)
            insertPath newFolder rest templateName authors

// Build the tree
let buildTree (data: (string * string * string) []) =
    let root = Dictionary<string, Node>()
    for (name, path, authors) in data do
        let segments = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) |> Array.toList
        insertPath root segments name authors
    root

// Format the tree as markdown
let rec formatTree (nodes: Dictionary<string, Node>) (indent: int) : string list =
    let indentStr = String.replicate indent "  "
    nodes
    |> Seq.sortBy (fun kvp -> kvp.Key)
    |> Seq.collect (fun kvp ->
        match kvp.Value with
        | Folder(name, children) ->
            seq {
                yield sprintf "%s- %s/" indentStr name
                yield! formatTree children (indent + 1)
            }
        | File(fileName, templateName, authors) ->
            seq {
                yield sprintf "%s- %s: %s (%s)" indentStr fileName templateName authors
            }
    )
    |> Seq.toList

// Entry point
let tree = buildTree templates
let markdownLines = formatTree tree 0
let markdown = String.Join("\n", markdownLines)

printfn "%s" markdown
System.IO.File.WriteAllLines("templates/FolderStructure.md", markdownLines)
