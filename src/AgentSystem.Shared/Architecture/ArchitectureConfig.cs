namespace AgentSystem.Shared.Architecture;

/// <summary>
/// Represents the architecture configuration model that maps to YAML files
/// </summary>
public class ArchitectureConfig
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ServiceConfig[] Services { get; set; } = Array.Empty<ServiceConfig>();
    public DependencyConfig[] Dependencies { get; set; } = Array.Empty<DependencyConfig>();
    public Dictionary<string, string> Properties { get; set; } = new();
}

/// <summary>
/// Configuration for a service within the architecture
/// </summary>
public class ServiceConfig
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EndpointConfig[] Endpoints { get; set; } = Array.Empty<EndpointConfig>();
    public Dictionary<string, string> Configuration { get; set; } = new();
}

/// <summary>
/// Configuration for an endpoint exposed by a service
/// </summary>
public class EndpointConfig
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // REST, gRPC, etc.
    public string Path { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Configuration for dependencies between services
/// </summary>
public class DependencyConfig
{
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "uses", "requires", etc.
}
