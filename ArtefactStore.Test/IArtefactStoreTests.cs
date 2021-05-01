using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using Xunit.Sdk;

namespace ArtefactStore.Test
{
    public abstract class IArtefactStoreTests
    {
        abstract protected IArtefactStore CreateArtefactStore();

        [Fact]
        public async Task GetPackages()
        {
            var sut = CreateArtefactStore();

            var packages = await sut.GetPackages(CancellationToken.None);

            Assert.Empty(packages);
        }

        [Fact]
        public async Task GetArtefacts()
        {
            var sut = CreateArtefactStore();

            var artefacts = await sut.GetArtefacts(new PackageId(), CancellationToken.None);

            Assert.Empty(artefacts);
        }

        [Fact]
        public async Task CreatePackage()
        {
            var sut = CreateArtefactStore();

            await sut.CreatePackage(new PackageId(), CancellationToken.None);
            var packages = await sut.GetPackages(CancellationToken.None);

            Assert.Single(packages);
            Assert.Equal(new PackageId(), packages.Single());
        }

        [Fact]
        public async Task CreatePackage_WhenAlreadyExists_Fails()
        {
            var sut = CreateArtefactStore();

            await sut.CreatePackage(new PackageId(), CancellationToken.None);
            await Assert.ThrowsAnyAsync<Exception>(async () => await sut.CreatePackage(new PackageId(), CancellationToken.None));
        }

       [Fact]
        public async Task DeletePackage()
        {
            var packageId = new PackageId();
            var sut = CreateArtefactStore();

            await sut.CreatePackage(packageId, CancellationToken.None);
            await sut.DeletePackage(packageId, CancellationToken.None);
            var packages = await sut.GetPackages(CancellationToken.None);

            Assert.Empty(packages);
        }

        [Fact]
        public async Task DeletePackage_WhenDoesNotExist_Fails()
        {
            var packageId = new PackageId();
            var sut = CreateArtefactStore();

            await Assert.ThrowsAnyAsync<Exception>(async () => await sut.DeletePackage(packageId, CancellationToken.None));
        }

        [Fact]
        public async Task CreateArtefact()
        {
            var artefact = new Artefact();
            var sut = CreateArtefactStore();

            await sut.CreateArtefact(artefact, CancellationToken.None);

            var packages = await sut.GetPackages(CancellationToken.None);
            Assert.Single(packages);
            Assert.Equal(artefact.ArtefactId.PackageId, packages.First());

            var artefacts = await sut.GetArtefacts(artefact.ArtefactId.PackageId, CancellationToken.None);
            Assert.Single(artefacts);
            Assert.Equal(artefact, artefacts.First());

            var latestArtefact = await sut.GetLatestArtefact(artefact.ArtefactId.PackageId, CancellationToken.None);
            Assert.Equal(artefact, latestArtefact);
        }

        [Fact]
        public async Task CreateArtefact_AlreadyExists_Fails()
        {
            var artefact = new Artefact();
            var sut = CreateArtefactStore();

            await sut.CreateArtefact(artefact, CancellationToken.None);
            await Assert.ThrowsAnyAsync<Exception>(async () => await sut.CreateArtefact(artefact, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteArtefact_DoesNotExist_Fails()
        {
            var artefactId = new ArtefactId();
            var sut = CreateArtefactStore();

            await Assert.ThrowsAnyAsync<Exception>(async () => await sut.DeleteArtefact(artefactId, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteArtefact()
        {
            var artefact = new Artefact();
            var sut = CreateArtefactStore();

            await sut.CreateArtefact(artefact, CancellationToken.None);
            await sut.DeleteArtefact(artefact.ArtefactId, CancellationToken.None);

            var packages = await sut.GetPackages(CancellationToken.None);
            Assert.Single(packages);
            Assert.Equal(artefact.ArtefactId.PackageId, packages.Single());

            var artefacts = await sut.GetArtefacts(artefact.ArtefactId.PackageId, CancellationToken.None);
            Assert.Empty(artefacts);

            var latestArtefact = await sut.GetLatestArtefact(artefact.ArtefactId.PackageId, CancellationToken.None);
            Assert.Null(latestArtefact);
        }

        [Fact]
        public async Task GetZipArchive()
        {
            var artefact = new Artefact();
            var sut = CreateArtefactStore();

            await sut.CreateArtefact(artefact, CancellationToken.None);
            await sut.SetZipArchive(artefact.ArtefactId, Stream.Null, CancellationToken.None);
            using var stream = sut.GetZipArchive(artefact.ArtefactId, CancellationToken.None);
            Assert.NotNull(stream);
        }
    }
}
