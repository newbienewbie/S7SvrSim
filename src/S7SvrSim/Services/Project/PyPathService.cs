using S7SvrSim.Shared;
using System.Linq;

namespace S7SvrSim.Services.Project
{
    public class PyPathService : IPyPathService
    {
        private readonly IEnvProvider envProvider;

        public PyPathService(IEnvProvider envProvider)
        {
            this.envProvider = envProvider;
        }

        public string ReplaceEnv(string value)
        {
            var containsEnv = value.FindWrapped("%").Distinct();

            foreach (var item in envProvider.GetAll().Where(item => containsEnv.Contains(item.Key)))
            {
                value = value.Replace($"%{item.Key}%", item.Value.Value);
            }

            return value;
        }
    }
}
