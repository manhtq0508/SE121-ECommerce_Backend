using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace ECommerceApp.Extensions;

public static class SerilogStartupExtensions
{
    public static ConfigurationManager AddSerilogEnvironmentOverrides(this ConfigurationManager configuration)
    {
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Serilog:Properties:Application"] = configuration["SERILOG_APPLICATION"],
            ["Serilog:Properties:Server"] = configuration["SERILOG_SERVER"],
            ["Serilog:MinimumLevel:Default"] = configuration["SERILOG_MINIMUM_LEVEL"],
            ["Serilog:MinimumLevel:Override:Microsoft"] = configuration["SERILOG_MINIMUM_LEVEL_MICROSOFT"],
            ["Serilog:MinimumLevel:Override:System"] = configuration["SERILOG_MINIMUM_LEVEL_SYSTEM"]
        });

        return configuration;
    }

    public static ConfigureHostBuilder UseApplicationSerilog(this ConfigureHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            var elasticSinkOptions = new ElasticsearchSinkOptions(new Uri(context.Configuration["SERILOG_ELASTIC_URI"] ?? "http://elasticsearch:9200"))
            {
                AutoRegisterTemplate = bool.TryParse(context.Configuration["SERILOG_ELASTIC_AUTO_REGISTER_TEMPLATE"], out var autoRegisterTemplate) && autoRegisterTemplate,
                IndexFormat = context.Configuration["SERILOG_ELASTIC_INDEX_FORMAT"] ?? "ecommerceapp-logs-{0:yyyy.MM}"
            };

            var elasticAuthHeader = CreateBasicAuthHeader(
                context.Configuration["SERILOG_ELASTIC_USERNAME"],
                context.Configuration["SERILOG_ELASTIC_PASSWORD"]);

            if (!string.IsNullOrWhiteSpace(elasticAuthHeader))
            {
                elasticSinkOptions.ModifyConnectionSettings = connectionConfiguration => connectionConfiguration.GlobalHeaders(
                    new System.Collections.Specialized.NameValueCollection
                    {
                        ["Authorization"] = elasticAuthHeader
                    });
            }

            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProperty("Application", context.Configuration["Serilog:Properties:Application"] ?? "ECommerceApp")
                .Enrich.WithProperty("Server", context.Configuration["Serilog:Properties:Server"] ?? Environment.MachineName)
                .WriteTo.Elasticsearch(elasticSinkOptions);
        });

        return hostBuilder;
    }

    private static string? CreateBasicAuthHeader(string? userName, string? password)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userName}:{password}"));
        return $"Basic {credentials}";
    }
}
