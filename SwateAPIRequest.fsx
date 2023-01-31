#r "nuget: FSharp.Data"

open FSharp.Data

[<Literal>]
let baseurl_local = @"https://localhost:3000"
[<Literal>]
let baseurl_production = @"https://swate.nfdi4plants.org"

[<Literal>]
let apiPath = @"/api/IISADotNetCommonAPIv1/toSwateTemplateJson"

let file_working = System.IO.File.ReadAllBytes @"./templates/dataplant/1SPL01_plants.xlsx"
let file_test = System.IO.File.ReadAllBytes @"./templates/dataplant/GEO/Growth_GEO_minimal.xlsx"

let tryParsing (url: string) (file: byte []) = 
    Http.RequestString(
        url + apiPath,
        body = HttpRequestBody.BinaryUpload file,
        httpMethod = "POST"
    )

tryParsing baseurl_local file_test