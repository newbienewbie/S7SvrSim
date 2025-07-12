using Microsoft.Extensions.DependencyInjection;
using S7SvrSim.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.S7Signal
{
    public class SignalAddressUesedCollection : ISignalAddressUesdCollection
    {
        private readonly Dictionary<Type, Func<SignalBase, IAddressUsed>> calcs;

        public SignalAddressUesedCollection(IMemCache<SignalType[]> signalTypes, IServiceProvider serviceProvider)
        {
            var calcType = typeof(IAddressUsedCalc<>);

            calcs = (from ty in signalTypes.Value
                     let calc = serviceProvider.GetRequiredService(calcType.MakeGenericType(ty.Type))
                     let method = calc?.GetType().GetMethod("CalcAddressUsed")
                     where method != null
                     select new
                     {
                         Type = ty.Type,
                         Method = new Func<SignalBase, IAddressUsed>((SignalBase signal) => (IAddressUsed)method.Invoke(calc, new object[] { signal }))
                     }).ToDictionary(item => item.Type, item => item.Method);
        }
        public IAddressUsed GetAddressUsed(SignalBase signal)

        {
            if (calcs.TryGetValue(signal.GetType(), out var calc))
            {
                return calc.Invoke(signal);
            }
            throw new NotSupportedException($"Not support signal type {signal.GetType().Name}");
        }

        public bool TryGetAddressUsed(SignalBase signal, out IAddressUsed addressUsed)
        {
            if (calcs.TryGetValue(signal.GetType(), out var calc))
            {
                addressUsed = calc.Invoke(signal);
                return true;
            }
            addressUsed = default;
            return false;
        }
    }
}
