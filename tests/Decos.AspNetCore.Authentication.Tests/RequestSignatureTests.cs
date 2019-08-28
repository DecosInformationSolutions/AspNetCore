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
using Xunit;

namespace Decos.AspNetCore.Authentication.Tests
{
    public class RequestSignatureTests
    {
        [Fact]
        public async Task RequestWithoutSignatureIsNotAuthenticated()
        {
            var server = CreateServer();
            var response = await SendAsync(server, "GET", "/", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private TestServer CreateServer(bool challenge = false)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        if (challenge && context.User?.Identity?.IsAuthenticated != true)
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
