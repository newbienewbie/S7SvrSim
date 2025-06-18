using System;

namespace S7SvrSim.S7Signal
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AddressUsedAttribute : Attribute
    {
        public int IndexSize { get; set; }
        public byte OffsetSize { get; set; }
        /// <summary>
        /// <para>如果指定了计算地址大小的方法，则会优先使用该方法</para>
        /// <para>指定的方法需要返回 <see cref="AddressUsedItem"/> 类型</para>
        /// </summary>
        public string CalcMethod { get; set; }
    }

    public class AddressUsedItem
    {
        public int IndexSize { get; set; }
        public byte OffsetSize { get; set; }
    }
}
