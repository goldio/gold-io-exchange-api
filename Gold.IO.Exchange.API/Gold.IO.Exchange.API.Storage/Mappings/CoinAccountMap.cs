using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain.Coin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class CoinAccountMap : ClassMap<CoinAccount>
    {
        public CoinAccountMap()
        {
            Table("coin_accounts");

            Id(u => u.ID, "id");

            References(e => e.Coin, "id_coin");

            Map(u => u.AccountKey, "account_key");
            Map(u => u.Derivations, "derivations_count");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
