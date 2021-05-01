using ArtefactStore;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PackMan
{
    public class AddCommand : ICommand
    {
        public async Task Run(IArtefactStore artefactStore, string[] commandArgs, CancellationToken cancellationToken)
        {
            if (commandArgs.Length < 3)
            {
                throw new SyntaxErrorException("add <packageId> <version> <zipFile> [<dependsOnPackageId> <dependsOnVersion>...]");
            }

            var packageId = new PackageId(commandArgs[0]);
            var version = new SemanticVersion(commandArgs[1]);
            var artefactId = new ArtefactId(packageId, version);
            var zipFile = commandArgs[2];
            var dependsOnPackageIds = commandArgs.Skip(3).Where((arg, i) => i % 2 == 0);
            var dependsOnVersions = commandArgs.Skip(4).Where((arg, i) => i % 2 == 0);

            var dependsOn = Enumerable
                .Zip(dependsOnPackageIds, dependsOnVersions)
                .Select(zippedArg => new ArtefactId(new PackageId(zippedArg.First), new SemanticVersion(zippedArg.Second)))
                .ToArray();

            var artefact = new Artefact(artefactId, dependsOn);

            var temporaryFile = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), ".zip"));
            if (Directory.Exists(zipFile))
            {
                // First zip up to temporary file
                Console.WriteLine($"Zipping folder {zipFile} to {temporaryFile}...");
                ZipFile.CreateFromDirectory(zipFile, temporaryFile);
                zipFile = temporaryFile;
            }

            if (File.Exists(zipFile))
            {
                using var stream = File.OpenRead(zipFile);
                Console.WriteLine($"Creating artefact {artefact}...");
                await artefactStore.CreateArtefact(artefact, cancellationToken);
                Console.WriteLine($"Creating artefact {artefact}...");
                await artefactStore.SetZipArchive(artefactId, stream, cancellationToken);
            }
            else
            {
                throw new Exception($"Did not find zip file {zipFile}");
            }

            if (File.Exists(temporaryFile))
            {
                Console.WriteLine($"Deleting temporary zip file {temporaryFile}...");
                File.Delete(temporaryFile);
            }
        }
    }
}
