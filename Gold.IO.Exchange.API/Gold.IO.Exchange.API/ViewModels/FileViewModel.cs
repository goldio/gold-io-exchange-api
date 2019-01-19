using Gold.IO.Exchange.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class FileViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }

        public FileViewModel() { }

        public FileViewModel(File file)
        {
            if (file != null)
            {
                ID = file.ID;
                Name = file.Name;
                URL = file.URL;
            }
        }
    }
}
