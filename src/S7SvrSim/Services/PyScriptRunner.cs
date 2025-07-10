using IronPython.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Scripting.Hosting;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.Services.S7Blocks;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S7SvrSim.Services
{
    public class PyScriptRunner
    {
        private readonly IS7DataBlockService _db;
        private readonly IS7MBlock _mb;
        private readonly IS7ServerService _server;
        private readonly MsgLoggerVM _loggerVM;
        private readonly ILogger<PyScriptRunner> _logger;
        public ScriptEngine PyEngine { get; }

        public PyScriptRunner(IS7DataBlockService db, IS7MBlock mb, IS7ServerService server, ILogger<PyScriptRunner> logger)
        {
            this._db = db;
            this._mb = mb;
            this._server = server;
            this._loggerVM =  Locator.Current.GetRequiredService<MsgLoggerVM>(); 
            this._logger = logger;
            this.PyEngine = Python.CreateEngine();
            var searchPaths = this.PyEngine.GetSearchPaths();
            var predefined = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "predefined"
                );
            searchPaths.Add(predefined);
            this.PyEngine.SetSearchPaths(searchPaths);
        }



        public void RunFile(ScriptScope pyScope, string scriptpath, CancellationToken token)
        {
            pyScope.SetVariable("s7_server_svc", this._db);
            pyScope.SetVariable("S7", this._db);

            pyScope.SetVariable("Server", this._server);
            pyScope.SetVariable("DB", this._db);
            pyScope.SetVariable("MB", this._mb);

            pyScope.SetVariable("Logger", this._loggerVM);
            pyScope.SetVariable("__PY_ENGINE__", this.PyEngine);

            pyScope.SetVariable("ct", token);


            var source = PyEngine.CreateScriptSourceFromFile(scriptpath);
            var code = source.Compile();

            code.Execute(pyScope);
            

        }
    }
}
