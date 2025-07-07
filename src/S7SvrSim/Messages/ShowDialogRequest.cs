using MediatR;
using S7SvrSim.ViewModels;

namespace S7SvrSim.Messages
{
    public class ShowDialogRequest : IRequest<ShowDialogResult>
    {
        public DialogViewModel ViewModel { get; }

        public ShowDialogRequest(DialogViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }

    public class ShowDialogResult
    {
        public ShowDialogResult(bool isCancel, string result)
        {
            IsCancel = isCancel;
            Result = result;
        }

        public bool IsCancel { get; }
        public string Result { get; }
    }
}
