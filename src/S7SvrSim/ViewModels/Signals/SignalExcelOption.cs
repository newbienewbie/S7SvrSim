namespace S7SvrSim.ViewModels.Signals
{
    public enum SameGroupImportRule
    {
        /// <summary>
        /// 不做任何操作
        /// </summary>
        None = 0,
        /// <summary>
        /// 替换同名信号组
        /// </summary>
        ReplaceGroup = 1,
        /// <summary>
        /// 将导入项作为新增项添加到同名信号组中
        /// </summary>
        ExtendGroup = 2,
    }

    public record SignalExcelOption(SameGroupImportRule SameGroupImportRule);
}
