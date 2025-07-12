using DynamicData;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace S7Svr.Simulator.ViewModels
{
    /// <summary>
    /// 正在运行的 S7 Server
    /// </summary>
    public partial class RunningSnap7ServerVM : ConfigSnap7ServerVM
    {

        [Reactive]
        public bool RunningStatus { get; set; }

        public ObservableCollection<RunningServerItem> RunningsItems { get; } = new ObservableCollection<RunningServerItem>();

        public RunningSnap7ServerVM()
        {
            registCommand = false;
        }

        public void Clear()
        {
            RunningsItems.Clear();
        }

        public void Add(RunningServerItem item)
        {
            RunningsItems.Add(item);
        }
    }


    /// <summary>
    /// S7 Server 中的区域（子项）
    /// </summary>
    public class RunningServerItem : ReactiveObject
    {
        [Reactive]
        public AreaKind AreaKind { get; set; }

        [Reactive]
        public int BlockNumber { get; set; }

        [Reactive]
        public int BlockSize { get; set; }

        [Reactive]
        public byte[] Bytes { get; set; }
    }
}
