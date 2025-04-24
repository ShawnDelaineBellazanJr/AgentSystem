using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OllamaSharp;
using AgentSystem.Shared.Architecture;

namespace AgentSystem.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeyedOllamaClientApi(this IServiceCollection services, string name, string baseUrl, string apiKey)
    {
        services.AddKeyedSingleton<IOllamaApiClient>(name, new OllamaApiClient(baseUrl, apiKey));
        return services;
    }

    /// <summary>
    /// Adds architecture services to the service collection
    /// </summary>
    public static IServiceCollection AddArchitectureServices(this IServiceCollection services)
    {
        services.AddSingleton<ArchitectureValidator>();
        services.AddSingleton<ArchitectureScanner>();
        
        return services;
    }

    /// <summary>
    /// Applies architecture configurations from YAML files in the application directory
    /// </summary>
    public static IServiceCollection ApplyArchitectureConfiguration(
        this IServiceCollection services, string baseDirectory)
    {
        // Create a service provider to get required services
        var serviceProvider = services.BuildServiceProvider();
        var scanner = serviceProvider.GetRequiredService<ArchitectureScanner>();
        
        // Scan for architecture files and apply them
        var configs = scanner.ScanDirectory(baseDirectory);
        scanner.ApplyArchitecture(configs);
        
        return services;
    }
}
