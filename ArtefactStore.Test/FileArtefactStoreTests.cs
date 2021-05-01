using System;
using System.Collections.Generic;
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
            return MockFileArtefactStore.Create();
        }
    }
}
