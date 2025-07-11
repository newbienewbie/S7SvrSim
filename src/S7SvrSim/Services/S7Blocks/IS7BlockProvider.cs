using S7Svr.Simulator.ViewModels;
using System.Collections.Generic;

namespace S7SvrSim.Services.S7Blocks;

public interface IS7BlockProvider
{
    IS7Block GetMemoryBlockService();
    IS7Block GetDataBlockService(int dbIndex);
}

public class S7BlockProvider : IS7BlockProvider
{
    private readonly IS7DataBlockService db;
    private readonly IS7MBlock mb;

    private Dictionary<int, IS7Block> DataBlocks { get; } = new Dictionary<int, IS7Block>();

    public S7BlockProvider(IS7DataBlockService db, IS7MBlock mb)
    {
        this.db = db;
        this.mb = mb;
    }

    public IS7Block GetDataBlockService(int dbIndex)
    {
        if (DataBlocks.TryGetValue(dbIndex, out IS7Block dataBlock))
        {
            return dataBlock;
        }

        var newBlock = new S7DBlock(db, dbIndex);
        DataBlocks.Add(dbIndex, newBlock);
        return DataBlocks[dbIndex];
    }


    public IS7Block GetMemoryBlockService()
    {
        return mb;
    }
}
