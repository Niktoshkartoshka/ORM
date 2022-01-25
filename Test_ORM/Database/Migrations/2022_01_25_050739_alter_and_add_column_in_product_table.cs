using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM.Migrations;

namespace Test_ORM.Database.Migrations
{
    public class AlterAndAddColumnInProductTable : Migration
    {
		public void Up()
		{
			Schema.Alter("Product", (TableBlueprint table) =>
            {
				table.AddCheck("CHKPrice", "[Price] > 0");
				table.Float("Description").Changed();
				table.Datetime("CreatedAt").Nullable();
            });
		}

		public void Down()
		{
			Schema.Alter("Product", (TableBlueprint table) =>
            {
				table.DropCheck("CHKPrice");
				table.Nvarchar("Description").Changed();
				table.DropColumn("CreatedAt");
            });
		}
	}
}
