namespace ArtefactStore.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class PackageIdTests
    {
        public static string[] PackageIdStrings => new[]
        {
            string.Empty,
            "a package",
            "My.Package",
        };

        public static IEnumerable<object[]> PackageIds => PackageIdStrings.Select(packageIdString => new object[] { packageIdString });

        [Fact]
        public void DefaultConstructor()
        {
            var sut = new PackageId();

            Assert.Equal("default-package", sut.Id);
        }

        [Theory]
        [MemberData(nameof(PackageIds))]
        public void Constructor(string packageId)
        {
            var sut = new PackageId(packageId);

            Assert.Equal(packageId, sut.Id);
        }

        [Theory]
        [MemberData(nameof(PackageIds))]
        public void ToString_ShowsId(string packageId)
        {
            var sut = new PackageId(packageId);
            Assert.Equal(packageId, sut.ToString());
        }

        [Theory]
        [MemberData(nameof(PackageIds))]
        public void IsHashable(string packageId)
        {
            var packageId1 = new PackageId(packageId);
            var packageId2 = new PackageId(packageId);

            var dictionary = new Dictionary<PackageId, bool>();
            dictionary[packageId1] = true;
            Assert.True(dictionary.ContainsKey(packageId1));
            Assert.True(dictionary[packageId1]);
            Assert.True(dictionary.ContainsKey(packageId2));
            Assert.True(dictionary[packageId2]);
        }

        [Theory]
        [MemberData(nameof(PackageIds))]
        public void ImplicitCast_Succeeds(string packageId)
        {
            var sut = (PackageId)packageId;

            Assert.Equal(packageId, sut.Id);
        }
    }
}
