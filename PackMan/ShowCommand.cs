using ArtefactStore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PackMan
{
    class ShowCommand : ICommand
    {
        public async Task Run(IArtefactStore artefactStore, string[] commandArgs, CancellationToken cancellationToken)
        {
            switch (commandArgs.Length)
            {
                case 0:
                    {
                        Console.WriteLine($"Reading all packages...");
                        var packageIds = await artefactStore.GetPackages(cancellationToken);
                        Console.WriteLine($"Got {packageIds.Length} packages");
                        Console.WriteLine($"{string.Join(Environment.NewLine, packageIds.Select(packageId => packageId.Id))}");
                    }
                    break;
                case 1:
                    {
                        var packageId = new PackageId(commandArgs[0]);
                        Console.WriteLine($"Reading all artefacts for package {packageId}...");
                        var artefacts = await artefactStore.GetArtefacts(packageId, cancellationToken);
                        Console.WriteLine($"Got {artefacts.Length} artefacts");
                        Console.WriteLine($"{string.Join(Environment.NewLine, artefacts.AsEnumerable())}");
                    }
                    break;
                case 2:
                    {
                        var packageId = new PackageId(commandArgs[0]);
                        var version = new SemanticVersion(commandArgs[1]);
                        var artefactId = new ArtefactId(packageId, version);
                        Console.WriteLine($"Reading artefact {artefactId}...");
                        var artefact = await artefactStore.GetArtefact(artefactId, cancellationToken);
                        Console.WriteLine($"PackageId : {artefact.ArtefactId.PackageId}");
                        Console.WriteLine($"Version   : {artefact.ArtefactId.Version}");
                        Console.WriteLine($"Depends on:");
                        Console.WriteLine($"{string.Join(Environment.NewLine, artefact.DependsOn.AsEnumerable())}");
                    }
                    break;
                default:
                    throw new SyntaxErrorException("show [<packageId> [version]]");
            }
        }
    }
}
