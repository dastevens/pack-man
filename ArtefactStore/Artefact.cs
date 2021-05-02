namespace ArtefactStore
{
    using System.Linq;

    public class Artefact
    {
        public Artefact()
            : this(new ArtefactId())
        {
        }

        public Artefact(ArtefactId artefactId, params ArtefactId[] dependsOn)
        {
            this.ArtefactId = artefactId;
            this.DependsOn = dependsOn
                .Distinct()
                .OrderBy(d => d.ToString())
                .ToArray();
        }

        public ArtefactId ArtefactId { get; set; }

        public ArtefactId[] DependsOn { get; set; }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString() => $"{this.ArtefactId} => [{string.Join(" ", this.DependsOn.Select(dependsOn => dependsOn.ToString()))}]";

        public override bool Equals(object obj)
        {
            var other = obj as Artefact;
            if (other != null)
            {
                return this.ToString().Equals(other.ToString());
            }

            return false;
        }
    }
}
