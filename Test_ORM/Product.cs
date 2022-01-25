using System;
using System.Collections.Generic;
using ORM;

namespace Test_ORM
{
    /// <summary>
    /// Тестовая модель таблицы Product
    /// </summary>
    class Product : Model
    {
        public int Id;
        public string Title;
        public string Description;
        public int Price;
        public int? OrderId;
        public int? UserId;
        public override string Connection { get; set; } = "ms_sql2";
    }
}
