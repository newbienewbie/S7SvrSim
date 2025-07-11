using System.Threading.Tasks;

namespace S7Svr.Simulator.ViewModels;

public interface IS7ServerService
{
    Task StartServerAsync();
    Task StopServerAsync();
}