using System;
using System.Collections.Generic;

namespace S7SvrSim.Services.Project
{
#nullable enable
    public class EnvProvider : IEnvProvider
    {
        private Dictionary<string, Env> Envs { get; } = new Dictionary<string, Env>();

        public Env? Get(string name)
        {
            if (Envs.TryGetValue(name, out var value)) return value;

            return null;
        }

        public void Set(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (Envs.ContainsKey(name))
            {
                var env = Envs[name];
                Envs[name] = new Env(value, env.Description);
            }
            else
            {
                Envs.Add(name, new Env(value));
            }
        }

        public void Set(string name, Env value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (Envs.ContainsKey(name)) Envs[name] = value;
            else Envs.Add(name, value);
        }

        public Env? Remove(string name)
        {
            Envs.Remove(name, out var env);
            return env;
        }

        public IEnumerable<KeyValuePair<string, Env>> GetAll()
        {
            return Envs;
        }
    }
#nullable restore
}
