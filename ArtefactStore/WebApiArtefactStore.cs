using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ArtefactStore
{
    public class WebApiArtefactStore : IArtefactStore
    {
        private readonly ArtefactStore.WebApiClient.Client apiClient;

        public WebApiArtefactStore(string baseUrl, System.Net.Http.HttpClient httpClient)
        {
            this.apiClient = new WebApiClient.Client(baseUrl, httpClient);
        }

        public Task CreatePackage(PackageId packageId, CancellationToken cancellationToken)
        {
            return this.apiClient.PackageAsync(
                packageId.Id,
                cancellationToken);
        }

        public Task DeletePackage(PackageId packageId, CancellationToken cancellationToken)
        {
            return this.apiClient.Package2Async(
                packageId.Id,
                cancellationToken);
        }

        public Task CreateArtefact(Artefact artefact, CancellationToken cancellationToken)
        {
            return this.apiClient.Artefact2Async(
                artefact.ArtefactId.PackageId.Id,
                artefact.ArtefactId.Version.Version,
                artefact.DependsOn.Select(Convert),
                cancellationToken);
        }

        public Task DeleteArtefact(ArtefactId artefactId, CancellationToken cancellationToken)
        {
            return this.apiClient.Artefact3Async(
                artefactId.PackageId.Id,
                artefactId.Version.Version,
                cancellationToken);
        }

        public async Task<Artefact> GetArtefact(ArtefactId artefactId, CancellationToken cancellationToken)
        {
            var apiArtefact = await this.apiClient.ArtefactAsync(
                artefactId.PackageId.Id,
                artefactId.Version.Version,
                cancellationToken);
            return Convert(apiArtefact);
        }

        public async Task<Artefact[]> GetArtefacts(PackageId packageId, CancellationToken cancellationToken)
        {
            var apiArtefacts = await this.apiClient.ArtefactsAsync(
                packageId.Id,
                cancellationToken);
            return apiArtefacts.Select(Convert).ToArray();
        }

        public async Task<Artefact> GetLatestArtefact(PackageId packageId, CancellationToken cancellationToken)
        {
            var apiArtefact = await this.apiClient.LatestArtefactAsync(
                packageId.Id,
                cancellationToken);
            return Convert(apiArtefact);
        }

        public async Task<PackageId[]> GetPackages(CancellationToken cancellationToken)
        {
            var packages = await this.apiClient.PackagesAsync(cancellationToken);
            return packages.Select(package => new PackageId(package.Id)).ToArray();
        }

        public Stream GetZipArchive(ArtefactId artefactId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetZipArchive(ArtefactId artefactId, Stream zipArchive, CancellationToken cancellationToken)
        {
            return this.apiClient.ZipArchive2Async(
                artefactId.PackageId.Id,
                artefactId.Version.Version,
                zipArchive,
                cancellationToken);
        }

        private static WebApiClient.ArtefactId Convert(ArtefactId artefactId)
        {
            return new WebApiClient.ArtefactId
            {
                PackageId = new WebApiClient.PackageId { Id = artefactId.PackageId.Id },
                Version = new WebApiClient.SemanticVersion { Version = artefactId.Version.Version },
            };
        }

        private static ArtefactId Convert(WebApiClient.ArtefactId artefactId)
        {
            return new ArtefactId(
                new PackageId(artefactId.PackageId.Id),
                new SemanticVersion(artefactId.Version.Version));
        }

        private static Artefact Convert(WebApiClient.Artefact artefact)
        {
            return new Artefact(
                Convert(artefact.ArtefactId),
                artefact.DependsOn.Select(Convert).ToArray());
        }
    }
}
