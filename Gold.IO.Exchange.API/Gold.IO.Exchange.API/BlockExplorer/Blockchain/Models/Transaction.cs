using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Blockchain.Models
{
    public class Transaction
    {
        [JsonProperty("block_height")]
        public long BlockHeight { get; set; }

        [JsonProperty("block_index")]
        public long BlockIndex { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("inputs")]
        public List<Input> Inputs { get; set; }

        [JsonProperty("lock_time")]
        public long LockTime { get; set; }

        [JsonProperty("out")]
        public List<Output> Out { get; set; }

        [JsonProperty("relayed_by")]
        public string RelayedBy { get; set; }

        [JsonProperty("result")]
        public long Result { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("tx_index")]
        public long TxIndex { get; set; }

        [JsonProperty("ver")]
        public int Ver { get; set; }

        [JsonProperty("vin_sz")]
        public long ValueInSize { get; set; }

        [JsonProperty("vout_sz")]
        public long ValueOutSize { get; set; }

        [JsonProperty("weight")]
        public long Weight { get; set; }
    }
}
