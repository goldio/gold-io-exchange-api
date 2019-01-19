using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Response
{
    public class DataResponse<T> : ResponseModel
    {
        public T Data { get; set; }
    }
}
