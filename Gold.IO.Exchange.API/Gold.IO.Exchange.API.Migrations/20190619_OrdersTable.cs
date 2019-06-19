using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Migrations
{
    [Migration(20190619121801)]
    public class OrdersTableLimit : Migration
    {
        public override void Up()
        {

            Create.Column("limit")
                .OnTable("orders")
                .AsDouble();
        }

        public override void Down()
        {
            Delete.Column("limit")
                .FromTable("orders");
        }
    }
}
