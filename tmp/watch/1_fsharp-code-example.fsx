(**
// can't yet format YamlFrontmatter (["title: F# code formatting example"; "category: sample content"; "categoryindex: 1"; "index: 2"], Some { StartLine = 2 StartColumn = 0 EndLine = 6 EndColumn = 8 }) to pynb markdown

[![Binder](http://localhost:8901/img/badge-binder.svg)](https://mybinder.org/v2/gh/plotly/Plotly.NET/gh-pages?filepath=1_fsharp-code-example.ipynb)&emsp;
[![Script](http://localhost:8901/img/badge-script.svg)](http://localhost:8901/1_fsharp-code-example.fsx)&emsp;
[![Notebook](http://localhost:8901/img/badge-notebook.svg)](http://localhost:8901/1_fsharp-code-example.ipynb)

[How to add these badges?](http://localhost:8901/4_download-badges.html)

# F# code formatting example

This page is rendered from a F# script as input, containing real F# code besides this markdown section.

hover above some bindings to get tooltips like in a real editor environment.

*)
///this is comment
let a = 42(* output: 
42*)
// see some operators/keywords:

if a > 0 then printfn "see, this is included: %i" a

// an interface:
type IA =
    abstract member B : string -> string

// an interface implementation:

type C() =
    interface IA with
        member _.B(a) = id a

let d = C() :> IA

let e = d.B("soos")(* output: 
"soos"*)
module ThisIsAModule =

    type Union =
        | First
        | Second of IA

    type Enum =
        | First = 1
        | Second = 2

