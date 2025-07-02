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

        public SignalAddressUesedCollection(IMemCache<Type[]> signalTypes, IServiceProvider serviceProvider)
        {
            var calcType = typeof(IAddressUsedCalc<>);

            calcs = signalTypes.Value.Select(ty =>
            {
                var calc = serviceProvider.GetRequiredService(calcType.MakeGenericType(ty));
                return (Type: ty, Calc: calc);
            })
                .Where(item => item.Calc != null)
                .ToDictionary(item => item.Type, item =>
                {
                    var method = item.Calc.GetType().GetMethod("CalcAddressUsed");
                    return new Func<SignalBase, IAddressUsed>((SignalBase signal) => (IAddressUsed)method.Invoke(item.Calc, new object[] { signal }));
                });
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
