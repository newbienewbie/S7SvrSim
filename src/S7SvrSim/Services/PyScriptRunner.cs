using IronPython.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Scripting.Hosting;
using S7Svr.Simulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7SvrSim.Services
{
    public class PyScriptRunner
    {
        private readonly S7ServerService _s7ServerSvc;
        private readonly ILogger<PyScriptRunner> _logger;
        private ScriptEngine pyEngine = null;
        private ScriptScope pyScope = null;

        public PyScriptRunner(S7ServerService s7ServerSvc, ILogger<PyScriptRunner> logger)
        {
            this._s7ServerSvc = s7ServerSvc;
            this._logger = logger;
        }

        public void RunFile(string scriptpath)
        {
            if (pyEngine is null)
            {
                pyEngine = Python.CreateEngine();
                pyScope = pyEngine.CreateScope();
                pyScope.SetVariable("s7_server_svc", this._s7ServerSvc);
                pyScope.SetVariable("S7", this._s7ServerSvc);
            }

            var source = pyEngine.CreateScriptSourceFromFile(scriptpath);
            var code = source.Compile();

            code.Execute(pyScope);
        }
    }
}
