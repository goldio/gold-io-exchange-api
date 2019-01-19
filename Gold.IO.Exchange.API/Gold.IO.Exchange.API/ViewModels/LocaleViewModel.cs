using Gold.IO.Exchange.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class LocaleViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public FileViewModel Icon { get; set; }
        public string LangCode { get; set; }

        public LocaleViewModel() { }

        public LocaleViewModel(Locale locale)
        {
            if (locale != null)
            {
                ID = locale.ID;
                Name = locale.Name;
                LangCode = locale.LangCode;

                Icon = new FileViewModel(locale.Icon);
            }
        }
    }
}
