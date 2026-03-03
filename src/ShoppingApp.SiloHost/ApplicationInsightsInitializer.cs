using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ShoppingApp.Common;
using System.Reflection;

namespace ShoppingApp.SiloHost;

internal static class ApplicationInsightsInitializer
{
    internal static void AddApplicationInsights(this WebApplicationBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = AppInfo.RetrieveInformationalVersion(assembly);
        const string roleName = "ShoppingApp.SiloHost";
        builder.Services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = GlobalConfig.AppInsightsConnectionString;
        });
        builder.Services.ConfigureOpenTelemetryTracerProvider((_, tracerBuilder) =>
            tracerBuilder.ConfigureResource(r => r.AddService(
                serviceName: roleName,
                serviceInstanceId: roleName,
                serviceVersion: version)));
        builder.Services.ConfigureOpenTelemetryLoggerProvider((_, loggerBuilder) =>
            loggerBuilder.ConfigureResource(r => r.AddService(
                serviceName: roleName,
                serviceInstanceId: roleName,
                serviceVersion: version)));
    }
}
