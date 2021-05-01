using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ArtefactStore
{
    public class FileArtefactStore : IArtefactStore
    {
        private readonly string storeFolder;
        private readonly System.IO.Abstractions.IFileSystem fileSystem;

        public FileArtefactStore(string storeFolder)
            : this(new FileSystem(), storeFolder)
        {
        }

        public FileArtefactStore(System.IO.Abstractions.IFileSystem fileSystem, string storeFolder)
        {
            this.fileSystem = fileSystem;
            this.storeFolder = storeFolder;
        }

        public Task CreatePackage(PackageId packageId, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var packageFolder = PackageFolder(packageId);
                if (!this.fileSystem.Directory.Exists(packageFolder))
                {
                    this.fileSystem.Directory.CreateDirectory(packageFolder);
                }
                else
                {
                    throw new Exception($"Package {packageId} already exists");
                }
            });
        }

        public Task DeletePackage(PackageId packageId, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var packageFolder = PackageFolder(packageId);
                if (this.fileSystem.Directory.Exists(packageFolder))
                {
                    this.fileSystem.Directory.Delete(packageFolder, true);
                }
                else
                {
                    throw new Exception($"Package {packageId} not found");
                }
            });
        }

        public Task CreateArtefact(Artefact artefact, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var artefactFolder = ArtefactFolder(artefact.ArtefactId);
                if (this.fileSystem.Directory.Exists(artefactFolder))
                {
                    throw new Exception($"Artefact {artefact.ArtefactId} already exists");
                }

                this.fileSystem.Directory.CreateDirectory(artefactFolder);
                this.fileSystem.File.WriteAllText(ArtefactFile(artefact.ArtefactId), JsonConvert.SerializeObject(artefact, Formatting.Indented));
            },
            cancellationToken);
        }

        public Task DeleteArtefact(ArtefactId artefactId, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var artefactFolder = ArtefactFolder(artefactId);
                if (this.fileSystem.Directory.Exists(artefactFolder))
                {
                    this.fileSystem.Directory.Delete(artefactFolder, true);
                }
                else
                {
                    throw new Exception($"Artefact {artefactId} not found");
                }
            });
        }

        public async Task<Artefact> GetArtefact(ArtefactId artefactId, CancellationToken cancellationToken)
        {
            var artefacts = await GetArtefacts(artefactId.PackageId, cancellationToken);
            return artefacts.SingleOrDefault(artefact => artefact.ArtefactId.Version.Equals(artefactId.Version));
        }

        public Task<Artefact[]> GetArtefacts(PackageId packageId, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var packageFolder = PackageFolder(packageId);
                if (!this.fileSystem.Directory.Exists(packageFolder))
                {
                    return Array.Empty<Artefact>();
                }

                return this.fileSystem.Directory.EnumerateDirectories(PackageFolder(packageId))
                    .Select(fullPath => this.fileSystem.Path.GetFileName(fullPath))
                    .Select(directoryName =>
                    {
                        if (SemanticVersion.TryParse(directoryName, out var version))
                        {
                            return version;
                        }
                        else
                        {
                            return null;
                        }
                    })
                    .Where(version => version != null)
                    .Select(version =>
                    {
                        var json = this.fileSystem.File.ReadAllText(ArtefactFile(new ArtefactId(packageId, version)));
                        var deserialized = JsonConvert.DeserializeObject<Artefact>(json);
                        return new Artefact(new ArtefactId(packageId, version), deserialized.DependsOn);
                    })
                    .OrderByDescending(artefact => artefact.ArtefactId.Version)
                    .ToArray();
            },
                cancellationToken);
        }

        public async Task<Artefact> GetLatestArtefact(PackageId packageId, CancellationToken cancellationToken)
        {
            return (await GetArtefacts(packageId, cancellationToken))
                .OrderByDescending(artefact => artefact.ArtefactId.Version)
                .FirstOrDefault();
        }

        public Task<PackageId[]> GetPackages(CancellationToken cancellationToken)
        {
            return Task.Run(() => PackageFolders().Select(packageFolder => new PackageId(packageFolder)).ToArray());
        }

        public Stream GetZipArchive(ArtefactId artefactId, CancellationToken cancellationToken)
        {
            return this.fileSystem.File.OpenRead(ZipArchiveFile(artefactId));
        }

        public Task SetZipArchive(ArtefactId artefactId, Stream zipArchive, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var artefactFolder = ArtefactFolder(artefactId);
                if (!this.fileSystem.Directory.Exists(artefactFolder))
                {
                    throw new Exception($"Artefact {artefactId} not found");
                }

                using var fileStream = this.fileSystem.File.OpenWrite(ZipArchiveFile(artefactId));
                zipArchive.CopyTo(fileStream);
            },
            cancellationToken);
        }

        private string[] PackageFolders() => this.fileSystem.Directory
            .EnumerateDirectories(storeFolder)
            .Select(packageFolder => this.fileSystem.Path.GetFileName(packageFolder)).ToArray();
        private string PackageFolder(PackageId packageId) => this.fileSystem.Path.Combine(storeFolder, packageId.Id);
        private string ArtefactFolder(ArtefactId artefactId) => this.fileSystem.Path.Combine(PackageFolder(artefactId.PackageId), artefactId.Version.ToString());
        private string ArtefactFile(ArtefactId artefactId) => this.fileSystem.Path.Combine(ArtefactFolder(artefactId), "Artefact.json");
        private string ZipArchiveFile(ArtefactId artefactId) => this.fileSystem.Path.Combine(ArtefactFolder(artefactId), "ZipArchive.zip");
    }
}
