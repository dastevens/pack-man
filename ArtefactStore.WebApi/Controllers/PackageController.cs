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
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly ILogger<PackageController> logger;
        private readonly IArtefactStore artefactStore;

        public PackageController(ILogger<PackageController> logger, IArtefactStore artefactStore)
        {
            this.logger = logger;
            this.artefactStore = artefactStore;
        }

        [HttpPost]
        [Route("Package/{packageId}")]
        [SwaggerOperation(OperationId = nameof(CreatePackage))]
        public Task CreatePackage(string packageId, CancellationToken cancellationToken)
        {
            return this.artefactStore.CreatePackage(
                packageId: new PackageId(packageId),
                cancellationToken);
        }

        [HttpDelete]
        [Route("Package/{packageId}")]
        [SwaggerOperation(OperationId = nameof(DeletePackage))]
        public Task DeletePackage(string packageId, CancellationToken cancellationToken)
        {
            return this.artefactStore.DeletePackage(
                packageId: new PackageId(packageId),
                cancellationToken);
        }

        [HttpGet]
        [Route("Packages")]
        [SwaggerOperation(OperationId = nameof(GetPackages))]
        public Task<PackageId[]> GetPackages(CancellationToken cancellationToken)
        {
            return artefactStore.GetPackages(cancellationToken);
        }
    }
}
