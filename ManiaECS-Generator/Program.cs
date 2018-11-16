using CommandLine;
using CommandLine.Text;
using ManiaECS_Generator.Systems;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ManiaECS_Generator
{
    public class Program
    {
        public class CmdOptions
        {
            [Option('i')]
            public string InputFolder { get; set; }
            [Option('o')]
            public string OutputFolder { get; set; }
            [Option('t')]
            public string TemplateFolder { get; set; }
            public string EcsLibraryFolder { get; set; }
            [Option('d')]
            public bool Daemon { get; set; }
        }

        public static void Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<CmdOptions>(args);

            parserResult
                .WithParsed((options) => Run(options, parserResult))
                .WithNotParsed((options) =>
                {
                    Console.WriteLine("Couldn't parse arguments");
                    Thread.Sleep(10000);

                    //Environment.Exit(1);
                });
        }

        public static void WaitBeforeClosing(CmdOptions options)
        {
            if (options.Daemon) return;
            Thread.Sleep(1000);
        }

        public static void Run(CmdOptions options, ParserResult<CmdOptions> parserResult)
        {
            Logger.CurrentLogger = new ConsoleLogger();

            var inputFolder = options.InputFolder;
            var outputFolder = options.OutputFolder;
            var templateFolder = options.TemplateFolder;
            var ecsFolder = options.EcsLibraryFolder;

            if (string.IsNullOrEmpty(inputFolder)) inputFolder = Environment.CurrentDirectory + "\\input";
            if (string.IsNullOrEmpty(outputFolder)) outputFolder = Environment.CurrentDirectory + "\\output";
            if (string.IsNullOrEmpty(templateFolder)) templateFolder = Environment.CurrentDirectory + "\\template";
            if (string.IsNullOrEmpty(ecsFolder)) ecsFolder = "Libs/ECS";

            Logger.WriteInfo("Info", $"Input Folder: {inputFolder}");
            Logger.WriteInfo("Info", $"Output Folder: {outputFolder}");
            Logger.WriteInfo("Info", $"Template Folder: {templateFolder}");
            Logger.WriteInfo("Info", $"ECS Library Folder location: {ecsFolder}");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var generatorWorld = new GeneratorWorld();

            generatorWorld
                .AddSystem<IOFolderSystem>()
                .AddSystem<ScriptFileSystem>()
                .AddSystem<InputDataSystem>()
                .AddSystem<CreateComponentFileSystem>();

            var ioFolderSystem = generatorWorld.GetSystem<IOFolderSystem>();
            var scriptFileSystem = generatorWorld.GetSystem<ScriptFileSystem>();
            var inputDataSystem = generatorWorld.GetSystem<InputDataSystem>();
            var createComponentFileSystem = generatorWorld.GetSystem<CreateComponentFileSystem>();

            ioFolderSystem.SetDirectories(inputFolder, outputFolder);
            var scripts = ioFolderSystem.GetScriptsFromPath(inputFolder);

            foreach (var script in scripts)
            {
                var file = script.Replace(inputFolder, string.Empty);

                if (!file.ToLower().EndsWith("script.txt"))
                    continue;

                var msScript = scriptFileSystem.AddFile(file, script);

                inputDataSystem.AddFile(msScript);
            }

            Logger.WriteInfo("Info", "Elapsed time for creating struct information: " + stopwatch.ElapsedMilliseconds + "ms");

            createComponentFileSystem.TemplateDirectory = templateFolder;
            createComponentFileSystem.EcsLibraryPath = ecsFolder;
            var result = createComponentFileSystem.CreateContent(inputDataSystem.AllStructures);

            File.WriteAllText(ioFolderSystem.OutputDirectory + "/Components.Script.txt", result);

            stopwatch.Stop();

            Logger.WriteInfo("Info", "Elapsed time for all: " + stopwatch.ElapsedMilliseconds + "ms");

            while (Console.ReadKey().Key == 0)
            {
            }
        }
    }
}
