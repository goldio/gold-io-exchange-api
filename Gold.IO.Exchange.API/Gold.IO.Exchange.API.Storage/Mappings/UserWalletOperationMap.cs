using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class UserWalletOperationMap : ClassMap<UserWalletOperation>
    {
        public UserWalletOperationMap()
        {
            Table("user_wallet_operations");

            Id(u => u.ID, "id");

            References(e => e.Wallet, "id_wallet");

            Map(u => u.Address, "address");
            Map(u => u.Amount, "amount");
            Map(u => u.Confirmations, "confirmation");
            Map(u => u.Type, "type").CustomType<UserWalletOperationType>();
            Map(u => u.Status, "status").CustomType<UserWalletOperationStatus>();
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
