using System;
using System.Collections.Generic;
using ORM;

namespace Test_ORM.Database.Models
{
    public class Note : Model
    {
        public int Id;
		public string Name;
		public double? Number;
    }
}
