namespace ArtefactStore
{
    public struct ArtefactId
    {
        public ArtefactId(PackageId packageId, SemanticVersion version)
        {
            PackageId = packageId;
            Version = version;
        }

        public PackageId PackageId { get; set; }
        public SemanticVersion Version { get; set; }

        public override string ToString() => $"{PackageId} {Version}";
    }
}
