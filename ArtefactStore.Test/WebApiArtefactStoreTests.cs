using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Extensions;
using Xunit.Sdk;

namespace ArtefactStore.Test
{
    public class WebApiArtefactStoreTests : IArtefactStoreTests,
        IClassFixture<WebApplicationFactory<ArtefactStore.WebApi.Startup>>
    {
        private readonly WebApplicationFactory<ArtefactStore.WebApi.Startup> factory;

        public WebApiArtefactStoreTests(WebApplicationFactory<ArtefactStore.WebApi.Startup> factory)
        {
            this.factory = factory;
        }

        protected override IArtefactStore CreateArtefactStore()
        {
            var artefactStore = MockFileArtefactStore.Create();
            var httpClient = this.factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddSingleton<IArtefactStore>(artefactStore);
                    });
                })
                .CreateClient();
            return new WebApiArtefactStore(
                baseUrl: "/",
                httpClient);
        }
    }
}
