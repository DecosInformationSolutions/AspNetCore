using System;

using Newtonsoft.Json;

namespace Decos.AspNetCore.Authorization.GraphApi
{
    /// <summary>
    /// Represents the request body properties of a <c>GetMemberGroups</c> request.
    /// </summary>
    public class GetMemberGroupsRequest
    {
        /// <summary>
        /// <c>true</c> to specify that only security groups that the user is a member of should be
        /// returned; <c>false</c> to specify that all groups that the user is a member of should be
        /// returned. Note: Setting this parameter to <c>true</c> is only supported when calling this
        /// method on a user.
        /// </summary>
        [JsonProperty("securityEnabledOnly")]
        public bool SecurityEnabledOnly { get; set; }
    }
}