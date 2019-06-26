using System;

namespace Decos.AspNetCore.Authorization.GraphApi
{
    /// <summary>
    /// Provides scopes used by the Graph API.
    /// </summary>
    public static class Scopes
    {
        /// <summary>
        /// Provides scopes for user permissions.
        /// </summary>
        public static class User
        {
            /// <summary>
            /// Allows users to sign-in to the app, and allows the app to read the profile of
            /// signed-in users. It also allows the app to read basic company information of
            /// signed-in users.
            /// </summary>
            public const string Read = "User.Read";
        }

        /// <summary>
        /// Provides scopes for directory permissions.
        /// </summary>
        public static class Directory
        {
            /// <summary>
            /// Allows the app to read data in the organization's directory, such as users, groups
            /// and apps.
            /// </summary>
            public const string ReadAll = "Directory.Read.All";
        }
    }
}