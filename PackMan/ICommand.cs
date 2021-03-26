using ArtefactStore;
using System.Threading;
using System.Threading.Tasks;

namespace PackMan
{
    interface ICommand
    {
        Task Run(IArtefactStore artefactStore, string[] commandArgs, CancellationToken cancellationToken);
    }
}
