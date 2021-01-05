using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Svr.Simulator.Messages
{
    public class MessageNotification : INotification
    {

        public string Message { get; set; }

    }
}
