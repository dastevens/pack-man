using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace ArtefactStore.WebApi.Controllers
{
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
        [SwaggerOperation(OperationId = nameof(GetArtefact))]
        public Task<Artefact> GetArtefact(string packageId, string version, CancellationToken cancellationToken)
        {
            var artefactId = new ArtefactId(new PackageId(packageId), new SemanticVersion(version));
            return this.artefactStore.GetArtefact(artefactId, cancellationToken);
        }

        [HttpGet]
        [Route("Artefacts/{packageId}")]
        [SwaggerOperation(OperationId = nameof(GetArtefacts))]
        public Task<Artefact[]> GetArtefacts(string packageId, CancellationToken cancellationToken)
        {
            return this.artefactStore.GetArtefacts(
                packageId: new PackageId(packageId),
                cancellationToken);
        }

        [HttpGet]
        [Route("LatestArtefact/{packageId}")]
        [SwaggerOperation(OperationId = nameof(GetLatestArtefact))]
        public Task<Artefact> GetLatestArtefact(string packageId, CancellationToken cancellationToken)
        {
            return this.artefactStore.GetLatestArtefact(
                packageId: new PackageId(packageId),
                cancellationToken);
        }

        [HttpPost]
        [Route("Artefact/{packageId}/{version}")]
        [SwaggerOperation(OperationId = nameof(CreateArtefact))]
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

        [HttpDelete]
        [Route("Artefact/{packageId}/{version}")]
        [SwaggerOperation(OperationId = nameof(DeleteArtefact))]
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
