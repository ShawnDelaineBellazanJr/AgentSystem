using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AgentSystem.Shared.Architecture;

/// <summary>
/// Scans for and processes architecture YAML files throughout the project
/// </summary>
public class ArchitectureScanner
{
    private readonly ILogger<ArchitectureScanner> _logger;
    private readonly ArchitectureValidator _validator;
    private readonly IDeserializer _yamlDeserializer;

    public ArchitectureScanner(ILogger<ArchitectureScanner> logger, ArchitectureValidator validator)
    {
        _logger = logger;
        _validator = validator;
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    /// <summary>
    /// Scans for architecture YAML files in the specified directory and its subdirectories
    /// </summary>
    public IEnumerable<ArchitectureConfig> ScanDirectory(string directory)
    {
        _logger.LogInformation("Scanning directory {Directory} for architecture files", directory);
        
        List<ArchitectureConfig> configs = new();
        
        try
        {
            string[] yamlFiles = Directory.GetFiles(directory, "architecture.yaml", SearchOption.AllDirectories);
            string[] ymlFiles = Directory.GetFiles(directory, "architecture.yml", SearchOption.AllDirectories);
            
            IEnumerable<string> allFiles = yamlFiles.Concat(ymlFiles);

            foreach (string file in allFiles)
            {
                _logger.LogInformation("Found architecture file: {File}", file);
                ArchitectureConfig? config = LoadArchitectureFile(file);
                
                if (config != null)
                {
                    configs.Add(config);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning for architecture files");
        }
        
        return configs;
    }

    /// <summary>
    /// Loads and parses a single architecture YAML file
    /// </summary>
    public ArchitectureConfig? LoadArchitectureFile(string filePath)
    {
        try
        {
            string yaml = File.ReadAllText(filePath);
            var config = _yamlDeserializer.Deserialize<ArchitectureConfig>(yaml);
            
            _validator.Validate(config);
            _logger.LogInformation("Successfully loaded architecture file: {File}", filePath);
            
            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading architecture file: {File}", filePath);
            return null;
        }
    }

    /// <summary>
    /// Applies the architecture configurations to the system
    /// </summary>
    public void ApplyArchitecture(IEnumerable<ArchitectureConfig> configs)
    {
        foreach (var config in configs)
        {
            _logger.LogInformation("Applying architecture configuration: {Name} v{Version}", 
                config.Name, config.Version);
            
            // Architecture application logic would be implemented here
            // This could include setting up service registration, validation, etc.
        }
    }
}
