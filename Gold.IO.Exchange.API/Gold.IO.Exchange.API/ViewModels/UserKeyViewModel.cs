using Gold.IO.Exchange.API.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class UserKeyViewModel
    {
        public long ID { get; set; }
        public string KeyValue { get; set; }
        public UserKeyType Type { get; set; }
        public DateTime ActivationTime { get; set; }
        public UserViewModel User { get; set; }
    }
}
