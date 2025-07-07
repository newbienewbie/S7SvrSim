using MaterialDesignThemes.Wpf;
using MediatR;
using S7SvrSim.Messages;
using S7SvrSim.UserControls;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace S7SvrSim.MessageHandlers
{
    public class ShowDialogRequestHandler : IRequestHandler<ShowDialogRequest, ShowDialogResult>
    {
        public async Task<ShowDialogResult> Handle(ShowDialogRequest request, CancellationToken cancellationToken)
        {
            var result = await Application.Current.MainWindow.ShowDialog(new DialogCtrl(request.ViewModel));

            return new ShowDialogResult(result == null, result?.ToString());
        }
    }
}
