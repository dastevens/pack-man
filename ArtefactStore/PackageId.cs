namespace ArtefactStore
{
    public class PackageId
    {
        public PackageId()
        {
        }

        public PackageId(string packageId)
        {
            Id = packageId;
        }

        public string Id { get; set; }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Id;

        public override bool Equals(object obj)
        {
            return Id.Equals(obj);
        }

        public static explicit operator PackageId(string packageId)
        {
            return new PackageId(packageId);
        }
    }
}
