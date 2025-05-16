module STRTest.Constants

[<Literal>]
let TemplateFolderPath = @"./templates"

[<RequireQualifiedAccess>]
module TagCoherence =

    let WhiteList : string [] [] =
        [|
            [|"RNASeq"; "mRNASeq"; "DNASeq"|]
            [|"MIAPE"; "MIAPPE"|]
        |]

    let WhiteListMap =
        [|
            for set in WhiteList do
            for item in set do
                item, set
        |]
        |> fun x -> x
        |> Array.groupBy fst
        |> Array.map (fun (name,set) -> name, Array.collect snd set |> Set.ofArray)
        |> Map.ofArray

    let TagSimiliarityThreshold = 0.8

let TemplateSimilarityThershold = 0.3 // Minimum 30% difference
