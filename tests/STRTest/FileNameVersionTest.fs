module STRTest.FileNameVersion

open System.IO
open System.Text.RegularExpressions

open ARCtrl

open STRCI

open Expecto

let private checkFileVersioning(file: FileInfo) =
    testCase $"{file.Name}" <| fun _ ->
        let versionPattern = @"_v\d+\.\d+\.\d+"
        let msg, isCorrectFileVersion =
            let sb = System.Text.StringBuilder()
            let regex = new Regex(versionPattern)
            sb.AppendLine(sprintf "## File version does not align with template version `%s` in:" file.Name) |> ignore
            let potMatch = regex.Match(file.Name)
            let fileVersion = if System.String.IsNullOrWhiteSpace(potMatch.Value) then "" else potMatch.Value.Substring(2)

            let template = STRCIController.CreateTemplateFromXlsx file

            let isCorrectFileVersion = (fileVersion = template.Version) && (not (System.String.IsNullOrWhiteSpace fileVersion))

            if not isCorrectFileVersion then
                let authors =
                  template.Authors
                  |> Seq.map (fun a ->
                    [|a.FirstName; a.MidInitials; a.LastName|]
                    |> Array.choose id
                    |> String.concat " "
                  )
                  |> String.concat ", "
                sb.AppendLine (sprintf "- **%s** by (*%s*)" (template.Name.Trim()) (authors.Trim())) |> ignore
            sb.ToString(), isCorrectFileVersion
        Expect.isTrue isCorrectFileVersion msg
            
            


let Main = testList "File names contain right version" [
    let files =
        ReadTemplates.getAllXLSXFilePaths()
        |> Array.map (fun path -> new FileInfo(path))
    for file in files do
        checkFileVersioning file
]
