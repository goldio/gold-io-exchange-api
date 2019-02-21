using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain.Coin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class CoinAddressMap : ClassMap<CoinAddress>
    {
        public CoinAddressMap()
        {
            Table("coin_addresses");

            Id(u => u.ID, "id");

            References(e => e.Wallet, "id_wallet");

            Map(u => u.PrivateKey, "private_key");
            Map(u => u.PublicAddress, "public_address");
            Map(u => u.IsUsing, "is_using").Not.Nullable();
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
