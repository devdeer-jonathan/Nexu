namespace Ui.Console
{
    using Logic.Interfaces;

    using Microsoft.Extensions.Logging;

    using Spectre.Console;

    using Console = System.Console;

    /// <summary>
    /// Contains the application code for the console app.
    /// </summary>
    public class App
    {
        #region member vars

        private readonly ILogger<App> _logger;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="dependencyAnalyzerLogic">The logic regarding dependency analyzing.</param>
        /// <param name="graphVisualizationLogic">The logic regarding graph visualization.</param>
        public App(
            ILogger<App> logger,
            IDependencyAnalyzerLogic dependencyAnalyzerLogic,
            IGraphVisualizationLogic graphVisualizationLogic)
        {
            _logger = logger;
            DependencyAnalyzerLogic = dependencyAnalyzerLogic;
            GraphVisualizationLogic = graphVisualizationLogic;
        }

        #endregion

        #region methods

        /// <summary>
        /// Represents the program logic of the console app.
        /// </summary>
        /// <param name="args">The command line arguments passed to the app at startup.</param>
        /// <returns>0 if the app ran succesfully otherwise 1.</returns>
        public async Task<int> StartAsync(string[] args)
        {
            Console.Clear();
            // Ensure the folder path is passed as an argument
            if (args.Length == 0)
            {
                AnsiConsole.MarkupLine("[red]Error:[/] Please provide a folder path as an argument.");
                return -1;
            }
            var folderPath = args[2];
            // Validate folder path
            if (!Directory.Exists(folderPath))
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] The folder path [blue]{folderPath}[/] does not exist.");
                return -1;
            }
            string[] solutionFiles = { };

            // Add a spinner while searching for solution files
            await AnsiConsole.Status()
                .StartAsync(
                    "Searching for solution files...",
                    async ctx =>
                    {
                        ctx.Status("Scanning directory...");
                        ctx.Spinner(Spinner.Known.Aesthetic);
                        ctx.SpinnerStyle(Style.Parse("yellow"));
                        await Task.Delay(10000);
                        // Find .sln files in the folder and subfolders
                        solutionFiles = Directory.GetFiles(folderPath, "*.sln", SearchOption.AllDirectories);
                    });
            if (solutionFiles.Length == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]No solution files found[/] in the folder [blue]{folderPath}[/].");
                return -1;
            }

            // Allow the user to select a solution file
            var selectedSolution = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Select a [green]solution file[/] to analyze:")
                    .PageSize(10)
                    .AddChoices(
                        solutionFiles.Select(Path.GetFileName)
                            .ToArray()));

            // Get the full path of the selected solution
            var solutionPath = solutionFiles.FirstOrDefault(file => Path.GetFileName(file) == selectedSolution);
            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                AnsiConsole.MarkupLine("[red]Error:[/] Could not find the selected solution.");
                return -1;
            }

            // Notify the user of progress
            var dependencies = await AnsiConsole.Status()
                .StartAsync(
                    "Analyzing dependencies...",
                    ctx =>
                    {
                        ctx.Status("Loading solution...");
                        ctx.Spinner(Spinner.Known.Dots);
                        ctx.SpinnerStyle(Style.Parse("green"));
                        // Call your dependency analyzer
                        return DependencyAnalyzerLogic.GetProjectDependenciesAsync(solutionPath);
                    });

            // Generate the output graph
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var outputPath = Path.Combine(desktop, "output.dot");
            AnsiConsole.Status()
                .Start(
                    "Generating graph...",
                    _ =>
                    {
                        GraphVisualizationLogic.GenerateGraphImage(dependencies, outputPath);
                    });

            // Display success message
            AnsiConsole.MarkupLine(
                $"[green]Graph generated successfully![/] Check your desktop: [blue]{outputPath}[/]");
            return 0;
        }

        #endregion

        #region properties

        private IDependencyAnalyzerLogic DependencyAnalyzerLogic { get; }

        private IGraphVisualizationLogic GraphVisualizationLogic { get; }

        #endregion
    }
}