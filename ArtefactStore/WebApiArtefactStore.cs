namespace ArtefactStore
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class WebApiArtefactStore : IArtefactStore
    {
        private readonly ArtefactStore.WebApiClient.Client apiClient;
        private readonly System.Net.Http.HttpClient httpClient;
        private readonly string baseUrl;

        public WebApiArtefactStore(string baseUrl, System.Net.Http.HttpClient httpClient)
        {
            this.apiClient = new WebApiClient.Client(baseUrl, httpClient);
            this.httpClient = httpClient;
            this.baseUrl = baseUrl;
        }

        public Task CreatePackage(PackageId packageId, CancellationToken cancellationToken)
        {
            try
            {
                return this.apiClient.CreatePackageAsync(
                    packageId.Id,
                    cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create package {packageId}", e);
            }
        }

        public Task DeletePackage(PackageId packageId, CancellationToken cancellationToken)
        {
            try
            {
                return this.apiClient.DeletePackageAsync(
                    packageId.Id,
                    cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to delete package {packageId}", e);
            }
        }

        public Task CreateArtefact(Artefact artefact, CancellationToken cancellationToken)
        {
            try
            {
                return this.apiClient.CreateArtefactAsync(
                    artefact.ArtefactId.PackageId.Id,
                    artefact.ArtefactId.Version.Version,
                    artefact.DependsOn.Select(Convert),
                    cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create artefact {artefact}", e);
            }
        }

        public Task DeleteArtefact(ArtefactId artefactId, CancellationToken cancellationToken)
        {
            try
            {
                return this.apiClient.DeleteArtefactAsync(
                    artefactId.PackageId.Id,
                    artefactId.Version.Version,
                    cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to delete artefact {artefactId}", e);
            }
        }

        public async Task<Artefact> GetArtefact(ArtefactId artefactId, CancellationToken cancellationToken)
        {
            try
            {
                var apiArtefact = await this.apiClient.GetArtefactAsync(
                    artefactId.PackageId.Id,
                    artefactId.Version.Version,
                    cancellationToken);
                return Convert(apiArtefact);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get artefact {artefactId}", e);
            }
        }

        public async Task<Artefact[]> GetArtefacts(PackageId packageId, CancellationToken cancellationToken)
        {
            try
            {
                var apiArtefacts = await this.apiClient.GetArtefactsAsync(
                    packageId.Id,
                    cancellationToken);
                return apiArtefacts.Select(Convert).ToArray();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get artefacts for package {packageId}", e);
            }
        }

        public async Task<Artefact> GetLatestArtefact(PackageId packageId, CancellationToken cancellationToken)
        {
            try
            {
                var apiArtefact = await this.apiClient.GetLatestArtefactAsync(
                    packageId.Id,
                    cancellationToken);
                return Convert(apiArtefact);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get latest artefact for package {packageId}", e);
            }
        }

        public async Task<PackageId[]> GetPackages(CancellationToken cancellationToken)
        {
            try
            {
                var packages = await this.apiClient.GetPackagesAsync(cancellationToken);
                return packages.Select(package => new PackageId(package.Id)).ToArray();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get packages", e);
            }
        }

        public async Task<Stream> GetZipArchive(ArtefactId artefactId, CancellationToken cancellationToken)
        {
            try
            {
                // Use httpclient as streams are fiddly with swagger
                var response = await this.httpClient.GetAsync(
                    ZipArchiveUrl(this.baseUrl, artefactId),
                    cancellationToken);
                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get zip archive for artefact {artefactId}", e);
            }
        }

        public async Task SetZipArchive(ArtefactId artefactId, Stream zipArchive, CancellationToken cancellationToken)
        {
            try
            {
                // Use httpclient as streams are fiddly with swagger
                await this.httpClient.PostAsync(
                    ZipArchiveUrl(this.baseUrl, artefactId),
                    new StreamContent(zipArchive),
                    cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to set zip archive for artefact {artefactId}", e);
            }
        }

        private static string ZipArchiveUrl(string baseUrl, ArtefactId artefactId)
        {
            return $"{baseUrl}/ZipArchive/"
                + System.Uri.EscapeDataString(artefactId.PackageId.Id)
                + "/"
                + System.Uri.EscapeDataString(artefactId.Version.Version);
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
            if (artefactId == null)
            {
                return null;
            }

            return new ArtefactId(
                new PackageId(artefactId.PackageId.Id),
                new SemanticVersion(artefactId.Version.Version));
        }

        private static Artefact Convert(WebApiClient.Artefact artefact)
        {
            if (artefact == null)
            {
                return null;
            }

            return new Artefact(
                Convert(artefact.ArtefactId),
                artefact.DependsOn.Select(Convert).ToArray());
        }
    }
}
