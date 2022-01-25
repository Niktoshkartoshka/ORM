using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ORM.Migrations
{
    /// <summary>
    /// Планировка таблицы
    /// </summary>
    public class TableBlueprint
    {
        /// <summary>
        /// Название таблицы
        /// </summary>
        private string _name;

        /// <summary>
        /// Список столбцов
        /// </summary>
        private List<ColumnBlueprint> _columns = new();

        /// <summary>
        /// Список измененных столбцов
        /// </summary>
        private List<string> _alterColumns = new();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Название таблицы</param>
        public TableBlueprint(string name)
        {
            _name = name;
        }
        /// <summary>
        /// Добавляет в список столбец
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="type">Тип столбца</param>
        /// <param name="length">Значение размерности типа</param>
        /// <returns>Экземпляр столбца</returns>
        private ColumnBlueprint AddColumn(string name, string type, int? length = null)
        {
            ColumnBlueprint column = new(name, type, length);
            _columns.Add(column);

            return column;
        }

        /// <summary>
        /// Строит строку запроса для создания таблицы в БД
        /// </summary>
        /// <returns>Строку запроса</returns>
        public string BuildSqlStringToCreate()
        {
            string sqlString = $"CREATE TABLE [{_name}] (";
            for (int i = 0; i < _columns.Count; i++)
            {
                if (_columns[i] != _columns[^1])
                {
                    sqlString += $"{_columns[i].BuildPartString()}, ";
                }
                else sqlString += $"{_columns[i].BuildPartString()} )";
            }
            return sqlString;
        }

        /// <summary>
        /// Строит строку запроса для изменения таблицы в БД
        /// </summary>
        /// <returns>Строку запроса<</returns>
        public string BuildSqlStringToAlter()
        {
            string sqlString = string.Join(" ", _alterColumns);
            for (int i = 0; i < _columns.Count; i++)
            {
                if (_columns[i]._drop)
                    continue;
                if (_columns[i]._changed)
                {
                    sqlString += $"ALTER TABLE [{_name}] ALTER COLUMN {_columns[i].BuildPartString()}; ";
                }
                else
                    sqlString += $"ALTER TABLE [{_name}] ADD {_columns[i].BuildPartString()}; ";
            }
            return sqlString;
        }

        /// <summary>
        /// Создает класс модели
        /// </summary>
        public void CreateModel()
        {
            string content = File.ReadAllText(@"..\..\..\..\ORM\Migrations\Stubs\Model.txt");
            content = content.Replace("{{namespace}}", MigrationKernel.ModelsNamespace);
            content = content.Replace("{{name}}", _name);
            string fields = "";
            for (int i = 0; i < _columns.Count; i++)
            {
                if (_columns[i] != _columns[^1])
                {
                    fields += _columns[i].CreateModelFields() + "\n\t\t";
                }
                else fields += _columns[i].CreateModelFields();
            }
            content = content.Replace("{{field}}", fields);
            string filename = $"{_name}.cs";
            string path = $"{MigrationKernel.ModelsDir}\\{filename}";
            File.WriteAllText(path, content);
        }

        public void ChangeModel()
        {
            string path = $"{MigrationKernel.ModelsDir}\\{_name}.cs";
            string content = File.ReadAllText(path);

            for (int i = 0; i < _columns.Count; i++)
            {
                if (_columns[i]._drop)
                {
                    string nameRegex = @$"[\s\t]+public\s\w+\??\s{_columns[i]._name};";
                    Regex rgx = new(nameRegex);
                    content = rgx.Replace(content, "");
                }
                else if (_columns[i]._changed)
                {
                    string nameRegex = @$"public\s\w+\??\s{_columns[i]._name};";
                    Regex rgx = new(nameRegex);
                    content = rgx.Replace(content, _columns[i].CreateModelFields());
                }
                else
                {
                    int n = content.LastIndexOf(";");
                    content = content.Insert(n + 1, $"\n\t\t{_columns[i].CreateModelFields()}");
                }
            }
            File.WriteAllText(path, content);
        }

        /// <summary>
        /// Указывает для столбца тип varchar
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="length">Размерность</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Varchar(string name, int? length = null)
        {
            return AddColumn(name, ColumnBlueprint.VarcharType, length);
        }
        /// <summary>
        /// Указывает для столбца тип int
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="length">Размерность</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Integer(string name, int? length = null)
        {
            return AddColumn(name, ColumnBlueprint.IntegerType, length);
        }
        /// <summary>
        /// Указывает для столбца тип nvarchar
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="length">Размерность</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Nvarchar(string name, int? length = null)
        {
            return AddColumn(name, ColumnBlueprint.NvarcharType, length);
        }
        /// <summary>
        /// Указывает для столбца тип char
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="length">Размерность</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Char(string name, int? length = null)
        {
            return AddColumn(name, ColumnBlueprint.CharType, length);
        }
        /// <summary>
        /// Указывает для столбца тип time
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="length">Размерность</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Time(string name, int? length = null)
        {
            return AddColumn(name, ColumnBlueprint.TimeType, length);
        }
        /// <summary>
        /// Указывает для столбца тип text
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="length">Размерность</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Text(string name, int? length = null)
        {
            return AddColumn(name, ColumnBlueprint.TextType, length);
        }
        /// <summary>
        /// Указывает для столбца тип binary
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="length">Размерность</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Binary(string name, int? length = null)
        {
            return AddColumn(name, ColumnBlueprint.BinaryType, length);
        }
        /// <summary>
        /// Указывает для столбца тип datetime
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="length">Размерность</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Datetime(string name, int? length = null)
        {
            return AddColumn(name, ColumnBlueprint.DatetimeType, length);
        }
        /// <summary>
        /// Указывает для столбца тип float
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="length">Размерность</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Float(string name, int? length = null)
        {
            return AddColumn(name, ColumnBlueprint.FloatType, length);
        }
        /// <summary>
        /// Указывает для столбца тип decimal
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="length">Размерность</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Decimal(string name, int? length = null)
        {
            return AddColumn(name, ColumnBlueprint.DecimalType, length);
        }
        /// <summary>
        /// Указывает для столбца тип real
        /// </summary>
        /// <param name="name">Название столбца</param>
        /// <param name="length">Размерность</param>
        /// <returns>Столбец</returns>
        public ColumnBlueprint Real(string name, int? length = null)
        {
            return AddColumn(name, ColumnBlueprint.RealType, length);
        }

        /// <summary>
        /// Удалить столбец из таблицы
        /// </summary>
        /// <param name="name">Название столбца</param>
        public void DropColumn(string name)
        {
            _alterColumns.Add($"ALTER TABLE [{_name}] DROP COLUMN [{name}];");

            ColumnBlueprint column = new(name, ColumnBlueprint.RealType);
            column._drop = true;
            _columns.Add(column);
        }

        /// <summary>
        /// Добавить ограничение в таблицу
        /// </summary>
        /// <param name="name"></param>
        /// <param name="check"></param>
        public void AddCheck(string name, string check)
        {
            _alterColumns.Add($"ALTER TABLE [{_name}] ADD CONSTRAINT {name} CHECK ({check});");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void DropCheck(string name)
        {
            _alterColumns.Add($"ALTER TABLE [{_name}] DROP CONSTRAINT {name};");
        }
    }
}
