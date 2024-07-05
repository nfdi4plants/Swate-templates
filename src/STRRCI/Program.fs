open Argu
open System.IO

open API
open CLIArgs
open CommandHandling

[<EntryPoint>]
let main argv =

    let parser = EntryCommand.createParser()

    try
        let args = parser.ParseCommandLine()

        let verbose = args.TryGetResult(EntryCommand.Verbose) |> Option.isSome
        
        let repo_root_path = args.TryGetResult(EntryCommand.Repo_Root_Path) |> Option.defaultValue "."

        handleEntryCommand verbose repo_root_path (args.GetSubCommand())
        |> int

    with
        | :? ArguParseException as ex ->
            match ex.ErrorCode with
            | ErrorCode.HelpText  -> 
                printfn "%s" (parser.PrintUsage())
                0 // printing usage is not an error

            | _ -> 
                printfn "%A" ex.Message
                1

        | ex ->
                printfn "%A" ex.Message
                1
