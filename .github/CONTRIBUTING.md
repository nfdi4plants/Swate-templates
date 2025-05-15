# Swate Templates Registry Contribution Guide

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

#### 4. In Visual Studio you have to select docker-compose as the starting project and then you can start it for local tests

![Logo](Swate-templates/img/SelectDockerDesktop.png)

#### How to add a new Template

* When adding new or updating existing templates, create an issue using the "Template(s) request" issue-template, add or update the templates and create a pull-request

#### How to contribute to STRService

* Update the STRClient.cs file after you have finished your work in order to avoid failings when pushing to main. Follow this [step](#3-strclient-generation)
