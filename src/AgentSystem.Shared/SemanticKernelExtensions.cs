using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentSystem.Shared
{
	public static class AIExtensions
	{
		public static IServiceCollection AddAgentSystem(this IServiceCollection services)
		{
			services.AddSingleton<IKernelBuilder>(p =>
			{
				var kernel = Kernel.CreateBuilder();
				return kernel;
			});
			return services;
		}
	}
}
