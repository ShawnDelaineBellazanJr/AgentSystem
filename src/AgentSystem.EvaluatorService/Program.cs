using AgentSystem.EvaluatorService.Services;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add Semantic Kernel
var kernel = Kernel.CreateBuilder()
    .Build();
builder.Services.AddSingleton(kernel);

// Add keyed Ollama clients
//builder.Services.AddKeyedChatClient("");

// Add gRPC service
builder.Services.AddGrpc();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
app.MapGrpcService<EvaluatorService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
