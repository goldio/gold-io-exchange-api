using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain.User;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
   
    public class UserWalletMap : ClassMap<UserWallet>
    {
        public UserWalletMap()
        {
            Table("user_wallets");

            Id(u => u.ID, "id");

            References(e => e.Coin, "id_coin");
            References(e => e.User, "id_user");
            Map(u => u.Balance, "balance");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
