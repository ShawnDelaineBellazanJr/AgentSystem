using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.AddServiceDefaults();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(static c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "My Aspire API",
		Version = "v1",
		Description = "A .NET Aspire API with comprehensive Swagger integration",
		Contact = new OpenApiContact
		{
			Name = "API Support",
			Email = "support@example.com",
			Url = new Uri("https://example.com/support")
		},
		License = new OpenApiLicense
		{
			Name = "MIT License",
			Url = new Uri("https://opensource.org/licenses/MIT")
		}
	});

	// Enable annotations

	// Set the comments path for the Swagger JSON and UI
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	c.IncludeXmlComments(xmlPath);

	// Add security definition and requirement (if you need authentication)
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});



	// Use fully qualified object names
	c.CustomSchemaIds(type => type.FullName);
});
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Aspire API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();