using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORM;

namespace TestProjectORM
{
    [TestClass]
    public class QueryTests
    {
        /// <summary>
        /// Проверяет правильность работы метода получения информации из БД
        /// </summary>
        [TestMethod]
        public void TestQueryGet()
        {
            List<Product> products = Product.Query<Product>().Select("Id", "Title", "Price").Where("Price", ">", "3000").OrderBy("[Price]").Get<Product>();

            Assert.AreEqual(products[0].Id, 5);
            Assert.AreEqual(products[0].Title, "Чайник");
            Assert.AreEqual(products[0].Price, 4999);

            Assert.AreEqual(products[1].Id, 14);
            Assert.AreEqual(products[1].Title, "Принтер");
            Assert.AreEqual(products[1].Price, 7900);

            Assert.AreEqual(products[2].Id, 2);
            Assert.AreEqual(products[2].Title, "Ноутбук");
            Assert.AreEqual(products[2].Price, 31099);
        }

        /// <summary>
        /// Проверяет правильность работы метода получения информации из БД при соединении таблиц
        /// </summary>
        [TestMethod]
        public void TestJoin()
        {
            List<dynamic> pr = Product.Query<Product>()
                .Select("Product].[Id", "Title", "Price")
                .Join("User", "Product.UserId", "=", "User.Id")
                .Where("User.Id", "=", "1")
                .Where("Price", "<", "1000")
                .OrderBy("[Price]")
                .Get();

            var obj1 = new { Id = pr[1][0], Title = pr[1][1], Price = pr[1][2] };
            var obj2 = new { Id = pr[2][0], Title = pr[2][1], Price = pr[2][2] };

            Assert.AreEqual(obj1.Id, 15);
            Assert.AreEqual(obj1.Title, "Тетрадь");
            Assert.AreEqual(obj1.Price, 30);

            Assert.AreEqual(obj2.Id, 22);
            Assert.AreEqual(obj2.Title, "Карандаши");
            Assert.AreEqual(obj2.Price, 199);
        }

        /// <summary>
        /// Проверяет правильность работы метода получения информации из БД при группировке
        /// </summary>
        [TestMethod]
        public void TestGroupBy()
        {
                List<dynamic> result = Product.Query<Product>()
                    .SelectRaw("[OrderId]", "SUM([Price]) AS [PriceSum]")
                    .WhereIn("OrderId", new string[] { "2", "3", "4"})
                    .GroupBy("OrderId")
                    .Having("SUM([Price])", ">", "1000")
                    .OrderByDesc("SUM([Price])")
                    .Get();

            var obj1 = new { OrderId = result[1][0], PriceSum = result[1][1] };
            var obj2 = new { OrderId = result[2][0], PriceSum = result[2][1] };
            var obj3 = new { OrderId = result[3][0], PriceSum = result[3][1] };

            Assert.AreEqual(obj1.OrderId, 4);
            Assert.AreEqual(obj1.PriceSum, 9628);

            Assert.AreEqual(obj2.OrderId, 3);
            Assert.AreEqual(obj2.PriceSum, 4224);

            Assert.AreEqual(obj3.OrderId, 2);
            Assert.AreEqual(obj3.PriceSum, 1887);
        }

        /// <summary>
        /// Проверяет правильность работы метода обновления информации в БД
        /// </summary>
        [TestMethod]
        public void TestUpdateObj()
        {
            List<Product> products = Product.Query<Product>().WhereIn("Id", new string[] { "1", "4" }).Get<Product>();
            Assert.AreNotEqual(products[0].Price, 3000);
            Assert.AreNotEqual(products[1].Price, 3000);

            Dictionary<string, string> values = new();
            values.Add("Price", "3000");
            Product.Query<Product>().WhereIn("Id", new string[] { "1", "4" })
                                    .Update<Product>(values);
            products = Product.Query<Product>().WhereIn("Id", new string[] { "1", "4" }).Get<Product>();

            Assert.AreEqual(products[0].Price, 3000);
            Assert.AreEqual(products[1].Price, 3000);
        }
    }
}
