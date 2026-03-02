using Azure.Data.Tables;
using MudBlazor.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ShoppingApp.Common;
using System.Reflection;
using Orleans.Configuration;
using ShoppingApp.WebUI;
using ShoppingApp.WebUI.Cart;
using ShoppingApp.WebUI.Extensions;
using ShoppingApp.WebUI.Products;
using ShoppingApp.WebUI.Services;
using ShoppingApp.WebUI.Shared;

var builder = WebApplication.CreateBuilder(args);

// Scalability on Azure Container Apps for Blazor based WebUI.
if (!builder.Environment.IsDevelopment())
{
    var azureBlobStorageFobWebUiUri = GlobalConfig.AzureBlobStorageFobWebUiUri;
    var azureKeyVaultFobWebUiUri = GlobalConfig.AzureKeyVaultFobWebUiUri;

    builder.UseCentralizedKeys(azureBlobStorageFobWebUiUri, azureKeyVaultFobWebUiUri);

    builder.Services.AddSignalR().AddAzureSignalR(options =>
    {
        options.ConnectionString = GlobalConfig.AzureSignalRConnection;
        options.ServerStickyMode = Microsoft.Azure.SignalR.ServerStickyMode.Required;
    });
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ShoppingCartService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ComponentStateChangedObserver>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddLocalStorageServices();

// Application Insights.
var assembly = Assembly.GetExecutingAssembly();
var version = AppInfo.RetrieveInformationalVersion(assembly);
const string roleName = "ShoppingApp.WebUI";
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = GlobalConfig.AppInsightsConnectionString;
});
builder.Services.ConfigureOpenTelemetryTracerProvider((sp, tracerBuilder) =>
    tracerBuilder.ConfigureResource(r => r.AddService(
        serviceName: roleName,
        serviceInstanceId: roleName,
        serviceVersion: version)));
builder.Services.ConfigureOpenTelemetryLoggerProvider((sp, loggerBuilder) =>
    loggerBuilder.ConfigureResource(r => r.AddService(
        serviceName: roleName,
        serviceInstanceId: roleName,
        serviceVersion: version)));

// Configure Microsoft Orleans Client
if (builder.Environment.IsDevelopment())
{
    builder.Host.UseOrleansClient((_, clientBuilder) =>
    {
        clientBuilder.Configure<ClusterOptions>(_ => { })
            .UseLocalhostClustering();
    });
}
else
{
    builder.Host.UseOrleansClient((_, clientBuilder) =>
    {
        clientBuilder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "ShoppingApp";
                options.ServiceId = "ShoppingAppService";
            })
            .UseAzureStorageClustering(options =>
            {
                options.TableServiceClient = new TableServiceClient(GlobalConfig.AzureStorageConnection);
            });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this
    // for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();