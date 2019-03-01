using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Сryptolions.Models
{
    public class Action
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("hex_data")]
        public string HexData { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("authorization")]
        public List<Authorization> Authorization { get; set; }

        private ActionData _data { get; set; }

        [JsonProperty("data")]
        public ActionData Data
        {
            get => _data;
            set
            {
                if (value.GetType() == typeof(ActionData))
                {
                    _data = value;
                    return;
                }

                _data = new ActionData();
                return;
            }
        }
    }
}
