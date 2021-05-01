using System.Linq;

namespace ArtefactStore
{
    public class Artefact
    {
        public Artefact()
            : this(new ArtefactId())
        {
        }

        public Artefact(ArtefactId artefactId, params ArtefactId[] dependsOn)
        {
            ArtefactId = artefactId;
            DependsOn = dependsOn
                .Distinct()
                .OrderBy(d => d.ToString())
                .ToArray();
        }

        public ArtefactId ArtefactId { get; set; }
        public ArtefactId[] DependsOn { get; set; }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString() => $"{ArtefactId} => [{string.Join(" ", DependsOn.Select(dependsOn => dependsOn.ToString()))}]";

        public override bool Equals(object obj)
        {
            var other = obj as Artefact;
            if (other != null)
            {
                return ToString().Equals(other.ToString());
            }
            return false;
        }
    }
}
