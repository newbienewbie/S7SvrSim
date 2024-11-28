using MediatR;
using S7Svr.Simulator.Messages;
using S7SvrSim.UserControls;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace S7Svr.Simulator.MessageHandlers
{
    public class MessageNotificationHandler : INotificationHandler<MessageNotification>,INotificationHandler<MessageScriptTaskNotification>
    {
        public Task Handle(MessageNotification notification, CancellationToken cancellationToken)
        {
            MessageBox.Show(notification.Message);
            return Task.CompletedTask;
        }
        public Task Handle(MessageScriptTaskNotification notification, CancellationToken cancellationToken)
        {
           var taskViewModle = Locator.Current.GetRequiredService<ScriptTaskWindowVM>();
            taskViewModle.SubjectTaskData.OnNext(new ScriptTask() { FilePath = notification.FilePath,TokenSource=notification.TaskDisposable});
            return Task.CompletedTask;
        }
    }
}
