using CommunityToolkit.Mvvm.ComponentModel;
using S7Svr.Simulator.ViewModels;
using System;
using System.Collections.Generic;

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
                if (value != null)
                {
                    Address = new SignalAddress(value);
                }
                else
                {
                    Address = null;
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is SignalBase @base &&
                   EqualityComparer<object>.Default.Equals(Value, @base.Value) &&
                   Name == @base.Name &&
                   EqualityComparer<SignalAddress>.Default.Equals(Address, @base.Address) &&
                   Remark == @base.Remark;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Address, Remark);
        }

        public abstract void Refresh(IS7DataBlockService db);
        public virtual void SetValue(IS7DataBlockService db, object value) { }

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
}
