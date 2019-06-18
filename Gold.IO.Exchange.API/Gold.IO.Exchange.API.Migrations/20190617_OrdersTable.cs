using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Migrations
{
    [Migration(20190617121801)]
    public class OrdersTable : Migration
    {
        public override void Up()
        {

            Rename.Column("type")
                .OnTable("orders")
                .To("side");

            Create.Column("type")
                .OnTable("orders")
                .AsInt32();
        }

        public override void Down()
        {
            Delete.Column("type")
                .FromTable("orders");

            Rename.Column("side")
                .OnTable("orders")
                .To("type");
        }
    }
}
