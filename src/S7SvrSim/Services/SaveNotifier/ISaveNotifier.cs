using System;

namespace S7SvrSim.Services;

public interface ISaveNotifier
{
    bool NeedSave { get; }
    event EventHandler<NeedSaveChangedEventArgs> NeedSaveChanged;
    internal void NotifyNeedSave(bool needSave = true);
}


public class NeedSaveChangedEventArgs: EventArgs
{
    public bool NeedSave { get; set; }
}
