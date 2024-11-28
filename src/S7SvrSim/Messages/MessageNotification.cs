using MediatR;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace S7Svr.Simulator.Messages
{
    public class MessageNotification : INotification
    {

        public string Message { get; set; }

    }


    public record ScriptTaskCreatedNotification(
        Guid TaskId, 
        ScriptScope PyScope, 
        string FilePath, 
        CancellationTokenSource TokenSource
        ) : INotification;

    public record ScriptTaskCompletedNotification(
        Guid TaskId
        ) : INotification;

}
