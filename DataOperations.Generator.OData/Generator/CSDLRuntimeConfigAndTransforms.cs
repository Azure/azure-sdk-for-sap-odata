using System.Xml.Xsl;
namespace Generator.Transforms
{

    /// <summary>
    /// This class is used to hold the runtime config for the transformation of the CSDL into V4
        public class CSDLRuntimeConfigAndTransforms : ICSDLRuntimeConfigAndTransforms
    {
        public XslCompiledTransform v2toV4xsl  {get;set;}
        public XslCompiledTransform CSDLToODataVersion  {get;set;}
    }
}