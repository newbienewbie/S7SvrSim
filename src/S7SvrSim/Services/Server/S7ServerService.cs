using MediatR;
using Microsoft.Extensions.Logging;
using S7Svr.Simulator.Messages;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.Threading.Tasks;

namespace S7Svr.Simulator.ViewModels;

internal class S7ServerService: IDisposable, IS7ServerService
{
    private readonly RunningSnap7ServerVM _runningVM;
    private readonly MsgLoggerVM _loggerVM;
    private readonly ILogger<S7DataBlockService> _logger;
    protected virtual IMediator _mediator { get; set; }
    protected virtual FutureTech.Snap7.S7Server S7Server { get; set; }

    public S7ServerService(IMediator mediator, ILogger<S7DataBlockService> logger)
    {
        this._mediator = mediator;
        this._runningVM = Locator.Current.GetRequiredService<RunningSnap7ServerVM>();
        this._loggerVM = Locator.Current.GetRequiredService<MsgLoggerVM>(); 
        this._logger = logger;
    }


    public async Task StartServerAsync()
    {
        try
        {
            this.S7Server = new FutureTech.Snap7.S7Server();

            if (_runningVM.AreaConfigs == null)
            {
                var msg = new MessageNotification() { Message = "当前 DBConfigs 为 NULL !" };
                await this._mediator.Publish(msg);
                return;
            }

            _runningVM.RunningsItems.Clear();

            foreach (var area in _runningVM.AreaConfigs)
            {
                var buffer = new byte[area.BlockSize];
                _runningVM.RunningsItems.Add(new RunningServerItem
                {
                    AreaKind = area.AreaKind,
                    BlockNumber = area.BlockNumber,
                    BlockSize = area.BlockSize,
                    Bytes = buffer,
                });
                switch (area.AreaKind)
                {
                    case AreaKind.DB:
                        this.S7Server.RegisterArea(FutureTech.Snap7.S7Server.srvAreaDB, area.BlockNumber, ref buffer, area.BlockSize);
                        break;
                    case AreaKind.MB:
                        this.S7Server.RegisterArea(FutureTech.Snap7.S7Server.srvAreaMK, area.BlockNumber, ref buffer, area.BlockSize);
                        break;
                    default:
                        throw new NotImplementedException($"未知的区域类型={area}");
                }
            }
            this.S7Server.StartTo(_runningVM.IpAddress);

            this._loggerVM.AddLogMsg(new LogMessage(DateTime.Now, Microsoft.Extensions.Logging.LogLevel.Information, "[+]服务启动..."));
        }
        catch (Exception ex)
        {
            var msg = $"启动服务器出错：{ex.Message}";
            this._logger.LogError(msg);
            await this._mediator.Publish(new MessageNotification
            {
                Message = msg
            });
        }
    }

    public async Task StopServerAsync()
    {
        try
        {
            this.S7Server?.Stop();
            this.S7Server = null;
            this._loggerVM.AddLogMsg(new LogMessage(DateTime.Now, Microsoft.Extensions.Logging.LogLevel.Information, "[!]服务停止..."));
        }
        catch (Exception ex)
        {
            var msg = $"停止服务器出错：{ex.Message}";
            this._logger.LogError(msg);
            await this._mediator.Publish(new MessageNotification { Message = msg });
        }
    }

    public void Dispose()
    {
        _ = this.StopServerAsync();
    }
}