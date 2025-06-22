using System.Windows.Input;

namespace S7SvrSim.Commands
{
    public static class AppCommands
    {
        public static RoutedUICommand OpenFolder { get; } = new RoutedUICommand("打开所在目录", "OpenFolder", typeof(AppCommands));
        public static RoutedUICommand StartServer { get; } = new RoutedUICommand("启动S7服务", "StartServer", typeof(AppCommands), new InputGestureCollection()
        {
            new KeyGesture(Key.F5, ModifierKeys.None, "F5")
        });
        public static RoutedUICommand StopServer { get; } = new RoutedUICommand("停止S7服务", "StopServer", typeof(AppCommands), new InputGestureCollection()
        {
            new KeyGesture(Key.F5, ModifierKeys.Shift, "Shift+F5")
        });
    }
}
