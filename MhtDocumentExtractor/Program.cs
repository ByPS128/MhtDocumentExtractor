using CommandLine;
using Microsoft.Extensions.Configuration;
using MimeExtractor.Models.Options;

namespace MimeExtractor;

internal class Program
{
    public class Options
    {
        [Option('i', "input-file", Required = true, HelpText = "Path to the MHT file.")]
        public string InputFile { get; set; }

        [Option('o', "output-dir", Required = true, HelpText = "Directory to store extracted files.")]
        public string OutputDirectory { get; set; }
    }

    private static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<Options>(args)
            .WithNotParsed(HandleParseError)
            .WithParsedAsync(RunOptions);
    }

    static async Task RunOptions(Options opts)
    {
        // Validate file and directory existence
        if (File.Exists(opts.InputFile) is false)
        {
            Console.WriteLine("Specified MHT file does not exist.");
            return;
        }

        if (Directory.Exists(opts.OutputDirectory) is false)
        {
            Console.WriteLine("Specified output directory does not exist.");
            return;
        }

        Console.WriteLine($"Processing MHT file: {opts.InputFile} into {opts.OutputDirectory}");
        
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var configuration = builder.Build();

        var appSettings = configuration.GetSection(ApplicationOptions.ConfigKey).Get<ApplicationOptions>();
        await new MhtDocumentProcessor(appSettings).Extract(opts.InputFile, opts.OutputDirectory);
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
        Console.WriteLine("Invalid arguments. Use --help for usage information.");
    }
}
