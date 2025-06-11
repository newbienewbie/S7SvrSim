using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;

namespace S7Svr.Simulator.ViewModels
{
    /// <summary>
    /// 正在运行的 S7 Server
    /// </summary>
    public class RunningSnap7ServerVM : ConfigSnap7ServerVM
    {

        [Reactive]
        public bool RunningStatus { get; set; }


        /// <summary>
        /// 正在运行的——一旦开始，就不在变化。停止后Clear、再重建。所以这里直接使用了List
        /// </summary>
        public IList<RunningServerItem> RunningsItems { get; } = new List<RunningServerItem>();

        public RunningSnap7ServerVM()
        {
            
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
