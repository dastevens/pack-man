﻿namespace PackMan
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using ArtefactStore;

    internal class RemoveCommand : ICommand
    {
        public async Task Run(IArtefactStore artefactStore, string[] commandArgs, CancellationToken cancellationToken)
        {
            if (commandArgs.Length != 2)
            {
                throw new SyntaxErrorException("remove <packageId> <version>");
            }

            var packageId = new PackageId(commandArgs[0]);
            var version = new SemanticVersion(commandArgs[1]);
            var artefactId = new ArtefactId(packageId, version);

            Console.WriteLine($"Deleting artefact {artefactId}...");
            await artefactStore.DeleteArtefact(artefactId, cancellationToken);
        }
    }
}
