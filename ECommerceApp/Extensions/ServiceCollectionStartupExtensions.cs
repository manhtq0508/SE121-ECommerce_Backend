using System.Text;
using ECommerceApp.Data;
using ECommerceApp.Filters;
using ECommerceApp.Mappings.Addresses;
using ECommerceApp.Mappings.Cancellations;
using ECommerceApp.Mappings.Carts;
using ECommerceApp.Mappings.Categories;
using ECommerceApp.Mappings.Customers;
using ECommerceApp.Mappings.Feedbacks;
using ECommerceApp.Mappings.Orders;
using ECommerceApp.Mappings.Payments;
using ECommerceApp.Mappings.Products;
using ECommerceApp.Mappings.Refunds;
using ECommerceApp.Repositories.Implements;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Caching;
using ECommerceApp.Services.Implements;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace ECommerceApp.Extensions;

public static class ServiceCollectionStartupExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        string bearerSecurityScheme)
    {
        services.AddApiControllers();
        services.AddOpenApiDocumentation(bearerSecurityScheme);
        services.AddApplicationDbContext(configuration);
        services.AddApplicationCache(configuration);
        services.AddJwtAuthentication(configuration);
        services.AddRepositories();
        services.AddDomainServices();
        services.AddUnitOfWork();
        services.AddMappers();

        return services;
    }

    private static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddScoped<ControllerActionLoggingFilter>();
        services.AddControllers(options => options.Filters.Add<ControllerActionLoggingFilter>());

        return services;
    }

    private static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, string bearerSecurityScheme)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes[bearerSecurityScheme] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                };

                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, context, cancellationToken) =>
            {
                var metadata = context.Description.ActionDescriptor.EndpointMetadata;
                var requiresAuthorization = metadata.OfType<IAuthorizeData>().Any();
                var allowsAnonymous = metadata.OfType<IAllowAnonymous>().Any();

                if (!requiresAuthorization || allowsAnonymous)
                {
                    return Task.CompletedTask;
                }

                operation.Security ??= [];
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(bearerSecurityScheme, context.Document, null)] = []
                });

                return Task.CompletedTask;
            });
        });

        return services;
    }

    private static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("EFCoreDBConnection")));

        return services;
    }

    private static IServiceCollection AddApplicationCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));

        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = configuration["Redis:InstanceName"] ?? "ECommerceApp:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT configuration is missing.")))
                };
            });

        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ICancellationRepository, CancellationRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IRefundRepository, RefundRepository>();

        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICancellationService, CancellationService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<IRefundService, RefundService>();

        services.AddHostedService<PendingPaymentService>();
        services.AddHostedService<RefundProcessingBackgroundService>();

        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddScoped<IAddressMapper, AddressMapper>();
        services.AddScoped<IProductMapper, ProductMapper>();
        services.AddScoped<ICategoryMapper, CategoryMapper>();
        services.AddScoped<ICustomerMapper, CustomerMapper>();
        services.AddScoped<IOrderMapper, OrderMapper>();
        services.AddScoped<ICartMapper, CartMapper>();
        services.AddScoped<IFeedbackMapper, FeedbackMapper>();
        services.AddScoped<ICancellationMapper, CancellationMapper>();
        services.AddScoped<IRefundMapper, RefundMapper>();
        services.AddScoped<IPaymentMapper, PaymentMapper>();

        return services;
    }
}
