using GoodHamburguer.Api.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCorsPolicies();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddRepositoryServices();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API is healthy"));

var app = builder.Build();

await app.SeedDataAsync();
app.UseSwaggerDocumentation();
app.UseConfiguredCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks();

app.Run();