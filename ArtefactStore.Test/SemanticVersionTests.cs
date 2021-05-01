using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Extensions;
using Xunit.Sdk;

namespace ArtefactStore.Test
{
    public class SemanticVersionTests
    {
        [Fact]
        public void DefaultConstructor()
        {
            var sut = new SemanticVersion();

            Assert.Equal("0.1.0", sut.Version);
        }

        public static string[] ValidVersionStrings => new[]
        {
            "1.0.0-0.3.7",
            "1.0.0-x.7.z.92",
            "1.0.0-alpha+001",
            "1.0.0+20130313144700",
            "1.0.0-beta+exp.sha.5114f85",
        }
            .Concat(OrderedVersionStrings)
            .ToArray();

        public static IEnumerable<object[]> ValidVersions => ValidVersionStrings.Select(versionString => new object[] { versionString });

        public static string[] InvalidVersionStrings => new[]
        {
            "invalid"
        };

        public static string[] UnsupportedVersionStrings => new[]
        {
            "1.0.0-x-y-z.–",
            "1.0.0+21AF26D3—-117B344092BD",
        };

        public static IEnumerable<object[]> InvalidVersions => 
            InvalidVersionStrings
                .Concat(UnsupportedVersionStrings)
                .Select(versionString => new object[] { versionString });

        public static string[] OrderedVersionStrings => new[]
        {
            "0.0.9",
            "0.0.10",
            "0.9.0",
            "0.10.0",
            "1.0.0-alpha",
            "1.0.0-alpha.1",
            "1.0.0-alpha.beta",
            "1.0.0-beta",
            "1.0.0-beta.2",
            "1.0.0-beta.11",
            "1.0.0-rc.1",
            "1.0.0",
            "1.9.0",
            "1.10.0",
            "1.11.0"
        };

        public static IEnumerable<object[]> OrderedVersions
        {
            get 
            {
                for (var i = 0; i < OrderedVersionStrings.Length; i++)
                {
                    for (var j = i + 1; j < OrderedVersionStrings.Length; j++)
                    {
                        yield return new object[] { OrderedVersionStrings[i], OrderedVersionStrings[j] };
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidVersions))]
        public void ConstructFromString_Succeeds(string versionString)
        {
            var sut = new SemanticVersion(versionString);

            Assert.Equal(versionString, sut.Version);
        }

        [Theory]
        [MemberData(nameof(InvalidVersions))]
        public void ConstructFromString_Throws(string invalidVersionString)
        {
            Assert.Throws<ArgumentException>(() => new SemanticVersion(invalidVersionString));
        }

        [Theory]
        [MemberData(nameof(ValidVersions))]
        public void TryParse_Succeeds(string versionString)
        {
            if (SemanticVersion.TryParse(versionString, out var sut))
            {
                Assert.Equal(versionString, sut.Version);
            }
            else
            {
                throw new XunitException();
            }
        }

        [Theory]
        [MemberData(nameof(InvalidVersions))]
        public void TryParse_Fails(string invalidVersionString)
        {
            if (SemanticVersion.TryParse(invalidVersionString, out var sut))
            {
                throw new XunitException();
            }
        }

        [Theory]
        [MemberData(nameof(ValidVersions))]
        public void ToString_ShowsVersionString(string versionString)
        {
            var sut = new SemanticVersion(versionString);
            Assert.Equal(versionString, sut.ToString());
        }

        [Theory]
        [MemberData(nameof(OrderedVersions))]
        public void CompareTo_InOrder(string earlierVersionString, string laterVersionString)
        {
            var earlierVersion = new SemanticVersion(earlierVersionString);
            var laterVersion = new SemanticVersion(laterVersionString);

            Assert.True(earlierVersion.CompareTo(laterVersion) < 0);
            Assert.True(laterVersion.CompareTo(earlierVersion) > 0);
        }

        [Theory]
        [MemberData(nameof(ValidVersions))]
        public void CompareTo_Equals(string versionString)
        {
            var version1 = new SemanticVersion(versionString);
            var version2 = new SemanticVersion(versionString);

            Assert.True(version1.CompareTo(version2) == 0);
            Assert.True(version2.CompareTo(version1) == 0);
        }

        [Theory]
        [MemberData(nameof(ValidVersions))]
        public void Equals_IsTrue(string versionString)
        {
            var version1 = new SemanticVersion(versionString);
            var version2 = new SemanticVersion(versionString);

            Assert.True(version1.Equals(version2));
            Assert.True(version2.Equals(version1));
        }

        [Theory]
        [MemberData(nameof(OrderedVersions))]
        public void Equals_IsFalse(string earlierVersionString, string laterVersionString)
        {
            var earlierVersion = new SemanticVersion(earlierVersionString);
            var laterVersion = new SemanticVersion(laterVersionString);

            Assert.False(earlierVersion.Equals(laterVersion));
            Assert.False(laterVersion.Equals(earlierVersion));
        }

        [Theory]
        [MemberData(nameof(ValidVersions))]
        public void Equals_ADifferentType_IsFalse(string versionString)
        {
            var version = new SemanticVersion(versionString);

            Assert.False(version.Equals(versionString));
            Assert.False(versionString.Equals(version));
        }

        [Theory]
        [MemberData(nameof(ValidVersions))]
        public void IsHashable(string versionString)
        {
            var version1 = new SemanticVersion(versionString);
            var version2 = new SemanticVersion(versionString);

            var dictionary = new Dictionary<SemanticVersion, bool>();
            dictionary[version1] = true;
            Assert.True(dictionary.ContainsKey(version1));
            Assert.True(dictionary[version1]);
            Assert.True(dictionary.ContainsKey(version2));
            Assert.True(dictionary[version2]);
        }

        [Theory]
        [MemberData(nameof(ValidVersions))]
        public void ImplicitCast_Succeeds(string versionString)
        {
            var sut = (SemanticVersion)versionString;

            Assert.Equal(versionString, sut.Version);
        }
    }
}
