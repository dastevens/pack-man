namespace ArtefactStore.WebApi.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Swashbuckle.AspNetCore.Annotations;

    [ApiController]
    public class ZipArchiveController : ControllerBase
    {
        private readonly ILogger<ArtefactController> logger;
        private readonly IArtefactStore artefactStore;

        public ZipArchiveController(ILogger<ArtefactController> logger, IArtefactStore artefactStore)
        {
            this.logger = logger;
            this.artefactStore = artefactStore;
        }

        [HttpGet]
        [Route("ZipArchive/{packageId}/{version}")]
        [SwaggerOperation(OperationId = nameof(GetZipArchive))]
        [SwaggerResponse(200)]
        [Produces(@"application/zip")]
        [ProducesResponseType(typeof(FileResult), 200)]
        public async Task<IActionResult> GetZipArchive(string packageId, string version, CancellationToken cancellationToken)
        {
            var artefactId = new ArtefactId(
                new PackageId(packageId),
                new SemanticVersion(version));
            var zipArchive = await this.artefactStore.GetZipArchive(
                artefactId,
                cancellationToken);
            return this.File(zipArchive, "application/zip", $"{artefactId}.zip");
        }

        [HttpPost]
        [Route("ZipArchive/{packageId}/{version}")]
        [SwaggerOperation(OperationId = nameof(SetZipArchive))]
        public async Task SetZipArchive(
            string packageId,
            string version,
            [System.ComponentModel.DataAnnotations.Required]
            [FromBody]
            Microsoft.AspNetCore.Http.IFormFile zipArchive,
            CancellationToken cancellationToken)
        {
            if (!zipArchive.ContentType.Equals("application/zip", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Bad content type: {this.Request.ContentType}, expecting application/zip");
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
    }
}
