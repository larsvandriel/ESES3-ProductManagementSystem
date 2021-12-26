using LoggingService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.API.Filters;
using ProductManagementSystem.Contracts;
using ProductManagementSystem.Entities;
using ProductManagementSystem.Entities.Helpers;
using ProductManagementSystem.Entities.Models;
using ProductManagementSystem.Repository;

namespace ProductManagementSystem.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureMsSqlContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["mssqlconnection:connectionString"];
            services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connectionString));
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<ISortHelper<Brand>, SortHelper<Brand>>();
            services.AddScoped<IDataShaper<Brand>, DataShaper<Brand>>();
            services.AddScoped<ISortHelper<Category>, SortHelper<Category>>();
            services.AddScoped<IDataShaper<Category>, DataShaper<Category>>();
            services.AddScoped<ISortHelper<Product>, SortHelper<Product>>();
            services.AddScoped<IDataShaper<Product>, DataShaper<Product>>();
            services.AddScoped<ISortHelper<Supplier>, SortHelper<Supplier>>();
            services.AddScoped<IDataShaper<Supplier>, DataShaper<Supplier>>();
            services.AddScoped<ISortHelper<SupplierOffer>, SortHelper<SupplierOffer>>();
            services.AddScoped<IDataShaper<SupplierOffer>, DataShaper<SupplierOffer>>();

            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

        public static void RegisterFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidateMediaTypeAttribute>();
        }

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.eses2.hateoas+json");
                }
                var xmlOutputFormatter = config.OutputFormatters.OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

                if (xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.eses2.hateoas+xml");
                }
            });
        }
    }
}
