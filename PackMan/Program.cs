using ArtefactStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PackMan
{
    class Program
    {
        private static readonly Dictionary<string, Func<ICommand>> commands = new Dictionary<string, Func<ICommand>>(StringComparer.CurrentCultureIgnoreCase)
        {
            ["add"] = () => new AddCommand(),
            ["extract"] = () => new ExtractCommand(),
            ["install"] = () => new InstallCommand(),
            ["remove"] = () => new RemoveCommand(),
            ["resolve"] = () => new ResolveCommand(),
            ["show"] = () => new ShowCommand(),
        };

        static async Task Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    throw new SyntaxErrorException($"Try {string.Join(", ", commands.Keys)}");
                }

                var command = args[0];
                var commandArgs = args.Skip(1).ToArray();

                var cancellationToken = CancellationToken.None;
                var artefactStorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), nameof(PackMan));
                Directory.CreateDirectory(artefactStorePath);
                var artefactStore = new FileArtefactStore(artefactStorePath);

                if (commands.TryGetValue(command, out var cmd))
                {
                    await cmd().Run(artefactStore, commandArgs, cancellationToken);
                }
                else
                {
                    throw new SyntaxErrorException($"Unknown command {command}. Try {string.Join(", ", commands.Keys)}");
                }
            }
            catch (SyntaxErrorException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(-1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(-1);
            }
        }
    }
}
