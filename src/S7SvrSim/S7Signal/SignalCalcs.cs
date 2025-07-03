using Microsoft.Extensions.DependencyInjection;
using S7SvrSim.Services.Settings;
using S7SvrSim.ViewModels;
using System;
using System.Linq;
using System.Reflection;

namespace S7SvrSim.S7Signal
{
    public class AddressUsedAttrCalc<T> : IAddressUsedCalc<T>
        where T : SignalBase
    {
        private readonly AddressUsedAttribute attr;

        public AddressUsedAttrCalc()
        {
            attr = typeof(T).GetCustomAttribute<AddressUsedAttribute>();
        }

        public IAddressUsed CalcAddressUsed(T signal)
        {
            return new AddressUsed()
            {
                IndexSize = attr.IndexSize,
                OffsetSize = attr.OffsetSize,
            };
        }
    }

    public class StringCalc : IAddressUsedCalc<String>
    {
        private bool UseTenCeiling { get; set; }

        public StringCalc(ISetting<UpdateAddressOptions> setting)
        {
            setting.Value.Subscribe(options =>
            {
                UseTenCeiling = options.StringUseTenCeiling;
            });
        }

        public IAddressUsed CalcAddressUsed(String signal)
        {
            var length = signal.Length;
            if (UseTenCeiling)
            {
                var remain = (length + 2) % 10;
                var number = (length + 2) - remain;
                return new AddressUsed()
                {
                    IndexSize = (number < 0 ? 0 : number) + (remain != 0 ? 10 : 0),
                };
            }
            else
            {
                return new AddressUsed()
                {
                    IndexSize = length + 2,
                };
            }
        }
    }

    public class HoldingCalc : IAddressUsedCalc<Holding>
    {
        public IAddressUsed CalcAddressUsed(Holding signal) =>
            new AddressUsed()
            {
                IndexSize = signal.Length
            };
    }

    public static class AddressUsedCalcExtensions
    {
        public static IServiceCollection AddAddressUsedCalc(this IServiceCollection services)
        {
            var signalTypes = from ty in typeof(SignalWatchVM).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(SignalBase)))
                              let attr = ty.GetCustomAttribute<AddressUsedAttribute>()
                              where attr != null
                              select ty;
            var calcType = typeof(IAddressUsedCalc<>);
            var attrCalcType = typeof(AddressUsedAttrCalc<>);

            foreach (var type in signalTypes)
            {
                var interfaceTy = calcType.MakeGenericType(type);
                var attrCalcTy = attrCalcType.MakeGenericType(type);
                services.AddTransient(interfaceTy, attrCalcTy);
            }

            services.AddTransient<IAddressUsedCalc<String>, StringCalc>();
            services.AddTransient<IAddressUsedCalc<Holding>, HoldingCalc>();

            return services;
        }
    }
}
