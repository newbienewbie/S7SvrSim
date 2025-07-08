using DynamicData.Kernel;
using S7SvrSim.Services.Settings;
using S7SvrSim.Shared;
using System.Xml.Linq;

namespace S7SvrSim.ViewModels.Signals
{
    public class SignalExcelOptionConverter : IConverter<SignalExcelOption>
    {
        private static class Structure
        {
            public const string Root = "Root";
            public const string SameGroupImportRule = "SameGroupImportRule";
            public const string DbSplit = "DbSplit";
        }

        public string Convert(SignalExcelOption value)
        {
            var root = new XElement(Structure.Root);
            root.Add(new XElement(Structure.SameGroupImportRule, value.SameGroupImportRule));
            var doc = new XDocument(root);
            return doc.ToString();
        }

        public SignalExcelOption Parse(string value)
        {
            var defaults = GetDefaultValue();
            if (string.IsNullOrEmpty(value)) return defaults;

            var doc = XDocument.Parse(value);
            var root = doc.Element(Structure.Root);
            var sameGroupImportRule = root?.Element(Structure.SameGroupImportRule)?.Value.ParseEnum<SameGroupImportRule>().ValueOr(() => defaults.SameGroupImportRule) ?? defaults.SameGroupImportRule;

            return new SignalExcelOption(sameGroupImportRule);
        }

        public SignalExcelOption GetDefaultValue()
        {
            return new SignalExcelOption(SameGroupImportRule.ReplaceGroup);
        }
    }
}
