module STRTest.Tests

open Fable
open Expecto

let testController = new TestController("https://str.nfdi4plants.org/api/v1")

[<EntryPoint>]
let main argv = testController.RunAllTests()
