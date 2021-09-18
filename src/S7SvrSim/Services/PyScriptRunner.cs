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
        public ScriptEngine PyEngine { get; }
        private ScriptScope pyScope = null;

        public PyScriptRunner(S7ServerService s7ServerSvc, ILogger<PyScriptRunner> logger)
        {
            this._s7ServerSvc = s7ServerSvc;
            this._logger = logger;
            this.PyEngine = Python.CreateEngine();
        }



        public void RunFile(string scriptpath)
        {
            if (pyScope is null)
            {
                pyScope = PyEngine.CreateScope();
                pyScope.SetVariable("s7_server_svc", this._s7ServerSvc);
                pyScope.SetVariable("S7", this._s7ServerSvc);
                pyScope.SetVariable("__PY_ENGINE__", this.PyEngine);
            }

            var source = PyEngine.CreateScriptSourceFromFile(scriptpath);
            var code = source.Compile();

            code.Execute(pyScope);
        }
    }
}
