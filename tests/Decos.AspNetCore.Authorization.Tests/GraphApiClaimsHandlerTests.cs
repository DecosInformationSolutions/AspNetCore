using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Decos.AspNetCore.Authorization.GraphApi;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Moq;

using Xunit;

namespace Decos.AspNetCore.Authorization.Tests
{
    public class GraphApiClaimsHandlerTests
    {
        private const string AccessToken = "test";
        private const string GroupId1 = "875cc9ec-e558-4e32-a5d3-3b60905cadde";
        private const string GroupId2 = "1fe7ec00-4ecf-465a-a43a-2858d9919e60";
        private const string UserOid1 = "aa2d3d31-6506-40c5-8313-b84b052498ac";
        private readonly Mock<IGraphApiClient> _graphApiClient;
        private readonly HttpContext _anonHttpContext;
        private readonly HttpContext _httpContext;
        private readonly Mock<IOptions<GraphApiClaimsOptions>> _options;
        private readonly Mock<ITokenAcquisition> _tokenAcquisition;
        private readonly IMemoryCache _memoryCache;

        public GraphApiClaimsHandlerTests()
        {
            _anonHttpContext = new DefaultHttpContext();
            _httpContext = new DefaultHttpContext();

            var identity = new ClaimsIdentity("test");
            identity.AddClaim(new Claim(ClaimConstants.ObjectId, UserOid1));
            _httpContext.User = new ClaimsPrincipal(identity);

            _tokenAcquisition = new Mock<ITokenAcquisition>();
            _tokenAcquisition.Setup(x => x.GetAccessTokenOnBehalfOfUserAsync(It.IsAny<IEnumerable<string>>(), null))
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

            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public async Task AddClaimsAsync_WithoutHttpContext_ThrowsException()
        {
            var handler = new GraphApiClaimsHandler(_tokenAcquisition.Object, _graphApiClient.Object, _memoryCache, _options.Object);

            Func<Task> task = async () => await handler.AddClaimsAsync(null, CancellationToken.None);
            await task.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddClaimsAsync_WithoutAuthenticatedUser_ReturnsTrue()
        {
            var handler = new GraphApiClaimsHandler(_tokenAcquisition.Object, _graphApiClient.Object, _memoryCache, _options.Object);

            var result = await handler.AddClaimsAsync(_anonHttpContext, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task AddClaimsAsync_WithInvalidAccessToken_ReturnsFalse()
        {
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity("test"));
            _tokenAcquisition.Setup(x => x.GetAccessTokenOnBehalfOfUserAsync(It.IsAny<IEnumerable<string>>(), null))
                .ThrowsAsync(new MsalUiRequiredException("test", "test"));
            var handler = new GraphApiClaimsHandler(_tokenAcquisition.Object, _graphApiClient.Object, _memoryCache, _options.Object);

            var result = await handler.AddClaimsAsync(_httpContext, CancellationToken.None);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task AddClaimsAsync_WithValidAccessToken_AddsGroupsClaims()
        {
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity("test"));
            var handler = new GraphApiClaimsHandler(_tokenAcquisition.Object, _graphApiClient.Object, _memoryCache, _options.Object);

            await handler.AddClaimsAsync(_httpContext, CancellationToken.None);

            _httpContext.User.HasClaim(_options.Object.Value.GroupsClaimType, GroupId1).Should().BeTrue();
            _httpContext.User.HasClaim(_options.Object.Value.GroupsClaimType, GroupId2).Should().BeTrue();
        }

        [Fact]
        public async Task AddClaimsAsync_AddsEntriesToCache()
        {
            var handler = new GraphApiClaimsHandler(_tokenAcquisition.Object, _graphApiClient.Object, _memoryCache, _options.Object);

            await handler.AddClaimsAsync(_httpContext, CancellationToken.None);

            _memoryCache.TryGetValue($"{UserOid1}:{_options.Object.Value.GroupsClaimType}", out _).Should().BeTrue();
        }

        [Fact]
        public async Task AddClaimsAsync_WithCachedEntries_DoesNotCallGraphApi()
        {
            IEnumerable<string> groupIds = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            _memoryCache.Set($"{UserOid1}:{_options.Object.Value.GroupsClaimType}", groupIds);
            var handler = new GraphApiClaimsHandler(_tokenAcquisition.Object, _graphApiClient.Object, _memoryCache, _options.Object);

            await handler.AddClaimsAsync(_httpContext, CancellationToken.None);

            _graphApiClient.Verify(x => x.GetMemberGroupsAsync(It.IsAny<string>(), It.IsAny<GetMemberGroupsRequest>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task AddClaimsAsync_WithCachedEntries_ReturnsCachedEntries()
        {
            IEnumerable<string> groupIds = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            _memoryCache.Set($"{UserOid1}:{_options.Object.Value.GroupsClaimType}", groupIds);
            var handler = new GraphApiClaimsHandler(_tokenAcquisition.Object, _graphApiClient.Object, _memoryCache, _options.Object);

            await handler.AddClaimsAsync(_httpContext, CancellationToken.None);

            _httpContext.User.FindAll(_options.Object.Value.GroupsClaimType).Select(x => x.Value)
                .Should().BeEquivalentTo(groupIds);
        }
    }
}