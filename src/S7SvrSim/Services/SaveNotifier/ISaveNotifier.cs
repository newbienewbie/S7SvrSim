using System;

namespace S7SvrSim.Services
{
    public interface ISaveNotifier
    {
        bool NeedSave { get; }
        event EventHandler NeedSaveChanged;
        void NotifyNeedSave(bool needSave = true);
    }
}
