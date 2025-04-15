using FutureTech.Snap7;
using MediatR;
using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator.Messages;
using S7SvrSim.Shared;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace S7Svr.Simulator.ViewModels
{
    /// <summary>
    /// S7 Server 的配置
    /// </summary>
    public class ConfigSnap7ServerVM : ReactiveObject
    {
        private const string SERVER_ITEMS_SAVED_FILE = "areaconfig.csv";
        /// <summary>
        /// IP Address
        /// </summary>
        [Reactive]
        public virtual string IpAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// DB Configs
        /// </summary>
        public virtual ObservableCollection<AreaConfigVM> AreaConfigs { get; } = new ObservableCollection<AreaConfigVM>();

        private string SavedFileName
        {
            get
            {
                var processPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                if (processPath != null)
                {
                    return Path.Combine(processPath, SERVER_ITEMS_SAVED_FILE);
                }
                else
                {
                    return SERVER_ITEMS_SAVED_FILE;
                }
            }
        }

        public ConfigSnap7ServerVM()
        {
            AreaConfigs.CollectionChanged += AreaConfigs_CollectionChanged;
            LoadAreaConfig();
        }

        private void AreaConfigs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SaveAreaConfig();
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems.Cast<AreaConfigVM>())
                    {
                        item.PropertyChanged += AreaConfigVM_PropertyChanged;
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems.Cast<AreaConfigVM>())
                    {
                        item.PropertyChanged -= AreaConfigVM_PropertyChanged;
                    }
                }
            }
        }

        private void AreaConfigVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveAreaConfig();
        }

        private void LoadAreaConfig(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = SavedFileName;
            }

            if (File.Exists(path))
            {
                var fileContent = File.ReadAllLines(path);
                var areaKindType = typeof(AreaKind);
                foreach (var line in fileContent.Skip(1))
                {
                    var items = line.Split(',');
                    if (items.Length < 3)
                    {
                        break;
                    }

                    AreaConfigs.Add(new AreaConfigVM()
                    {
                        AreaKind = (AreaKind)Enum.Parse(areaKindType, items[0]),
                        DBNumber = int.Parse(items[1]),
                        DBSize = int.Parse(items[2]),
                    });
                }
            }
        }

        private void SaveAreaConfig(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = SavedFileName;
            }

            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fileStream.WriteString(string.Join(",", nameof(AreaConfigVM.AreaKind), nameof(AreaConfigVM.DBNumber), nameof(AreaConfigVM.DBSize)));
            foreach (var item in AreaConfigs)
            {
                fileStream.WriteString($"{Environment.NewLine}{string.Join(",", item.AreaKind, item.DBNumber, item.DBSize)}");
            }
        }
    }
}
