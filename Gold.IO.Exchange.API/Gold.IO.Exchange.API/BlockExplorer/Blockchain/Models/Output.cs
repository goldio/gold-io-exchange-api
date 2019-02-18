using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Blockchain.Models
{
    public class Output
    {
        [JsonProperty("value")]
        private long _value { get; set; }

        [JsonProperty("addr")]
        public string Addr { get; set; }

        [JsonProperty("n")]
        public int N { get; set; }

        [JsonProperty("script")]
        public string Script { get; set; }

        [JsonProperty("spending_outpoints")]
        public List<SpendingOutpoint> SpendingOutpoints { get; set; }

        [JsonProperty("spent")]
        public bool Spent { get; set; }

        [JsonProperty("tx_index")]
        public long TxIndex { get; set; }

        // TO-DO
        [JsonProperty("type")]
        public int Type { get; set; }

        public double Value
        {
            get => _value / 100000000;
        }
    }
}
