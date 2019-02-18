using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class WalletAddressMap : ClassMap<WalletAddress>
    {
        public WalletAddressMap()
        {
            Table("wallet_addresses");

            Id(x => x.ID, "id");

            References(e => e.Coin, "id_coin");

            Map(u => u.Address, "address");
            Map(u => u.IsUsed, "is_used");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
