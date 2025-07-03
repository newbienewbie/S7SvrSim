using System.Windows.Input;

namespace S7SvrSim.Commands
{
    public static class AppCommands
    {
        public static RoutedUICommand OpenFolder { get; } = new RoutedUICommand("打开所在目录", "OpenFolder", typeof(AppCommands));
    }
}
