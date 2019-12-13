using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace Decos.AspNetCore.Authorization.GraphApi
{
    /// <summary>
    /// Provides claims for the currently authenticated user.
    /// </summary>
    public class GraphApiClaimsHandler : IGraphApiClaimsHandler
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IGraphApiClient _graphApiClient;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphApiClaimsHandler"/> class with the
        /// specified token acquisition and options,
        /// </summary>
        /// <param name="tokenAcquisition">
        /// Defines a method for acquiring a new token to authenticate Graph API requests.
        /// </param>
        /// <param name="graphApiClient">A client used to access the Graph API.</param>
        /// <param name="cache">A cache to temporarily store Graph API results in.</param>
        /// <param name="options">Provides options used to control the Graph API claims.</param>
        public GraphApiClaimsHandler(ITokenAcquisition tokenAcquisition, IGraphApiClient graphApiClient, IMemoryCache cache, IOptions<GraphApiClaimsOptions> options)
        {
            if (tokenAcquisition == null)
                throw new ArgumentNullException(nameof(tokenAcquisition));

            _tokenAcquisition = tokenAcquisition;
            _graphApiClient = graphApiClient;
            _cache = cache;
            Options = options.Value;
        }

        /// <summary>
        /// Gets the options used to control the Graph API claims.
        /// </summary>
        protected GraphApiClaimsOptions Options { get; }

        /// <summary>
        /// Adds claims for the currently authenticated user and returns a value indicating whether
        /// the pipeline should continue processing or not.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// <c>true</c> if the next middleware should run; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> AddClaimsAsync(HttpContext httpContext, CancellationToken cancellationToken)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            try
            {
                var claimsPrincipal = httpContext.User as ClaimsPrincipal;
                if (claimsPrincipal?.Identity?.IsAuthenticated != true)
                    return true;

                var accessToken = await _tokenAcquisition.GetAccessTokenOnBehalfOfUserAsync(Options.RequestedScopes).ConfigureAwait(false);
                await AddGroupsClaimsAsync(claimsPrincipal, accessToken, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (MsalUiRequiredException ex)
            {
                _tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeader(Options.RequestedScopes, ex);
                return false;
            }
        }

        private async Task AddGroupsClaimsAsync(ClaimsPrincipal claimsPrincipal, string accessToken, CancellationToken cancellationToken)
        {
            var cacheKey = $"{claimsPrincipal.GetObjectId()}:{Options.GroupsClaimType}";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<string> groupIds))
            {
                groupIds = await GetMemberGroupsAsync(accessToken, cancellationToken).ConfigureAwait(false);
                _cache.Set(cacheKey, groupIds, TimeSpan.FromHours(1));
            }

            var claimsIdentity = (ClaimsIdentity)claimsPrincipal.Identity;
            foreach (var groupId in groupIds)
                claimsIdentity.AddClaim(new Claim(Options.GroupsClaimType, groupId));
        }

        private async Task<IEnumerable<string>> GetMemberGroupsAsync(string accessToken, CancellationToken cancellationToken)
        {
            if (accessToken == null)
                throw new ArgumentNullException(nameof(accessToken));

            var response = await _graphApiClient.GetMemberGroupsAsync(
                accessToken,
                new GetMemberGroupsRequest { SecurityEnabledOnly = Options.SecurityGroupsOnly },
                cancellationToken).ConfigureAwait(false);
            return response.Value;
        }
    }
}