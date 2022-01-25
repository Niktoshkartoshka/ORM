using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Migrations
{
    /// <summary>
    /// Планировка столбцов
    /// </summary>
    public class ColumnBlueprint
    {
        /// <summary>
        /// Тип столбца varchar
        /// </summary>
        public const string VarcharType = "varchar";

        /// <summary>
        /// Тип столбца int
        /// </summary>
        public const string IntegerType = "int";

        /// <summary>
        /// Тип столбца nvarchar
        /// </summary>
        public const string NvarcharType = "nvarchar";

        /// <summary>
        /// Тип столбца char
        /// </summary>
        public const string CharType = "char";

        /// <summary>
        /// Тип столбца time
        /// </summary>
        public const string TimeType = "time";

        /// <summary>
        /// Тип столбца text
        /// </summary>
        public const string TextType = "text";

        /// <summary>
        /// Тип столбца binary
        /// </summary>
        public const string BinaryType = "binary";

        /// <summary>
        /// Тип столбца datetime2
        /// </summary>
        public const string DatetimeType = "datetime2";

        /// <summary>
        /// Тип столбца float
        /// </summary>
        public const string FloatType = "float";

        /// <summary>
        /// Тип столбца decimal
        /// </summary>
        public const string DecimalType = "decimal";

        /// <summary>
        /// Тип столбца real
        /// </summary>
        public const string RealType = "real";

        /// <summary>
        /// Название столбца
        /// </summary>
        internal string _name;

        /// <summary>
        /// Тип столбца
        /// </summary>
        private string _type;

        /// <summary>
        /// Размерность
        /// </summary>
        private int? _length;

        /// <summary>
        /// Начальное значение для автоинкремента
        /// </summary>
        private int _seed;

        /// <summary>
        /// Значение инкремента
        /// </summary>
        private int _increment;

        /// <summary>
        /// Может ли быть значение NULL
        /// </summary>
        private bool _nullable = false;

        /// <summary>
        /// Автоинкремент
        /// </summary>
        private bool _identity = false;

        /// <summary>
        /// Первичный ключ
        /// </summary>
        private bool _primary = false;

        /// <summary>
        /// Меняется существующий столбец
        /// </summary>
        internal bool _changed = false;

        /// <summary>
        /// Удаляется столбец
        /// </summary>
        internal bool _drop = false;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="type">Тип</param>
        /// <param name="length">Размерность</param>
        public ColumnBlueprint(string name, string type, int? length = null)
        {
            _name = name;
            _type = type;
            _length = length;
        }

        /// <summary>
        /// Строит строку запроса для создания или изменения таблицы в БД
        /// </summary>
        /// <returns>Cтрока запроса</returns>
        public string BuildPartString()
        {
            string sqlString = $"[{_name}] {_type}";
            if (_length != null)
            {
                sqlString += $"({_length})";
            }
            if (!_nullable)
            {
                sqlString += " NOT ";
            }
            sqlString += " NULL ";
            if (_primary)
            {
                sqlString += $" PRIMARY KEY ";
            }
            if (_identity)
            {
                sqlString += $" IDENTITY({_seed}, {_increment}) ";
            }
            return sqlString;
        }

        public string BuildPartStringToAlter(string nameTable)
        {
            string sqlString = $"ALTER TABLE [{nameTable}] ";
            if (_changed)
            {
                sqlString += " ALTER COLUMN ";
            }
            else
            {
                sqlString += " ADD";
            }
                sqlString = $"[{_name}] {_type}";
            if (_length != null)
            {
                sqlString += $"({_length})";
            }
            if (!_nullable)
            {
                sqlString += " NOT ";
            }
            sqlString += " NULL ";
            if (_primary)
            {
                sqlString += $" PRIMARY KEY ";
            }
            if (_identity)
            {
                sqlString += $" IDENTITY({_seed}, {_increment}) ";
            }
            return sqlString;
        }


        /// <summary>
        /// Строит строку запроса для создания таблицы в БД
        /// </summary>
        /// <returns>Строку запроса</returns>
        public string CreateModelFields()
        {
            string modelString = "";
            if (_primary && _name != "Id")
            {
                modelString += "public override string Primary { get; set; }" + $" = \"{_name}\";\n\t\t";
            }
            modelString += "public ";
            switch (_type)
            {
                case VarcharType:
                case NvarcharType:
                case CharType:
                case TextType:
                case BinaryType:
                    if (_nullable)
                        modelString += "string? ";
                    else
                        modelString += "string ";
                    break;
                case IntegerType:
                    if (_nullable)
                        modelString += "int? ";
                    else
                        modelString += "int ";
                    break;

                case TimeType:
                case DatetimeType:
                    if (_nullable)
                        modelString += "DateTime? ";
                    else
                        modelString += "DateTime ";
                    break;

                case FloatType:
                    if (_nullable)
                        modelString += "float? ";
                    else
                        modelString += "float ";
                    break;

                case DecimalType:
                    if (_nullable)
                        modelString += "decimal? ";
                    else
                        modelString += "decimal ";
                    break;

                case RealType:
                    if (_nullable)
                        modelString += "double? ";
                    else
                        modelString += "double ";
                    break;
            }
            return modelString + $"{_name};";
        }

        /// <summary>
        /// Является первичным ключом
        /// </summary>
        /// <param name="isPrimary">Флаг для запроса</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Primary(bool isPrimary = true)
        {
            _primary = isPrimary;

            return this;
        }

        /// <summary>
        /// Может принимать значения NULL
        /// </summary>
        /// <param name="isNullable">Флаг для запроса</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Nullable(bool isNullable = true)
        {
            _nullable = isNullable;

            return this;
        }

        /// <summary>
        /// Автоинкремент
        /// </summary>
        /// <param name="seed">Начальное знаечние</param>
        /// <param name="increment">Значение инкремента</param>
        /// <param name="isIdentity">Флаг для запроса</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Identity(int seed = 1, int increment = 1, bool isIdentity = true)
        {
            _seed = seed;
            _increment = increment;
            _identity = isIdentity;

            return this;
        }

        public ColumnBlueprint Changed(bool isChanged = true)
        {
            _changed = isChanged;

            return this;
        }
    }
}
