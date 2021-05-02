namespace PackMan
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ArtefactStore;

    internal class ShowCommand : ICommand
    {
        public async Task Run(IArtefactStore artefactStore, string[] commandArgs, CancellationToken cancellationToken)
        {
            switch (commandArgs.Length)
            {
                case 0:
                    await ShowPackages(artefactStore, cancellationToken);
                    break;
                case 1:
                    var packageId = new PackageId(commandArgs[0]);
                    await ShowArtefacts(artefactStore, packageId, cancellationToken);
                    break;
                case 2:
                    packageId = new PackageId(commandArgs[0]);
                    var version = new SemanticVersion(commandArgs[1]);
                    await ShowArtefact(artefactStore, packageId, version, cancellationToken);
                    break;
                default:
                    throw new SyntaxErrorException("show [<packageId> [version]]");
            }
        }

        private static async Task ShowPackages(
            IArtefactStore artefactStore,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"Reading all packages...");
            var packageIds = await artefactStore.GetPackages(cancellationToken);
            Console.WriteLine($"Got {packageIds.Length} packages");
            Console.WriteLine($"{string.Join(Environment.NewLine, packageIds.Select(packageId => packageId.Id))}");
        }

        private static async Task ShowArtefacts(
            IArtefactStore artefactStore,
            PackageId packageId,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"Reading all artefacts for package {packageId}...");
            var artefacts = await artefactStore.GetArtefacts(packageId, cancellationToken);
            Console.WriteLine($"Got {artefacts.Length} artefacts");
            Console.WriteLine($"{string.Join(Environment.NewLine, artefacts.AsEnumerable())}");
        }

        private static async Task ShowArtefact(
            IArtefactStore artefactStore,
            PackageId packageId,
            SemanticVersion version,
            CancellationToken cancellationToken)
        {
            var artefactId = new ArtefactId(packageId, version);
            Console.WriteLine($"Reading artefact {artefactId}...");
            var artefact = await artefactStore.GetArtefact(artefactId, cancellationToken);
            Console.WriteLine($"PackageId : {artefact.ArtefactId.PackageId}");
            Console.WriteLine($"Version   : {artefact.ArtefactId.Version}");
            Console.WriteLine($"Depends on:");
            Console.WriteLine($"{string.Join(Environment.NewLine, artefact.DependsOn.AsEnumerable())}");
        }
    }
}
