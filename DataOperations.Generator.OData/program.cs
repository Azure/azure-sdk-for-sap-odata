using Microsoft.Extensions.DependencyInjection;
using System.CommandLine; 
using System.Reflection;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System;
namespace Generator
{
    // Spin up the host with a main method
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
                    
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("                                                                                                            ");
            Console.WriteLine("     ______   _______  _______                _____         ______      _____   _____                       ");
            Console.WriteLine("    |  ___ \\ (_______)(_______)     _        | ____|  /\\   (_____ \\    / ___ \\ (____ \\          _           ");
            Console.WriteLine("    | |   | | _____    _          _| |_       \\ \\    /  \\   _____) )  | |   | | _   \\ \\   ____ | |_    ____ ");
            Console.WriteLine("    | |   | ||  ___)  | |        (_   _)       \\ \\  / /\\ \\ |  ____/   | |   | || |   | | / _  ||  _)  / _  |");
            Console.WriteLine("  _ | |   | || |_____ | |_____     |_|     _____) )| |__| || |        | |___| || |__/ / ( ( | || |__ ( ( | |");
            Console.WriteLine(" (_)|_|   |_||_______) \\______)           (______/ |______||_|         \\_____/ |_____/   \\_||_| \\___) \\_||_|");
            Console.WriteLine("                                                                                                            ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                                                                                                            ");
            Console.WriteLine("____________________________________________________________________________________________________________");
            Console.WriteLine("");
            Console.WriteLine("Better Together ... Better with Microsoft .... ");
            Console.WriteLine("____________________________________________________________________________________________________________");

            if (args.Any(e => e == "--version" || e == "-v"))
            {
                Console.WriteLine("Version: " + Assembly.GetExecutingAssembly().GetName().Version);
                return 0;
            }
            
            if (args.Any(e => e == "--help" || e == "-h"))
            {
                Console.WriteLine("** DataOperations.Generator.OData**");
                Console.WriteLine("___________________________________"); 
                Console.WriteLine("Generates a .net SDK and Azure Functions Bindings for an SAP OData service metadata file.");
                Console.WriteLine("");
                Console.WriteLine("Usage: DataOperations.Generator.OData.exe --inputfile (-i) <path to input file> --outputfolder (-o) <path to output folder> --templatefolder (-t) <path to folder to read templates from>");
                Console.WriteLine("Defaults: inputfile=metadata.xml outputfolder=./output/ templatefolder=./Templates/");
                return 0;
            }

            RootCommand rootCommand = new RootCommand(description: "Generates a .net SDK for an SAP OData service.");

            var input = new Option<FileInfo>(name: "--inputfile", description: "The file to read and process.");
            input.IsRequired = true;       
            input.SetDefaultValue(new FileInfo("metadata.xml"));    
            input.AddAlias("-i");    
            rootCommand.Add(input);

            var output = new Option<DirectoryInfo>(name: "--outputfolder", description: "The folder to output.");            
            output.SetDefaultValue(new DirectoryInfo("./Output/"));
            output.AddAlias("-o");
            output.IsRequired = true;
            rootCommand.Add(output);

            var templates = new Option<DirectoryInfo>(name: "--templatefolder", description: "The folder to read templates from"); 
            templates.SetDefaultValue(new DirectoryInfo("./Templates/"));      
            templates.AddAlias("-t");
            templates.IsRequired = true;
            rootCommand.Add(templates);

            var templates2 = new Option<bool>(name: "--clean", description: "Clear down the output folder first? "); 
            templates2.SetDefaultValue(false);      
            templates2.AddAlias("-c");
            templates2.IsRequired = true;
            rootCommand.Add(templates2);

            var samples = new Option<bool>(name: "--samples", description: "Output GWSAMPLE_BASIC SAP Integration samples as source and add dependencies? This is useful if you are generating the GWSAMPLE_BASIC service. Or want an example of how to call the service."); 
            samples.SetDefaultValue(false);      
            samples.AddAlias("-s");
            samples.IsRequired = true;
            rootCommand.Add(samples);
            
            ParseResult pr = rootCommand.Parse(args);
            Console.ForegroundColor = ConsoleColor.Green;
            bool clean = pr.GetValueForOption<bool>(templates2);
            bool sam = pr.GetValueForOption<bool>(samples);

            Console.WriteLine("Clean Output File on build (-c): " + clean.ToString());
            Console.WriteLine("Input file (-i): " + pr.GetValueForOption<FileInfo>(input).FullName);
            Console.WriteLine("Output folder (-o): " + pr.GetValueForOption<DirectoryInfo>(output).FullName);
            Console.WriteLine("Template folder (-t): " + pr.GetValueForOption<DirectoryInfo>(templates).FullName);
            Console.WriteLine("Output Samples (-s): " + sam.ToString());

            if (clean)
            {
                Console.WriteLine("Cleaning output folder");
                DirectoryInfo di = new DirectoryInfo(pr.GetValueForOption<DirectoryInfo>(output).FullName);
                di.Delete(true);
                di.Create();
            }
            try
            {
                IServiceProvider services = new ServiceCollection()
                .RegisterGenerator(pr.GetValueForOption(templates).FullName)
                .BuildServiceProvider();

            var sve = services.GetService<IODataToSDKGenerator>();

            sve.GenerateAsync(
                await File.ReadAllTextAsync(pr.GetValueForOption(input).FullName), 
                pr.GetValueForOption(output).FullName,
                sam)
            .Wait();
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception Occurred:" + ex.Source + " - " + ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
                return 1;
            }
            return 0;
        }
    }
}