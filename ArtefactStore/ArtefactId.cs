namespace ArtefactStore
{
    public class ArtefactId
    {
        public ArtefactId()
            : this(new PackageId(), new SemanticVersion())
        {
        }

        public ArtefactId(PackageId packageId, SemanticVersion version)
        {
            this.PackageId = packageId;
            this.Version = version;
        }

        public PackageId PackageId { get; set; }

        public SemanticVersion Version { get; set; }

        public override int GetHashCode() => this.ToString().GetHashCode();

        public override string ToString() => $"{this.PackageId}.{this.Version}";

        public override bool Equals(object obj)
        {
            if (obj is ArtefactId other)
            {
                return this.PackageId.Equals(other.PackageId)
                    && this.Version.Equals(other.Version);
            }
            else
            {
                return false;
            }
        }
    }
}
