using System.Reflection;
using GoodHamburguer.Api.Repositories;
using GoodHamburguer.Domain.Repositories;
using Microsoft.OpenApi;

namespace GoodHamburguer.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public const string AllowBlazorClientPolicy = "AllowBlazorClient";

    public static IServiceCollection AddCorsPolicies(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(AllowBlazorClientPolicy, policy =>
            {
                policy.WithOrigins("http://localhost:5218", "https://localhost:7018")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "GoodHamburguer API",
                Version = "v1",
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            c.EnableAnnotations();
            c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Default" });
            c.DocInclusionPredicate((name, api) => true);
        });

        return services;
    }

    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        services.AddSingleton<ICartRepository, InMemoryCartRepository>();
        services.AddSingleton<IDiscountRepository, InMemoryDiscountRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IOrderQuantityRuleRepository, InMemoryOrderQuantityRuleRepository>();

        return services;
    }
}

