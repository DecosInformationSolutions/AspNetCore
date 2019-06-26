using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Decos.AspNetCore.Authorization.GraphApi;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.Client;

using Moq;

using Xunit;

namespace Decos.AspNetCore.Authorization.Tests
{
    public class GraphApiClaimsHandlerTests
    {
        private const string AccessToken = "test";
        private const string GroupId1 = "875cc9ec-e558-4e32-a5d3-3b60905cadde";
        private const string GroupId2 = "1fe7ec00-4ecf-465a-a43a-2858d9919e60";
        private readonly Mock<IGraphApiClient> _graphApiClient;
        private readonly HttpContext _httpContext;
        private readonly Mock<IOptions<GraphApiClaimsOptions>> _options;
        private readonly Mock<ITokenAcquisition> _tokenAcquisition;

        public GraphApiClaimsHandlerTests()
        {
            _httpContext = new DefaultHttpContext();

            _tokenAcquisition = new Mock<ITokenAcquisition>();
            _tokenAcquisition.Setup(x => x.GetAccessTokenOnBehalfOfUser(_httpContext, It.IsAny<IEnumerable<string>>(), null))
                .ReturnsAsync(AccessToken);

            _graphApiClient = new Mock<IGraphApiClient>();
            _graphApiClient.Setup(x => x.GetMemberGroupsAsync(AccessToken, It.IsAny<GetMemberGroupsRequest>(), CancellationToken.None))
                .ReturnsAsync(new GetMemberGroupsResponse
                {
                    Value = new string[] { GroupId1, GroupId2 }
                });

            _options = new Mock<IOptions<GraphApiClaimsOptions>>();
            _options.SetupGet(x => x.Value)
                .Returns(new GraphApiClaimsOptions());
        }

        [Fact]
        public async Task AddClaimsAsync_WithoutHttpContext_ThrowsException()
        {
            var handler = new GraphApiClaimsHandler(_tokenAcquisition.Object, _graphApiClient.Object, _options.Object);

            Func<Task> task = async () => await handler.AddClaimsAsync(null, CancellationToken.None);
            await task.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddClaimsAsync_WithoutAuthenticatedUser_ReturnsTrue()
        {
            var handler = new GraphApiClaimsHandler(_tokenAcquisition.Object, _graphApiClient.Object, _options.Object);

            var result = await handler.AddClaimsAsync(_httpContext, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddClaimsAsync_WithInvalidAccessToken_ReturnsFalse()
        {
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity("test"));
            _tokenAcquisition.Setup(x => x.GetAccessTokenOnBehalfOfUser(_httpContext, It.IsAny<IEnumerable<string>>(), null))
                .ThrowsAsync(new MsalUiRequiredException("test", "test"));

            var handler = new GraphApiClaimsHandler(_tokenAcquisition.Object, _graphApiClient.Object, _options.Object);

            var result = await handler.AddClaimsAsync(_httpContext, CancellationToken.None);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task AddClaimsAsync_WithValidAccessToken_AddsGroupsClaims()
        {
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity("test"));

            var handler = new GraphApiClaimsHandler(_tokenAcquisition.Object, _graphApiClient.Object, _options.Object);

            await handler.AddClaimsAsync(_httpContext, CancellationToken.None);

            _httpContext.User.HasClaim(_options.Object.Value.GroupsClaimType, GroupId1).Should().BeTrue();
            _httpContext.User.HasClaim(_options.Object.Value.GroupsClaimType, GroupId2).Should().BeTrue();
        }
    }
}