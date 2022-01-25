using System;
using System.Collections.Generic;
using ORM;

namespace Test_ORM.Database.Models
{
    public class Product : Model
    {
        public int Id;
		public string Name;
		public float Description;
		public int? Price;
		public DateTime? CreatedAt;
    }
}
