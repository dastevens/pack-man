namespace ArtefactStore.Test
{
    using System.IO.Abstractions.TestingHelpers;

    public static class MockFileArtefactStore
    {
        public static FileArtefactStore Create()
        {
            var fileSystem = new MockFileSystem();
            var storeFolder = fileSystem.Path.Combine("artefact", "store");
            fileSystem.Directory.CreateDirectory(storeFolder);
            return new FileArtefactStore(fileSystem, storeFolder);
        }
    }
}
