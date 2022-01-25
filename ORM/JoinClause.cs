using System;
using System.Collections.Generic;
using System.Text;

namespace ORM
{
    /// <summary>
    /// Информация о присоединяемой таблице и условиях присоединения
    /// </summary>
    public class JoinClause : Builder
    {
        /// <summary>
        /// Тип присоединения
        /// </summary>
        internal string Type;

        /// <summary>
        /// Название присоединяемой таблицы
        /// </summary>
        internal string TableName;

        /// <summary>
        /// Подзапрос присоединяемой таблицы
        /// </summary>
        internal Builder TableSub;

        /// <summary>
        /// Конструктор объекта с условиями присоединения таблицы
        /// </summary>
        /// <param name="model">Текущая модель</param>
        /// <param name="tableName">Название присоединяемой таблицы</param>
        /// <param name="type">Тип присоединения</param>
        /// <param name="tableSub">Подзапрос присоединяемой таблицы</param>
        internal JoinClause(Model model, string tableName, string type, Builder tableSub = null) : base(model)
        {
            TableName = tableName;
            Type = type;
            TableSub = tableSub;
        }

        /// <summary>
        /// Добавляет условие для присоединения
        /// </summary>
        /// <param name="first">Первый столбец в условии присоединения</param>
        /// <param name="comparisonOperator">Оператор в условии присоединения</param>
        /// <param name="second">Второй столбец в условии присоединения</param>
        /// <param name="conditionalOperator">Оператор условия</param>
        /// <returns>Объект с условиями присоединения таблицы</returns>
        public JoinClause On(string first, string comparisonOperator, string second, string conditionalOperator = "AND")
        {
            return (JoinClause)Where(first, comparisonOperator, WhereClause.ColumnRender(second), conditionalOperator);
        }

        /// <summary>
        /// Добавляет условие условие для присоединения c условием OR
        /// </summary>
        /// <param name="first">Первый столбец в условии присоединения</param>
        /// <param name="comparisonOperator">Оператор в условии присоединения</param>
        /// <param name="second">Второй столбец в условии присоединения</param>
        /// <returns>Объект с условиями присоединения таблицы</returns>
        public JoinClause OrOn(string first, string comparisonOperator, string second)
        {
            return On(first, comparisonOperator, second, "OR");
        }
    }
}
