namespace STRIndex

open System
open System.IO
open System.Text.Json
open Domain
open Globals

open ARCtrl
open ARCtrl.Template
open FsSpreadsheet
open FsSpreadsheet.Net

type STRRepo =

    ///! Paths are relative to the root of the project, since the script is executed from the repo root in CI
    /// Path is adjustable by passing `RepoRoot`
    static member getStagedTemplates(?RepoRoot: string) = 

        let path = 
            defaultArg 
                (RepoRoot |> Option.map (fun p -> Path.Combine(p, STAGING_AREA_RELATIVE_PATH))) 
                STAGING_AREA_RELATIVE_PATH

        Directory.GetFiles(path, "*.xlsx", SearchOption.AllDirectories)
        |> Array.map (fun x -> x.Replace('\\',Path.DirectorySeparatorChar).Replace('/',Path.DirectorySeparatorChar))
        |> Array.map (FsWorkbook.fromXlsxFile >> Spreadsheet.Template.fromFsWorkbook)
        