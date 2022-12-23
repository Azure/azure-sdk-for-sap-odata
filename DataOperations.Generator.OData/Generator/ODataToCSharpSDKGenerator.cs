using Generator.Transforms;
using System.Text;
namespace Generator
{
    /// <summary>
    /// This class is used to generate the C# code for the OData client it requires an IOutputGenerator to be passed in to it to perform the actual mappings
    /// </summary>

    public class ODataToCSharpSDKGenerator : IODataToSDKGenerator
    {

        // Define the configAndTransforms object
        public ODataToCSharpSDKGenerator(ICSDLRuntimeConfigAndTransforms _configAndTransforms, IODataEDMImporter eDMImporter, IOutputGenerator outputGenerator)
        {
            configAndTransforms = _configAndTransforms;
            EDMImporter = eDMImporter;
            OutputGenerator = outputGenerator;
        }

        private readonly ICSDLRuntimeConfigAndTransforms configAndTransforms;
        private readonly IODataEDMImporter EDMImporter;
        private readonly IOutputGenerator OutputGenerator;

        private static void copyDirectory(string strSource, string strDestination)
        {
            if (!Directory.Exists(strDestination))
            {
                Directory.CreateDirectory(strDestination);
            }

            DirectoryInfo dirInfo = new DirectoryInfo(strSource);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo tempfile in files)
            {
                tempfile.CopyTo(Path.Combine(strDestination, tempfile.Name));
            }

