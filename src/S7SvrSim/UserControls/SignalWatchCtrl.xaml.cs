using MediatR;
using Microsoft.Extensions.DependencyInjection;
using S7Svr.Simulator;
using S7Svr.Simulator.Messages;
using S7SvrSim.Shared;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace S7SvrSim.UserControls
{
    /// <summary>
    /// SignalWatchCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class SignalWatchCtrl : UserControl, IViewFor<SignalWatchVM>
    {
        private readonly IMediator mediator;
        public SignalWatchCtrl()
        {
            InitializeComponent();

            mediator = ((App)Application.Current).ServiceProvider.GetRequiredService<IMediator>();

            this.WhenActivated(d =>
            {
                ViewModel = Locator.Current.GetRequiredService<SignalWatchVM>();
                ViewModel.ScanSpans.Each(span =>
                {
                    span.WhenAnyValue(s => s.Boolean)
                        .Subscribe(enable =>
                        {
                            if (enable)
                            {
                                ViewModel.ScanSpans.Where(s => s != span).Each(s => s.Boolean = false);
                            }
                        })
                        .DisposeWith(d);
                });
            });
        }

        public SignalWatchVM ViewModel { get => (SignalWatchVM)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (SignalWatchVM)value; }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SignalWatchVM), typeof(SignalWatchCtrl), new PropertyMetadata(null));
    }
}
