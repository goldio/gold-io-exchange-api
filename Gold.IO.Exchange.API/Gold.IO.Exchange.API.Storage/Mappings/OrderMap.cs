using FluentNHibernate.Mapping;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Domain.Enum;

namespace Gold.IO.Exchange.API.Storage.Mappings
{
    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Table("orders");

            Id(u => u.ID, "id");

            References(e => e.BaseAsset, "id_base");
            References(e => e.BaseAsset, "id_quote");

            Map(u => u.Amount, "amount");
            Map(u => u.Price, "price");
            Map(u => u.Type, "type").CustomType<OrderType>();
            Map(u => u.Status, "status").CustomType<OrderStatus>();
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
