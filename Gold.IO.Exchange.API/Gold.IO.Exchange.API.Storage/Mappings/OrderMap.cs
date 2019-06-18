using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.Order;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Table("orders");

            Id(u => u.ID, "id");

            References(e => e.User, "id_user");
            References(e => e.BaseAsset, "id_base");
            References(e => e.QuoteAsset, "id_quote");

            Map(u => u.Amount, "amount");
            Map(u => u.Balance, "balance");
            Map(u => u.Price, "price");
            Map(u => u.Type, "type").CustomType<OrderType>();
            Map(u => u.Side, "side").CustomType<OrderSide>();
            Map(u => u.Status, "status").CustomType<OrderStatus>();
            Map(u => u.Time, "time");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
