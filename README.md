# Production Plant Evaluation

A .NET 10 ASP.NET Core Web API that evaluates orders against enabled production rulesets and returns the matching production plant and rule.

## Prerequisites

- .NET 10 SDK
- SQL Server LocalDB (the default connection string is configured for Windows LocalDB)
- Visual Studio 2026 or another .NET-compatible editor (optional)

## Setup

1. Clone or open the repository and change to its root directory.
2. Restore dependencies:

   ```powershell
   dotnet restore .\ProductionPlantEvaluation.slnx
   ```

3. Ensure SQL Server LocalDB is installed and available. In Development, the API applies Entity Framework Core migrations at startup, creating the database and adding the migration-defined sample rules.

## Configuration

The API reads its database connection string from `ConnectionStrings:DefaultConnection` in `src/ProductionPlantEvaluation.API/appsettings.json`. The default is:

```text
Server=(localdb)\MSSQLLocalDB;Database=ProductionPlantEvaluationDB;Trusted_Connection=True;TrustServerCertificate=True;
```

To use another SQL Server, override the connection string with an environment variable before running the API. For example, in PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=ProductionPlantEvaluationDB;User Id=your-user;Password=your-password;TrustServerCertificate=True;"
```

Use a secure configuration mechanism for credentials outside local development; do not commit secrets. `Database:ApplyMigrationsOnStartup` is enabled in `appsettings.Development.json`. For other environments, configure database migrations as part of deployment before starting the API.

The `RuleEngine` settings in `appsettings.json` control rule caching (`EnableCaching` and `CacheDurationMinutes`).

## Run the API

From the repository root, run the Development HTTP profile:

```powershell
dotnet run --project .\src\ProductionPlantEvaluation.API\ProductionPlantEvaluation.API.csproj --launch-profile http
```

The API listens at `http://localhost:5136`. When running in Development, Swagger UI is available at [http://localhost:5136/swagger](http://localhost:5136/swagger).

## Evaluate an order

Send a `POST` request to `/api/Evaluate` with an order ID and order details. For example, using PowerShell:

```powershell
$body = @{
	orderId = "order-123"
	publisherNumber = "publisher-1"
	publisherName = "Example Publisher"
	orderMethod = "Standard"
	shipments = @(
		@{
			shipTo = @{
				isoCountry = "US"
			}
		}
	)
	items = @(
		@{
			sku = "book-001"
			printQuantity = 100
			components = @()
		}
	)
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "http://localhost:5136/api/Evaluate" -Method Post -ContentType "application/json" -Body $body
```

The response reports whether a rule matched, the selected production plant, the matched ruleset and rule, and a reason. A missing or blank `orderId` returns `400 Bad Request`.

## Build and test

Build the solution:

```powershell
dotnet build .\ProductionPlantEvaluation.slnx
```

Run all tests:

```powershell
dotnet test .\ProductionPlantEvaluation.slnx
```
