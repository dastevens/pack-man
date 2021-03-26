using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ArtefactStore
{
    public interface IArtefactStore
    {
        Task<PackageId[]> GetPackages(CancellationToken cancellationToken);
        Task<Artefact> GetLatestArtefact(PackageId packageId, CancellationToken cancellationToken);
        Task<Artefact> GetArtefact(ArtefactId artefactId, CancellationToken cancellationToken);
        Task<Artefact[]> GetArtefacts(PackageId packageId, CancellationToken cancellationToken);
        Stream GetZipArchive(ArtefactId artefactId, CancellationToken cancellationToken);
        Task SetArtefact(Artefact artefact, Stream zipArchive, CancellationToken cancellationToken);
        Task DeleteArtefact(ArtefactId artefactId, CancellationToken cancellationToken);
    }
}
