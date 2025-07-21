using System;
using System.Diagnostics.CodeAnalysis;

namespace S7SvrSim.Services.Project
{
    public class Env
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }

    public class EnvReadonly
    {
        private readonly Env _env;

        public string Value => _env.Value;
        public string Description => _env.Description;

        public EnvReadonly([NotNull] string value, string description = null)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            _env = new Env() { Value = value, Description = description };
        }

        public EnvReadonly(Env env)
        {
            _env = env;
        }

        public static implicit operator EnvReadonly(Env env)
        {
            return new EnvReadonly(env);
        }

        public static implicit operator Env(EnvReadonly env)
        {
            return env._env;
        }
    }
}
