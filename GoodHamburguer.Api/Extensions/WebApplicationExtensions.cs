using GoodHamburguer.Api.Data;
using GoodHamburguer.Domain.Configuration;
using GoodHamburguer.Domain.Repositories;

namespace GoodHamburguer.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task SeedDataAsync(this WebApplication app)
    {
        await SeedInitialDataAsync(app.Services, app.Logger);
        await SeedDefaultQuantityRulesAsync(app.Services, app.Logger);
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
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

        return app;
    }

    public static WebApplication UseConfiguredCors(this WebApplication app)
    {
        app.UseCors(ServiceCollectionExtensions.AllowBlazorClientPolicy);
        return app;
    }

    private static async Task SeedInitialDataAsync(IServiceProvider services, ILogger logger)
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

    private static async Task SeedDefaultQuantityRulesAsync(IServiceProvider services, ILogger logger)
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
                var category =
                    allCategories.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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
}