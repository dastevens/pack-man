using ArtefactStore;
using System;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace PackMan
{
    class ExtractCommand : ICommand
    {
        public async Task Run(IArtefactStore artefactStore, string[] commandArgs, CancellationToken cancellationToken)
        {
            switch (commandArgs.Length)
            {
                case 3:
                    var packageId = new PackageId(commandArgs[0]);
                    var version = new SemanticVersion(commandArgs[1]);
                    var artefactId = new ArtefactId(packageId, version);
                    var destinationFolder = commandArgs[2];
                    Console.WriteLine($"Extracting artefact {artefactId} to {destinationFolder}...");
                    await Extract(artefactStore, artefactId, destinationFolder, cancellationToken);
                    break;
                default:
                    throw new SyntaxErrorException("extract <packageId> <version> <folder>");
            }
        }

        internal static async Task Extract(IArtefactStore artefactStore, ArtefactId artefactId, string destinationFolder, CancellationToken cancellationToken)
        {
            using var stream = artefactStore.GetZipArchive(artefactId, cancellationToken);
            await Task.Run(() =>
            {
                new ZipArchive(stream, ZipArchiveMode.Read).ExtractToDirectory(destinationFolder, true);
            }, cancellationToken);
        }
    }
}
