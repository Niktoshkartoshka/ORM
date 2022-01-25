using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Migrations
{
    /// <summary>
    /// Схема таблицы
    /// </summary>
    public class Schema
    {
        /// <summary>
        /// Название объекта подключения
        /// </summary>
        public string Connection;

        /// <summary>
        /// Конструктор схемы таблицы
        /// </summary>
        /// <param name="connection">Название объекта подключения</param>
        public Schema(string connection)
        {
            Connection = connection;
        }

        /// <summary>
        /// Callback, устанавливающий таблицу
        /// </summary>
        /// <param name="table"></param>
        public delegate void TableCallback(TableBlueprint table);

        /// <summary>
        /// Метод для создания таблицы в БД и класса модели
        /// </summary>
        /// <param name="name">Название таблицы</param>
        /// <param name="blueprint">Callback, устанавливающий таблицу</param>
        public void Create(string name, TableCallback blueprint)
        {
            TableBlueprint table = new(name);
            blueprint(table);

            SqlConnection sqlConnection = NktOrm.GetConnection(Connection);
            string sqlString = table.BuildSqlStringToCreate();
            sqlConnection.Open();
            SqlCommand command = new(sqlString, sqlConnection);
            try
            {
                Convert.ToInt32(command.ExecuteScalar());
                sqlConnection.Close();
            }
            catch (SqlException ex)
            {
                sqlConnection.Close();
                Console.WriteLine(ex.Message);
            }

            if (name != "Migration")
            {
                table.CreateModel();
            }
        }

        /// <summary>
        /// Метод для осуществления изменений в уже существующих таблицах и моделях.
        /// </summary>
        /// <param name="name">Название таблицы</param>
        /// <param name="blueprint">Callback, устанавливающий изменения в таблице</param>
        public void Alter(string name, TableCallback blueprint)
        {
            TableBlueprint table = new(name);
            blueprint(table);

            SqlConnection sqlConnection = NktOrm.GetConnection(Connection);
            string sqlString = table.BuildSqlStringToAlter();
            sqlConnection.Open();
            SqlCommand command = new(sqlString, sqlConnection);
            try
            {
                Convert.ToInt32(command.ExecuteScalar());
                sqlConnection.Close();
            }
            catch (SqlException ex)
            {
                sqlConnection.Close();
                Console.WriteLine(ex.Message);
                return;
            }

            table.ChangeModel();
        }

        /// <summary>
        /// Метод для удаления таблицы из БД и класса модели
        /// </summary>
        /// <param name="name">Название таблицы</param>
        public void DropIfExists(string name)
        {
            SqlConnection sqlConnection = NktOrm.GetConnection(Connection);
            string sqlString = $"DROP TABLE IF EXISTS [{name}]";
            sqlConnection.Open();
            SqlCommand command = new(sqlString, sqlConnection);
            Convert.ToInt32(command.ExecuteScalar());
            sqlConnection.Close();

            string filename = $"{name}.cs";
            string path = $"{MigrationKernel.ModelsDir}\\{filename}";
            File.Delete(path);
        }
    }
}
