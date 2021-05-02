namespace PackMan
{
    using System.Threading;
    using System.Threading.Tasks;
    using ArtefactStore;

    internal interface ICommand
    {
        Task Run(IArtefactStore artefactStore, string[] commandArgs, CancellationToken cancellationToken);
    }
}
