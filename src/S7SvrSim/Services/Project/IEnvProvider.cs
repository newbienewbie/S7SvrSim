using System.Collections.Generic;

namespace S7SvrSim.Services.Project
{
#nullable enable
    public interface IEnvProvider
    {
        Env? Get(string name);
        void Set(string name, string value);
        void Set(string name, Env value);
        Env? Remove(string name);
        IEnumerable<KeyValuePair<string, Env>> GetAll();
    }
#nullable restore
}
