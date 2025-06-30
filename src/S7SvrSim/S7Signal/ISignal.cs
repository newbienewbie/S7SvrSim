using CommunityToolkit.Mvvm.ComponentModel;
using S7SvrSim.Project;
using S7SvrSim.Services;
using System;

namespace S7SvrSim.S7Signal
{
    public interface ISignal
    {
        /// <summary>
        /// 信号名
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 信号地址
        /// </summary>
        SignalAddress Address { get; set; }
        /// <summary>
        /// 格式化地址
        /// </summary>
        string FormatAddress { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        string Remark { get; set; }
    }

    public abstract partial class SignalBase : ObservableObject, ISignal
    {
        [ObservableProperty]
        private object value;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormatAddress))]
        private SignalAddress address;

        [ObservableProperty]
        private string remark;

        public virtual string FormatAddress
        {
            get => Address?.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Address = null;
                }
                else
                {
                    Address = new SignalAddress(value);
                }
            }
        }

        public abstract void Refresh(IS7Block block);
        public virtual void SetValue(IS7Block block, object value) { }

        public virtual void CopyFromSignalItem(SignalItem signal)
        {
            Name = signal.Name;
            FormatAddress = signal.FormatAddress;
            Remark = signal.Remark;
        }

        public virtual SignalItem ToSignalItem()
        {
            return new SignalItem()
            {
                Name = Name,
                FormatAddress = FormatAddress,
                Remark = Remark,
                Type = GetType().Name
            };
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Address, Remark);
        }

        public static bool operator ==(SignalBase lhs, SignalBase rhs)
        {
            if (lhs is null || rhs is null)
            {
                return lhs is null && rhs is null;
            }

            return lhs.Name == rhs.Name &&
                lhs.Address == rhs.Address &&
                lhs.Remark == rhs.Remark;
        }

        public static bool operator !=(SignalBase lhs, SignalBase rhs)
        {
            return !(lhs == rhs);
        }
    }

    public abstract partial class SignalWithLengthBase : SignalBase
    {
        [ObservableProperty]
        private int length;

        public override void CopyFromSignalItem(SignalItem signal)
        {
            base.CopyFromSignalItem(signal);

            if (signal.Length != null)
            {
                Length = signal.Length.Value;
            }
        }

        public override SignalItem ToSignalItem()
        {
            var item = base.ToSignalItem();
            item.Length = Length;
            return item;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(base.GetHashCode(), Length);
        }

        public static bool operator ==(SignalWithLengthBase lhs, SignalWithLengthBase rhs)
        {
            if (lhs is null || rhs is null)
            {
                return lhs is null && rhs is null;
            }
            return lhs.Length == rhs.Length && lhs == (SignalBase)rhs;
        }

        public static bool operator !=(SignalWithLengthBase lhs, SignalWithLengthBase rhs)
        {
            return !(lhs == rhs);
        }
    }
}
