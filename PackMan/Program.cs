namespace PackMan
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ArtefactStore;

    internal class Program
    {
        private static readonly Dictionary<string, Func<ICommand>> Commands = new Dictionary<string, Func<ICommand>>(StringComparer.CurrentCultureIgnoreCase)
        {
            ["add"] = () => new AddCommand(),
            ["extract"] = () => new ExtractCommand(),
            ["install"] = () => new InstallCommand(),
            ["remove"] = () => new RemoveCommand(),
            ["resolve"] = () => new ResolveCommand(),
            ["show"] = () => new ShowCommand(),
        };

        private static async Task Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    throw new SyntaxErrorException($"Try <store> then {string.Join(", ", Commands.Keys)}");
                }

                var store = args[0];
                var command = args[1];
                var commandArgs = args.Skip(2).ToArray();

                var cancellationToken = CancellationToken.None;

                IArtefactStore artefactStore;
                if (store.StartsWith("http"))
                {
#if DEBUG
                    var handler = new System.Net.Http.HttpClientHandler();
                    handler.ClientCertificateOptions = System.Net.Http.ClientCertificateOption.Manual;
                    handler.ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };
                    var httpClient = new System.Net.Http.HttpClient(handler);
#else
                    var httpClient = new System.Net.Http.HttpClient();
#endif
                    artefactStore = new WebApiArtefactStore(store, httpClient);
                }
                else
                {
                    artefactStore = new FileArtefactStore(store);
                }

                if (Commands.TryGetValue(command, out var cmd))
                {
                    await cmd().Run(artefactStore, commandArgs, cancellationToken);
                }
                else
                {
                    throw new SyntaxErrorException($"Unknown command {command}. Try <store> then {string.Join(", ", Commands.Keys)}");
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
