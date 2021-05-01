using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Extensions;
using Xunit.Sdk;

namespace ArtefactStore.Test
{
    public class ArtefactIdTests
    {
        [Fact]
        public void DefaultConstructor()
        {
            var sut = new ArtefactId();
            Assert.Equal("default-package.0.1.0", sut.ToString());
        }

        [Fact]
        public void ToString_CheckFormat()
        {
            var packageId = new PackageId("my-package");
            var version = new SemanticVersion("1.2.3");
            var sut = new ArtefactId(packageId, version);
            Assert.Equal("my-package.1.2.3", sut.ToString());
        }

        [Fact]
        public void Equals_IsTrue()
        {
            var packageId1 = new PackageId("my-package");
            var version1 = new SemanticVersion("1.2.3");
            var artefactId1 = new ArtefactId(packageId1, version1);

            var packageId2 = new PackageId("my-package");
            var version2 = new SemanticVersion("1.2.3");
            var artefactId2 = new ArtefactId(packageId2, version2);

            Assert.Equal(artefactId1, artefactId2);
            Assert.Equal(artefactId2, artefactId1);
        }

        [Fact]
        public void Equals_WhenPackageIdsAreDifferent_IsFalse()
        {
            var packageId1 = new PackageId("my-package");
            var version1 = new SemanticVersion("1.2.3");
            var artefactId1 = new ArtefactId(packageId1, version1);

            var packageId2 = new PackageId("not-my-package");
            var version2 = new SemanticVersion("1.2.3");
            var artefactId2 = new ArtefactId(packageId2, version2);

            Assert.NotEqual(artefactId1, artefactId2);
            Assert.NotEqual(artefactId2, artefactId1);
        }

        [Fact]
        public void Equals_WhenVersionsAreDifferent_IsFalse()
        {
            var packageId1 = new PackageId("my-package");
            var version1 = new SemanticVersion("1.2.3");
            var artefactId1 = new ArtefactId(packageId1, version1);

            var packageId2 = new PackageId("my-package");
            var version2 = new SemanticVersion("1.2.4");
            var artefactId2 = new ArtefactId(packageId2, version2);

            Assert.NotEqual(artefactId1, artefactId2);
            Assert.NotEqual(artefactId2, artefactId1);
        }

        [Fact]
        public void Equals_WhenNotPackageId_IsFalse()
        {
            var packageId = new PackageId("my-package");
            var version = new SemanticVersion("1.2.3");
            var artefactId = new ArtefactId(packageId, version);

            var otherObject = new object();

            Assert.NotEqual(artefactId, otherObject);
            Assert.NotEqual(otherObject, artefactId);
        }
    }
}
