using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace S7SvrSim.Services.Project
{
    public class EnvProvider : IEnvProvider
    {
        private Dictionary<string, Env> Envs { get; } = new Dictionary<string, Env>();

        public EnvReadonly Get(string name)
        {
            if (Envs.TryGetValue(name, out var value)) return value;

            return null;
        }

        public void Set([NotNull] string name, [NotNull] string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (Envs.ContainsKey(name)) Envs[name].Value = value;
            else Envs.Add(name, new Env() { Value = value });
        }

        public void Set([NotNull] string name, [NotNull] EnvReadonly value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (Envs.ContainsKey(name)) Envs[name] = value;
            else Envs.Add(name, value);
        }

        public IEnumerable<KeyValuePair<string, EnvReadonly>> GetAll()
        {
            return Envs.Select(item => new KeyValuePair<string, EnvReadonly>(item.Key, item.Value));
        }
    }
}
