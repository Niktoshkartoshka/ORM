using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;

namespace ORM
{
    /// <summary>
    /// Одна таблица
    /// </summary>
    public abstract class Model : ICloneable
    {
        /// <summary>
        /// Название таблицы
        /// </summary>
        internal string TableName = null;

        /// <summary>
        /// Флаг наличия записи в БД
        /// </summary>
        internal bool Exists = false;

        /// <summary>
        /// Объект подключения к БД
        /// </summary>
        internal SqlConnection SqlConnection;

        /// <summary>
        /// Названия базовых полей
        /// </summary>
        internal List<string> BaseFields = new()
        {
                                                                "TableName",
                                                                "Exists",
                                                                "Columns",
                                                                "BaseFields"
                                                             };

        /// <summary>
        /// Хранит копию изначального объекта
        /// </summary>
        internal Model Original { get; set; }

        /// <summary>
        /// Первичный ключ
        /// </summary>
        public virtual string Primary { get; set; } = "Id";

        /// <summary>
        /// Название строки подключения
        /// </summary>
        public virtual string Connection { get; set; } = null;

        /// <summary>
        /// Конструктор модели
        /// </summary>
        public Model() // string connectionName = null
        {
            TableName = GetType().Name;
            if (Connection == null)
            {
                Connection = NktOrm.GetDefaultConnectionName();
            }
            SqlConnection = NktOrm.GetConnection(Connection);
        }

        /// <summary>
        /// Используется для создания копии объекта, на котором метод вызывается
        /// </summary>
        /// <returns>Копия объекта</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Позволяет получить список названий столбцов таблицы
        /// </summary>
        internal List<string> GetColumns<T>() where T : Model
        {
            List<string> columns = new();
            MemberInfo[] members = typeof(T).GetFields();
            foreach (var member in members)
            {
                if (!BaseFields.Contains(member.Name))
                    columns.Add(member.Name);
            }
            return columns;
        }

        /// <summary>
        /// Позволяет писать запрос к БД
        /// </summary>
        /// <typeparam name="T">Название текущего класса</typeparam>
        /// <returns>Построитель запросов</returns>
        public static Builder Query<T>() where T : Model, new() //string connectionName = null
        {
            //T model = (T)Activator.CreateInstance(typeof(T), connectionName);
            T model = new();
            return new Builder(model);
        }

        /// <summary>
        /// Позволяет получить список измененных столбцов
        /// </summary>
        /// <typeparam name="T">Название таблицы</typeparam>
        /// <returns>Список измененных столбцов</returns>
        private List<string> GetChangedColumns<T>() where T : Model
        {
            List<string> columns = GetColumns<T>();
            List<string> changedColumns = new();
            foreach (var col in columns)
            {
                FieldInfo fi = typeof(T).GetField(col);
                var value = fi.GetValue(this);
                var originalValue = fi.GetValue(Original);
                if ((originalValue == null) && (value == null))
                    continue;
                if (!value.Equals(originalValue))
                {
                    changedColumns.Add(col);
                }
            }
            return changedColumns;
        }

        /// <summary>
        /// Сохраняет текущий объект в БД
        /// </summary>
        /// <typeparam name="T">Класс текущей таблицы</typeparam>
        public void Save<T>() where T : Model
        {
            if (Exists == false)
            {
                InsertModel<T>();
            }
            else
            {
                UpdateModel<T>();
            }
        }

        /// <summary>
        /// Метод для осуществления вставки текущего объекта в БД
        /// </summary>
        /// <typeparam name="T">Класс текущей таблицы</typeparam>
        private void InsertModel<T>() where T : Model
        {
            List<string> columns = GetColumns<T>();
            string columnString = "";
            string valueString = "";
            int k = 2;
            foreach (var column in columns)
            {
                if (column == Primary)
                    continue;
                FieldInfo fi = typeof(T).GetField(column);
                if (fi.GetValue(this) == null)
                    continue;
                if (column == columns[1])
                {
                    columnString += column;
                    valueString += $"N'{fi.GetValue(this)}'";
                }
                else if (columns.Count > k)
                {
                    columnString += ", " + column;
                    valueString += $", N'{fi.GetValue(this)}'";
                }
                k++;
            }
            string sqlString = $"INSERT INTO {TableName} ({columnString}) VALUES ({valueString}); SELECT SCOPE_IDENTITY();";
            SqlConnection.Open();
            SqlCommand command = new(sqlString, SqlConnection);
            int n = Convert.ToInt32(command.ExecuteScalar());
            typeof(T).GetField(Primary).SetValue(this, n);
            Original = (T)Clone();
            Exists = true;
            SqlConnection.Close();
        }

        /// <summary>
        /// Метод для осуществления обновления информации о текущем объекте в БД
        /// </summary>
        /// <typeparam name="T">Класс текущей таблицы</typeparam>
        private void UpdateModel<T>() where T : Model
        {
            List<string> changedColumns = GetChangedColumns<T>();
            if (changedColumns.Count == 0)
                return; // or throw new Exeption() ?
            string sqlString = $"UPDATE {GetType().Name} SET ";
            int k = 1;
            foreach (string changedColumn in changedColumns)
            {
                FieldInfo fi = typeof(T).GetField(changedColumn);
                if (fi.GetValue(this) == null)
                {
                    continue;
                }
                sqlString += $"{changedColumn}=N'{fi.GetValue(this)}'";
                if (changedColumns.Count > k)
                {
                    sqlString += ", ";
                }
                k++;
            }
            FieldInfo fiOriginal = typeof(T).GetField(Primary);
            sqlString += $" WHERE {Primary}={fiOriginal.GetValue(Original)}";
            SqlConnection.Open();
            SqlCommand command = new(sqlString, SqlConnection);
            int n = command.ExecuteNonQuery();
            Original = (T)Clone();
            SqlConnection.Close();
        }

        /// <summary>
        /// Удалить объект из БД
        /// </summary>
        /// <typeparam name="T">Класс текущей таблицы</typeparam>
        public void Delete<T>() where T : Model
        {
            if (Exists == false)
                return;
            FieldInfo fi = typeof(T).GetField(Primary);
            string sqlString = $"DELETE FROM [{TableName}] WHERE [{Primary}]='{fi.GetValue(this)}'";
            SqlConnection.Open();
            SqlCommand command = new(sqlString, SqlConnection);
            int n = command.ExecuteNonQuery();
            Exists = false;
            Original = (T)Clone();
            SqlConnection.Close();
        }
    }
}
