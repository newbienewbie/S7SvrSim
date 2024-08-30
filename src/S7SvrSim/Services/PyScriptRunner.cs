using IronPython.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Scripting.Hosting;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7SvrSim.Services
{
    public class PyScriptRunner
    {
        private readonly IS7DataBlockService _db;
        private readonly IS7MBService _mb;
        private readonly IS7ServerService _server;
        private readonly MsgLoggerVM _loggerVM;
        private readonly ILogger<PyScriptRunner> _logger;
        public ScriptEngine PyEngine { get; }
        private ScriptScope pyScope = null;

        public PyScriptRunner(IS7DataBlockService db, IS7MBService mb, IS7ServerService server, ILogger<PyScriptRunner> logger)
        {
            this._db = db;
            this._mb = mb;
            this._server = server;
            this._loggerVM =  Locator.Current.GetRequiredService<MsgLoggerVM>(); ;
            this._logger = logger;
            this.PyEngine = Python.CreateEngine();
        }



        public void RunFile(string scriptpath)
        {
            if (pyScope is null)
            {
                pyScope = PyEngine.CreateScope();
                pyScope.SetVariable("s7_server_svc", this._db);
                pyScope.SetVariable("S7", this._db);

                pyScope.SetVariable("Server", this._server);
                pyScope.SetVariable("DB", this._db);
                pyScope.SetVariable("MB", this._mb);

                pyScope.SetVariable("Logger", this._loggerVM);
                pyScope.SetVariable("__PY_ENGINE__", this.PyEngine);
            }

            var source = PyEngine.CreateScriptSourceFromFile(scriptpath);
            var code = source.Compile();

            code.Execute(pyScope);

        }
    }
}
