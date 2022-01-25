using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using ORM;
using ORM.Migrations;

namespace Test_ORM
{
    class Program
    {
        public const bool serviceMode = true;
        static void Main(string[] args)
        {
            ConnectToDB();
            if (serviceMode)
            {
                ServiceApp();
            } else
            {
                MainApp();
            }
        }

        static void ServiceApp()
        {
            MigrationKernel.Start(@"..\..\..\..\Test_ORM\Database\Migrations",
                                  @"..\..\..\..\Test_ORM\Database\Models");
        }

        static void MainApp()
        {
            //TestSelect();
            //TestWhereInfo();
            //TestWhere();
            //TestGet();
            //TestSave();
            TestInsert();
            //TestDeleteBuilder();
            //TestUpdate();
            //TestQuery1();
            //TestQuery2();
            //TestJoin();
            //TestQuery3();
            Console.ReadKey();
        }
        static void OldTestModel()
        {
            /*List<Product> products = new List<Product>();
            products = Product.Select<Product>();
            foreach (var item in products)
            {
                Console.WriteLine(item);
            }
            products[0].Title = "jjfj";
            products[0].Description = "pp";
            products[0].Price = 69;
            products[0].Save<Product>();*/
        }

        static void ConnectToDB()
        {
            NktOrm.SetConnection("ms_sql", @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestDb;Integrated Security=True");
            NktOrm.SetConnection("ms_sql2", @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestDb;Integrated Security=True");
            
        }

        /*static void TestColumns()
        {
            Product product = new Product();
            List<string> columns = product.GetColumns<Product>();
            foreach (var column in columns)
                Console.WriteLine(column);
        }*/

        static void TestSelect()
        {
            Builder bild1 = Product.Query<Product>().Select("Id", "Description");
            Builder bild2 = Product.Query<Product>();
        }

       /* static void TestWhereInfo()
        {
            WhereInfo whereInfo = new WhereInfo();
            whereInfo.ValueDataType = "String";
            whereInfo.GetValue();
            whereInfo.ValueDataType = "Array";
            whereInfo.GetValue();
            whereInfo.ValueDataType = "Builder";
            whereInfo.GetValue();
        }*/

        static void TestWhere()
        {
            Builder builder1 = Product.Query<Product>().Where("Id", ">", "3").OrWhere("Id", "=", "2")
                                                       .WhereGroup(Product.Query<Product>().WhereIn("Price", new string[] { "55", "144", "146" })
                                                                                           .WhereInSub("Title", Product.Query<Product>().Select("Title")))
                                                       .WhereNull("Description");
            //string h1 = builder1.WhereRender();

            Builder builder2 = Product.Query<Product>().Select("Id", "Price").Where("Id", ">", "3").OrWhere("Id", "=", "2")
                                                       .WhereGroup(Product.Query<Product>().WhereIn("Price", new string[] { "55", "144", "146" })
                                                                                           .WhereInSub("Title", Product.Query<Product>().Select("Title")))
                                                       .WhereNull("Description");
            //string h2 = builder2.SelectRender();
        }

        static void TestGet()
        {
            List<Product> products = Product.Query<Product>()
                                            .Select("Title", "Price")
                                            .WhereIn("Id",new string[] { "1", "12", "17", "20"})
                                            .Get<Product>();

            List<Product> products2 = Product.Query<Product>().WhereIn("Id", new string[] { "17" }).Get<Product>();
        }

        static void TestSave()
        {
            /*Product product = new Product();
            product.Price = 78;
            product.Title = "Жимолость";
            product.Description = "Что-то очень странное";
            product.Save<Product>();*/

            List<Product> products = Product.Query<Product>().Get<Product>();
            products[5].Price = 6000;
            //products[2].Save<Product>();
        }

        static void TestInsert()
        {
            Product product = new();
            product.Price = 54;
            product.Title = "Шоколад";
            product.Description = "Черничный вкус";

            Product product2 = new();
            product2.Price = 654;
            product2.Title = "Стол";
            product2.Description = "Материал: сосна";

            Product product3 = new();
            product3.Price = 999;
            product3.Title = "Сим-карта";
            product3.Description = "MTS";

            List<Product> products = new() { product, product2, product3};

            Product.Query<Product>().Insert<Product>(products);
        }

        static void TestDeleteBuilder()
        {
            Product.Query<Product>().WhereIn("Price", new string[] { "100", "130" }).Delete();
        }

        static void TestUpdate()
        {
            Dictionary<string, string> values = new();
            values.Add("Price", "3000");
            Product.Query<Product>().WhereIn("Id", new string[] { "1", "4" }).Update<Product>(values);
        }

        static void TestQuery1()
        {
            //Product.Query<Product>().SelectRaw("Count(Id)").GroupBy("OrderBy")
            List<Product> pr = Product.Query<Product>().Get<Product>();
        }

        static void TestQuery2()
        {
            List<dynamic> result = Product.Query<Product>()
                    .SelectRaw("[OrderId]", "SUM([Price]) AS [PriceSum]")
                    .WhereIn("OrderId", new string[] { "2", "3", "4" })
                    .GroupBy("OrderId")
                    .Having("SUM([Price])", ">", "1000")
                    .OrderByDesc("SUM([Price])")
                    .Get();
            List<dynamic> pr = new();
            for (int i = 1; i < result.Count; i++)
            {
                var obj = new { OrderId = result[i][0], PriceSum = result[i][1] };
                pr.Add(obj);
            }


            List<Product> pr2 = Product.Query<Product>().Select("Id","Title","Price").WhereIn("Id",new string[] { "14","15","16","17"}).OrderBy("Price").Get<Product>();
        }

        static void TestJoin()
        {
            List<dynamic> result = Product.Query<Product>()
                .Select("Product].[Id", "Title", "Price")
                .Join("User", "Product.UserId", "=", "User.Id")
                .Where("User.Id", "=", "1")
                .Where("Price", "<", "1000")
                .OrderBy("[Price]")
                .Get();

            List<dynamic> pr = new();
            for (int i = 1; i < result.Count; i++)
            {
                var obj = new { Id = result[i][0], Title = result[i][1], Price = result[i][2] };
                pr.Add(obj);
            }


            var pr0 = Product.Query<Product>()
                .Select("Product].[Id", "Title", "Price")
                .Join("User", "Product.UserId", "=", "User.Id")
                .LeftJoinSubExtended(builder =>
                {
                    builder.Select("Id")
                    .From("Order")
                    .JoinExtended("User", join =>
                    {
                        join.On("User.Id", "*", "Order.UserId");
                    })

                    .WhereIn("Id", new string[] { "1", "2", "3" });
                }, "OrderUser", join =>
                {
                    join.On("Product.OrderId", "=", "OrderUser.Id")
                    .OrOn("Product.OrderId", "!=", "OrderUser.Id")
                    .WhereNotNull("OrderUser.Id");
                })
                .Where("User.Id", "=", "1")
                .OrderBy("Price")
                .Get();

        }
        static void TestQuery3()
        {
            Product product = new();
            product.Price = 169;
            product.Title = "Конфета-вафля";
            product.Description = "KitKat с лимонным вкусом";
            product.Save<Product>();
            List<Product> products = Product.Query<Product>().WhereIn("Id", new string[] { product.Id.ToString() }).Get<Product>();
        }
    }
}