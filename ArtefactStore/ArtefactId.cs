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
            PackageId = packageId;
            Version = version;
        }

        public PackageId PackageId { get; set; }
        public SemanticVersion Version { get; set; }

        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString() => $"{PackageId}.{Version}";

        public override bool Equals(object obj)
        {
            if (obj is ArtefactId other)
            {
                return PackageId.Equals(other.PackageId)
                    && Version.Equals(other.Version);
            }
            else
            {
                return false;
            }
        }
    }
}
