using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

var builder = DistributedApplication.CreateBuilder(args);

// Add architecture scanner services

// Add Ollama for LLM capabilities
var ollama = builder.AddOllama("ollama")
    .AddModel("llama3");

// Get architecture configurations
var baseDirectory = Directory.GetCurrentDirectory();



// Log found architecture files
//foreach (var config in configs)
//{
//    Console.WriteLine($"Found architecture: {config.Name} v{config.Version}");
//}

// Add gRPC services
var planner = builder.AddProject<Projects.AgentSystem_PlannerService>("planner")
    .WithReference(ollama);
    
var executor = builder.AddProject<Projects.AgentSystem_ExecutorService>("executor")
    .WithReference(ollama);
    
var evaluator = builder.AddProject<Projects.AgentSystem_EvaluatorService>("evaluator")
    .WithReference(ollama);

// Apply architecture configurations
// Optional: Add orchestrator service
var orchestrator = builder.AddProject<Projects.AgentSystem_Orchestrator>("orchestrator")
    .WithReference(planner)
    .WithReference(executor)
    .WithReference(evaluator);

builder.AddProject<Projects.AgentSystem_Orchestrator>("agentsystem-orchestrator");

await builder.Build().RunAsync();
