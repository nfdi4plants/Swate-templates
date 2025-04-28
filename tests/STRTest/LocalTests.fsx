
#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/Newtonsoft.Json.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/DocumentFormat.OpenXml.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/FsSpreadsheet.dll"
#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/FsSpreadsheet.Net.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/System.IO.Packaging.dll"

#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/STRIndex.dll"
#r @"C:/Users/Patri/source/repos/Swate-templates/bin/Debug/STRService.dll"

#r "nuget: Fable.Core"
#r "nuget: ARCtrl"
#r "nuget: FSharp.Data"
#r "nuget: Expecto"

#load @"../..\src/STRApplication/TemplateController.fs"
#load @"../STRTest/TestController.fs"

open System
open System.IO
open System.Text.RegularExpressions

open ARCtrl

open FsSpreadsheet
open FsSpreadsheet.Net

open Expecto

open STRTest
open STRIndex
open STRApplication
open STRService.Data
open STRService.Models

let url = "https://localhost:50135/api/v1"

let testController = new STRTest.TestController(url)

testController.RunAllTests()
