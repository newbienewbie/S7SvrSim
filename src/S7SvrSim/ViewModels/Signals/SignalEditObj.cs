using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using S7Svr.Simulator;
using S7SvrSim.Project;
using S7SvrSim.S7Signal;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
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
                if (!UndoRedoManager.IsInUndoRedo)
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
                        UndoRedoManager.StartTransaction();
                        Other = typeQuery.First().Type;
                        if (Value is SignalWithLengthBase lengthValue)
                        {
                            lengthValue.Length = len;
                        }
                        UndoRedoManager.EndTransaction();
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

        partial void OnValueChanged(SignalBase value)
        {
            Value.NameChanged += OnNameChanged;
            Value.RemarkChanged += OnRemarkChanged;
            Value.FormatAddressChanged += OnFormtAddressChanged;
            if (Value is SignalWithLengthBase lengthValue)
            {
                lengthValue.LengthChanged += OnSignalLengthChanged;
                lengthValue.NotifyLengthChanged();
            }
        }

        void ReleaseBind()
        {
            Value.NameChanged -= OnNameChanged;
            Value.RemarkChanged -= OnRemarkChanged;
            Value.FormatAddressChanged -= OnFormtAddressChanged;
            if (Value is SignalWithLengthBase lengthValue)
            {
                lengthValue.LengthChanged -= OnSignalLengthChanged;
                lengthValue.NotifyLengthChanged();
            }
        }

        partial void OnOtherChanged(Type value)
        {
            if (UndoRedoManager.IsInUndoRedo)
            {
                return;
            }

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

            if (isInit)
            {
                var command = new ValueChangedCommand<SignalEditObj>(signal =>
                {
                    Other = signal.Other;
                    ReleaseBind();
                    this.value = signal.Value;
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(SignalType));
                }, new SignalEditObj(preValue.GetType()) { Value = preValue }, CloneCurrent());
                command.AfterExecute += CommandEventHandle;
                command.AfterUndo += CommandEventHandle;
                UndoRedoManager.Regist(command);
            }
        }

        private void OnSignalLengthChanged(int oldValue, int newValue)
        {
            RegistValueChangedCommand(val =>
            {
                if (Value is SignalWithLengthBase lengthSignal)
                {
                    lengthSignal.Length = val;
                    OnPropertyChanged(nameof(SignalType));
                }
            }, oldValue, newValue);

        }

        private void OnFormtAddressChanged(string oldValue, string newValue)
        {
            RegistValueChangedCommand(val => Value.FormatAddress = val, oldValue, newValue);
        }

        private void OnRemarkChanged(string oldValue, string newValue)
        {
            RegistValueChangedCommand(val => Value.Remark = val, oldValue, newValue);
        }

        private void OnNameChanged(string oldValue, string newValue)
        {
            RegistValueChangedCommand(val => Value.Name = val, oldValue, newValue);
        }

        private void RegistValueChangedCommand<T>(Action<T> update, T oldValue, T newValue)
        {
            if (UndoRedoManager.IsInUndoRedo || !isInit || EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return;
            }

            var command = new ValueChangedCommand<T>(val =>
            {
                update?.Invoke(val);
            }, oldValue, newValue);
            command.AfterExecute += CommandEventHandle;
            command.AfterUndo += CommandEventHandle;
            UndoRedoManager.Regist(command);
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

        private SignalEditObj CloneCurrent()
        {
            return new SignalEditObj(Other)
            {
                Other = Other,
                Value = CloneValue()
            };
        }

        private void CommandEventHandle(object _object, EventArgs _args)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SwitchTab(2);
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
