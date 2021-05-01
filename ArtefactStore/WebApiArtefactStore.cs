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
            try
            {
                return this.apiClient.PackageAsync(
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
                return this.apiClient.Package2Async(
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
                return this.apiClient.Artefact2Async(
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
                return this.apiClient.Artefact3Async(
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
                var apiArtefact = await this.apiClient.ArtefactAsync(
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
                var apiArtefacts = await this.apiClient.ArtefactsAsync(
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
                var apiArtefact = await this.apiClient.LatestArtefactAsync(
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
                var packages = await this.apiClient.PackagesAsync(cancellationToken);
                return packages.Select(package => new PackageId(package.Id)).ToArray();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get packages", e);
            }
        }

        public Stream GetZipArchive(ArtefactId artefactId, CancellationToken cancellationToken)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get zip archive for artefact {artefactId}", e);
            }
        }

        public Task SetZipArchive(ArtefactId artefactId, Stream zipArchive, CancellationToken cancellationToken)
        {
            try
            {
                return this.apiClient.ZipArchive2Async(
                    artefactId.PackageId.Id,
                    artefactId.Version.Version,
                    zipArchive,
                    cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to set zip archive for artefact {artefactId}", e);
            }
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
