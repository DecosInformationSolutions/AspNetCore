using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Decos.AspNetCore.Authorization.GraphApi
{
    /// <summary>
    /// Represents the response to a <see cref="GetMemberGroupsRequest"/>.
    /// </summary>
    public class GetMemberGroupsResponse
    {
        /// <summary>
        /// A string collection that contains the IDs of the groups that the user is a member of.
        /// </summary>
        [JsonProperty("value")]
        public IEnumerable<string> Value { get; set; }
    }
}