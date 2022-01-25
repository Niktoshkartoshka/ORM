using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ORM
{
    /// <summary>
    /// Информация об условии WHERE
    /// </summary>
    internal class WhereClause
    {
        /// <summary>
        /// Тип данных string
        /// </summary>
        internal readonly string ValueDataTypeString = "String";
        /// <summary>
        /// Тип данных string[]
        /// </summary>
        internal readonly string ValueDataTypeArray = "Array";
        /// <summary>
        /// Тип данных Builder
        /// </summary>
        internal readonly string ValueDataTypeBuilder = "Builder";

        /// <summary>
        /// Столбец
        /// </summary>
        internal string Column;
        /// <summary>
        /// Оператор сравнения
        /// </summary>
        internal string ComparisonOperator;
        /// <summary>
        /// Строчное значение условия
        /// </summary>
        public string StringValue;
        /// <summary>
        /// Множество строчных значений, собранных в массив
        /// </summary>
        public string[] ArrayValue;
        /// <summary>
        /// Значение с подзапросом
        /// </summary>
        public Builder BuilderValue;
        /// <summary>
        /// Оператор условия
        /// </summary>
        public string ConditionalOperator;
        /// <summary>
        /// Флаг присутсвия NOT в запросе
        /// </summary>
        internal string Not;
        /// <summary>
        /// Какой тип данных используется в условии (нужно для их получения)
        /// </summary>
        internal string ValueDataType;

        /// <summary>
        /// Конструктор для условий, формируемых методами Where, OrWhere, WhereNull, OrWhereNull, WhereNotNull, OrWhereNotNull.
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="comparisonOperator">Оператор сравнения</param>
        /// <param name="value">Значение для условия</param>
        /// <param name="conditionalOperator">Оператор условия</param>
        /// <param name="not">Частица NOT</param>
        internal WhereClause(string column, string comparisonOperator, string value, string conditionalOperator, string not)
        {
            Column = ColumnRender(column);
            ComparisonOperator = comparisonOperator;
            StringValue = value;
            ConditionalOperator = conditionalOperator;
            Not = not;
            ValueDataType = ValueDataTypeString;
        }

        /// <summary>
        /// Конструктор для условий, формируемых методами WhereGroup, OrWhereGroup
        /// </summary>
        /// <param name="values">Значение для условия</param>
        /// <param name="conditionalOperator">Оператор условия</param>
        internal WhereClause(Builder values, string conditionalOperator)
        {
            Column = "";
            ComparisonOperator = "";
            BuilderValue = values;
            ConditionalOperator = conditionalOperator;
            ValueDataType = ValueDataTypeBuilder;
        }

        /// <summary>
        /// Конструктор для условий, формируемых методами WhereIn, OrWhereIn, WhereNotIn, OrWhereNotIn
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="comparisonOperator">Оператор сравнения</param>
        /// <param name="values">Значение для условия</param>
        /// <param name="conditionalOperator">Оператор условия</param>
        /// <param name="not">Частица NOT</param>
        internal WhereClause(string column, string comparisonOperator, string[] values, string conditionalOperator, string not)
        {
            Column = ColumnRender(column);
            ComparisonOperator = comparisonOperator;
            ArrayValue = values;
            ConditionalOperator = conditionalOperator;
            Not = not;
            ValueDataType = ValueDataTypeArray;
        }

        /// <summary>
        /// Конструктор для условий, формируемых методами WhereInSub, OrWhereInSub
        /// </summary>
        /// <param name="column">Название столбца, по которому делается условие</param>
        /// <param name="comparisonOperator">Оператор сравнения</param>
        /// <param name="values">Значение для условия</param>
        /// <param name="conditionalOperator">Оператор условия</param>
        /// <param name="not">Частица NOT</param>
        internal WhereClause(string column, string comparisonOperator, Builder values, string conditionalOperator, string not)
        {
            Column = ColumnRender(column);
            ComparisonOperator = comparisonOperator;
            BuilderValue = values;
            ConditionalOperator = conditionalOperator;
            Not = not;
            ValueDataType = ValueDataTypeBuilder;
        }

        /// <summary>
        /// Получает значение данных, динамически определяя его тип
        /// </summary>
        /// <returns>Значения условия</returns>
        internal dynamic GetValue()
        {
            return typeof(WhereClause).GetField(ValueDataType + "Value").GetValue(this);
        }

        /// <summary>
        /// Оборачивает скобками [] название таблицы/столбца
        /// </summary>
        /// <param name="column">Столбец</param>
        /// <returns>Преобразованный столбец</returns>
        internal static string ColumnRender(string column)
        {
            return String.Join('.', Array.ConvertAll(column.Split('.'), word => "[" + word + "]"));
        }
    }
}
