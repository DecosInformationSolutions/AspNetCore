using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Decos.AspNetCore.Authorization.GraphApi
{
    /// <summary>
    /// Defines a method for adding claims to an authenticated user.
    /// </summary>
    public interface IGraphApiClaimsHandler
    {
        /// <summary>
        /// Adds claims for the currently authenticated user and returns a value indicating whether
        /// the pipeline should continue processing or not.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// <c>true</c> if the next middleware should run; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> AddClaimsAsync(HttpContext httpContext, CancellationToken cancellationToken);
    }
}