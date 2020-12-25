using MediatR;
using S7Svr.Simulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace S7Svr.Simulator.MessageHandlers
{
    public class MessageNotificationHandler : INotificationHandler<MessageNotification>
    {
        public Task Handle(MessageNotification notification, CancellationToken cancellationToken)
        {
            MessageBox.Show(notification.Message);
            return Task.CompletedTask;
        }
    }
}
