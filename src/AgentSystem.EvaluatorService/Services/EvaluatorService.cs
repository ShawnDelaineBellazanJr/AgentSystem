using Grpc.Core;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using AgentSystem.Shared;
using OllamaSharp.Models;

namespace AgentSystem.EvaluatorService.Services;

public class EvaluatorService : Evaluator.EvaluatorBase
{
    private readonly Kernel _kernel;
    private readonly IOllamaApiClient _ollamaClient;

    public EvaluatorService(Kernel kernel, [FromKeyedServices("embeddings")] IOllamaApiClient ollamaClient)
    {
        _kernel = kernel;
        _ollamaClient = ollamaClient;
    }

	public override async Task<EvaluateResponse> EvaluateResult(EvaluateRequest request, ServerCallContext context)
	{
		var prompt = $"Objective: {request.Objective}\nResult: {request.Result}\nEvaluate if the objective is met. Provide feedback.";
		string feedback = string.Empty;

		// Use Ollama if API key is provided, else fall back to Semantic Kernel
		if (!string.IsNullOrEmpty(request.Objective))
		{
			try
			{
				var generateRequest = new GenerateRequest { Prompt = prompt };
				await foreach (var response in _ollamaClient.GenerateAsync(generateRequest))
				{
					if (response?.Model != null)
					{
						feedback = response.Model;
						break;
					}
				}
			}
			catch (HttpRequestException)
			{
				var result = await _kernel.InvokePromptAsync(prompt);
				feedback = result.GetValue<string>() ?? string.Empty;
			}
		}
		else
		{
			var result = await _kernel.InvokePromptAsync(prompt);
			feedback = result.GetValue<string>() ?? string.Empty;
		}

		return new EvaluateResponse
		{
			IsGoalMet = feedback.Contains("Objective met", StringComparison.OrdinalIgnoreCase),
			Feedback = feedback
		};
	}
}
