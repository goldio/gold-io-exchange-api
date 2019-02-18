using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Blockchain.Models
{
    public class Input
    {
        [JsonProperty("prev_out")]
        public Output PrevOut { get; set; }

        [JsonProperty("script")]
        public string Script { get; set; }

        [JsonProperty("sequence")]
        public long Sequence { get; set; }

        [JsonProperty("witness")]
        public string Witness { get; set; }
    }
}
