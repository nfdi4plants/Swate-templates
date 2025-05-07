using Microsoft.EntityFrameworkCore;
using STRService.Models;
using Microsoft.AspNetCore.HttpOverrides;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Net.Mime.MediaTypeNames;
using STRService.API;
using STRService.OpenAPI;
using Microsoft.OpenApi.Models;
using NSwag.Generation.Processors.Security;
using NSwag.Generation.Processors;
using STRService.API.Endpoints;
using STRService.Data;
using Microsoft.AspNetCore.Http.Json;

using STRService.Components; // import razor components

// ------------------------- ApplicationBuilder -------------------------
// in this section, we will add the necessary code to configure the application builder,
// which defines the application's configuration and services.
// This is the main Dependency Injection (DI) container.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add services to the container.

// configurte NSwag OpenAPI document with document-level settings

builder.Services.AddOpenApiDocument(DocGen.GeneratorSetup);

builder.Services.AddEndpointsApiExplorer();

// Add database related services
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDbContextFactory<SwateTemplateDb>(opt => 
    opt.UseNpgsql(
        // retrieve connection string from configuration via "PostgressConnectionString" key
        // this is found in the appsettings.json file locally, and in the environment variables when deployed
        connectionString: builder.Configuration.GetConnectionString("PostgressConnectionString")
    )
);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.PropertyNamingPolicy = null);

// ------------------------- WebApplication -------------------------
// in this section, we will add the necessary code to configure the WebApplication,
// which defines the HTTP request pipeline.

var app = builder.Build();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseAntiforgery();

app.UseStaticFiles(); // serve wwwroot content https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-8.0

// serve OpenAPI document and Swagger UI
app.UseOpenApi();
app.UseSwaggerUi(settings => {
    settings.DocExpansion = "list";
    settings.ValidateSpecification = true;
}); 

// if we are in development mode, apply migrations and seed the database
// otherwise do not touch the database, and apply necessary migrations by exporting migration sql scripts
// e.g. via `dotnet ef migrations script`
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<SwateTemplateDb>();
    ctx.Database.Migrate();
    if (app.Environment.IsDevelopment())
    {
        DataInitializer.SeedData(ctx);
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();

}

// Configure the HTTP request pipeline.

// ======================== Templates endpoints =========================

// app.MapGet binds a response handler function to a HTTP request on a specific route pattern

app.MapGroup("/api/v1/metadata")
    .MapTemplateMetadataApiV1();

app.MapGroup("/api/v1/templates")
    .MapTemplateApiV1();

app.MapGroup("/api/v1/statistics")
    .MapStatisticsApiV1();

//app.MapGroup("/")
//    .MapPageEndpoints();

app.Run();
