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
            this.Id = packageId;
        }

        public string Id { get; set; }

        public static explicit operator PackageId(string packageId)
        {
            return new PackageId(packageId);
        }

        public override int GetHashCode() => this.Id.GetHashCode();

        public override string ToString() => this.Id;

        public override bool Equals(object obj)
        {
            var other = obj as PackageId;
            if (other != null)
            {
                return this.Id.Equals(other.Id);
            }
            else
            {
                return false;
            }
        }
    }
}
