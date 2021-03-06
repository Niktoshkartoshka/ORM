using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM.Migrations;

namespace Test_ORM.Database.Migrations
{
    public class CreateMigrationTable : Migration
    {
		public void Up()
		{
			Schema.Create("Migration", (TableBlueprint table) =>
            {
                table.Integer("Id").Identity().Primary();
                table.Varchar("NameMigration", 1024);
				table.Integer("Batch");
            });
		}

		public void Down()
		{
			Schema.DropIfExists("Migration");
		}
	}
}
