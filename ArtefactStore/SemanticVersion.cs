namespace ArtefactStore
{
    using System;
    using Semver;

    public class SemanticVersion : IComparable<SemanticVersion>
    {
        private const string DefaultVersionString = "0.1.0";
        private SemVersion semVersion;

        public SemanticVersion()
        {
            this.Version = DefaultVersionString;
        }

        public SemanticVersion(string versionString)
        {
            this.Version = versionString;
        }

        private SemanticVersion(SemVersion semVersion)
        {
            this.semVersion = semVersion;
        }

        public string Version
        {
            get
            {
                return this.semVersion.ToString();
            }

            set
            {
                this.semVersion = SemVersion.Parse(value);
            }
        }

        public static explicit operator SemanticVersion(string semanticVersion)
        {
            return new SemanticVersion(semanticVersion);
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

        public override string ToString() => this.Version;

        public int CompareTo(SemanticVersion other)
        {
            return this.semVersion.CompareTo(other.semVersion);
        }

        public override bool Equals(object obj)
        {
            if (obj is SemanticVersion other)
            {
                return this.semVersion.Equals(other.semVersion);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.semVersion.GetHashCode();
        }
    }
}
