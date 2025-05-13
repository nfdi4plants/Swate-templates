# Swate Template Registry

Swate template registry is a tool used to add new templates to the database, update existing ones, and enable viewing those.

You can finde the Template Registry Service [here](https://str.nfdi4plants.org/).

You can find the OpenAPI (Swagger) specification [here](https://str.nfdi4plants.org/swagger/index.html).

You can find a representation of the STR porject relationships [here](tree/str/src).

#### Requirements

- [npm and nswag]
    - install nswag with 'dotnet tool install --global NSwag.ConsoleCore' (Tested with v14.4.0)
    - verify with `npm --version` (Tested with 9.2.0)
    - verify with `nswag --version` (Tested with 14.4.0)
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - verify with `dotnet --version` (Tested with 7.0.306)
-  [Docker Desktop](https://www.docker.com/products/docker-desktop/)
    - verify with `docker --version` (Tested with 4.40.0)

## Worflow

#### Create Template

* Check whether a similar template exists already or not and you really need a new one
* Create a new template with [Swate](https://github.com/nfdi4plants/Swate) or [ARCtrl](https://github.com/nfdi4plants/ARCtrl)
* Move the template into a subdirectory of [Templates](templates)
* Create a directory with the name of the template and move the template into it
* Rename the template file based on this pattern: Filename_vMaijorversion.MinorVersion.Patchversion
* Commit changes and create PR to main

#### Update Template

* Copy existing template
* The newly copied template stays in the same folder as the original one
* Rename the copied template file based on this pattern: Filename_vMaijorversion.MinorVersion.Patchversion
* Update the file content, using [Swate](https://github.com/nfdi4plants/Swate) or [ARCtrl](https://github.com/nfdi4plants/ARCtrl)
* Commit changes and create PR to main

#### Update STRService

* Add new functionality or fix a bug
* Generate STRClient anew by follwong this [step](#3-strclient-generation)
* Fix potential problems in the newly generated STRClient.cs
* Commit changes and create PR to main

#### Pull-Request to main

[(wip) Graph of the workflow](tree/Feature_STR_UpdateContributionGuide/src/STRCI)
