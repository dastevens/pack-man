namespace ArtefactStore.Test
{
    public class FileArtefactStoreTests : IArtefactStoreTests
    {
        protected override IArtefactStore CreateArtefactStore()
        {
            return MockFileArtefactStore.Create();
        }
    }
}
