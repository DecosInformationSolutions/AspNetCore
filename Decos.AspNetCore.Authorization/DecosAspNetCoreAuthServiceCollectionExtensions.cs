using System;

using Decos.AspNetCore.Authorization.GraphApi;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides a set of static methods for adding and configuring the Decos ASP.NET Core
    /// authorization services.
    /// </summary>
    public static class DecosAspNetCoreAuthServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the services required for adding Graph API claims.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to add to.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddGraphApiClaims(this IServiceCollection services)
        {
            return services.AddGraphApiClaims(_ => { });
        }

        /// <summary>
        /// Adds and configures the services required for adding Graph API claims.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to add to.</param>
        /// <param name="configureOptions">A delegate for configuring the options.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddGraphApiClaims(this IServiceCollection services, Action<GraphApiClaimsOptions> configureOptions)
        {
            services.TryAddScoped<IGraphApiClaimsHandler, GraphApiClaimsHandler>();
            services.TryAddScoped<IGraphApiClient, GraphApiClient>();
            services.Configure(configureOptions);
            return services;
        }
    }
}