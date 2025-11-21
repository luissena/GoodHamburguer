using GoodHamburguer.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCorsPolicies();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddRepositoryServices();

var app = builder.Build();

await app.SeedDataAsync();
app.UseSwaggerDocumentation();
app.UseConfiguredCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();