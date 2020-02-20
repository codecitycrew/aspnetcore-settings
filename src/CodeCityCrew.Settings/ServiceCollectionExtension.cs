using System;
using CodeCityCrew.Settings.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeCityCrew.Settings
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds the settings.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public static void AddSettings(this IServiceCollection services,
            string connectionStringName = "DefaultConnection")
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            services.AddDbContext<SettingDbContext>(
                options => { options.UseSqlServer(configuration.GetConnectionString(connectionStringName)); },
                ServiceLifetime.Singleton);

            services.AddSingleton<ISettingService, SettingService>();
        }

        /// <summary>
        /// Adds the settings.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The database context options builder.</param>
        public static void AddSettings(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            services.AddDbContext<SettingDbContext>(optionsAction, ServiceLifetime.Singleton);

            services.AddSingleton<ISettingService, SettingService>();
        }

        /// <summary>
        /// Settings the specified services. Use this method only in your Startup Configure service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static T Setting<T>(this IServiceCollection services) where T : new()
        {
            return services.BuildServiceProvider().GetRequiredService<ISettingService>().As<T>();
        }

        /// <summary>
        /// Settings the specified application. Use this method only in your Startup Configure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static T Setting<T>(this IApplicationBuilder app) where T : new()
        {
            return app.ApplicationServices.GetRequiredService<ISettingService>().As<T>();
        }
    }
}
