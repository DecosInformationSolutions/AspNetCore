using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Decos.AspNetCore.Authorization.GraphApi;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using Moq;
using Xunit;

namespace Decos.AspNetCore.Authorization.Tests
{
    public class GraphApiClaimsMiddlewareTests
    {
        [Fact]
        public async Task Invoke_WithoutAuthenticatedUser_ContinuesPipeline()
        {
            var httpContext = new DefaultHttpContext();
            var handler = new Mock<IGraphApiClaimsHandler>();
            var nextExecuted = false;
            var middleware = new GraphApiClaimsMiddleware(_ =>
            {
                nextExecuted = true;
                return Task.CompletedTask;
            });

            await middleware.Invoke(httpContext, handler.Object);

            nextExecuted.Should().BeTrue();
        }

        [Fact]
        public async Task Invoke_WithValidAuthenticatedUser_ContinuesPipeline()
        {
            var httpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity("test"))
            };
            var handler = new Mock<IGraphApiClaimsHandler>();
            handler.Setup(x => x.AddClaimsAsync(httpContext, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var nextExecuted = false;
            var middleware = new GraphApiClaimsMiddleware(_ =>
            {
                nextExecuted = true;
                return Task.CompletedTask;
            });

            await middleware.Invoke(httpContext, handler.Object);

            nextExecuted.Should().BeTrue();
        }


        [Fact]
        public async Task Invoke_WithInsufficientlyAuthenticatedUser_ContinuesPipeline()
        {
            var httpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity("test"))
            };
            var handler = new Mock<IGraphApiClaimsHandler>();
            handler.Setup(x => x.AddClaimsAsync(httpContext, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var nextExecuted = false;
            var middleware = new GraphApiClaimsMiddleware(_ =>
            {
                nextExecuted = true;
                return Task.CompletedTask;
            });

            await middleware.Invoke(httpContext, handler.Object);

            nextExecuted.Should().BeFalse();
        }
    }
}
