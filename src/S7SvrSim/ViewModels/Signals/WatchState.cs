using S7SvrSim.Services;

namespace S7SvrSim.ViewModels.Signals
{
    public record WatchState(bool IsInWatch);

    public class WatchStateCache : IMemCache<WatchState>
    {
        public WatchState Value { get; private set; }

        public void Write(WatchState value)
        {
            Value = value;
        }
    }
}
