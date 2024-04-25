using System.IO;
using System.Xml.Serialization;

namespace Poq.Api.Utility.Http.ResponseParsers
{
    public class XmlResponseParser<T> : IResponseParser<T> where T : class
    {
        public T Parse(string result)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            xmlSerializer.UnknownAttribute += XmlSerializer_UnknownAttribute;
            xmlSerializer.UnknownElement += XmlSerializer_UnknownElement;

            var stringReader = new StringReader(result);
            return xmlSerializer.Deserialize(stringReader) as T;
        }

        private static void XmlSerializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
        }

        private static void XmlSerializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
        }
    }
}
