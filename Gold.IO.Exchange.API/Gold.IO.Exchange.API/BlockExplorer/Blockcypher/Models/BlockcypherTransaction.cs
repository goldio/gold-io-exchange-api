using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Blockcypher.Models
{
    public class BlockcypherTransaction
    {
        [JsonProperty("block_height")]
        public long BlockNumber { get; set; }

        [JsonProperty("confirmed")]
        public string TimeStamp { get; set; }

        [JsonProperty("tx_hash")]
        public string Hash { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }

        [JsonProperty("confirmations")]
        public long Confirmations { get; set; }

        [JsonProperty("tx_input_n")]
        public int TxInputN { get; set; }

        [JsonProperty("tx_output_n")]
        public int TxOutputN { get; set; }
    }
}
