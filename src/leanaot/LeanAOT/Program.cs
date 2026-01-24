using LeanAOT.GenerationPlan;
using LeanAOT.ToCpp;
using NLog;
using System.Text;
using CommandLine;

namespace LeanAOT;

internal class Program
{
    private sealed class CliOptions
    {
        [Option('d', Required = true, HelpText = "DLL search path. Can be specified multiple times.")]
        public IEnumerable<string> DllSearchPaths { get; set; }

        [Option('a', "assembly", Required = true, HelpText = "Assembly name to AOT (without .dll extension). Can be specified multiple times.")]
        public IEnumerable<string> Assemblies { get; set; }

        [Option('o', Required = true, HelpText = "Output directory for generated C++ code.")]
        public string OutputCodeDir { get; set; }
    }

    private static Logger s_logger;

    private static void SetupApp()
    {
        ConsoleUtil.EnableQuickEditMode(false);
        Console.OutputEncoding = Encoding.UTF8;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        int processorCount = Environment.ProcessorCount;
        ThreadPool.SetMinThreads(Math.Max(4, processorCount), 0);
        ThreadPool.SetMaxThreads(Math.Max(16, processorCount * 2), 2);

        LogManager.Setup().LoadConfigurationFromFile("nlog.xml");
        s_logger = LogManager.GetCurrentClassLogger();
    }

    static void Main(string[] args)
    {
        SetupApp();

        int exitCode = 0;
        var parser = new Parser(settings =>
        {
            settings.AllowMultiInstance = true;
        });
        parser.ParseArguments<CliOptions>(args)
            .WithParsed(options => Run(options))
            .WithNotParsed(_ => exitCode = 1);
        Environment.ExitCode = exitCode;
    }

    private static void Run(CliOptions options)
    {
        var generator = new CppGenerator();
        var dllSearchPaths = options.DllSearchPaths.ToList();
        var aotAssemblyNames = options.Assemblies.ToList();
        var outputCodeDir = options.OutputCodeDir;

        var assemblyCache = new Core.AssemblyCache(new Core.MultiDirectoryAssemblyResolver(dllSearchPaths));
        var manifestArgs = new ManifestArgs()
        {
            assemblyCache = assemblyCache,
            aotAssemblyNames = aotAssemblyNames,
        };
        var manifest = new Manifest(manifestArgs);

        var metaService = new MetadataService();
        var globalServices = new GlobalServices()
        {
            Config = new GlobalConfig(),
            ManifestService = new ManifestService(manifest),
            TypeNameService = new TypeNameService(metaService),
            InvokerService = new InvokerService(metaService),
            MetadataService = metaService,
        };
        GlobalServices.Inst = globalServices;

        var confBuilder = new GenerationConfigBuilder()
        {
            outputCodeDir = outputCodeDir,
            manifest = manifest,
            dllSearchPaths = dllSearchPaths,
            assemblyCache = assemblyCache,
        };
        if (Directory.Exists(confBuilder.outputCodeDir))
        {
            Directory.Delete(confBuilder.outputCodeDir, true);
        }
        Directory.CreateDirectory(confBuilder.outputCodeDir);
        var conf = confBuilder.Build();
        generator.Generate(conf);
    }
}
