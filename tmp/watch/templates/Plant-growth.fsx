(**
// can't yet format YamlFrontmatter (["title: Plant growth"; "category: Templates"; "categoryindex: 1"; "index: 1"], Some { StartLine = 2 StartColumn = 0 EndLine = 6 EndColumn = 8 }) to pynb markdown

this is the title: input

mdtext

*)
#r "nuget: FSharp.Formatting"

open FSharp.Formatting.Common
open FSharp.Formatting.Markdown

let document = """
# F# Hello world
Hello world in [F#](http://fsharp.net) looks like this:

    printfn "Hello world!"

For more see [fsharp.org][fsorg].

  [fsorg]: http://fsharp.org "The F# organization." """

let doc2 = sprintf "# header\n%s\nmore markdown text here." show

let parsed = Markdown.Parse(document)
let parsed2 = Markdown.Parse(doc2)

Markdown.ToFsx(parsed)
Markdown.ToFsx(parsed2)
Markdown.ToMd(parsed2)

let html = Markdown.ToHtml(parsed)

