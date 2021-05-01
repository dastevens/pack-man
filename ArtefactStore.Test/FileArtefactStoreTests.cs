using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using Xunit.Sdk;

namespace ArtefactStore.Test
{
    public class FileArtefactStoreTests : IArtefactStoreTests
    {
        protected override IArtefactStore CreateArtefactStore()
        {
            var fileSystem = new MockFileSystem();
            var storeFolder = fileSystem.Path.Combine("artefact", "store");
            fileSystem.Directory.CreateDirectory(storeFolder);
            return new FileArtefactStore(fileSystem, storeFolder);
        }
    }
}
