using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Generator.Transforms;
namespace Generator
{
    public class ODataEDMFromCSDLImporter : IODataEDMImporter
    {
        public ICSDLRuntimeConfigAndTransforms _configAndTransforms { get; }
        public ODataEDMFromCSDLImporter(ICSDLRuntimeConfigAndTransforms configAndTransforms)
        {
            _configAndTransforms = configAndTransforms;

        }
        public async ValueTask<string> ConvertToV4EDM(string inputCSDL)
        {
            string v4OdataXml = "";
            string inputVersion = await ApplyTransform(_configAndTransforms.CSDLToODataVersion, inputCSDL);
            if (!inputVersion.StartsWith("4"))
            {
                v4OdataXml = await ApplyTransform(_configAndTransforms.v2toV4xsl, inputCSDL);
            }
            else
            {
                v4OdataXml = inputCSDL;
            }
            inputCSDL = v4OdataXml;
            return inputCSDL;
        }
        public IEnumerable<XElement> GetDescendentsByLocalName(XElement element, string LocalName)
        {
            return element.Descendants().Where(e => e.Name.LocalName == LocalName);
        }
        public XElement GetFirstDescendentByLocalName(XElement element, string LocalName)
        {
            return element.Descendants().Where(e => e.Name.LocalName == LocalName).First();
        }
        public T DeserializeTo<T>(XElement element)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (StringReader reader = new StringReader(element.ToString()))
            using (XmlReader xmlReader = XmlReader.Create(reader, settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(xmlReader);
            }
        }
        public IEnumerable<T> GetEnumerableOfDescendantsAndDeserializeByLocalName<T>(XElement element)
        {
            var _ = new List<T>();
            foreach (var e in GetDescendentsByLocalName(element, nameof(T)))
            {
                _.Add(DeserializeTo<T>(e));
            }
            return _;
        }
        public T GetFirstDescendantAndDeserializeByLocalName<T>(XElement element)
        {
            return DeserializeTo<T>(GetFirstDescendentByLocalName(element, typeof(T).FullName));
        }
        public Schema LoadSchemaFromCSDL(string v4csdl)
        {
            XElement csdlData = XElement.Parse(v4csdl);
            XElement ds = GetFirstDescendentByLocalName(csdlData, "DataServices");
            return GetFirstDescendantAndDeserializeByLocalName<Schema>(ds);
        }
        public async ValueTask<string> ApplyTransform(XslCompiledTransform xslTrans, string input, XsltArgumentList args = null)
        {
            // Apply an XSLT transform on another thread
            return await Task.Factory.StartNew<string>(
            () =>
                {
                    using (StringReader str = new StringReader(input))
                    {
                        using (XmlReader xr = XmlReader.Create(str))
                        {
                            using (StringWriter sw = new StringWriter())
                            {
                                xslTrans.Transform(xr, args, sw);
                                return sw.ToString();
                            }
                        }
                    }
                }
            );
        }
      
    }
}