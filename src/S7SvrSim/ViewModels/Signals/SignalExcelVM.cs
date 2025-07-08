using DynamicData;
using DynamicData.Binding;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using S7SvrSim.Services.Settings;
using Splat;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace S7SvrSim.ViewModels.Signals
{
    public class SignalExcelVM : ReactiveObject
    {
        private readonly SignalsCollection signals;

        [Reactive]
        public SameGroupImportRule SameGroupImportRule { get; set; }

        public ICommand ExportSignalsCommand { get; }
        public ICommand ImportSignalsCommand { get; }
        public ICommand SetSameGroupRuleCommand { get; }

        public SignalExcelVM(ISetting<SignalExcelOption> setting)
        {
            signals = Locator.Current.GetRequiredService<SignalsCollection>();

            ExportSignalsCommand = ReactiveCommand.Create(ExportSignals);
            ImportSignalsCommand = ReactiveCommand.Create(ImportSignals);
            SetSameGroupRuleCommand = ReactiveCommand.Create<SameGroupImportRule>(SetSameGroupRule);

            setting.Value.Subscribe(options =>
            {
                SameGroupImportRule = options.SameGroupImportRule;
            });

            this.WhenAnyPropertyChanged()
                .Subscribe(vm =>
                {
                    setting.Write(new SignalExcelOption(SameGroupImportRule));
                });
        }

        private void CommandEventHandle(object _object, EventArgs _args)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SwitchTab(2);
        }

        private void RegistCommandEventHandle(IHistoryCommand command)
        {
            command.AfterExecute += CommandEventHandle;
            command.AfterUndo += CommandEventHandle;
        }

        private void SetSameGroupRule(SameGroupImportRule rule)
        {
            SameGroupImportRule = rule;
        }

        public class ExcelSignalModel
        {
            public string Name { get; set; }
            public string SignalType { get; set; }
            public string FormatAddress { get; set; }
            public string Remark { get; set; }

            public SignalEditObj ToEditObj()
            {
                return new SignalEditObj(SignalType, Name, FormatAddress, Remark);
            }
        }

        private void ExportSignals()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "选择信号导出位置",
                FileName = "signals.xlsx",
                Filter = "Excel(*.xlsx)|*.xlsx",
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            var sheets = signals.SignalGroups.ToDictionary(sg => sg.Name, sg => sg.Signals.Select(s => new ExcelSignalModel
            {
                Name = s.Value.Name,
                SignalType = s.SignalType,
                FormatAddress = s.Value.FormatAddress,
                Remark = s.Value.Remark
            }));
            try
            {
                using var stream = File.Create(saveFileDialog.FileName);
                using var package = new ExcelPackage(stream);
                foreach (var item in sheets)
                {
                    var sheet = package.Workbook.Worksheets.Add(item.Key);
                    sheet.Cells.LoadFromCollection(item.Value, true);
                    sheet.Cells.Style.Font.Name = "Arial";

                    var headerFirst = sheet.Cells[1, 1];
                    headerFirst.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerFirst.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 112, 192));
                    headerFirst.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerFirst.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    headerFirst.CopyStyles(sheet.Cells[1, 1, 1, 4]);

                    var dataCount = item.Value.Count();
                    if (dataCount > 0)
                    {
                        var dataFirst = sheet.Cells[2, 1];
                        dataFirst.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        dataFirst.CopyStyles(sheet.Cells[2, 1, dataCount + 1, 4]);
                    }

                    sheet.Columns.AutoFit();
                }
                package.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("导出成功！");
        }

        private void ImportSignals()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "选择需要导入的Excel",
                Filter = "Excel(*.xlsx)|*.xlsx",
                RestoreDirectory = true,
                Multiselect = false,
            };

            if (openFileDialog.ShowDialog() != true)
                return;
            var groupName = signals.GroupName;

            UndoRedoManager.StartTransaction();
            try
            {
                using var package = new ExcelPackage(openFileDialog.FileName);
                var headers = new string[] { "Name", "SignalType", "FormatAddress", "Remark" };

                foreach (var sheet in package.Workbook.Worksheets)
                {
                    var models = sheet.Cells[1, 1, sheet.Rows.Count(), sheet.Columns.Count()].ToCollection<ExcelSignalModel>().Where(sm => !string.IsNullOrEmpty(sm.SignalType));
                    if (!models.Any()) continue;

                    var signals = models.Select(s => s.ToEditObj());

                    if (this.signals.SignalGroups.Any(sg => sg.Name == sheet.Name))
                    {
                        switch (SameGroupImportRule)
                        {
                            case SameGroupImportRule.ReplaceGroup:
                                var signalGroup = new SignalEditGroup(sheet.Name, signals);
                                UndoRedoManager.Run(ListChangedCommand.Replace(this.signals.SignalGroups, [(OldItem: this.signals.SignalGroups.First(sg => sg.Name == signalGroup.Name), NewItem: signalGroup)]));
                                break;
                            case SameGroupImportRule.ExtendGroup:
                                var first = this.signals.SignalGroups.First(sg => sg.Name == sheet.Name);
                                UndoRedoManager.Run(ListChangedCommand.Add(first.Signals, signals));
                                break;
                            case SameGroupImportRule.None:
                            default:
                                break;
                        }
                    }
                    else
                    {
                        UndoRedoManager.Run(ListChangedCommand.Add(this.signals.SignalGroups, [new SignalEditGroup(sheet.Name, signals)]));
                    }
                }
            }
            catch (Exception ex)
            {
                UndoRedoManager.RollbackTransaction();
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                signals.GroupName = groupName;
            }
            var command = UndoRedoManager.EndTransaction();
            RegistCommandEventHandle(command);
            command.AfterExecute += (_, _) =>
            {
                if (string.IsNullOrEmpty(signals.GroupName))
                {
                    signals.GroupName = groupName;
                }
            };
            command.AfterUndo += (_, _) =>
            {
                if (string.IsNullOrEmpty(signals.GroupName))
                {
                    signals.GroupName = groupName;
                }
            };
            MessageBox.Show("导入成功！");
        }
    }
}
