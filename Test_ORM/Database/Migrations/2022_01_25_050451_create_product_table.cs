using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM.Migrations;

namespace Test_ORM.Database.Migrations
{
    public class CreateProductTable : Migration
    {
		public void Up()
		{
			Schema.Create("Product", (TableBlueprint table) =>
            {
				table.Integer("Id").Identity().Primary();
				table.Nvarchar("Name");
				table.Nvarchar("Description");
				table.Integer("Price").Nullable();
			});
		}

		public void Down()
		{
			Schema.DropIfExists("Product");
		}
	}
}
