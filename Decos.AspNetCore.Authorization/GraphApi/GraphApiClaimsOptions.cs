using System;

namespace Decos.AspNetCore.Authorization.GraphApi
{
    /// <summary>
    /// Represents the options used to control the Graph API claims.
    /// </summary>
    public class GraphApiClaimsOptions
    {
        /// <summary>
        /// Gets or sets the scopes to request for making Graph API calls.
        /// </summary>
        public string[] RequestedScopes { get; set; } = new[] { Scopes.User.Read, Scopes.Directory.ReadAll };

        /// <summary>
        /// Gets or sets the claim type to use for storing Active Directory group IDs.
        /// </summary>
        public string GroupsClaimType { get; set; } = "groups";

        /// <summary>
        /// Gets or sets a value indicating whether to include only security groups rather than all
        /// groups the user is a member of.
        /// </summary>
        public bool SecurityGroupsOnly { get; set; }
    }
}