(**
---
title: Plant growth
category: Templates
categoryindex: 1
index: 1
---
*)

(*** hide ***)

let srcPath = System.IO.Path.Combine(__SOURCE_DIRECTORY__,__SOURCE_FILE__)
let title = System.IO.Path.GetFileName srcPath |> fun s -> s.Replace(".fsx","")

let show = sprintf "this is the title: %s" title

show

(*** include-it-raw ***)

(**

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