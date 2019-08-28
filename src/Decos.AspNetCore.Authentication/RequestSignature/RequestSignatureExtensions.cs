using System;
using System.Collections.Generic;
using System.Text;
using Decos.AspNetCore.Authentication.RequestSignature;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RequestSignatureExtensions
    {
        public static AuthenticationBuilder AddRequestSignature(this AuthenticationBuilder builder)
            => builder.AddRequestSignature(RequestSignatureDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddRequestSignature(this AuthenticationBuilder builder, Action<RequestSignatureOptions> configureOptions)
            => builder.AddRequestSignature(RequestSignatureDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddRequestSignature(this AuthenticationBuilder builder, string authenticationScheme, Action<RequestSignatureOptions> configureOptions)
            => builder.AddRequestSignature(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddRequestSignature(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<RequestSignatureOptions> configureOptions)
        {
            return builder.AddScheme<RequestSignatureOptions, RequestSignatureHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
