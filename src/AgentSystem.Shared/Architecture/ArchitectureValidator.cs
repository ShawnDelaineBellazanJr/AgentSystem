using Microsoft.Extensions.Logging;

namespace AgentSystem.Shared.Architecture;

/// <summary>
/// Validates architecture configuration files for correctness and consistency
/// </summary>
public class ArchitectureValidator
{
    private readonly ILogger<ArchitectureValidator> _logger;

    public ArchitectureValidator(ILogger<ArchitectureValidator> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Validates a single architecture configuration
    /// </summary>
    /// <param name="config">The configuration to validate</param>
    /// <returns>True if the configuration is valid, otherwise false</returns>
    public bool Validate(ArchitectureConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.Name))
        {
            _logger.LogError("Architecture configuration is missing a name");
            return false;
        }

        if (string.IsNullOrWhiteSpace(config.Version))
        {
            _logger.LogWarning("Architecture configuration is missing a version");
        }

        // Validate services
        foreach (var service in config.Services)
        {
            if (string.IsNullOrWhiteSpace(service.Name))
            {
                _logger.LogError("Service in {ConfigName} is missing a name", config.Name);
                return false;
            }

            if (string.IsNullOrWhiteSpace(service.Type))
            {
                _logger.LogError("Service {ServiceName} in {ConfigName} is missing a type", 
                    service.Name, config.Name);
                return false;
            }
        }

        // Validate dependencies
        foreach (var dependency in config.Dependencies)
        {
            if (string.IsNullOrWhiteSpace(dependency.Source))
            {
                _logger.LogError("Dependency in {ConfigName} is missing a source", config.Name);
                return false;
            }

            if (string.IsNullOrWhiteSpace(dependency.Target))
            {
                _logger.LogError("Dependency in {ConfigName} is missing a target", config.Name);
                return false;
            }

            // Check that source and target services exist
            bool sourceExists = config.Services.Any(s => s.Name == dependency.Source);
            if (!sourceExists)
            {
                _logger.LogWarning("Dependency source {Source} in {ConfigName} does not reference an existing service", 
                    dependency.Source, config.Name);
            }

            bool targetExists = config.Services.Any(s => s.Name == dependency.Target);
            if (!targetExists)
            {
                _logger.LogWarning("Dependency target {Target} in {ConfigName} does not reference an existing service", 
                    dependency.Target, config.Name);
            }
        }

        return true;
    }

    /// <summary>
    /// Validates multiple architecture configurations for cross-configuration consistency
    /// </summary>
    public bool ValidateMultiple(IEnumerable<ArchitectureConfig> configs)
    {
        bool isValid = true;
        var allConfigs = configs.ToList();
        
        // Check for duplicate service names across configurations
        var serviceGroups = allConfigs
            .SelectMany(c => c.Services)
            .GroupBy(s => s.Name)
            .Where(g => g.Count() > 1);
            
        foreach (var group in serviceGroups)
        {
            _logger.LogWarning("Service name {ServiceName} appears in multiple architecture configurations", group.Key);
        }

        return isValid;
    }
}
