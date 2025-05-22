# Swate Templates Registry Contribution Guide

---

## 📚 Requirements

The following tools need to be installed to work on the STR service. For adding or updating templates installation is not required. 

| Tool                | Tested Version | Notes                                                              |
| ------------------- | -------------- | ------------------------------------------------------------------ |
| `NSwag.ConsoleCore` | 14.4.0         | Install with: `dotnet tool restore` as local dotnet tool           |
| `.NET SDK`          | 9.0.300        | [Download .NET SDK](https://dotnet.microsoft.com/en-us/download)   |
| Docker Desktop      | 4.40.0         | [Download Docker](https://www.docker.com/products/docker-desktop/) |

Verify installations with:

```bash
dotnet nswag version
dotnet --version
docker --version
```

---

## 🧩 Workflow Overview

### 📄 1. Create a New Template

1. **Check for existing templates** — make sure you're not duplicating.
2. **Create a template** using [Swate](https://github.com/nfdi4plants/Swate) or [ARCtrl](https://github.com/nfdi4plants/ARCtrl), following the [official guide](https://nfdi4plants.github.io/nfdi4plants.knowledgebase/swate/swate-template-contribution/).
3. **Organize the file**:

   * Move it into a subfolder of [Templates](templates) corresponding to your organization.
   * Create a folder named after the template itself and place the file inside.
   * Rename the file using this format:
     `TemplateName_v<Major>.<Minor>.<Patch>.xlsx`
4. **Commit & PR**: Commit your changes and open a pull request to the `main` branch.

   * This will trigger a CI workflow that validates your template for best practices.

---

### ✏️ 2. Update an Existing Template

1. **Duplicate** the existing template.
2. **Place the copy** in the same folder as the original.
3. **Rename** the new file with an updated version number:
   `TemplateName_v<NewMajor>.<NewMinor>.<NewPatch>.xlsx`
4. **Edit** using [Swate](https://github.com/nfdi4plants/Swate) or [ARCtrl](https://github.com/nfdi4plants/ARCtrl).
5. **Update metadata** with the new version.
6. **Commit & PR**: Submit your changes via a pull request to `main`.

   * This will trigger a CI workflow that validates your template for best practices.

---

### 🛠️ 3. Update the STR Service

1. Implement your feature or bug fix.
2. Regenerate `STRClient.cs` using this [guide](.github/CONTRIBUTING.md#3-strclient-generation).
3. Fix any issues in the generated file.
4. Commit your changes and create a PR to `main`.

---

### ✅ 4. Pull Request Validation

When you create a PR to `main`, the following checks will run automatically:

* ✅ Templates must be parsable.
* ✅ Tags must be:

  * Distinct between endpoint and general use.
  * Unambiguous.
  * Non-redundant.
* ✅ Versioning in filenames must be correct.
* ✅ All templates in the database must exist locally.

**Note**: If tests fail, either fix the issue or wait for curators to respond.

