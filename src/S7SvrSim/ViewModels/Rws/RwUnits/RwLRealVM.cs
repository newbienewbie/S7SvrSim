﻿using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.ViewModels.Rw;

public class RwLRealVM : RwVMBase<double>
{
    public RwLRealVM(IS7DataBlockService s7ServerService) : base(s7ServerService)
    {
    }


    protected override void CmdWrite_Impl()
    {
        this._s7ServerService.WriteLReal(this.RwVM.TargetDBNumber, this.RwVM.TargetPos, this.ToBeWritten);
    }

    protected override double CmdRead_Impl()
    {
        var val = this._s7ServerService.ReadLReal(this.RwVM.TargetDBNumber, this.RwVM.TargetPos);
        this.ValueRead = val;
        return val;
    }

}
