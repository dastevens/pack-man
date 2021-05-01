namespace ArtefactStore
{
    public class PackageId
    {
        private const string DefaultPackageIdString = "default-package";
        public PackageId()
            : this(DefaultPackageIdString)
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
            var other = obj as PackageId;
            if (other != null)
            {
                return Id.Equals(other.Id);
            }
            else
            {
                return false;
            }
        }

        public static explicit operator PackageId(string packageId)
        {
            return new PackageId(packageId);
        }
    }
}
