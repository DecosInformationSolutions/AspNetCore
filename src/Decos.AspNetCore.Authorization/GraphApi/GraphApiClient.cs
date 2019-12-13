using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Decos.AspNetCore.Authorization.GraphApi
{
    /// <summary>
    /// Represents a client that uses HTTP requests to access the Microsoft Graph API.
    /// </summary>
    public class GraphApiClient : IGraphApiClient
    {
        private static readonly HttpClient s_httpClient = new HttpClient();
        private static readonly JsonMediaTypeFormatter s_jsonMediaTypeFormatter = new JsonMediaTypeFormatter();

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphApiClient"/> class.
        /// </summary>
        public GraphApiClient()
        {
        }

        /// <summary>
        /// <para>
        /// Return all the groups that the user is a member of. The check is transitive, unlike
        /// reading the memberOf navigation property, which returns only the groups that the user is
        /// a direct member of.
        /// </para>
        /// <para>
        /// This function supports Office 365 and other types of groups provisioned in Azure AD.The
        /// maximum number of groups each request can return is 2046. Note that Office 365 Groups
        /// cannot contain groups. So membership in an Office 365 Group is always direct.
        /// </para>
        /// </summary>
        /// <param name="accessToken">
        /// The access token used to authenticate with the Graph API.
        /// </param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that returns the groups the user is a member of.</returns>
        public async Task<GetMemberGroupsResponse> GetMemberGroupsAsync(string accessToken, GetMemberGroupsRequest requestBody, CancellationToken cancellationToken)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/v1.0/me/getMemberGroups");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            requestMessage.Headers.Accept.ParseAdd("application/json");
            requestMessage.Content = new ObjectContent<GetMemberGroupsRequest>(requestBody, s_jsonMediaTypeFormatter);

            var response = await s_httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsAsync<GetMemberGroupsResponse>(cancellationToken).ConfigureAwait(false);
        }
    }
}