module CommandHandling

open CLIArgs
open API
open Argu

let handleEntryCommand (verbose:bool) (repo_root: string) command = 
    match command with
    | EntryCommand.Gen_index subcommand -> 
        if verbose then 
            printfn ""
            printfn "Command: gen-index"
            printfn ""
        API.GenIndexAPI.generatePreviewIndex verbose (repo_root) (subcommand)

    | EntryCommand.Check -> 
        if verbose then 
            printfn ""
            printfn "Command: check"
            printfn ""
        API.CheckAPI.prePublishChecks verbose (repo_root)

    | EntryCommand.Publish subcommand -> 
        if verbose then
            printfn ""
            printfn "Command: publish"
            printfn ""
        API.PublishAPI.publishPendingPackages verbose (repo_root) (subcommand)

    | _ -> failwith $"unrecognized command '{command}"