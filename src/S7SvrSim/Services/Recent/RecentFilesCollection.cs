using DynamicData;
using DynamicData.Binding;
using S7SvrSim.Services.Settings;
using System;
using System.Linq;

namespace S7SvrSim.Services.Recent
{
    public class RecentFilesCollection
    {
        private readonly ISourceCache<RecentFile, string> _files = new SourceCache<RecentFile, string>(f => f.Path);

        public IObservableList<RecentFile> Files { get; }

        public RecentFilesCollection(ISetting<RecentFile[]> setting)
        {
            Files = _files
                .Connect()
                .RemoveKey()
                .AsObservableList();

            setting.Value.Subscribe(files =>
            {
                _files.Edit(innerCache =>
                {
                    var newItems = files.Where(f => !innerCache.Lookup(f.Path).HasValue).ToArray();
                    innerCache.AddOrUpdate(newItems);
                });
            });

            _files.Connect().ToCollection().Subscribe(items => setting.Write(items.ToArray()));
        }

        public void AddFile(RecentFile file)
        {
            _files.AddOrUpdate(file);
        }

        public void RemoveFile(RecentFile file)
        {
            _files.Remove(file);
        }
    }
}
