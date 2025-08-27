using ReactiveUI.Fody.Helpers;

namespace S7SvrSim.Shared
{
    public class ObjectWithBool<T> : ReactiveObject
    {
        [Reactive]
        public T Value { get; set; }
        [Reactive]
        public bool Boolean { get; set; }
    }

    public partial class ObjectWith<T, S> : ReactiveObject
    {
        [Reactive]
        public T Value { get; set; }
        [Reactive]
        public S Other { get; set; }
    }
}
