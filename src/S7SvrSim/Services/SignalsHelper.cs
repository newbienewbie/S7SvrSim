using S7SvrSim.S7Signal;
using S7SvrSim.ViewModels;
using System;
using System.Linq;

namespace S7SvrSim.Services
{
    public class SignalsHelper
    {

        public SignalsHelper()
        {
            SignalTypes = [.. typeof(SignalWatchVM).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(SignalBase)))];
        }

        public Type[] SignalTypes { get; }
    }
}
