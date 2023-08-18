using System.IO;

namespace Ookii.FormatC
{
    internal static class TextWriterExtensions
    {
        public static void WriteStartElement(this TextWriter writer, string element, string? className)
        {
            writer.Write("<");
            writer.Write(element);
            if (className != null)
            {
                writer.Write(" class=\"");
                writer.Write(className);
                writer.Write("\"");
            }

            writer.Write('>');
        }

        public static void WriteStartElement(this TextWriter writer, string? className = null)
        {
            writer.WriteStartElement("span", className);
        }

        public static void WriteEndElement(this TextWriter writer, string element = "span")
        {
            writer.Write("</");
            writer.Write(element);
            writer.Write(">");
        }
    }
}
