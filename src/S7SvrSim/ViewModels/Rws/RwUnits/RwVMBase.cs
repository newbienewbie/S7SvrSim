using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator.Messages;
using S7Svr.Simulator.ViewModels;
using Splat;
using System;

namespace S7SvrSim.ViewModels.Rw;

/// <summary>
/// 读写
/// </summary>
public abstract class RwVMBase<TValue> : ReactiveObject
{
    protected readonly IS7DataBlockService _s7ServerService;
    protected readonly MediatR.IMediator mediator;

    public RwTargetVM RwVM { get; }

    public RwVMBase(IS7DataBlockService s7ServerService, MediatR.IMediator mediator)
    {
        this._s7ServerService = s7ServerService;
        this.mediator = mediator;
        this.RwVM = Locator.Current.GetRequiredService<RwTargetVM>();

        this.CmdWrite = ReactiveCommand.Create(CmdWrite_Base);
        this.CmdRead = ReactiveCommand.Create(CmdRead_Base);
    }

    private void CmdWrite_Base()
    {
        try
        {
            CmdWrite_Impl();
        }
        catch (ArgumentException argEx)
        {
            this.mediator.Publish(new MessageNotification { Message = argEx.Message });
        }
    }

    private TValue CmdRead_Base()
    {
        try
        {
            return CmdRead_Impl();
        }
        catch (ArgumentException argEx)
        {
            this.mediator.Publish(new MessageNotification { Message = argEx.Message });
            if (typeof(TValue) == typeof(string))
            {
                return (TValue)(object)string.Empty;
            }
            return default;
        }
    }

    #region Byte Read And Write
    /// <summary>
    /// 要写入的
    /// </summary>
    [Reactive]
    public TValue ToBeWritten { get; set; }

    /// <summary>
    /// 所读取的
    /// </summary>
    [Reactive]
    public TValue ValueRead { get; set; }


    public ReactiveCommand<Unit, Unit> CmdWrite { get; protected set; }
    protected abstract void CmdWrite_Impl();

    public ReactiveCommand<Unit, TValue> CmdRead { get; protected set; }
    protected abstract TValue CmdRead_Impl();
    #endregion
}
