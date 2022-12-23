using System.Xml.Xsl;
namespace Generator.Transforms
{
    public interface ICSDLRuntimeConfigAndTransforms
    {
        XslCompiledTransform v2toV4xsl {get;set;}
        XslCompiledTransform CSDLToODataVersion {get;set;}
    }
}