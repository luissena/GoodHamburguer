using GoodHamburguer.Api.Repositories;
using GoodHamburguer.Api.Data;
using GoodHamburguer.Domain.Configuration;
using GoodHamburguer.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5218", "https://localhost:7018")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "GoodHamburguer API",
        Version = "v1",
    
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.EnableAnnotations();
    
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Default" });
    c.DocInclusionPredicate((name, api) => true);
});

builder.Services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<ICartRepository, InMemoryCartRepository>();
builder.Services.AddSingleton<IDiscountRepository, InMemoryDiscountRepository>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<IOrderQuantityRuleRepository, InMemoryOrderQuantityRuleRepository>();

var app = builder.Build();

await SeedInitialDataAsync(app.Services, app.Logger);
await SeedDefaultQuantityRulesAsync(app.Services, app.Logger);

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoodHamburguer API v1");
    c.RoutePrefix = string.Empty;
    c.DocumentTitle = "GoodHamburguer API Documentation";
    c.DefaultModelsExpandDepth(-1);
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
});

app.UseCors("AllowBlazorClient");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

static async Task SeedInitialDataAsync(IServiceProvider services, ILogger logger)
{
    try
    {
        var categoryRepository = services.GetRequiredService<ICategoryRepository>();
        var productRepository = services.GetRequiredService<IProductRepository>();
        var discountRepository = services.GetRequiredService<IDiscountRepository>();

        await DataSeeder.SeedAsync(categoryRepository, productRepository, discountRepository);

        logger.LogInformation("Initial data (categories, products and discounts) were created.");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Error creating initial data.");
    }
}

static async Task SeedDefaultQuantityRulesAsync(IServiceProvider services, ILogger logger)
{
    try
    {
        var ruleRepository = services.GetRequiredService<IOrderQuantityRuleRepository>();
        var categoryRepository = services.GetRequiredService<ICategoryRepository>();
        var productRepository = services.GetRequiredService<IProductRepository>();

        var existingRules = await ruleRepository.GetAllAsync();
        if (existingRules.Any())
        {
            return;
        }

        var allCategories = await categoryRepository.GetAllAsync();
        var allProducts = await productRepository.GetAllAsync();

        Guid? GetCategoryIdByName(string name)
        {
            var category = allCategories.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return category?.Id;
        }

        Guid? GetProductIdByName(string name)
        {
            var product = allProducts.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return product?.Id;
        }

        var defaultRules = OrderQuantityRulesConfiguration.GetDefaultRules(GetCategoryIdByName, GetProductIdByName);
        
        foreach (var rule in defaultRules)
        {
            await ruleRepository.CreateAsync(rule);
        }

        logger.LogInformation("Default quantity rules were created.");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Error creating default quantity rules. Rules can be created manually via API.");
    }
}
