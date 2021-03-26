using ArtefactStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackMan
{
    class InstallCommand : ICommand
    {
        public async Task Run(IArtefactStore artefactStore, string[] commandArgs, CancellationToken cancellationToken)
        {
            switch (commandArgs.Length)
            {
                case 3:
                    var packageId = new PackageId(commandArgs[0]);
                    var version = new SemanticVersion(commandArgs[1]);
                    var artefactId = new ArtefactId(packageId, version);
                    Console.WriteLine($"Resolving artefact {artefactId}...");

                    var dependencies = await ResolveCommand
                        .Resolve(artefactStore, artefactId, cancellationToken);

                    var destinationFolder = commandArgs[2];
                    foreach (var dependency in dependencies.Reverse())
                    {
                        Console.WriteLine($"Extracting artefact {artefactId} to {destinationFolder}...");
                        await ExtractCommand.Extract(artefactStore, dependency.ArtefactId, destinationFolder, cancellationToken);
                    }
                    break;
                default:
                    throw new SyntaxErrorException("extract <packageId> <version> <folder>");
            }
        }
    }
}
