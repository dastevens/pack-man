using System;

namespace ArtefactStore
{
    public class SemanticVersion : IComparable<SemanticVersion>
    {
        private const string DEFAULT_VERSION_STRING = "0.1.0";
        private NuGet.Versioning.SemanticVersion nugetVersion;

        public SemanticVersion()
        {
            Version = DEFAULT_VERSION_STRING;
        }

        public SemanticVersion(string versionString)
        {
            Version = versionString;
        }

        private SemanticVersion(NuGet.Versioning.SemanticVersion nugetVersion)
        {
            this.nugetVersion = nugetVersion;
        }

        public string Version
        {
            get
            {
                return nugetVersion.ToNormalizedString();
            }
            set
            {
                nugetVersion = NuGet.Versioning.SemanticVersion.Parse(value);
            }
        }

        public static bool TryParse(string version, out SemanticVersion result)
        {
            if (NuGet.Versioning.SemanticVersion.TryParse(version, out var nugetResult))
            {
                result = new SemanticVersion(nugetResult);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        public override string ToString() => Version;

        public int CompareTo(SemanticVersion other)
        {
            return nugetVersion.CompareTo(other.nugetVersion);
        }

        public override bool Equals(object obj)
        {
            if (obj is SemanticVersion other)
            {
                return nugetVersion.Equals(other.nugetVersion);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return nugetVersion.GetHashCode();
        }

        public static explicit operator SemanticVersion(string semanticVersion)
        {
            return new SemanticVersion(semanticVersion);
        }
    }
}
