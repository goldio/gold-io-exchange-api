using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Blockchain.Models
{
    public class SpendingOutpoint
    {
        [JsonProperty("n")]
        public int N { get; set; }

        [JsonProperty("tx_index")]
        public long TxIndex { get; set; }
    }
}
