using System;
using System.Collections.Generic;
using System.Text;

namespace Test_ORM
{
    /// <summary>
    /// Cкопление старого кода на всякий случай
    /// </summary>
    class Old
    {
        void ProgramCode()
        {
            /*public class Xamps
        {
            public Xamps original { get; set; }
            public int Id;
            public string Product;
            public string Creator;
            public int Rating;
            public bool exists = false;
            public string[] columns = new string[] { "Id", "Product", "Creator", "Rating" };
            public List<string> getChangedColumns()
            {
                List<string> changedColumns = new List<string>();
                foreach (var col in columns)
                {
                    FieldInfo fi = typeof(Xamps).GetField(col);
                    var value = fi.GetValue(this);
                    var originalValue = fi.GetValue(this.original);
                    if (!value.Equals(originalValue))
                    {
                        changedColumns.Add(col);
                    }
                }

                return changedColumns;
            }
        }

        public static List<Xamps> models = new List<Xamps>();*/

            /*        /// <summary>
                    /// что-то с объектом
                    /// </summary>
                    public static void Select()
                    {
                        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Example;Integrated Security=True";
                        using SqlConnection sqlConnection = new SqlConnection(connectionString);
                        string sql = "SELECT * FROM Xamp";
                        using (SqlCommand command = new SqlCommand(sql, sqlConnection))
                        {
                            sqlConnection.Open();

                            using SqlDataReader reader = command.ExecuteReader();

                            for (int i = 0; i < reader.FieldCount; i++)
                                Console.Write(reader.GetName(i).ToString() + "   ");

                            Console.WriteLine();

                            while (reader.Read())
                            {
                                Xamps model = new Xamps();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string name = reader.GetName(i).ToString();
                                    FieldInfo fi = typeof(Xamps).GetField(name);
                                    object value = reader.GetValue(i);
                                    fi.SetValue(model, value);
                                }
                                model.original = model;
                                model.exists = true;
                                models.Add(model);
                                Console.WriteLine();

                            }
                        }
                        //Console.ReadKey();
                    }

                    /// <summary>
                    /// без объекта
                    /// </summary>
                    public static void Insert()
                    {
                        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Example;Integrated Security=True";
                        using SqlConnection sqlConnection = new SqlConnection(connectionString);
                        string sql = "INSERT INTO Xamp (Product, Creator, Rating) VALUES ('Pivo', 'Duma', 18)";
                        sqlConnection.Open();
                        SqlCommand command = new SqlCommand(sql, sqlConnection);
                        int n = command.ExecuteNonQuery();
                        Console.ReadKey();
                    }

                    /// <summary>
                    /// без объекта
                    /// </summary>
                    public static void Delete()
                    {
                        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Example;Integrated Security=True";
                        using SqlConnection sqlConnection = new SqlConnection(connectionString);
                        string sql = "DELETE  FROM Xamp WHERE Product='Pivo'";
                        sqlConnection.Open();
                        SqlCommand command = new SqlCommand(sql, sqlConnection);
                        int n = command.ExecuteNonQuery();
                        Console.ReadKey();
                    }

                    /// <summary>
                    /// просто к бд без объекта
                    /// </summary>
                    public static void Update()
                    {
                        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Example;Integrated Security=True";
                        using SqlConnection sqlConnection = new SqlConnection(connectionString);
                        string sql = "UPDATE Xamp SET Rating=30 WHERE Product='Pivo'";
                        sqlConnection.Open();
                        SqlCommand command = new SqlCommand(sql, sqlConnection);
                        int n = command.ExecuteNonQuery();
                        //Console.ReadKey();
                    }

                    /// <summary>
                    /// сохранить объект в виде записи в бд
                    /// </summary>
                    public static void Save()
                    {
                        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Example;Integrated Security=True";
                        using SqlConnection sqlConnection = new SqlConnection(connectionString);
                        Xamps exp1 = models[0];
                        Xamps exp2 = new Xamps();
                        //exp2.Id = 5;
                        exp2.Product = "pivasik";
                        exp2.Rating = 111;
                        exp2.Creator = "somebody ones told me";
                        Xamps[] mas = new Xamps[2] { exp1, exp2 };
                        for (int i = 0; i < 2; i++)
                        {
                            if (mas[i].exists == false)
                            {
                                string sql = string.Format("INSERT INTO Xamp (Product, Creator, Rating) VALUES ('{0}', '{1}', {2})", mas[i].Product, mas[i].Creator, mas[i].Rating);
                                sqlConnection.Open();
                                SqlCommand command = new SqlCommand(sql, sqlConnection);
                                int n = command.ExecuteNonQuery();
                            }
                            else 
                            {
                                string sql = "UPDATE Xamp SET ";

                                foreach (string changedColumn in mas[i].getChangedColumns())
                                {
                                    int k = 0;
                                    FieldInfo fi = typeof(Xamps).GetField(changedColumn);
                                    if (mas[i].getChangedColumns().Count > k)
                                    {
                                        sql += string.Format("{0}='{1}', ", changedColumn, fi.GetValue(mas[i]));
                                    }
                                    else
                                    {
                                        sql += string.Format("{0}='{1}' WHERE Id={2}", changedColumn, fi.GetValue(mas[i]), mas[i].Id);
                                    }
                                    k++;
                                }
                                sqlConnection.Open();
                                SqlCommand command = new SqlCommand(sql, sqlConnection);
                                int n = command.ExecuteNonQuery();
                            }
                        }
                        *//* должна делаться проверка найдена ли сохраняемая запись в бд
                         * если не найдена, то выполняется Insert()
                         *      если найдена, то Update()
                         *            причем! раз найдена, то (в идеале) надо определить какие стобцы изменились
                         *                    чтобы не обращаться к бд слишком много  *//*

                    }*/
            // private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Example;Integrated Security=True";
        }

