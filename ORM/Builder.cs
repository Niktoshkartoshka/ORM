using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;

namespace ORM
{
    /// <summary>
    /// Построитель запросов
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Текущая таблица
        /// </summary>
        public Model Model;

        /// <summary>
        /// Значение условия HAVING
        /// </summary>
        private string _having = "";

        /// <summary>
        /// Список столбцов
        /// </summary>
        private List<string> _selects = new() { "*" };

        /// <summary>
        /// Название таблицы
        /// </summary>
        private string _fromClause;

        /// <summary>
        /// Список условий для выборки, часть запроса
        /// </summary>
        private List<WhereClause> _wheres = new();

        /// <summary>
        /// Список столбцов для сортировки, часть запроса
        /// </summary>
        private List<string> _orders = new();

        /// <summary>
        /// Список условий для объединения таблиц, часть запроса
        /// </summary>
        private List<JoinClause> _joins = new();

        /// <summary>
        /// Список условий для гурппировки таблицы, часть запроса
        /// </summary>
        private List<string> _groups = new();

        /// <summary>
        /// Конструктор построителя запросов
        /// </summary>
        /// <param name="model">Класс таблицы</param>
        public Builder(Model model)
        {
            Model = model;
            _fromClause = Model.TableName;
        }

        /// <summary>
        /// Добавляет название таблицы для текущей части запроса
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Построитель запроса</returns>
        public Builder From(string tableName)
        {
            _fromClause = tableName;
            return this;
        }

        /// <summary>
        /// Добавляет в построитель запроса столбцы, необходимые для выборки из БД. По умолчанию указано значение "*".
        /// </summary>
        /// <param name="columns">Столбцы, необходимые для выборки из БД</param>
        /// <returns>Построитель запроса</returns>
        public Builder Select(params string[] columns)
        {
            if (columns.Length == 0)
            {
                throw new Exception("Метод Builder.Select() принимает минимум один параметр. Передано 0");
            }
            _selects = new List<string>();
            foreach (var column in columns)
            {
                _selects.Add("[" + column + "]");
            }
            return this;
        }

        /// <summary>
        /// Добавляет в построитель запроса столбцы, необходимые для выборки из БД. По умолчанию указано значение "*". 
        /// При использовании агрегатных функций воспользуйтесь этим методом.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns>Построитель запроса</returns>
        public Builder SelectRaw(params string[] columns)
        {
            if (columns.Length == 0)
            {
                throw new Exception("Метод Builder.Select() принимает минимум один параметр. Передано 0");
            }
            _selects = new List<string>();
            foreach (var column in columns)
            {
                _selects.Add(column);
            }
            return this;
        }

        /// <summary>
        /// Добавляет условие для выборки с условием AND
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="comparisonOperator">Оператор сравнения</param>
        /// <param name="value">Значение для условия</param>
        /// <param name="conditionalOperator">Оператор условия</param>
        /// <returns>Построитель запроса</returns>
        public Builder Where(string column, string comparisonOperator, string value, string conditionalOperator = "AND")
        {
            _wheres.Add(new WhereClause(column, comparisonOperator, value, conditionalOperator, ""));
            return this;
        }

        /// <summary>
        /// Добавляет условие для выборки c условием OR
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="comparisonOperator">Оператор сравнения</param>
        /// <param name="value">Значение для условия</param>
        /// <returns>Построитель запроса</returns>
        public Builder OrWhere(string column, string comparisonOperator, string value)
        {
            return Where(column, comparisonOperator, value, "OR");
        }

        /// <summary>
        /// Добавляет сгруппированные условия для выборки
        /// </summary>
        /// <param name="values">Значение для условия</param>
        /// <param name="conditionalOperator">Оператор условия</param>
        /// <returns>Построитель запроса</returns>
        public Builder WhereGroup(Builder values, string conditionalOperator = "AND")
        {
            _wheres.Add(new WhereClause(values, conditionalOperator));
            return this;
        }

        /// <summary>
        /// Добавляет сгруппированные условия для выборки с условием OR
        /// </summary>
        /// <param name="values">Значение для условия</param>
        /// <returns>Построитель запроса</returns>
        public Builder OrWhereGroup(Builder values)
        {
            return WhereGroup(values, "OR");
        }

