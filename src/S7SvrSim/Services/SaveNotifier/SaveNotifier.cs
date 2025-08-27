using System;

namespace S7SvrSim.Services;

internal class SaveNotifier : ISaveNotifier
{
    public bool NeedSave { get; private set; }

    public event EventHandler<NeedSaveChangedEventArgs> NeedSaveChanged;

    void ISaveNotifier.NotifyNeedSave(bool needSave)
    {
        if (NeedSave != needSave)
        {
            NeedSave = needSave;
            var args = new NeedSaveChangedEventArgs()
            {
                NeedSave = needSave,
            };
            NeedSaveChanged?.Invoke(this, args);
        }
    }
}
