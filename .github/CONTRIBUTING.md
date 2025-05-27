# Swate Template Registry Contribution Guide

---

## 📚 Requirements

The following tools need to be installed to work on the STR service. 

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

## 🧩 Workflow 

---

### 🛠️ Update the STR Service

1. Implement your feature or bug fix.
2. Regenerate `STRClient.cs` using this [guide](.github/CONTRIBUTING.md#3-strclient-generation).
3. Fix any issues in the generated file.
4. Commit your changes and create a PR to `main`.

---