        /// <summary>
        /// Добавляет условие для выборки с IN
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="values">Значение для условия</param>
        /// <param name="conditionalOperator">Оператор условия</param>
        /// <param name="not">Оператор отрицания</param>
        /// <returns>Построитель запроса</returns>
        public Builder WhereIn(string column, string[] values, string conditionalOperator = "AND", string not = "")
        {
            _wheres.Add(new WhereClause(column, "IN", values, conditionalOperator, not));
            return this;
        }

        /// <summary>
        /// Добавляет условие для выборки с IN с условием OR
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="values">Значение для условия</param>
        /// <returns>Построитель запроса</returns>
        public Builder OrWhereIn(string column, string[] values)
        {
            return WhereIn(column, values, "OR");
        }

        /// <summary>
        /// Добавляет условие для выборки
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="values">Значение для условия</param>
        /// <returns>Построитель запроса</returns>
        public Builder WhereNotIn(string column, string[] values)
        {
            return WhereIn(column, values, "AND", "NOT");
        }

        /// <summary>
        /// Добавляет условие для выборки
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="values">Значение для условия</param>
        /// <returns>Построитель запроса</returns>
        public Builder OrWhereNotIn(string column, string[] values)
        {
            return WhereIn(column, values, "OR", "NOT");
        }

        /// <summary>
        /// Добавляет условие с подзапросом для выборки
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="values">Подзапрос</param>
        /// <param name="conditionalOperator">Оператор условия</param>
        /// <param name="not">Оператор отрицания</param>
        /// <returns>Построитель запроса</returns>
        public Builder WhereInSub(string column, Builder values, string conditionalOperator = "AND", string not = "")
        {
            _wheres.Add(new WhereClause(column, "IN", values, conditionalOperator, not));
            return this;
        }

        /// <summary>
        /// Добавляет условие с подзапросом для выборки с условием OR
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="values">Подзапрос</param>
        /// <returns>Построитель запроса</returns>
        public Builder OrWhereInSub(string column, Builder values)
        {
            return WhereInSub(column, values, "OR");
        }

        /// <summary>
        /// Добавляет условие условие для выборки
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="conditionalOperator">Оператор условия</param>
        /// <param name="not">Оператор отрицания</param>
        /// <returns>Построитель запроса</returns>
        public Builder WhereNull(string column, string conditionalOperator = "AND", string not = "")
        {
            _wheres.Add(new WhereClause(column, "IS", "NULL", conditionalOperator, not));
            return this;
        }

        /// <summary>
        /// Добавляет условие условие для выборки
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <returns>Построитель запроса</returns>
        public Builder OrWhereNull(string column)
        {
            return WhereNull(column, "OR");
        }

        /// <summary>
        /// Добавляет условие условие для выборки
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <returns>Построитель запроса</returns>
        public Builder WhereNotNull(string column)
        {
            return WhereNull(column, "AND", "NOT");
        }

        /// <summary>
        /// Добавляет условие условие для выборки
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <returns>Построитель запроса</returns>
        public Builder OrWhereNotNull(string column)
        {
            return WhereNull(column, "OR", "NOT");
        }

