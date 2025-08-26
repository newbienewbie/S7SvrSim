using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using S7Svr.Simulator;
using S7SvrSim.Project;
using S7SvrSim.S7Signal;
using S7SvrSim.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace S7SvrSim.ViewModels
{
    public partial class SignalEditObj : ObservableObject
    {
        private readonly IMemCache<SignalType[]> signalTypes;
        private bool isInit = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SignalType))]
        private Type other;

        [ObservableProperty]
        private SignalBase value;

        public string SignalType
        {
            get
            {
                if (Other == null) return null;

                var signalTypeName = signalTypes.Value.Where(ty => ty.Type == Other).FirstOrDefault();
                if (signalTypeName == null) return null;

                return (Other.IsSubclassOf(typeof(SignalWithLengthBase)) && Value is SignalWithLengthBase lengthSignal) ? $"{signalTypeName.Name}[{lengthSignal?.Length}]" : signalTypeName.Name;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                if (SignalType?.Equals(value, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return;
                }

                var tyStr = value.Split('[')[0];
                var len = GetLength(value);

                var typeQuery = signalTypes.Value.Where(ty => ty.Name.Equals(tyStr, StringComparison.OrdinalIgnoreCase) || ty.Type.Name.Equals(tyStr, StringComparison.OrdinalIgnoreCase));
                if (typeQuery.Any())
                {
                    Other = typeQuery.First().Type;
                    if (Value is SignalWithLengthBase lengthValue)
                    {
                        lengthValue.Length = len;
                    }
                }

                OnPropertyChanged();
            }
        }

        private Type ParseType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            var tyStr = value.Split('[')[0];

            var typeQuery = signalTypes.Value.Where(ty => ty.Name.Equals(tyStr, StringComparison.OrdinalIgnoreCase) || ty.Type.Name.Equals(tyStr, StringComparison.OrdinalIgnoreCase));

            if (typeQuery.Any())
            {
                return typeQuery.First().Type;
            }

            throw new NotSupportedException($"不支持的 S7 类型: {tyStr}");
        }

        private int GetLength(string value)
        {
            var leftBaket = value.IndexOf('[');
            var rightBaket = value.IndexOf(']');

            if (leftBaket != -1 && rightBaket != -1)
            {
                if (int.TryParse(value[(leftBaket + 1)..rightBaket], out var res))
                {
                    return res;
                }
            }
            return default;
        }

        public SignalEditObj(Type type)
        {
            Other = type;
            signalTypes = ((App)App.Current).ServiceProvider.GetRequiredService<IMemCache<SignalType[]>>();

            isInit = true;
        }

        public SignalEditObj(string signalType, string signalName, string formatAddress, string remark)
        {
            signalTypes = ((App)App.Current).ServiceProvider.GetRequiredService<IMemCache<SignalType[]>>();

            Other = ParseType(signalType);

            Value.Name = signalName;
            Value.FormatAddress = formatAddress;
            Value.Remark = remark;

            if (Value is SignalWithLengthBase lenSignal)
            {
                lenSignal.Length = GetLength(signalType);
            }

            isInit = true;
        }

        partial void OnOtherChanged(Type value)
        {
            var newVal = (SignalBase)Activator.CreateInstance(value);
            var preValue = CloneValue();

            if (Value != null)
            {
                newVal.Name = Value.Name;
            }
            else
            {
                newVal.Name = value.Name;
            }

            newVal.FormatAddress = (string)Value?.FormatAddress?.Clone();
            newVal.Remark = Value?.Remark;

            this.value = newVal;
            OnPropertyChanged(nameof(Value));
            OnValueChanged(Value);
        }

        private SignalBase CloneValue()
        {
            if (Value == null)
            {
                return null;
            }

            var value = (SignalBase)Activator.CreateInstance(Value.GetType());
            value.Value = Value.Value;
            value.Address = Value.Address ?? null;
            value.Name = Value.Name;
            value.Remark = Value.Remark;

            if (value is S7Signal.SignalWithLengthBase lenSignal && Value is S7Signal.SignalWithLengthBase curLenSignal)
            {
                lenSignal.Length = curLenSignal.Length;
            }

            return value;
        }
    }

    public static class SignalEditObjExtensions
    {
        public static string ToXml(this IEnumerable<SignalEditObj> obj)
        {
            var serialize = new XmlSerializer(typeof(List<SignalItem>));
            using var stream = new MemoryStream();
            serialize.Serialize(stream, obj.Select(s => s.Value.ToSignalItem()).ToList());

            stream.Position = 0;

            using var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        public static IEnumerable<SignalEditObj> FromXml(this string xml, SignalsHelper helper)
        {
            var serialize = new XmlSerializer(typeof(List<SignalItem>));
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            var result = serialize.Deserialize(stream);

            if (result == null) throw new Exception("解析为空");

            if (result is List<SignalItem> list) return list.Select(helper.ItemToEditObj);

            throw new Exception("解析类型错误");
        }
    }
}
