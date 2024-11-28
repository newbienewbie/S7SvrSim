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
    public class MessageNotificationHandler : 
        INotificationHandler<MessageNotification>,
        INotificationHandler<ScriptTaskCreatedNotification>,
        INotificationHandler<ScriptTaskCompletedNotification>
    {
        public Task Handle(MessageNotification notification, CancellationToken cancellationToken)
        {
            MessageBox.Show(notification.Message);
            return Task.CompletedTask;
        }
        public Task Handle(ScriptTaskCreatedNotification notification, CancellationToken cancellationToken)
        {
           var taskViewModle = Locator.Current.GetRequiredService<ScriptTaskWindowVM>();
            taskViewModle.SubjectTaskData.OnNext(new ScriptTask(
                notification.TaskId, 
                notification.FilePath, 
                notification.TokenSource, 
                notification.PyScope));
            return Task.CompletedTask;
        }

        public Task Handle(ScriptTaskCompletedNotification notification, CancellationToken cancellationToken)
        {
            var taskid = notification.TaskId;
            var taskViewModle = Locator.Current.GetRequiredService<ScriptTaskWindowVM>();
            var task = taskViewModle.ScriptTaskDatas.FirstOrDefault(t => t.TaskId ==  taskid);
            if (task is not null)
            {
                App.Current.Dispatcher.Invoke(() => {
                    taskViewModle.ScriptTaskDatas.Remove(task);
                });
            }
            return Task.CompletedTask;
        }
    }
}
