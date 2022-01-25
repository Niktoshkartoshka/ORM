using System;
using System.Collections.Generic;
using System.Text;
using ORM;

namespace TestProjectORM
{
    class Product : Model
    {
        public int Id;
        public string Title;
        public string Description;
        public int Price;
        public int? OrderId;
        public int? UserId;
        public override string Connection { get; set; } = "ms_sql";
    }
}
