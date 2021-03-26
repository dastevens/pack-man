namespace ArtefactStore
{
    public struct Artefact
    {
        public Artefact(ArtefactId artefactId) : this(artefactId, null)
        {
        }

        public Artefact(ArtefactId artefactId, ArtefactId? dependsOn)
        {
            ArtefactId = artefactId;
            DependsOn = dependsOn;
        }

        public ArtefactId ArtefactId { get; set; }
        public ArtefactId? DependsOn { get; set; }

        public override string ToString() => ArtefactId.ToString();
    }
}
