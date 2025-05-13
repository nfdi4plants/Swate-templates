# Swate Template Registry

Swate template registry is a tool used to add new templates to the database, publish existing ones and enable viewing those.

The service for checking and downloading templates can be found [here](https://str.nfdi4plants.org/).

The swagger code for the open api is [here](https://str.nfdi4plants.org/swagger/index.html).

The relationship of the different projects are represented [here](https://github.com/nfdi4plants/Swate-templates/tree/str/src).

#### Requirements

- [npm and nswag]
    - install nswag with 'dotnet tool install --global NSwag.ConsoleCore' (Tested with v14.4.0)
    - verify with `npm --version` (Tested with v9.2.0)
    - verify with `nswag --version` (Tested with v14.4.0)
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - verify with `dotnet --version` (Tested with 7.0.306)
-  [Docker Desktop] (https://www.docker.com/products/docker-desktop/)
    - verify with `docker --version` (Tested with 28.0.0)

#### Worflow

[(wip) Graph of the workflow](https://github.com/nfdi4plants/Swate-templates/tree/Feature_STR_UpdateContributionGuide/src/STRCI)

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

* Update the STRClient.cs file after you have finished your work in order to avoid failings when pushing to main. Run the following [command](#strclient-generation)

