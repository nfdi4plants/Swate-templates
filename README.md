# Swate Template Registry

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

* Run tests for templates
* Checks whether all local templates are parable or not
* Checks for required diversity of official templates
* Check for distinct tags
* Check for Ambiguousness
* Check for similarity of provided tags
* Check for right named parent folder of templates
* Check for right versioning of template file name
* Check whether all database tempaltes are locally available or not

## Overview

Swate template registry is a tool used to add new templates to the database, update existing ones, and enable viewing those.

You can finde the Template Registry Service [here](https://str.nfdi4plants.org/).

You can find the OpenAPI (Swagger) specification [here](https://str.nfdi4plants.org/swagger/index.html).

You can find a representation of the STR porject relationships [here](str/src).

#### Requirements

- [npm and nswag]
    - install nswag with 'dotnet tool install --global NSwag.ConsoleCore' (Tested with v14.4.0)
    - verify with `npm --version` (Tested with 9.2.0)
    - verify with `nswag --version` (Tested with 14.4.0)
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - verify with `dotnet --version` (Tested with 7.0.306)
-  [Docker Desktop](https://www.docker.com/products/docker-desktop/)
    - verify with `docker --version` (Tested with 4.40.0)

[(wip) Graph of the workflow](Feature_STR_UpdateContributionGuide/src/STRCI)

## Contributing

### Local Setup

#### 1. Setup dotnet tools

   `dotnet tool restore`

#### 2. Install NPM dependencies
   
    `npm install`

#### 3. STRClient Generation

Run one of the following commands, in the project root of STRClient, depending on the nswag version you are using.

Utilize a local version of nswag

```bash
<path to nswag tool>\NSwag\Net80\dotnet-nswag.exe openapi2csclient /input:https://str.nfdi4plants.org/swagger/v1/swagger.json /namespace:STRClient /output:STRClient.cs
```

or

Utilize the nswag CLI being installed as part of the .NET project

```bash
nswag openapi2csclient /input:https://str.nfdi4plants.org/swagger/v1/swagger.json /output:STRClient.cs /namespace:STRClient
```

4. In Visual Studio you have to select docker-compose as the starting project and then you can start it for local tests

![Logo](images/SelectDockerDesktop.png)

#### How to add a new Template

* Use the Template(s) requested issue-template when adding new or updating existing templates

#### How to contribute to STRService

* Update the STRClient.cs file after you have finished your work in order to avoid failings when pushing to main. Follow this [step](#3-strclient-generation)

