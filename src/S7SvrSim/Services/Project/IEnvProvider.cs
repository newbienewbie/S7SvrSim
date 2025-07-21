using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace S7SvrSim.Services.Project
{
    public interface IEnvProvider
    {
        EnvReadonly Get(string name);
        void Set([NotNull] string name, [NotNull] string value);
        void Set([NotNull] string name, [NotNull] EnvReadonly value);
        IEnumerable<KeyValuePair<string, EnvReadonly>> GetAll();
    }
}
