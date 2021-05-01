using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ArtefactStore
{
    public interface IArtefactStore
    {
        Task CreatePackage(PackageId packageId, CancellationToken cancellationToken);

        Task DeletePackage(PackageId packageId, CancellationToken cancellationToken);

        Task<PackageId[]> GetPackages(CancellationToken cancellationToken);

        Task<Artefact> GetLatestArtefact(PackageId packageId, CancellationToken cancellationToken);

        Task<Artefact> GetArtefact(ArtefactId artefactId, CancellationToken cancellationToken);

        Task<Artefact[]> GetArtefacts(PackageId packageId, CancellationToken cancellationToken);

        Task CreateArtefact(Artefact artefact, CancellationToken cancellationToken);

        Task DeleteArtefact(ArtefactId artefactId, CancellationToken cancellationToken);

        Stream GetZipArchive(ArtefactId artefactId, CancellationToken cancellationToken);

        Task SetZipArchive(ArtefactId artefactId, Stream zipArchive, CancellationToken cancellationToken);
    }
}
