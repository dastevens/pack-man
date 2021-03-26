using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ArtefactStore
{
    public class FileArtefactStore : IArtefactStore
    {
        private readonly string storeFolder;

        public FileArtefactStore(string storeFolder)
        {
            this.storeFolder = storeFolder;
        }

        public Task DeleteArtefact(ArtefactId artefactId, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var artefactFolder = ArtefactFolder(artefactId);
                if (Directory.Exists(artefactFolder))
                {
                    Directory.Delete(artefactFolder, true);
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
                if (!Directory.Exists(packageFolder))
                {
                    return Array.Empty<Artefact>();
                }

                return Directory.EnumerateDirectories(PackageFolder(packageId))
                    .Select(fullPath => Path.GetFileName(fullPath))
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
                        var json = File.ReadAllText(ArtefactFile(new ArtefactId(packageId, version)));
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
            return File.OpenRead(ZipArchiveFile(artefactId));
        }

        public Task SetArtefact(Artefact artefact, Stream zipArchive, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var artefactFolder = ArtefactFolder(artefact.ArtefactId);
                if (Directory.Exists(artefactFolder))
                {
                    throw new Exception($"Artefact folder {artefactFolder} already exists");
                }

                Directory.CreateDirectory(artefactFolder);
                File.WriteAllText(ArtefactFile(artefact.ArtefactId), JsonConvert.SerializeObject(artefact, Formatting.Indented));
                using var fileStream = File.OpenWrite(ZipArchiveFile(artefact.ArtefactId));
                zipArchive.CopyTo(fileStream);
            },
            cancellationToken);
        }

        private string[] PackageFolders() => Directory.EnumerateDirectories(storeFolder).Select(packageFolder => Path.GetFileName(packageFolder)).ToArray();
        private string PackageFolder(PackageId packageId) => Path.Combine(storeFolder, packageId.Id);
        private string ArtefactFolder(ArtefactId artefactId) => Path.Combine(PackageFolder(artefactId.PackageId), artefactId.Version.ToString());
        private string ArtefactFile(ArtefactId artefactId) => Path.Combine(ArtefactFolder(artefactId), "Artefact.json");
        private string ZipArchiveFile(ArtefactId artefactId) => Path.Combine(ArtefactFolder(artefactId), "ZipArchive.zip");
    }
}