            DirectoryInfo[] directories = dirInfo.GetDirectories();
            foreach (DirectoryInfo tempdir in directories)
            {
                copyDirectory(Path.Combine(strSource, tempdir.Name), Path.Combine(strDestination, tempdir.Name));
            }

        }
        public async Task GenerateAsync(string input, string location, bool genSamples = false)
        {

            // Extract the Schema Classes.
            Schema sch = EDMImporter.LoadSchemaFromCSDL(await EDMImporter.ConvertToV4EDM(input));
            string templateparent = new DirectoryInfo(OutputGenerator.TemplatePath).Parent.FullName;

            if (templateparent.EndsWith("DataOperations.Generator.OData"))
            {
                // we are running in dev mode and the dependencies will be in the folder above source folder not the template folder
                templateparent = new DirectoryInfo(OutputGenerator.TemplatePath).Parent.Parent.FullName;
            }

            // Copy the Dependencies to the Output Folder.           
            copyDirectory(
                Path.Combine(templateparent, "Dependencies", "DataOperations.Core"),
                Path.Combine(location, "Dependencies", "DataOperations.Core")
            );

            copyDirectory(
                Path.Combine(templateparent, "Dependencies", "DataOperations.OData"),
                Path.Combine(location, "Dependencies", "DataOperations.OData")
            );

            copyDirectory(
                Path.Combine(templateparent, "Dependencies", "DataOperations.WebJobs"),
                Path.Combine(location, "Dependencies", "DataOperations.WebJobs")
            );

            // Only run this part if started with --source

            if (genSamples)
            {

                copyDirectory(
                    Path.Combine(templateparent, "Samples", "TestClientSample.GWSAMPLE_BASIC"),
                    Path.Combine(location, "Samples", "TestClientSample.GWSAMPLE_BASIC")
                );

                copyDirectory(
                    Path.Combine(templateparent, "Samples", "FunctionsSample.GWSAMPLE_BASIC"),
                    Path.Combine(location, "Samples", "FunctionsSample.GWSAMPLE_BASIC")
                );
            }

            string webjobslocation = Path.Combine(location, sch.Namespace, $"DataOperations.WebJobs.{sch.Namespace}");
            string datalocation = Path.Combine(location, sch.Namespace, $"DataOperations.Data.{sch.Namespace}");
            Directory.CreateDirectory(datalocation);
            Directory.CreateDirectory(webjobslocation);

            StringBuilder thisClassSb = new StringBuilder();
            StringBuilder entitySetSb = new StringBuilder();
            StringBuilder globalActionsSb = new StringBuilder();
            StringBuilder diHsb = new StringBuilder();

            // Stage 1. Build the SDK POCOs
            // 1a Write out each EntityType 
            foreach (var entity in sch.EntityTypes)
            {
                Console.WriteLine("Writing Entity Type Classes: " + entity.Name);
                await OutputGenerator.WriteEntityTypeClass(location, sch, thisClassSb, entity);
            }

            // 1b Write out each of the Complex Types
            foreach (var entity in sch.ComplexTypes)
            {
                Console.WriteLine("Writing Complex Type Classes: " + entity.Name);
                await OutputGenerator.WriteComplexTypeClass(location, sch, entitySetSb, entity);
            }

            // 1c Write the EntitySet Classes  
            foreach (var entSet in sch.EntityContainer.EntitySets)
            {
                Console.WriteLine("Writing EntitySet Classes: " + entSet.Name);
                await OutputGenerator.WriteEntitySetClass(location, sch, entitySetSb, entSet);
            }

            // Stage 2. Build the ServiceRoot Classes
            // 2a Write the ServiceRoot Class
            Console.WriteLine("Writing ServiceRoot Class");
            await OutputGenerator.WriteCustomCodeClass(location, sch, globalActionsSb, "ServiceRoot", "AutoGenerated Code", "ClassHeaderInherits", $"BaseDTOWithIDAndETag", "", "", "", "", sch.Namespace);

            // 2b Write the ServiceRootSet Class
            Console.WriteLine("Writing ServiceRootSet Class");
            await OutputGenerator.WriteCustomCodeClass(location, sch, globalActionsSb, "ServiceRootSet", "AutoGenerated Code", "ClassHeaderInherits", $"ODataEntitySetOperations<ServiceRoot>", "ServiceRoot", "IOperationsDispatcher", "dispatcher", "", sch.Namespace, false,
                async (location, sch, globalActionsSb) =>
                {
                    // Loop through the ActionImports that have no entityset attachment
                    foreach (var actionI in sch.EntityContainer.ActionImports.Where(e => e.EntitySet == null))
                    {
                        await OutputGenerator.WriteActionImport(location, sch, globalActionsSb, actionI);
                    }
                }
            );

            // Stage 3. Build the DI Helper Classes to register the SDK with the DI Container
            // 3a Write the DI Helper Static Extension Class containing the DI Helper Methods to register the EntitySets
            Console.WriteLine("Writing DI Helper Extension Class");
            await OutputGenerator.WriteCustomCodeClass(location, sch, diHsb, "DISetup", "AutoGenerated Code", "DIExtension", "", "", "", "", "            return services;\r\n        }", sch.Namespace, true, async (location, sch, diHsb) =>
                {
                    // Write The Method Header from DIExtension
                    await OutputGenerator.WriteTextFragment(diHsb, "DIExtension");
                    // Loop through the EntitySets
                    foreach (var es in sch.EntityContainer.EntitySets)
                    {
                        await OutputGenerator.WriteEntitySetFragment(location, sch, diHsb, es, "DISet");
                    }
                }
            );

            // Stage 4a. Build the project files
            Console.WriteLine("Writing SDK Project File");
            await OutputGenerator.WriteProjectFile(datalocation, sch, "DataOperations.Data", "Data");

            // Stage 5. Build the WebJobs Classes
            // 5a Write the WebJobs Project File
            Console.WriteLine("Writing WebJobs SDK Project File");
            await OutputGenerator.WriteProjectFile(webjobslocation, sch, "DataOperations.WebJobs", "WebJobs");

            // Stage 5a. Copy the generic WebJobS class to the output folder
            // ExtensionConfigProvider.txt to ExtensionConfigProvider.cs

            File.Copy(
                Path.Combine(templateparent, "Templates", "ExtensionConfigProvider.txt"), 
                Path.Combine(webjobslocation, "ExtensionConfigProvider.cs")
            );

            
            // File.Copy(
            //     Path.Combine(templateparent, "Templates", "FunctionBindingExtensions.txt"), 
            //     Path.Combine(webjobslocation, "FunctionBindingExtensions.cs")
            // );

            // Stage 6. Build the WebJobs Binding Classes
            Console.WriteLine("Writing WebJobs SDK OData SDK Binding file");

            StringBuilder WJInSetBindingsClass = new StringBuilder();
            StringBuilder WJOutBindingsClass = new StringBuilder();
            StringBuilder WJInBindingsClass = new StringBuilder();
            StringBuilder WJConfigureBindings = new StringBuilder();

            // Build the Usings and Namespace file header and write it out to the SB
            Console.WriteLine("Function Binding Generation in progress ... Writing Classes");

            WJInSetBindingsClass.AppendLine(await OutputGenerator.GenerateMember("WebJobs_ClassHeader", new Dictionary<string, string>(){{"NameSpace", sch.Namespace}}));
            WJOutBindingsClass.AppendLine(await OutputGenerator.GenerateMember("WebJobs_ClassHeader", new Dictionary<string, string>(){{"NameSpace", sch.Namespace}}));
            WJInBindingsClass.AppendLine(await OutputGenerator.GenerateMember("WebJobs_ClassHeader", new Dictionary<string, string>(){{"NameSpace", sch.Namespace}}));
            WJConfigureBindings.AppendLine(await OutputGenerator.GenerateMember("WebJobs_ClassHeader", new Dictionary<string, string>(){{"NameSpace", sch.Namespace}}));

            // Write the ConfigureBindings Method Header
            await OutputGenerator.WriteTextFragment(WJConfigureBindings, "WebJobs_ConfigureBindings");

            // Loop through the EntitySets and generate the Input Bindings as well as the ConfigureBindings Stubs
            foreach (var et in sch.EntityTypes)
            {
                Console.WriteLine("Binding Generation In progress ... Input BindingAttributes for " + et.Name);
                WJConfigureBindings.AppendLine(
                    await OutputGenerator.GenerateMember("WJ_InputBindingExtensions", new Dictionary<string, string>()
                        {
                            { "NameSpace", sch.Namespace},
                            { "ClassName", et.Name},
                            { "KeyField", et.Key.PropertyRef.First().Name}
                        }
                    )
                );

                WJInBindingsClass.AppendLine(
                    await OutputGenerator.GenerateMember("WJ_InputBinding", new Dictionary<string, string>()
                        {
                            { "NameSpace", sch.Namespace},
                            { "InputClass", et.Name},
                            { "KeyName", et.Key.PropertyRef.First().Name},
                            { "KeyType", OutputGenerator.GetNativePropertyType(et.Properties.First(e => e.Name == et.Key.PropertyRef.First().Name).Type,false)}
                        }
                    )
                );

                WJConfigureBindings.AppendLine(
                    await OutputGenerator.GenerateMember("WJ_OutputBindingExtensions", new Dictionary<string, string>()
                        {
                            { "NameSpace", sch.Namespace},
                            { "ClassName", et.Name}
                        }
                    )
                );

                WJOutBindingsClass.AppendLine(
                    await OutputGenerator.GenerateMember("WJ_OutputBinding", new Dictionary<string, string>()
                        {
                            { "NameSpace", sch.Namespace},
                            { "InputClass", et.Name}
                        }
                    )
                );
            }

            // Build the Attributes and classes for the Sets
            foreach (var ent in sch.EntityContainer.EntitySets)
            {

                string keyname = ent.GetEntityType(sch).Key.PropertyRef.First().Name;
                string keyType = OutputGenerator.GetNativePropertyType(
                    ent.GetEntityType(sch).Properties.First(e => e.Name == keyname).Type,
                    false
                );

                WJInSetBindingsClass.AppendLine(
                    await OutputGenerator.GenerateMember("WJ_InputSetBinding", new Dictionary<string, string>()
                        {
                            { "NameSpace", sch.Namespace},
                            { "SetName", ent.Name},
                            { "KeyType", keyType},         // ToDo: Support for Composite keys (loop through the entries instead of .First())
                            { "KeyName", keyname},
                            { "InputClass", ent.Name}
                        }
                    )
                );

                WJConfigureBindings.AppendLine(
                    await OutputGenerator.GenerateMember("WJ_InputSetExtensions", new Dictionary<string, string>()
                        {
                            { "NameSpace", sch.Namespace},
                            { "SetName", ent.Name},
                            { "EntityName", ent.EntityType}
                        }
                    )
                );
            };

            // Close and Write out the files 
            WJInSetBindingsClass.AppendLine("\r\n}");
            WJOutBindingsClass.AppendLine("\r\n}");
            WJInBindingsClass.AppendLine("\r\n}");
            WJConfigureBindings.AppendLine("\r\n        }\r\n   }\r\n}");

            Console.WriteLine("Binding Generation In progress ... Input Bindings");

            await OutputGenerator.WriteWebJobsClassCodeFile(webjobslocation, sch, WJInBindingsClass, "BindingsInput");
            await OutputGenerator.WriteWebJobsClassCodeFile(webjobslocation, sch, WJOutBindingsClass, "BindingsOutput");
            await OutputGenerator.WriteWebJobsClassCodeFile(webjobslocation, sch, WJInSetBindingsClass, "BindingsInputSet");
            await OutputGenerator.WriteWebJobsClassCodeFile(webjobslocation, sch, WJConfigureBindings, "BindingHelper");

            Console.WriteLine("Done!");

        }
    }
}