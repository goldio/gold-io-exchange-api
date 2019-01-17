using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class FileMap : ClassMap<File>
    {
        public FileMap()
        {
            Table("files");

            Id(u => u.ID, "id");

            Map(u => u.Name, "name");
            Map(u => u.LocalPath, "local_path");
            Map(u => u.URL, "url");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
