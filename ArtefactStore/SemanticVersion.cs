using System;
using Semver;

namespace ArtefactStore
{
    public class SemanticVersion : IComparable<SemanticVersion>
    {
        private const string DefaultVersionString = "0.1.0";
        private SemVersion semVersion;

        public SemanticVersion()
        {
            Version = DefaultVersionString;
        }

        public SemanticVersion(string versionString)
        {
            Version = versionString;
        }

        private SemanticVersion(SemVersion semVersion)
        {
            this.semVersion = semVersion;
        }

        public string Version
        {
            get
            {
                return semVersion.ToString();
            }
            set
            {
                semVersion = SemVersion.Parse(value);
            }
        }

        public static bool TryParse(string version, out SemanticVersion result)
        {
            if (SemVersion.TryParse(version, out var nugetResult))
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
            return semVersion.CompareTo(other.semVersion);
        }

        public override bool Equals(object obj)
        {
            if (obj is SemanticVersion other)
            {
                return semVersion.Equals(other.semVersion);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return semVersion.GetHashCode();
        }

        public static explicit operator SemanticVersion(string semanticVersion)
        {
            return new SemanticVersion(semanticVersion);
        }
    }
}
