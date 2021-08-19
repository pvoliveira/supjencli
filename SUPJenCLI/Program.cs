using Spectre.Console.Cli;

namespace SUPJenCLI
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp();

            app.Configure(config =>
            {
#if DEBUG
                config.PropagateExceptions();
                config.ValidateExamples();
#endif

                config.AddCommand<InspectCommand>("inspect");
            });

            return app.Run(args);
        }
    }
}
