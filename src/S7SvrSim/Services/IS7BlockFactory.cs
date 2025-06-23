using S7Svr.Simulator.ViewModels;
using S7SvrSim.Services.DB;
using System.Collections.Generic;

namespace S7SvrSim.Services
{
    public interface IS7BlockFactory
    {
        IS7Block GetMemoryBlockService();
        IS7Block GetDataBlockService(int dbIndex);
    }

    public class S7BlockFactory : IS7BlockFactory
    {
        private readonly IS7DataBlockService db;
        private readonly IS7MBService mb;

        private Dictionary<int, IS7Block> DataBlocks { get; } = new Dictionary<int, IS7Block>();

        public S7BlockFactory(IS7DataBlockService db, IS7MBService mb)
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

            var newBlock = new S7DbBlockService(db, dbIndex);
            DataBlocks.Add(dbIndex, newBlock);
            return DataBlocks[dbIndex];
        }

        public IS7Block GetMemoryBlockService()
        {
            return mb;
        }
    }
}
