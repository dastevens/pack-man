using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Extensions;
using Xunit.Sdk;

namespace ArtefactStore.Test
{
    public class ArtefactTests
    {
        [Fact]
        public void DefaultConstructor()
        {
            var sut = new Artefact();

            Assert.Equal(new ArtefactId(), sut.ArtefactId);
            Assert.Empty(sut.DependsOn);
            Assert.Equal("default-package.0.1.0 => []", sut.ToString());
        }

        [Fact]
        public void Constructor()
        {
            var packageId = new PackageId("my-package");
            var version = new SemanticVersion("1.2.3");
            var artefactId = new ArtefactId(packageId, version);

            var sut = new Artefact(artefactId);

            Assert.Equal(artefactId, sut.ArtefactId);
            Assert.Empty(sut.DependsOn);
            Assert.Equal("my-package.1.2.3 => []", sut.ToString());
        }
    }
}
