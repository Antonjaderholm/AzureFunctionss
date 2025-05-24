using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AzureFunction.Data;
using Microsoft.Extensions.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Register MongoDB services
builder.Services.AddSingleton<CameraData>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new CameraData("cameras", config);
});

builder.Build().Run();