module API

open System
open System.IO
open STRIndex
open System.IO
open System.Text.Json

open Argu
open CLIArgs
open Domain

open STRIndex
//open STRClient

type GenIndexAPI = 
    static member generatePreviewIndex (verbose: bool) (repo_root: string) (args: ParseResults<GeneratePreviewIndexArgs>) = 
        0

type CheckAPI =
    static member prePublishChecks (verbose: bool) (repo_root: string) = 
        0

type PublishAPI =
    static member publishPendingPackages (verbose: bool) (repo_root: string) (args: ParseResults<PublishArgs>) = 
        0
