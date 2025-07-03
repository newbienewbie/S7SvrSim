using S7SvrSim.Services.Settings;
using S7SvrSim.Shared;
using System;
using System.Linq;
using System.Xml.Linq;

namespace S7SvrSim.Services.Recent
{
    public class RecentFileConverter : IConverter<RecentFile[]>
    {
        private static class Structure
        {
            public const string Root = "Files";
            public const string File = "File";
            public const string FilePath = "Path";
            public const string OpenTime = "OpenTime";
        }

        public string Convert(RecentFile[] value)
        {
            var root = new XElement(Structure.Root);
            var files = value.Select(f => new XElement(Structure.File
                , new XAttribute(Structure.FilePath, f.Path)
                , new XAttribute(Structure.OpenTime, f.OpenTime)));
            files.Each(root.Add);
            XDocument doc = new(root);
            return doc.ToString();
        }

        public RecentFile[] Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Array.Empty<RecentFile>();

            var doc = XDocument.Parse(value);
            var root = doc.Element(Structure.Root);
            var files = root.Elements(Structure.File)
                .Select(element =>
                {
                    var file = element.Attribute(Structure.File).Value;
                    var time = element.Attribute(Structure.OpenTime).Value;
                    return new RecentFile(file, DateTime.Parse(time).ToUniversalTime());
                });

            return files.ToArray();
        }

        public RecentFile[] GetDefaultValue()
        {
            return Array.Empty<RecentFile>();
        }
    }
}
