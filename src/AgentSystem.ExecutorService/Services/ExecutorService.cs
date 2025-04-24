using AgentSystem.Shared;
using Grpc.Core;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using OllamaSharp.Models;

namespace AgentSystem.ExecutorService.Services;

public class ExecutorService : Executor.ExecutorBase
{
    private readonly Kernel _kernel;
    private readonly IOllamaApiClient _ollamaClient;

    public ExecutorService(Kernel kernel, [FromKeyedServices("chat")] IOllamaApiClient ollamaClient)
    {
        _kernel = kernel;
        _ollamaClient = ollamaClient;
    }

    public override async Task<ExecuteResponse> ExecuteTask(ExecuteRequest request, ServerCallContext context)
    {
        var prompt = $"Given the plan: {request.Plan}\nExecute the plan and provide the result.";
        
        // Use Ollama if available, else fall back to Semantic Kernel
        string result = string.Empty;
        if (!string.IsNullOrEmpty(request.Plan))
        {
            try
            {
                var generateRequest = new GenerateRequest { Prompt = prompt };
                await foreach (var response in _ollamaClient.GenerateAsync(generateRequest))
                {
                    if (response?.Model != null)
                    {
                        result += response.Model;
                    }
                }
            }
            catch (HttpRequestException)
            {
                var skResult = await _kernel.InvokePromptAsync(prompt);
                result = skResult.GetValue<string>() ?? string.Empty;
            }
        }
        else
        {
            var skResult = await _kernel.InvokePromptAsync(prompt);
            result = skResult.GetValue<string>() ?? string.Empty;
        }
        
        return new ExecuteResponse
        {
            Result = result
        };
    }
}

