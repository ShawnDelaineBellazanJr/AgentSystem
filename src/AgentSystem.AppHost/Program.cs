var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AgentSystem_PlannerService>("agentsystem-plannerservice");

builder.AddProject<Projects.AgentSystem_ExecutorService>("agentsystem-executorservice");

builder.AddProject<Projects.AgentSystem_EvaluatorService>("agentsystem-evaluatorservice");

builder.Build().Run();
