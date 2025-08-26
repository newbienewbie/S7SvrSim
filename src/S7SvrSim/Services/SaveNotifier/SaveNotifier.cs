using System;

namespace S7SvrSim.Services
{
    internal class SaveNotifier : ISaveNotifier
    {
        public bool NeedSave { get; private set; }

        public event EventHandler NeedSaveChanged;

        public void NotifyNeedSave(bool needSave = true)
        {
            if (NeedSave != needSave)
            {
                NeedSave = needSave;
                NeedSaveChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
