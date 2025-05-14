# Swate Template Registry

## Workflow

#### Create Template

* Check whether a similar template exists already or not and you really need a new one
* Create a new template with [Swate](https://github.com/nfdi4plants/Swate) or [ARCtrl](https://github.com/nfdi4plants/ARCtrl), following the [template creation guide}(https://nfdi4plants.github.io/nfdi4plants.knowledgebase/swate/swate-template-contribution/)
* Move the template into a subdirectory of [Templates](templates) corresponding to your organization
* Create a directory with the name of the template and move the template into it
* Rename the template file based on this pattern: Filename_vMaijorversion.MinorVersion.Patchversion
* Commit changes and create PR to main

#### Update Template

* Copy existing template
* The newly copied template stays in the same folder as the original one
* Rename the copied template file based on this pattern: Filename_vMaijorversion.MinorVersion.Patchversion, updating the version
* Update the file content, using [Swate](https://github.com/nfdi4plants/Swate) or [ARCtrl](https://github.com/nfdi4plants/ARCtrl), and update the version in the template metadata
* Commit changes and create PR to main

#### Update STRService

* Add new functionality or fix a bug
* Generate STRClient.cs by following this [step](.github/CONTRIBUTING.md#3-strclient-generation)
* Fix potential problems in the newly generated STRClient.cs
* Commit changes and create PR to main

#### Pull-Request to main

* The following tests are run, when a pull-request to main is created:
* Ccheck whether all local templates can be parsed
* Checks for required diversity of official templates
* Check for distinct tags
* Check for Ambiguousness
* Check for similarity of provided tags
* Check for correctly named parent folder of templates
* Check for correct versioning of template file name
* Check whether all database templates are locally available or not
* Generates new STRClient.cs when a change to api related code happened -> Can break the release
* Release of STR and generation of new JSON file

## Overview

Swate template registry is a tool used to add new templates to the database, update existing ones, and enable viewing those.

You can find the Template Registry Service [here](https://str.nfdi4plants.org/).

You can find the OpenAPI (Swagger) specification [here](https://str.nfdi4plants.org/swagger/index.html).

You can find a representation of the STR project relationships [here](src).

#### Requirements

- [npm and nswag]
    - install nswag with 'dotnet tool install --global NSwag.ConsoleCore' (Tested with v14.4.0)
    - verify with `npm --version` (Tested with 9.2.0)
    - verify with `nswag --version` (Tested with 14.4.0)
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - verify with `dotnet --version` (Tested with 7.0.306)
-  [Docker Desktop](https://www.docker.com/products/docker-desktop/)
    - verify with `docker --version` (Tested with 4.40.0)

[(wip) Graph of the workflow](src/STRCI)
