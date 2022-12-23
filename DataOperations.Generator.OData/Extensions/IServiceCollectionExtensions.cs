using System.Xml.Xsl;
using Generator.Transforms;
using Generator.Outputs;
using Microsoft.Extensions.DependencyInjection;
namespace Generator
{
    public static class IServiceCollectionExtensionsAndHelpers
    {
        public static IServiceCollection RegisterGenerator(this IServiceCollection a, string TemplateFolder)
        {
            return a.AddSingleton<ICSDLRuntimeConfigAndTransforms>((s) =>
            {
                return new CSDLRuntimeConfigAndTransforms()
                {
                    v2toV4xsl = LoadTransform("V2-to-V4-CSDL.xsl"),
                    CSDLToODataVersion = LoadTransform("OData-Version.xsl")
                };
            })
            .AddSingleton<IOutputGenerator, CSharpSDKTemplateBasedOutputGenerator>(e => new CSharpSDKTemplateBasedOutputGenerator(TemplateFolder))
            .AddSingleton<IODataEDMImporter, ODataEDMFromCSDLImporter>()
            .AddSingleton<IODataToSDKGenerator, ODataToCSharpSDKGenerator>();
        }

        public static XslCompiledTransform LoadTransform(string path)
        {
            var t = new XslCompiledTransform();
            t.Load(path);
            return t;
        }
    }
}
    
