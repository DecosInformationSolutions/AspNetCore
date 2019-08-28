using System;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Decos.AspNetCore.Authentication.RequestSignature
{
    public class RequestSignatureHandler : AuthenticationHandler<RequestSignatureOptions>
    {
        public RequestSignatureHandler(IOptionsMonitor<RequestSignatureOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var requestFeature = Context.Features.Get<IHttpRequestFeature>();
            // keyId: used to look up the key, e.g. API key
            // algorithm: hmac-sha256 (can be omitted)
            // content-algorithm: sha256 (can be omitted)
            // nonce: any unique value
            // signature: base64 output
            // --> method: get, post (lowercase)
            // --> uri: IHttpRequestFeature.RawTarget, GetEncodedUrl, GetDisplayUrl, or both?
            // --> nonce
            // --> timestamp (from Date header)
            // --> headers? fixed? (draft spec makes this configurable but it seems unsafe to let the client determine this)
            // --> content? (base64 output of content-algorithm of request body, draft spec uses Digest header instead)

            return Task.FromResult(AuthenticateResult.NoResult());
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers.Add("WWW-Authenticate", Scheme.Name);
            return base.HandleChallengeAsync(properties);
        }
    }
}