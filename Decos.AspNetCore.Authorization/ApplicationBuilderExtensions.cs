using System;

using Microsoft.AspNetCore.Builder;

namespace Decos.AspNetCore.Authorization
{
    /// <summary>
    /// Provides a set of static methods for adding the Decos ASP.NET Core authorization middleware
    /// classes.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the Graph API claims middleware to the application's request pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder UseGraphApiClaims(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseMiddleware<GraphApiClaimsMiddleware>();
        }
    }
}