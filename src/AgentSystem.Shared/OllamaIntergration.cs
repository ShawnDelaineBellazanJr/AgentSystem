using OllamaSharp;
using OllamaSharp.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentSystem.Shared
{
	public class OllamaIntergration
	{
		private readonly IOllamaApiClient _ollama;

		public OllamaIntergration(IOllamaApiClient ollama)
		{
			_ollama = ollama;
		}

		public async Task<string> ChatAsync(ChatRequest request)
		{
			// Fix for CS1061: Use 'await foreach' to iterate over IAsyncEnumerable
			await foreach (var responseStream in _ollama.ChatAsync(request))
			{
				if (responseStream != null)
				{
					return responseStream.Message.Content.ToString(); // Assuming 'Content' is a property in ChatResponseStream
				}
			}

			return "No response from Ollama.";
		}
	}
}
