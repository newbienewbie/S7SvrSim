using S7SvrSim.S7Signal;
using System;
using IOPath = System.IO.Path;

namespace S7SvrSim.Services.Project
{
    public class DefaultProject : SoftwareProject
    {
        protected const string DEFAULT_FILENAME = "unamed";

        public DefaultProject(IServiceProvider serviceProvider, SignalsHelper signalsHelper)
            : base(IOPath.Combine(IOPath.GetDirectoryName(Environment.ProcessPath)
                , DEFAULT_FILENAME
                , $"{DEFAULT_FILENAME}{ProjectConst.FILE_EXTENSION}")
              , serviceProvider, signalsHelper)
        {
        }
    }
}