        void ProductCode()
        {
            /*
        /protected override List<string> Columns { get; set; } = new List<string> {
                                                                                    "Id",
                                                                                    "Title",
                                                                                    "Description",
                                                                                    "Price"
                                                                                };*/
        }

        void ModelCode()
        {
            /*public static List<T> Select<T>() where T : Model, new()
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Example;Integrated Security=True";
            using SqlConnection sqlConnection = new SqlConnection(connectionString);
            string sql = "SELECT * FROM Product";
            using (SqlCommand command = new SqlCommand(sql, sqlConnection))
            {
                sqlConnection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                List<T> models = new List<T>();
                while (reader.Read())
                {
                    T model = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string name = reader.GetName(i).ToString();
                        FieldInfo fi = typeof(T).GetField(name);
                        object value = reader.GetValue(i);
                        fi.SetValue(model, value);
                    }
                    model.Original = (T)model.Clone();
                    model.exists = true;
                    models.Add(model);
                }
                return models;
            }
        }

        public void Delete()
        {

        }
        public void Save<T>() where T : Model
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Example;Integrated Security=True";
            using SqlConnection sqlConnection = new SqlConnection(connectionString);
            if (this.exists == false)
            {
                string columnString = ""; string valueString = "";
                foreach (var column in columns)
                {
                    int k = 1;
                    FieldInfo fi = typeof(T).GetField(column);
                    if (columns.Count > k)
                    {
                        columnString += column + ", ";
                        valueString += fi.GetValue(this) + ", ";
                    }
                    else
                    {
                        columnString += column;
                        valueString += fi.GetValue(this);
                    }
                }
                string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", this.GetType().Name, columnString, valueString);
                sqlConnection.Open();
                SqlCommand command = new SqlCommand(sql, sqlConnection);
                int n = command.ExecuteNonQuery();
            }
            else
            {
                string table = this.GetType().Name;
                string sql = string.Format("UPDATE {0} SET ", this.GetType().Name);

                List<string> changedColumns = this.getChangedColumns<T>();
                int k = 1;
                foreach (string changedColumn in changedColumns)
                {
                    FieldInfo fi = typeof(T).GetField(changedColumn);
                    sql += string.Format("{0}=N'{1}'", changedColumn, fi.GetValue(this));
                    if (changedColumns.Count > k)
                    {
                        sql += ", ";
                    }                    
                    k++;
                }
                FieldInfo fiOriginal = typeof(T).GetField(Primary);
                sql += string.Format(" WHERE {0}={1}",  Primary, fiOriginal.GetValue(this.Original));
                sqlConnection.Open();
                SqlCommand command = new SqlCommand(sql, sqlConnection);
                int n = command.ExecuteNonQuery();
                this.Original = this;
            }*/
        }
    }
}