        /// <summary>
        /// Добавление нескольких объектов в базу данных. Обновление данных передаваемого в метод списка не происходит.
        /// </summary>
        /// <typeparam name="T">Класс текущей таблицы</typeparam>
        /// <param name="models">Список объектов, которые необходимо записать в БД</param>
        public void Insert<T>(List<T> models) where T : Model
        {
            List<string> columns = Model.GetColumns<T>();
            columns.RemoveAt(0);
            List<string> values = new();
            foreach (var model in models)
            {
                if (model.Exists == false)
                {
                    string valueString = "";
                    int k = 1;
                    foreach (var column in columns)
                    {
                        FieldInfo fi = typeof(T).GetField(column);
                        if (column == columns[0])
                        {
                            if (fi.GetValue(model) == null)
                            {
                                valueString += "(NULL";
                            }
                            else
                                valueString += $"(N'{fi.GetValue(model)}'";
                        }
                        else if (columns.Count > k)
                        {
                            if (fi.GetValue(model) == null)
                            {
                                valueString += ", NULL";
                            }
                            else
                                valueString += $", N'{fi.GetValue(model)}'";
                        }
                        else
                        {
                            if (fi.GetValue(model) == null)
                            {
                                values.Add(valueString += string.Format(", NULL)", fi.GetValue(model)));
                            }
                            else
                                values.Add(valueString += string.Format(", N'{0}')", fi.GetValue(model)));
                        }
                            
                        k++;
                    }
                }
                else
                {
                    continue;
                }
            }
            string sqlString = string.Format("INSERT INTO {0} ({1}) VALUES {2};", Model.TableName, string.Join(", ", columns.ToArray()), string.Join(", ", values.ToArray()));
            Model.SqlConnection.Open();
            SqlCommand command = new(sqlString, Model.SqlConnection);
            int n = command.ExecuteNonQuery();
            Model.SqlConnection.Close();
        }

        /// <summary>
        /// Обновление данных в БД для указанных условий
        /// </summary>
        /// <typeparam name="T">Класс текущей таблицы</typeparam>
        /// <param name="changedValues">Новые значения. Первое - название столбца, второе - новое значение</param>
        public void Update<T>(Dictionary<string, string> changedValues) where T : Model
        {
            string sqlString = "UPDATE " + Model.TableName + " SET " + string.Join(", ", SetRender(changedValues).ToArray()) + WhereRender();
            SqlConnection sqlConnection = Model.SqlConnection;
            sqlConnection.Open();
            SqlCommand command = new(sqlString, sqlConnection);
            int n = command.ExecuteNonQuery();
            sqlConnection.Close();
        }

        /// <summary>
        /// Удаляет данные в БД в соответствии с построенным запросом
        /// </summary>
        public void Delete()
        {
            string sqlString = DeleteSqlStringRender();
            SqlConnection sqlConnection = Model.SqlConnection;
            sqlConnection.Open();
            SqlCommand command = new(sqlString, sqlConnection);
            int n = command.ExecuteNonQuery();
            sqlConnection.Close();
        }

        /// <summary>
        /// Добавляет присоединение таблицы
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="first">Первый столбец в условии присоединения</param>
        /// <param name="comparisonOperator">Оператор в условии присоединения</param>
        /// <param name="second">Второй столбец в условии присоединения</param>
        /// <param name="type">Тип присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder Join(string tableName, string first, string comparisonOperator, string second, string type = "INNER")
        {
            return JoinExtended(tableName, joinClause => joinClause.On(first, comparisonOperator, second), type);
        }
        /// <summary>
        /// Добавляет левое присоединение таблицы
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="first">Первый столбец в условии присоединения</param>
        /// <param name="comparisonOperator">Оператор в условии присоединения</param>
        /// <param name="second">Второй столбец в условии присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder LeftJoin(string tableName, string first, string comparisonOperator, string second)
        {
            return Join(tableName, first, comparisonOperator, second, "LEFT");
        }
        /// <summary>
        /// Добавляет перекрестное присоединение таблицы
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="first">Первый столбец в условии присоединения</param>
        /// <param name="comparisonOperator">Оператор в условии присоединения</param>
        /// <param name="second">Второй столбец в условии присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder CrossJoin(string tableName, string first, string comparisonOperator, string second)
        {
            return Join(tableName, first, comparisonOperator, second, "CROSS");
        }

        /// <summary>
        /// Callback для формирования подзапроса присоединяемой таблицы
        /// </summary>
        /// <param name="builder">Построитель запросов</param>
        public delegate void JoinSubCallback(Builder builder);

