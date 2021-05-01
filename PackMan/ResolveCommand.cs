using ArtefactStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackMan
{
    class ResolveCommand : ICommand
    {
        public async Task Run(IArtefactStore artefactStore, string[] commandArgs, CancellationToken cancellationToken)
        {
            switch (commandArgs.Length)
            {
                case 2:
                    {
                        var packageId = new PackageId(commandArgs[0]);
                        var version = new SemanticVersion(commandArgs[1]);
                        var artefactId = new ArtefactId(packageId, version);
                        Console.WriteLine($"Resolving artefact {artefactId}...");

                        var dependencies = await Resolve(artefactStore, artefactId, cancellationToken);

                        // Print out its dependency chain
                        dependencies
                            .ToList()
                            .ForEach(dependency => Console.WriteLine($"{dependency}"));
                    }
                    break;
                default:
                    throw new SyntaxErrorException("install <packageId> <version> <folder>");
            }
        }

        internal static Task<Artefact[]> Resolve(IArtefactStore artefactStore, ArtefactId artefactId, CancellationToken cancellationToken)
            => Resolve(artefactStore, artefactId, Array.Empty<Artefact>(), maxDepth: 100, cancellationToken);

        static async Task<Artefact[]> Resolve(IArtefactStore artefactStore, ArtefactId artefactId, Artefact[] resolved, int maxDepth, CancellationToken cancellationToken)
        {
            if (maxDepth <= 0)
            {
                throw new Exception($"Failed to resolve artefactId {artefactId}: exceeded max depth");
            }

            if (resolved.Any(resolvedArtefact => resolvedArtefact.ArtefactId.Equals(artefactId)))
            {
                // Already resolved this artefactId
                return resolved;
            }

            var artefact = await artefactStore.GetArtefact(artefactId, cancellationToken);
            resolved = resolved.Append(artefact).ToArray();
            foreach (var dependsOn in artefact.DependsOn)
            {
                var dependencies = await Resolve(artefactStore, dependsOn, resolved, maxDepth - 1, cancellationToken);
                resolved = resolved.Concat(dependencies).ToArray();
            }

            return resolved.Distinct().ToArray();
        }
    }
}
