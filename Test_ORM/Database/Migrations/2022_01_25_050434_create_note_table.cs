using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM.Migrations;

namespace Test_ORM.Database.Migrations
{
    public class CreateNoteTable : Migration
    {
		public void Up()
		{
			Schema.Create("Note", (TableBlueprint table) =>
            {
				table.Integer("Id").Identity().Primary();
				table.Varchar("Name");
				table.Real("Number").Nullable();
			});
		}

		public void Down()
		{
			Schema.DropIfExists("Note");
		}
	}
}
