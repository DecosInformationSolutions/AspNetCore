using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.Client;

namespace Decos.AspNetCore.Authorization.GraphApi
{
    /// <summary>
    /// Provides claims for the currently authenticated user.
    /// </summary>
    public class GraphApiClaimsHandler : IGraphApiClaimsHandler
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IGraphApiClient _graphApiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphApiClaimsHandler"/> class with the
        /// specified token acquisition and options,
        /// </summary>
        /// <param name="tokenAcquisition">
        /// Defines a method for acquiring a new token to authenticate Graph API requests.
        /// </param>
        /// <param name="options">Provides options used to control the Graph API claims.</param>
        public GraphApiClaimsHandler(ITokenAcquisition tokenAcquisition, IGraphApiClient graphApiClient, IOptions<GraphApiClaimsOptions> options)
        {
            if (tokenAcquisition == null)
                throw new ArgumentNullException(nameof(tokenAcquisition));

            _tokenAcquisition = tokenAcquisition;
            _graphApiClient = graphApiClient;
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
                var claimsIdentity = httpContext.User.Identity as ClaimsIdentity;
                if (claimsIdentity?.IsAuthenticated != true)
                    return true;

                var accessToken = await _tokenAcquisition.GetAccessTokenOnBehalfOfUser(httpContext, Options.RequestedScopes).ConfigureAwait(false);
                await AddGroupsClaimsAsync(claimsIdentity, accessToken, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (MsalUiRequiredException ex)
            {
                _tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeader(httpContext, Options.RequestedScopes, ex);
                return false;
            }
        }

        private async Task AddGroupsClaimsAsync(ClaimsIdentity claimsIdentity, string accessToken, CancellationToken cancellationToken)
        {
            var groupIds = await GetMemberGroupsAsync(accessToken, cancellationToken).ConfigureAwait(false);
            foreach (var groupId in groupIds)
                claimsIdentity.AddClaim(new Claim(Options.GroupsClaimType, groupId));
        }

        private async Task<IEnumerable<string>> GetMemberGroupsAsync(string accessToken, CancellationToken cancellationToken)
        {
            if (accessToken == null)
                throw new ArgumentNullException(nameof(accessToken));

            var response = await _graphApiClient.GetMemberGroupsAsync(accessToken, new GetMemberGroupsRequest { SecurityEnabledOnly = Options.SecurityGroupsOnly }, cancellationToken);
            return response.Value;
        }
    }
}