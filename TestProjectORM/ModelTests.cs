using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORM;

namespace TestProjectORM
{
    [TestClass]
    public class ModelTests
    {
        [AssemblyInitialize()]
        public static void TestInitialize(TestContext testContext)
        {
            NktOrm.SetConnection("ms_sql", @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = Example; Integrated Security = True");
        }
        /// <summary>
        /// Проверяет правильность работы сохранения объекта в БД
        /// </summary>
        [TestMethod]
        public void TestSaveObj()
        {
            Product product = new();
            product.Price = 169;
            product.Title = "Конфета-вафля";
            product.Description = "KitKat с лимонным вкусом";
            product.Save<Product>();
            List<Product> products = Product.Query<Product>().WhereIn("Id", new string[] { product.Id.ToString() }).Get<Product>();
            Assert.AreEqual(products[0].Id, product.Id);
            Assert.AreEqual(products[0].Title, product.Title);
            Assert.AreEqual(products[0].Description, product.Description);
            Assert.AreEqual(products[0].Price, product.Price);
            Assert.AreEqual(products[0].OrderId, product.OrderId);
            Assert.AreEqual(products[0].UserId, product.UserId);

            product.Price = 200;
            Assert.AreNotEqual(products[0].Price, product.Price);
            product.Save<Product>();
            List<Product> products2 = Product.Query<Product>().WhereIn("Id", new string[] { product.Id.ToString() }).Get<Product>();
            Assert.AreEqual(products2[0].Id, product.Id);
            Assert.AreEqual(products2[0].Title, product.Title);
            Assert.AreEqual(products2[0].Description, product.Description);
            Assert.AreEqual(products2[0].OrderId, product.OrderId);
            Assert.AreEqual(products2[0].UserId, product.UserId);
        }
    }
}
