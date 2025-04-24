using Grpc.Core;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using AgentSystem.Shared;
using OllamaSharp.Models;

namespace AgentSystem.PlannerService.Services;

public class PlannerService : Planner.PlannerBase
{
    private readonly Kernel _kernel;
    private readonly IOllamaApiClient _ollamaClient;

    public PlannerService(Kernel kernel, [FromKeyedServices("chat")] IOllamaApiClient ollamaClient)
    {
        _kernel = kernel;
        _ollamaClient = ollamaClient;
    }

    public override async Task<PlanResponse> PlanTask(TaskRequest request, ServerCallContext context)
    {
        var prompt = $"Given the objective: {request.Objective}\nContext: {request.Context}\nGenerate a detailed plan to achieve the objective.";

        // Use Ollama if API key is provided, else fall back to Semantic Kernel
        string plan = string.Empty;
        if (!string.IsNullOrEmpty(request.Objective))
        {
            try
            {
                var generateRequest = new GenerateRequest { Prompt = prompt };
                await foreach (var response in _ollamaClient.GenerateAsync(generateRequest))
                {
                    if (response?.Model != null)
                    {
                        plan += response.Model;
                    }
                }
            }
            catch (HttpRequestException)
            {
                // Fallback to Semantic Kernel if Ollama fails
                var result = await _kernel.InvokePromptAsync(prompt);
                plan = result.GetValue<string>() ?? string.Empty;
            }
        }
        else
        {
            var result = await _kernel.InvokePromptAsync(prompt);
            plan = result.GetValue<string>() ?? string.Empty;
        }

        return new PlanResponse
        {
            Plan = plan,
            IsComplete = !string.IsNullOrEmpty(plan)
        };
    }
}
