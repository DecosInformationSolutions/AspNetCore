using System;
using System.Threading.Tasks;

using Decos.AspNetCore.Authorization.GraphApi;

using Microsoft.AspNetCore.Http;

namespace Decos.AspNetCore.Authorization
{
    /// <summary>
    /// Represents an ASP.NET Core middleware that adds claims to an authenticated user based on the
    /// Microsoft Graph API.
    /// </summary>
    public class GraphApiClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphApiClaimsMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public GraphApiClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Adds claims to an authenticated user.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
        /// <param name="handler">An <see cref="IGraphApiClaimsHandler"/> instance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Invoke(HttpContext httpContext, IGraphApiClaimsHandler handler)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (httpContext.User.Identity.IsAuthenticated)
            {
                var shouldContinue = await handler.AddClaimsAsync(httpContext, httpContext.RequestAborted).ConfigureAwait(false);
                if (!shouldContinue)
                    return;
            }

            await _next(httpContext).ConfigureAwait(false);
        }
    }
}