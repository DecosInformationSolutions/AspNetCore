using System;
using System.Collections.Generic;
using System.Text;
using Decos.AspNetCore.Authorization.GraphApi;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Client.TokenCacheProviders;
using Xunit;

namespace Decos.AspNetCore.Authorization.Tests
{
    public class ServiceCollectionTests
    {
        [Fact]
        public void AddGraphApiClaims_WithoutOptions_AddsDefaultOptions()
        {
            var services = new ServiceCollection();

            services.AddGraphApiClaims();

            services.BuildServiceProvider().GetService<IOptions<GraphApiClaimsOptions>>().Should().NotBeNull();
        }

        [Fact]
        public void AddGraphApiClaims_AddsHandler()
        {
            var services = new ServiceCollection();
            services.AddTokenAcquisition();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

            services.AddGraphApiClaims();

            services.BuildServiceProvider().GetService<IGraphApiClaimsHandler>().Should().NotBeNull();
        }

        [Fact]
        public void AddGraphApiClaims_AddsGraphApiClient()
        {
            var services = new ServiceCollection();
            services.AddTokenAcquisition();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

            services.AddGraphApiClaims();

            services.BuildServiceProvider().GetService<IGraphApiClient>().Should().NotBeNull();
        }

        [Fact]
        public void AddGraphApiClaims_WithConfigure_AddsConfiguredOptions()
        {
            var services = new ServiceCollection();
            services.AddTokenAcquisition();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

            services.AddGraphApiClaims(options => options.GroupsClaimType = "test");

            var result = services.BuildServiceProvider().GetRequiredService<IOptions<GraphApiClaimsOptions>>().Value;
            result.GroupsClaimType.Should().Be("test");
        }
    }
}
