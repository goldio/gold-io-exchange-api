using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Domain
{
    public interface IDeletableObject
    {
        /// <summary>
        /// Архивирован
        /// </summary>
        bool Deleted { get; set; }
    }
}
