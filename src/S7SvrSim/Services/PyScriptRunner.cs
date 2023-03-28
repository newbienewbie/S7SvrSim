using IronPython.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Scripting.Hosting;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7SvrSim.Services
{
    public class PyScriptRunner
    {
        private readonly IS7ServerService _s7ServerSvc;
        private readonly MsgLoggerVM _loggerVM;
        private readonly ILogger<PyScriptRunner> _logger;
        public ScriptEngine PyEngine { get; }
        private ScriptScope pyScope = null;

        public PyScriptRunner(IS7ServerService s7ServerSvc, MsgLoggerVM loggerVM ,ILogger<PyScriptRunner> logger)
        {
            this._s7ServerSvc = s7ServerSvc;
            this._loggerVM = loggerVM;
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
                pyScope.SetVariable("Logger", this._loggerVM);
                pyScope.SetVariable("__PY_ENGINE__", this.PyEngine);
            }

            var source = PyEngine.CreateScriptSourceFromFile(scriptpath);
            var code = source.Compile();

            code.Execute(pyScope);

        }
    }
}
