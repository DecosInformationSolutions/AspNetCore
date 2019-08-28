using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Decos.AspNetCore.Authentication.RequestSignature;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Xunit;

namespace Decos.AspNetCore.Authentication.Tests
{
    public class RequestSignatureTests
    {
        [Fact]
        public async Task AnonymousRequestWithoutSignatureIsOK()
        {
            var server = CreateServer(allowAnonymous: true);
            var response = await SendAsync(server, "GET", "/", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RequestWithoutSignatureIsUnauthorized()
        {
            var server = CreateServer();
            var response = await SendAsync(server, "GET", "/", null);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RequestWithoutSignatureContainsChallenge()
        {
            var server = CreateServer();
            var response = await SendAsync(server, "GET", "/", null);
            response.Headers.WwwAuthenticate.Should().NotBeNullOrEmpty();
        }

        private TestServer CreateServer(bool allowAnonymous = false)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        if (!allowAnonymous && context.User?.Identity?.IsAuthenticated != true)
                        {
                            await context.ChallengeAsync();
                            return;
                        }

                        return;
                    });
                })
                .ConfigureServices(services =>
                {
                    services
                        .AddAuthentication(RequestSignatureDefaults.AuthenticationScheme)
                        .AddRequestSignature();
                });
            return new TestServer(builder);
        }

        private static async Task<HttpResponseMessage> SendAsync(TestServer server, string method, string uri, string authorization = null)
        {
            var request = server.CreateRequest(uri);
            if (authorization != null)
            {
                request.AddHeader("Authorization", authorization);
            }

            return await request.SendAsync(method);
        }
    }
}
