#r "nuget: FSharp.Formatting"
#r "nuget: Newtonsoft.Json"

open FSharp.Formatting.Markdown
open Newtonsoft.Json

type TemplateJson = {
    Name        : string
    Version     : string
    Type        : string
    Author      : string list
    Description : string
    FollowUp    : string list
    Docslink    : string
    ER          : string list
    Tags        : string list
    Workbook    : string
    Worksheet   : string
    Table       : string
}

type DocType =
    | Markdown
    | HTML

type Document = {
    DocType     : DocType
    Filepath    : string
    Doc         : string
}

let dirs = System.IO.Directory.GetDirectories "../../templates/"
let dirNames = 
    dirs 
    |> Array.map (System.IO.DirectoryInfo >> fun dir -> dir.Name)

let templatesJsons =
    let filesPerDir = dirs |> Array.map (fun dir -> System.IO.Directory.GetFiles (dir, "*.json"))
    filesPerDir
    |> Array.concat
    |> Array.take 1
    |> Array.map (
        System.IO.File.ReadAllText
        >> Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateJson>
    )

let getFollowupTempString followupTemps =
    followupTemps
    |> List.map (fun followupTemp -> sprintf "- %s" followupTemp)
    |> String.concat "\n"

let getAuthorsOrTagsString authors =
    authors
    |> String.concat ", "

let createDocTemplate name followupTemps tags authors version = 
    sprintf 
        "## Template description\n%s\n## Follow-up templates\n%s\n## Column description\n(WIP)\n## Minor details\n- Tags: %s\n- Author(s): %s\n- Version: %s" 
        name 
        (getFollowupTempString followupTemps)
        (getAuthorsOrTagsString tags)
        (getAuthorsOrTagsString authors)
        version

let generateDocs docType jsonsArr =
    let fileExt =
        match docType with
        | Markdown -> ".md"
        | HTML -> ".html"
    jsonsArr
    |> Array.mapi (
        fun i tempJson -> 
            let filename = sprintf "%s%s" tempJson.Name fileExt |> fun s -> s.Replace(' ', '-')
            let filepath = System.IO.Path.Combine(dirs.[i], filename)
            {
                DocType     = docType
                Filepath    = filepath
                Doc         = createDocTemplate tempJson.Description tempJson.FollowUp tempJson.Tags tempJson.Author tempJson.Version
            }
    )

let writeDocs (docsArr : Document []) =
    let docType = docsArr.[0].DocType
    let writeAsMd (docs : Document []) =
        docs
        |> Array.iter (
            fun doc -> System.IO.File.WriteAllText(doc.Filepath, doc.Doc)
        )
    let writeAsHtml (docs : Document []) =
        docs
        |> Array.iter (
            fun doc -> 
                doc.Doc
                |> fun t -> Markdown.Parse(t, newline = "\n")
                |> Markdown.ToHtml
                |> fun d -> System.IO.File.WriteAllText(doc.Filepath, d)
        )
    match docType with
    | Markdown -> writeAsMd docsArr
    | HTML -> writeAsHtml docsArr

generateDocs Markdown templatesJsons
|> writeDocs

generateDocs HTML templatesJsons
|> writeDocs

let x = templatesJsons.[0]
let y =
    [|x|] |> generateDocs HTML
    |> fun d -> d.[0].Doc
    |> fun t -> Markdown.Parse(t, newline = "  ")
    |> Markdown.ToHtml

y