        /// <summary>
        /// Присоединение таблицы, формируемой подзпросом
        /// </summary>
        /// <param name="joinSubCallback">Подзапрос для присоединяемой таблицы</param>
        /// <param name="tableNameAlias">Название таблицы</param>
        /// <param name="first">Первый столбец в условии присоединения</param>
        /// <param name="comparisonOperator">Оператор в условии присоединения</param>
        /// <param name="second">Второй столбец в условии присоединения</param>
        /// <param name="type">Тип присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder JoinSub(JoinSubCallback joinSubCallback,
                               string tableNameAlias,
                               string first,
                               string comparisonOperator,
                               string second,
                               string type = "INNER")
        {
            Builder builder = new(Model);
            joinSubCallback(builder);
            JoinClause joinClause = new(Model, tableNameAlias, type, builder);
            joinClause.On(first, comparisonOperator, second);
            _joins.Add(joinClause);
            return this;
        }

        /// <summary>
        /// Левое присоединение таблицы, формируемой подзпросом
        /// </summary>
        /// <param name="joinSubCallback">Подзапрос для присоединяемой таблицы</param>
        /// <param name="tableNameAlias">Название таблицы</param>
        /// <param name="first">Первый столбец в условии присоединения</param>
        /// <param name="comparisonOperator">Оператор в условии присоединения</param>
        /// <param name="second">Второй столбец в условии присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder LeftJoinSub(JoinSubCallback joinSubCallback,
                               string tableNameAlias,
                               string first,
                               string comparisonOperator,
                               string second)
        {
            return JoinSub(joinSubCallback, tableNameAlias, first, comparisonOperator, second, "LEFT");
        }

        /// <summary>
        /// Перекрестное присоединение таблицы, формируемой подзпросом
        /// </summary>
        /// <param name="joinSubCallback">Подзапрос для присоединяемой таблицы</param>
        /// <param name="tableNameAlias">Название таблицы</param>
        /// <param name="first">Первый столбец в условии присоединения</param>
        /// <param name="comparisonOperator">Оператор в условии присоединения</param>
        /// <param name="second">Второй столбец в условии присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder CrossJoinSub(JoinSubCallback joinSubCallback,
                               string tableNameAlias,
                               string first,
                               string comparisonOperator,
                               string second)
        {
            return JoinSub(joinSubCallback, tableNameAlias, first, comparisonOperator, second, "CROSS");
        }

        /// <summary>
        /// Callback для формирования нескольких условий присоединения таблицы
        /// </summary>
        /// <param name="joinClause">Объект с условиями присоединения таблицы</param>
        public delegate void JoinParameters(JoinClause joinClause);

        /// <summary>
        /// Присоединение таблицы с несколькими условиями
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="joinParameters">Условия присоединения</param>
        /// <param name="type">Тип присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder JoinExtended(string tableName, JoinParameters joinParameters, string type = "INNER")
        {
            JoinClause joinClause = new(Model, tableName, type);
            joinParameters(joinClause);
            _joins.Add(joinClause);
            return this;
        }

        /// <summary>
        /// Левое присоединение таблицы с несколькими условиями
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="joinParameters">Условия присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder LeftJoinExtended(string tableName, JoinParameters joinParameters)
        {
            return JoinExtended(tableName, joinParameters, "LEFT");
        }

        /// <summary>
        /// Перекрестное присоединение таблицы с несколькими условиями
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="joinParameters">Условия присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder CrossJoinExtended(string tableName, JoinParameters joinParameters)
        {
            return JoinExtended(tableName, joinParameters, "CROSS");
        }

        /// <summary>
        /// Присоединение таблицы, формируемой подзпросом, с несколькими условиями
        /// </summary>
        /// <param name="joinSubCallback">Подзапрос для присоединяемой таблицы</param>
        /// <param name="tableNameAlias">Название таблицы</param>
        /// <param name="joinParameters">Условия присоединения</param>
        /// <param name="type">Тип присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder JoinSubExtended(JoinSubCallback joinSubCallback,
                                       string tableNameAlias,
                                       JoinParameters joinParameters,
                                       string type = "INNER")
        {
            Builder builder = new(Model);
            joinSubCallback(builder);
            JoinClause joinClause = new(Model, tableNameAlias, type, builder);
            joinParameters(joinClause);
            _joins.Add(joinClause);
            return this;
        }

        /// <summary>
        /// Левое присоединение таблицы, формируемой подзпросом, с несколькими условиями
        /// </summary>
        /// <param name="joinSubCallback">Подзапрос для присоединяемой таблицы</param>
        /// <param name="tableNameAlias">Название таблицы</param>
        /// <param name="joinParameters">Условия присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder LeftJoinSubExtended(JoinSubCallback joinSubCallback, string tableNameAlias, JoinParameters joinParameters)
        {
            return JoinSubExtended(joinSubCallback, tableNameAlias, joinParameters, "LEFT");
        }

        /// <summary>
        /// Перекрестное присоединение таблицы, формируемой подзпросом, с несколькими условиями
        /// </summary>
        /// <param name="joinSubCallback">Подзапрос для присоединяемой таблицы</param>
        /// <param name="tableNameAlias">Название таблицы</param>
        /// <param name="joinParameters">Условия присоединения</param>
        /// <returns>Построитель запроса</returns>
        public Builder CrossJoinSubExtended(JoinSubCallback joinSubCallback, string tableNameAlias, JoinParameters joinParameters)
        {
            return JoinSubExtended(joinSubCallback, tableNameAlias, joinParameters, "CROSS");
        }

        /// <summary>
        /// Осуществляет группировку таблицы по указанным столбцам
        /// </summary>
        /// <param name="columns">Столбцы, по которым нужно сделать группировку</param>
        /// <returns>Построитель запросов</returns>
        public Builder GroupBy(params string[] columns)
        {
            if (columns.Length == 0)
            {
                throw new Exception("Метод Builder.Group() принимает минимум один параметр. Передано 0");
            }
            _groups = new List<string>();
            foreach (var column in columns)
            {
                _groups.Add("[" + column + "]");
            }
            return this;
        }

        /// <summary>
        /// Формирует условие having
        /// </summary>
        /// <param name="column">столбец</param>
        /// <param name="conditionOperator">оператор условия</param>
        /// <param name="value">значение</param>
        /// <returns>значение having</returns>
        public Builder Having(string column, string conditionOperator, string value)
        {
            _having = column + " " + conditionOperator + " " + value;
            return this;
        }

        /// <summary>
        /// Выполняет сортировку по возрастанию по указаным столбцам
        /// </summary>
        /// <param name="columns">Столбцы, по которым необходимо сделать сортировку</param>
        /// <param name="desc">Флаг направления сортировки. Для сортировки по убыванию используйте метод OrderByDesc()</param>
        /// <returns>Построитель запросов</returns>
        public Builder OrderBy(string column, bool desc = false)
        {
                if (desc == false)
                    _orders.Add(column + " ASC");
                else
                    _orders.Add(column + " DESC");
            return this;
        }

        /// <summary>
        /// Выполняет сортировку по убыванию по указаным столбцам
        /// </summary>
        /// <param name="columns">Столбцы, по которым необходимо сделать сортировку</param>
        /// <returns>Построитель запросов</returns>
        public Builder OrderByDesc(string column)
        {
            return OrderBy(column, true);
        }

        /// <summary>
        /// Осуществляет выборку и возвращает список моделей по написанному запросу
        /// </summary>
        /// <typeparam name="T">Класс текущей таблицы</typeparam>
        /// <returns>Список моделей</returns>
        public List<T> Get<T>() where T : Model, new()
        {
            string sqlString = GetSqlStringRender();
            SqlConnection sqlConnection = Model.SqlConnection;
            using SqlCommand command = new (sqlString, sqlConnection);
            sqlConnection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            List<T> models = new();
            while (reader.Read())
            {
                T model = new();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string name = reader.GetName(i).ToString();
                    FieldInfo fi = typeof(T).GetField(name);
                    object value = reader.GetValue(i);
                    if (value == DBNull.Value)
                        fi.SetValue(model, null);
                    else
                        fi.SetValue(model, value);
                }
                model.Original = (T)model.Clone();
                model.Exists = true;
                models.Add(model);
            }
            sqlConnection.Close();
            return models;
        }

        /// <summary>
        /// Используется совместно с JOIN, GROUP BY и агрегатными функциями.
        /// Осуществляет выборку и возвращает набор данных, согласно написанному запросу. 
        /// Для работы с данными пользователь должен дополнительно их обработать
        /// </summary>
        /// <returns>Список данных, где первый элемент - список названий столбцов</returns>
        public List<dynamic> Get()
        {
            string sqlString = GetSqlStringRender();
            SqlConnection sqlConnection = Model.SqlConnection;
            using SqlCommand command = new(sqlString, sqlConnection);
            sqlConnection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            List<dynamic> models = new();
            List<dynamic> columnsName = new();
            int n = 0;
            while (reader.Read())
            {
                List<dynamic> model = new();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (columnsName.Count != reader.FieldCount)
                        columnsName.Add(reader.GetName(i).ToString());
                    if ((columnsName.Count == reader.FieldCount) && (n==0))
                        models.Add(columnsName);

                    object value = reader.GetValue(i);
                    if (value == DBNull.Value)
                        model.Add(null);
                    else
                        model.Add(value);
                }
                models.Add(model);
                n++;
            }
            sqlConnection.Close();
            return models;
        }

        /// <summary>
        /// Обрабатывает список столбцов и превращает в фрагмент строки запроса
        /// </summary>
        /// <returns>Фрагмент строки запроса</returns>
        private string SelectRender()
        {
            return " SELECT " + string.Join(", ", _selects.ToArray());
        }

        /// <summary>
        /// Фрагмент запроса, где указано, с какой таблицей идет работа
        /// </summary>
        /// <returns>Фрагмент строки запроса</returns>
        private string FromRender()
        {
            return " FROM [" + _fromClause + "]";
        }

        /// <summary>
        /// Обрабатывает список присоединений и превращает в фрагмент строки запроса
        /// </summary>
        /// <returns>Фрагмент строки запроса</returns>
        private string JoinRender()
        {
            if (_joins.Count == 0)
                return "";

            string joinString = " ";
            foreach (var join in _joins)
            {
                joinString += join.Type + " JOIN ";

                if (join.TableSub != null)
                {
                    joinString += "(" + join.TableSub.GetSqlStringRender() + ") AS ";
                }

                joinString += "[" + join.TableName + "] ";
                joinString += join.WhereRender(true);
            }

            return joinString;
        }

        /// <summary>
        /// Обрабатывает список столбцов для группировки и превращает в фрагмент строки запроса
        /// </summary>
        /// <returns>Фрагмент строки запроса</returns>
        private string GroupRender()
        {
            if (_groups.Count != 0)
                return " GROUP BY " + string.Join(", ", _groups.ToArray());
            else return "";
        }

        /// <summary>
        /// Обрабатывает условие для группировки и превращает в фрагмент строки запроса
        /// </summary>
        /// <returns>Фрагмент строки запроса</returns>
        private string HavingRender()
        {
            if (_having != "")
                return " HAVING " + _having;
            else return "";
        }

        /// <summary>
        /// Обрабатывает список столбцов и превращает в фрагмент строки запроса
        /// </summary>
        /// <returns>Фрагмент строки запроса</returns>
        private string OrderByRender()
        {
            if (_orders.Count != 0)
                return " ORDER BY " + string.Join(", ", _orders.ToArray());
            else return "";
        }

        /// <summary>
        /// Обрабатывает список условий и превращает в фрагмент строки запроса
        /// </summary>
        /// <param name="fromJoin">Флаг, определяющий принадлежность к присоединению</param>
        /// <returns>Фрагмент строки запроса</returns>
        private string WhereRender(bool fromJoin = false)
        {
            if (_wheres.Count == 0)
                return "";
            string whereString = " " + (fromJoin ? "ON" : "WHERE") + " ";
            foreach (var where in _wheres)
            {
                if (where.ValueDataType == where.ValueDataTypeString)
                {
                    whereString += WhereStringRender(where);
                }
                else if (where.ValueDataType == where.ValueDataTypeArray)
                {
                    whereString += WhereArrayRender(where);
                }
                else if (where.GetValue().Selects.Contains("*"))
                {
                    whereString += WhereGroupBuilderRender(where);
                }
                else
                {
                    whereString += WhereInSubBuilderRender(where);
                }
            }
            return whereString;
        }

        /// <summary>
        /// Обрабатывает список условий, где значение - строка, и превращает в фрагмент строки запроса
        /// </summary>
        /// <param name="where">Объект условия</param>
        /// <returns>Фрагмент строки запроса, связанной с Where</returns>
        private string WhereStringRender(WhereClause where)
        {
            if (where == _wheres[0])
            {
                return string.Format("{0} {1} {2} '{3}' ", where.Column, where.ComparisonOperator, where.Not, where.GetValue());
            }
            return string.Format("{0} {1} {2} {3} '{4}' ", where.ConditionalOperator, where.Column, where.ComparisonOperator, where.Not, where.GetValue());
        }

        /// <summary>
        /// Обрабатывает список условий, где значение - массив строк, и превращает в фрагмент строки запроса
        /// </summary>
        /// <param name="where">Объект условия</param>
        /// <returns>Фрагмент строки запроса, связанной с Where</returns>
        private string WhereArrayRender(WhereClause where)
        {
            string[] values = where.GetValue();
            string valueString = "";
            for (int i = 0; i < values.Length; i++)
            {
                if (values.Length == 1)
                {
                    valueString += string.Format("(N'{0}') ", values[i]);
                }
                else if (values[i] == values[0])
                {
                    valueString += string.Format("(N'{0}', ", values[i]);
                }
                else if (values.Length - 2 >= i)
                {
                    valueString += string.Format("N'{0}', ", values[i]);
                }
                else
                {
                    valueString += string.Format("N'{0}') ", values[i]);
                }
            }
            if (where == _wheres[0])
            {
                return string.Format("{0} {1} {2} {3} ", where.Column, where.ComparisonOperator, where.Not, valueString);
            }
            return string.Format("{0} {1} {2} {3} {4} ", where.ConditionalOperator, where.Column, where.ComparisonOperator, where.Not, valueString);
        }

        /// <summary>
        /// Обрабатывает список условий, где значение - построитель запроса(несколько условий), и превращает в фрагмент строки запроса
        /// </summary>
        /// <param name="where">Объект условия</param>
        /// <returns>Фрагмент строки запроса, связанной с Where</returns>
        private string WhereGroupBuilderRender(WhereClause where)
        {
            if (where == _wheres[0])
            {
                return "(" + where.GetValue().WhereRender() + ") "; ;
            }
            return where.ConditionalOperator + " (" + where.GetValue().WhereRender() + ") ";
        }

        /// <summary>
        /// Обрабатывает список условий, где значение - построитель запроса(подзапрос), и превращает в фрагмент строки запроса
        /// </summary>
        /// <param name="where">Объект условия</param>
        /// <returns>Фрагмент строки запроса, связанной с Where</returns>
        private string WhereInSubBuilderRender(WhereClause where)
        {
            if (where == _wheres[0])
            {
                return "(" + where.GetValue().SelectRender() + where.GetValue().WhereRender() + ") ";
            }
            return where.ConditionalOperator + " WHERE " + where.Column + " " + where.ComparisonOperator + " (" + where.GetValue().SelectRender()
                                             + where.GetValue().FromRender() + where.GetValue().WhereRender() + ") ";
        }

        /// <summary>
        /// Формирует фрагмент строки запроса для Update (Set)
        /// </summary>
        /// <param name="values">Список значений, которые нужно установить</param>
        /// <returns>Фрагмент строки запроса</returns>
        private List<string> SetRender(Dictionary<string, string> values)
        {
            List<string> setString = new();
            foreach (var column in values.Keys)
            {
                setString.Add(column + " = " + values[column]);
            }
            return setString;
        }

        /// <summary>
        /// Собирает все необходимые фрагменты в полноценную строку запроса для команды Get
        /// </summary>
        private string GetSqlStringRender()
        {
            return SelectRender() + FromRender() + JoinRender() + WhereRender() + GroupRender() + HavingRender() + OrderByRender();
        }

        /// <summary>
        /// Собирает все необходимые фрагменты в полноценную строку запроса для команды Delete
        /// </summary>
        /// <returns></returns>
        private string DeleteSqlStringRender()
        {
            return "DELETE" + FromRender() + WhereRender() + GroupRender() + HavingRender() + OrderByRender();
        }
    }
}
