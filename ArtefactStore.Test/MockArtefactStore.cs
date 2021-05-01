using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;
using Xunit.Extensions;
using Xunit.Sdk;

namespace ArtefactStore.Test
{
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
