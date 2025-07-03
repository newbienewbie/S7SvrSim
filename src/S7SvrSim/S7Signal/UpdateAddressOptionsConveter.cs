using DynamicData.Kernel;
using S7SvrSim.Services.Settings;
using S7SvrSim.Shared;
using System.Xml.Linq;

namespace S7SvrSim.S7Signal
{
    public class UpdateAddressOptionsConveter : IConverter<UpdateAddressOptions>
    {
        private static class Structure
        {
            public const string Root = "Root";
            public const string UpdateAddressByDbIndex = "UpdateAddressByDbIndex";
            public const string ForbidIndexHasOddNumber = "ForbidIndexHasOddNumber";
            public const string AllowBoolIndexHasOddNumber = "AllowBoolIndexHasOddNumber";
            public const string AllowByteIndexHAsOddNumber = "AllowByteIndexHAsOddNumber";
            public const string StringUseTenCeiling = "StringUseTenCeiling";
        }

        public string Convert(UpdateAddressOptions value)
        {
            var root = new XElement(Structure.Root);
            root.Add(new XElement(Structure.UpdateAddressByDbIndex, value.UpdateAddressByDbIndex));
            root.Add(new XElement(Structure.ForbidIndexHasOddNumber, value.ForbidIndexHasOddNumber));
            root.Add(new XElement(Structure.AllowBoolIndexHasOddNumber, value.AllowBoolIndexHasOddNumber));
            root.Add(new XElement(Structure.AllowByteIndexHAsOddNumber, value.AllowByteIndexHAsOddNumber));
            root.Add(new XElement(Structure.StringUseTenCeiling, value.StringUseTenCeiling));
            var doc = new XDocument(root);
            return doc.ToString();
        }

        public UpdateAddressOptions Parse(string value)
        {
            var defaults = GetDefaultValue();
            if (string.IsNullOrEmpty(value)) return defaults;

            var doc = XDocument.Parse(value);
            var root = doc.Element(Structure.Root);
            var updateAddressByDbIndex = root?.Element(Structure.UpdateAddressByDbIndex)?.Value.ParseBool().ValueOr(() => defaults.UpdateAddressByDbIndex) ?? defaults.UpdateAddressByDbIndex;
            var forbidIndexHasOddNumber = root?.Element(Structure.ForbidIndexHasOddNumber)?.Value.ParseBool().ValueOr(() => defaults.ForbidIndexHasOddNumber) ?? defaults.ForbidIndexHasOddNumber;
            var allowBoolIndexHasOddNumber = root?.Element(Structure.AllowBoolIndexHasOddNumber)?.Value.ParseBool().ValueOr(() => defaults.AllowBoolIndexHasOddNumber) ?? defaults.AllowBoolIndexHasOddNumber;
            var allowByteIndexHAsOddNumber = root?.Element(Structure.AllowByteIndexHAsOddNumber)?.Value.ParseBool().ValueOr(() => defaults.AllowByteIndexHAsOddNumber) ?? defaults.AllowByteIndexHAsOddNumber;
            var stringUseTenCeiling = root?.Element(Structure.StringUseTenCeiling)?.Value.ParseBool().ValueOr(() => defaults.StringUseTenCeiling) ?? defaults.StringUseTenCeiling;

            return new UpdateAddressOptions(updateAddressByDbIndex, forbidIndexHasOddNumber, allowBoolIndexHasOddNumber, allowByteIndexHAsOddNumber, stringUseTenCeiling);
        }

        public UpdateAddressOptions GetDefaultValue()
        {
            return new UpdateAddressOptions(true, true, true, true, true);
        }
    }
}
