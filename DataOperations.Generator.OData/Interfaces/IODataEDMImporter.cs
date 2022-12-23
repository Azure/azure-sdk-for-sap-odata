using System.Xml.Xsl;
using System.Xml.Linq;

namespace Generator
{
    public interface IODataEDMImporter
    {
        ValueTask<string> ApplyTransform(XslCompiledTransform xslTrans, string input, XsltArgumentList args = null);
        ValueTask<string> ConvertToV4EDM(string inputCSDL);
        Schema LoadSchemaFromCSDL(string v4csdl);
        T DeserializeTo<T>(XElement element);
        IEnumerable<T> GetEnumerableOfDescendantsAndDeserializeByLocalName<T>(XElement element);
        T GetFirstDescendantAndDeserializeByLocalName<T>(XElement element);
        XElement GetFirstDescendentByLocalName(XElement element, string LocalName);
        IEnumerable<XElement> GetDescendentsByLocalName(XElement element, string LocalName);
    }
}