using S7SvrSim.S7Signal;
using S7SvrSim.ViewModels;
using System;
using System.Linq;

namespace S7SvrSim.Services
{
    public record SignalType(string Name, Type Type);

    public class SignalTypeCache : IMemCache<SignalType[]>
    {
        private const string SIGNAL_TYPE_SUFFIX = "Signal";

        public SignalTypeCache()
        {
            Value = typeof(SignalWatchVM).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(SignalBase))).Select(ty =>
            {
                var tyName = ty.Name;
                var suffixIndex = tyName.IndexOf(SIGNAL_TYPE_SUFFIX, StringComparison.OrdinalIgnoreCase);
                if (suffixIndex >= 0)
                {
                    tyName = tyName.Remove(suffixIndex, SIGNAL_TYPE_SUFFIX.Length);
                }

                return new SignalType(tyName, ty);
            }).ToArray();
        }

        public SignalType[] Value { get; }

        public void Write(SignalType[] value)
        {

        }
    }
}
