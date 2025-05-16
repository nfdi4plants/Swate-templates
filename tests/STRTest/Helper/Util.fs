module STRTest.Util

module SorensenDice =
    let inline calculateDistance (x : Set<'T>) (y : Set<'T>) =
        match  (x.Count, y.Count) with
        | (0,0) -> 1.
        | (xCount,yCount) -> (2. * (Set.intersect x y |> Set.count |> float)) / ((xCount + yCount) |> float)

    let createBigrams (s:string) =
        s
            .ToUpperInvariant()
            .ToCharArray()
        |> Array.windowed 2
        |> Array.map (fun inner -> sprintf "%c%c" inner.[0] inner.[1])
        |> set

    let createDistanceScores (searchStr:string) (f: 'a -> string) (arrayToSort:'a []) =
        let searchSet = searchStr |> createBigrams
        arrayToSort
        |> Array.map (fun result ->
            let resultSet = f result |> createBigrams
            calculateDistance resultSet searchSet, result
        )
