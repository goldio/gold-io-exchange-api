using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Migrations
{
    [Migration(20190617121800)]
    public class ApiKeysTable : Migration
    {
        public override void Up()
        {
            Delete.Column("secret_key")
                .Column("account_permissions")
                .Column("orders_permissions")
                .Column("funds_permissions")
                .FromTable("api_keys");

            Alter.Table("api_keys")
                .AddColumn("role")
                .AsInt16()
                .NotNullable();

            Alter.Table("api_keys")
                .AddColumn("expired")
                .AsDateTime()
                .NotNullable();
        }

        public override void Down()
        {
            Delete.Table("Log");
        }
    }
}
