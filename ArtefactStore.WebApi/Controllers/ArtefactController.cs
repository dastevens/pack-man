using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ArtefactStore.WebApi.Controllers
{
    [ApiController]
    public class ArtefactController : ControllerBase
    {
        private readonly ILogger<ArtefactController> logger;
        private readonly IArtefactStore artefactStore;

        public ArtefactController(ILogger<ArtefactController> logger, IArtefactStore artefactStore)
        {
            this.logger = logger;
            this.artefactStore = artefactStore;
        }

        [HttpGet]
        [Route("Artefact/{packageId}/{version}")]
        public Task<Artefact> GetArtefact(string packageId, string version, CancellationToken cancellationToken)
        {
            var artefactId = new ArtefactId(new PackageId(packageId), new SemanticVersion(version));
            return this.artefactStore.GetArtefact(artefactId, cancellationToken);
        }

        [HttpGet]
        [Route("Artefacts/{packageId}")]
        public Task<Artefact[]> GetArtefacts(string packageId, CancellationToken cancellationToken)
        {
            return this.artefactStore.GetArtefacts(
                packageId: new PackageId(packageId),
                cancellationToken);
        }

        [HttpGet]
        [Route("LatestArtefact/{packageId}")]
        public Task<Artefact> GetLatestArtefact(string packageId, CancellationToken cancellationToken)
        {
            return this.artefactStore.GetLatestArtefact(
                packageId: new PackageId(packageId),
                cancellationToken);
        }

        [HttpPost]
        [Route("Artefact/{packageId}/{version}")]
        public Task CreateArtefact(string packageId, string version, ArtefactId[] dependsOn, CancellationToken cancellationToken)
        {
            var artefactId = new ArtefactId(
                new PackageId(packageId),
                new SemanticVersion(version));
            var artefact = new Artefact(artefactId, dependsOn);
            return this.artefactStore.CreateArtefact(
                artefact,
                cancellationToken);
        }

        [HttpGet]
        [Route("ZipArchive/{packageId}/{version}")]
        public IActionResult GetZipArchive(string packageId, string version, CancellationToken cancellationToken)
        {
            var artefactId = new ArtefactId(
                new PackageId(packageId),
                new SemanticVersion(version));
            var zipArchive = this.artefactStore.GetZipArchive(
                artefactId,
                cancellationToken);
            return File(zipArchive, "application/zip", $"{artefactId}.zip");
        }

        [HttpPost]
        [Route("ZipArchive/{packageId}/{version}")]
        public async Task SetZipArchive(string packageId, string version, [System.ComponentModel.DataAnnotations.Required] Microsoft.AspNetCore.Http.IFormFile zipArchive, CancellationToken cancellationToken)
        {
            if (!zipArchive.ContentType.Equals("application/zip", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Bad content type: {Request.ContentType}, expecting application/zip");
            }

            var artefactId = new ArtefactId(
                new PackageId(packageId),
                new SemanticVersion(version));
            using var stream = zipArchive.OpenReadStream();
            await this.artefactStore.SetZipArchive(
                artefactId,
                stream,
                cancellationToken);
        }

        [HttpDelete]
        [Route("Artefact/{packageId}/{version}")]
        public Task DeleteArtefact(string packageId, string version, CancellationToken cancellationToken)
        {
            var artefactId = new ArtefactId(
                new PackageId(packageId),
                new SemanticVersion(version));
            return this.artefactStore.DeleteArtefact(
                artefactId,
                cancellationToken);
        }
    }
}
