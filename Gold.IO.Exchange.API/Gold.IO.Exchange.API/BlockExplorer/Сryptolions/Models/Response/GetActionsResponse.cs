using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Сryptolions.Models.Response
{
    public class GetActionsResponse
    {
        [JsonProperty("actions")]
        public List<GlobalAction> Actions { get; set; }
    }
}
