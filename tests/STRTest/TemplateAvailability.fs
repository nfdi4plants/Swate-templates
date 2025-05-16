module STRTest.TemplateAvailability

open System
open System.IO
open System.Text.RegularExpressions

open ARCtrl
open ARCtrl.Helper

open STRCI

open Expecto

let private checkLocalTemplateAvailability(dbTemplate: STRClient.SwateTemplate) (localTemplates: Template []) =
    testCase $"{dbTemplate.TemplateName}_{dbTemplate.TemplateId}_{dbTemplate.TemplateMajorVersion}.{dbTemplate.TemplateMinorVersion}.{dbTemplate.TemplatePatchVersion}" <| fun _ ->
        let msg, isAvailable =
            let sb = System.Text.StringBuilder()
            sb.AppendLine(sprintf "## Did not find template locally for `%s`" dbTemplate.TemplateName) |> ignore
            let dbVersion = SemVer.SemVer.create(dbTemplate.TemplateMajorVersion, dbTemplate.TemplateMinorVersion, dbTemplate.TemplatePatchVersion).AsString()
            let isAvailable = localTemplates |> Array.tryFind (fun localTemplate -> localTemplate.Id = dbTemplate.TemplateId && localTemplate.Version = dbVersion)
            if isAvailable.IsNone then
                sb.AppendLine (sprintf "- **%s** with version (*%s*)" (dbTemplate.TemplateName.Trim()) dbVersion) |> ignore
            sb.ToString(), isAvailable
        Expect.isTrue (isAvailable.IsSome) msg
            
let Main = testList "Databse templates are locally available" [
    let client = STRCIController.Client("")
    let dbTemplates = client.GetAllTemplatesAsync().Result |> Array.ofSeq
    let localTemplates = TestData.getTemplates()

    for dbTemplate in dbTemplates do
        checkLocalTemplateAvailability dbTemplate localTemplates
]
