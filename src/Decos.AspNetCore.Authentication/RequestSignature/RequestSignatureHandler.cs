using System;
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
            // RawTarget, EncodedUrl, DisplayUrl or allow any?
            
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            return base.HandleChallengeAsync(properties);
        }
    }
}