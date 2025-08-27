using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
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
    public partial class SignalEditObj : ReactiveObject
    {
        private readonly IMemCache<SignalType[]> signalTypes;
        private readonly ISaveNotifier saveNotifier;

        /// <summary>
        /// 信号类型对应的实际反射类型
        /// </summary>
        [Reactive]
        public Type Other { get; set; }

        [Reactive]
        public SignalBase Value { get; set; }

        private IDisposable valueDisposable;
        private IDisposable typeDisposable;
        private IDisposable valueChangedDisposable;

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

                this.RaisePropertyChanged();
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
            saveNotifier = ((App)App.Current).ServiceProvider.GetRequiredService<ISaveNotifier>();

            this.WhenAnyValue(vm => vm.Other).Subscribe(OnOtherChanged);

            ObserverToNeedSave();
        }

        public SignalEditObj(string signalType, string signalName, string formatAddress, string remark)
        {
            signalTypes = ((App)App.Current).ServiceProvider.GetRequiredService<IMemCache<SignalType[]>>();
            saveNotifier = ((App)App.Current).ServiceProvider.GetRequiredService<ISaveNotifier>();

            this.WhenAnyValue(vm => vm.Other).Subscribe(OnOtherChanged);

            Other = ParseType(signalType);

            Value.Name = signalName;
            Value.FormatAddress = formatAddress;
            Value.Remark = remark;

            if (Value is SignalWithLengthBase lenSignal)
            {
                lenSignal.Length = GetLength(signalType);
            }

            ObserverToNeedSave();
        }

        private void ObserverToNeedSave()
        {
            typeDisposable = this.WhenAnyPropertyChanged(nameof(SignalType)).Subscribe(_ => saveNotifier.NotifyNeedSave(true));
            valueChangedDisposable = this.WhenAnyValue(vm => vm.Value).Subscribe(_ =>
            {
                ObserverValueToNeedSave();
            });
            ObserverValueToNeedSave();
        }

        private void ObserverValueToNeedSave()
        {
            if (valueDisposable != null)
            {
                try
                {
                    valueDisposable.Dispose();
                }
                catch (Exception)
                {

                }

                valueDisposable = null;
            }

            if (Value != null)
            {
                if (Value is SignalWithLengthBase signalWithLength)
                {
                    valueDisposable = signalWithLength.WhenAnyValue(v => v.Name, v => v.Remark, v => v.FormatAddress, v => v.Length).Subscribe(_ => saveNotifier.NotifyNeedSave(true));
                }
                else
                {
                    valueDisposable = Value.WhenAnyValue(v => v.Name, v => v.Remark, v => v.FormatAddress).Subscribe(_ => saveNotifier.NotifyNeedSave(true));
                }
            }
        }

        private void OnOtherChanged(Type value)
        {
            var newVal = (SignalBase)Activator.CreateInstance(value);

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

            Value = newVal;

            ObserverValueToNeedSave();

            this.RaisePropertyChanged(nameof(Value));
            this.RaisePropertyChanged(nameof(SignalType));
        }

        ~SignalEditObj()
        {
            try
            {
                valueDisposable?.Dispose();
                valueChangedDisposable?.Dispose();
                typeDisposable?.Dispose();
            }
            catch (Exception)
            {

            }
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
