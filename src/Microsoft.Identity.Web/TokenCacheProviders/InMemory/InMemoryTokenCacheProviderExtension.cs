// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Identity.Web.TokenCacheProviders.InMemory
{
    /// <summary>
    /// Extension class used to add an in-memory token cache serializer to MSAL
    /// </summary>
    public static class InMemoryTokenCacheProviderExtension
    {
        /// <summary>Adds both the app and per-user in-memory token caches.</summary>
        /// <param name="services">The services collection to add to.</param>
        /// <param name="cacheOptions">The MSALMemoryTokenCacheOptions allows the caller to set the token cache expiration</param>
        /// <param name="configuration">The Configuration object</param>
        /// <returns></returns> 
        public static IServiceCollection AddInMemoryTokenCaches(
            this IServiceCollection services, IConfiguration configuration)
        {      
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.Configure<MsalMemoryTokenCacheOptions>(configuration.GetSection("MsalMemoryTokenCacheOptions"));
            services.AddSingleton<IMsalTokenCacheProvider, MsalMemoryTokenCacheProvider>();
            return services;
        }
    }
}