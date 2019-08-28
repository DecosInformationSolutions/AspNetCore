using System.Threading;
using System.Threading.Tasks;

namespace Decos.AspNetCore.Authorization.GraphApi
{
    /// <summary>
    /// Defines methods in the Microsoft Graph API.
    /// </summary>
    public interface IGraphApiClient
    {
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
        Task<GetMemberGroupsResponse> GetMemberGroupsAsync(string accessToken, GetMemberGroupsRequest requestBody, CancellationToken cancellationToken);
    }
}