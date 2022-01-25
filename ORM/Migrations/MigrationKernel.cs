using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ORM.Migrations
{
    /// <summary>
    /// Класс, отвечающий за генерацию файлов миграций, таблиц и моделей
    /// </summary>
    public abstract class MigrationKernel
    {
        /// <summary>
        /// Директория, где должны будут храниться классы миграций
        /// </summary>
        public static string MigrationsDir;
        /// <summary>
        /// Пространство имён для миграций
        /// </summary>
        public static string MigrationsNamespace;
        /// <summary>
        /// Директория, где должны будут храниться классы моделей
        /// </summary>
        public static string ModelsDir;
        /// <summary>
        /// Пространство имён для моделей
        /// </summary>
        public static string ModelsNamespace;
        /// <summary>
        /// Название объекта подключения
        /// </summary>
        private static string _connection;
        /// <summary>
        /// Запускает интейрфейс управления миграциями.<br/><br/>
        /// <b>Примечание.</b> Названия миграций должны соответствовать следующим правилам: <br/>
        /// 1. Для создания таблицы - <i>create_example_table</i>. Вместо "example" название таблицы.<br/>
        /// 2. Для изменения таблицы - <i>(xxx)_to_example_table ; (xxx)_from_example_table ; (xxx)_in_example_table </i>.<br/>
        /// Вместо "example" название таблицы, вместо "xxx" любые обозначения, например, <i>add_to_example_table </i>. <br/>
        /// 3. В названии может не быть вышеуказанных слов, например - <i>changing_example_table</i> . Главное чтобы было не менее 3 слов.<br/>
        /// </summary>
        /// <param name="migrationsDir">Директория, где должны будут храниться классы миграций</param>
        /// <param name="modelsDir">Директория, где должны будут храниться классы моделей</param>
        /// <param name="connectionName">Название объекта подключения</param>
        public static void Start(string migrationsDir, string modelsDir, string connectionName = null)
        {
            if (connectionName == null)
            {
                _connection = NktOrm.GetDefaultConnectionName();
            }
            MigrationsDir = migrationsDir;
            MigrationsNamespace = Regex.Match(MigrationsDir, @"(\w+\\?)+").Value.Replace("\\", ".");
            ModelsDir = modelsDir;
            ModelsNamespace = Regex.Match(ModelsDir, @"(\w+\\?)+").Value.Replace("\\", ".");

            while (true)
            {
                Console.WriteLine("Что Вы хотите сделать? Выберите цифру:" +
                      "\n создать миграцию - 1" +
                      "\n запустить миграцию - 2" +
                      "\n произвести откат миграции - 3" +
                      "\n опубликовать конфигурации - 4 (нужно выбрать перед созданием миграций)" +
                      "\n выход - любая клавиша");

                string operationNumber = Console.ReadLine();
                switch (operationNumber)
                {
                    case "1":
                        Console.WriteLine("Введите название:");
                        Console.WriteLine(Make(Console.ReadLine()));
                        break;
                    case "2":
                        CallUp("Up");
                        break;
                    case "3":
                        CallDown("Down");
                        break;
                    case "4":
                        Console.WriteLine(PublishConfig());
                        break;
                    default:
                        return;
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Создает конфигурационный файл: файл миграции, создающей таблицу, в которой содержится информация 
        /// о существующих миграциях и порядках их добавления. <br/>
        /// </summary>
        /// <returns>Заключение об удачности</returns>
        public static string PublishConfig()
        {
            if (Directory.EnumerateFileSystemEntries(MigrationsDir).Any())
            {
                return $"Ошибка: директория {MigrationsDir} должна быть пустой";
            }
            string contentTable = File.ReadAllText(@"..\..\..\..\ORM\Migrations\Stubs\MigrationTable.txt");
            string filename = "0000_00_00_000000_create_migration_table.cs";
            string pathTable = $"{MigrationsDir}\\{filename}";
            contentTable = contentTable.Replace("{{namespace}}", MigrationsNamespace);

            File.WriteAllText(pathTable, contentTable);
            return $"Создана начальная миграция {filename}";
        }
        /// <summary>
        /// Создает миграцию с соответствующим названием.
        /// </summary>
        /// <param name="name">Название файла миграции</param>
        /// <returns>Заключение об удачности</returns>
        public static string Make(string name)
        {
            string[] nameParts = name.Split('_');
            if (name == "" || nameParts.Length < 3)
            {
                return "Ошибка: не передано имя миграции либо имя миграции некорректно";
            }

            string content = File.ReadAllText(@"..\..\..\..\ORM\Migrations\Stubs\Migration.txt");

            if (nameParts[^3] == "create" && nameParts[^1] == "table") // для create
            {
                //bool createTableBlank = true;
                content = content.Replace("{{method}}", ToCamelCase("Create"));
                content = content.Replace("{{example}}", "table.Integer(" + "Id" + ").Unsigned();");
            }
            else if ((nameParts[^3] == "to" || nameParts[^3] == "from" || nameParts[^3] == "in") && nameParts[^1] == "table") // для alter
            {
                const string nameRegexDown = @"Schema\.DropIfExists.+";
                Regex rgxDown = new(nameRegexDown);
                const string nameRegexUp = @"Schema\.{{method}}.+[\r\n\t\s] +{[\r\n\t\s\/]+}\);";
                content = rgxDown.Replace(content, Regex.Match(content, nameRegexUp).Value);

                content = content.Replace("{{method}}", ToCamelCase("Alter"));
                content = content.Replace("{{example}}", "//");
            }
            else // рандомные команды
            {
                const string nameRegexUp = @"Schema\.{{method}}.+[\r\n\t\s] +{[\r\n\t\s\/]+}\);";
                Regex rgxUp = new(nameRegexUp);
                content = rgxUp.Replace(content, "//");

                const string nameRegexDown = @"Schema\.DropIfExists.+";
                Regex rgxDown = new(nameRegexDown);
                content = rgxDown.Replace(content, "//");
            }

            content = content.Replace("{{namespace}}", MigrationsNamespace);

            content = content.Replace("{{class}}", ToCamelCase(name));

            content = content.Replace("{{name}}", ToCamelCase(nameParts[^2]));


            string filename = $"{DateTime.Now:yyyy_MM_dd_HHmmss}_{name}.cs";
            string path = $"{MigrationsDir}\\{filename}";

            File.WriteAllText(path, content);

            return $"Создана миграция {filename}";
        }
        /// <summary>
        /// Вызывает метод прямой миграции
        /// </summary>
        /// <param name="methodName">Название метода</param>
        private static void CallUp(string methodName)
        {
            string currentAssembly = MigrationsNamespace.Split(".")[0];

            const string nameRegex = @"(?<=\d{4}_\d{2}_\d{2}_\d{6}_)\w+";

            string[] migrationsNames = Directory.GetFiles(MigrationsDir);

            List<string> values = new();

            string? lastName = null;
            int? lastBatch = null;
            int indexLastMigration = 0;
            SqlConnection sqlConnection = NktOrm.GetConnection(_connection);
            string sqlStringName = $"SELECT TOP 1 [NameMigration] FROM [Migration] ORDER BY [Id] DESC";
            string sqlStringBatch = $"SELECT TOP 1 [Batch] FROM [Migration] ORDER BY [Id] DESC";
            try
            {
                sqlConnection.Open();
                SqlCommand commandName = new(sqlStringName, sqlConnection);
                lastName = Convert.ToString(commandName.ExecuteScalar());

                SqlCommand commandBatch = new(sqlStringBatch, sqlConnection);
                lastBatch = Convert.ToInt32(commandBatch.ExecuteScalar()) + 1;
                sqlConnection.Close();

                indexLastMigration = Array.IndexOf(migrationsNames, MigrationsDir + "\\" + lastName) + 1;
            }
            catch (SqlException)
            {
                sqlConnection.Close();
                //Console.WriteLine($"{ex.Message}");
            }
            lastBatch ??= 1;

            foreach (string migrationName in migrationsNames.Skip<string>(indexLastMigration))
            {
                string migrationNameWithoutPath = migrationName.Replace($"{MigrationsDir}\\", "");
                string migrationNameWithoutDate = Regex.Match(migrationNameWithoutPath, nameRegex).Value;

                string type = $"{MigrationsNamespace}.{ToCamelCase(migrationNameWithoutDate)}, {currentAssembly}";
                Type migrationType = Type.GetType(type);
                ConstructorInfo migrationConstructor = migrationType?.GetConstructor(Type.EmptyTypes);
                dynamic migration = migrationConstructor?.Invoke(null);
                MethodInfo schemaInitMethodInfo = migrationType?.GetMethod("Init");
                schemaInitMethodInfo?.Invoke(migration, null);
                MethodInfo methodInfo = migrationType?.GetMethod(methodName);

                try
                {
                    methodInfo.Invoke(migration, null);        
                    Console.WriteLine($"Миграция {migrationNameWithoutPath} успешно выполнена\n");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    continue;
                }

                values.Add($"('{migrationNameWithoutPath}', '{lastBatch}')");
            }
            string sqlString = $"INSERT INTO [Migration] ([NameMigration], [Batch]) VALUES {string.Join(", ", values.ToArray())};";
            sqlConnection.Open();
            SqlCommand command = new(sqlString, sqlConnection);
            command.ExecuteNonQuery();
            sqlConnection.Close();
        }
        /// <summary>
        /// Вызывает метод обратной миграции
        /// </summary>
        /// <param name="methodName">Название метода</param>
        private static void CallDown(string methodName)
        {
            string currentAssembly = MigrationsNamespace.Split(".")[0];

            const string nameRegex = @"(?<=\d{4}_\d{2}_\d{2}_\d{6}_)\w+";

            string[] migrationsNames = Directory.GetFiles(MigrationsDir);
            int? lastBatch = null;
            SqlConnection sqlConnection = NktOrm.GetConnection(_connection);
            string sqlStringBatch = $"SELECT TOP 1 [Batch] FROM [Migration] ORDER BY [Id] DESC";
            try
            {
                sqlConnection.Open();
                SqlCommand commandBatch = new(sqlStringBatch, sqlConnection);
                lastBatch = Convert.ToInt32(commandBatch.ExecuteScalar());
                sqlConnection.Close();
            }
            catch (SqlException)
            {
                sqlConnection.Close();
            }
            List<Stubs.Migration> migrations = Model.Query<Stubs.Migration>().Select("NameMigration")
                                                                             .OrderByDesc("Id")
                                                                             .Where("Batch", "=", $"{lastBatch}")
                                                                             .Get<Stubs.Migration>();

            Array.Reverse(migrationsNames);

            for (int i = 0; i < migrations.Count; i++)
            {
                /*if (MigrationsDir + "\\" + migrations[i].NameMigration != migrationsNames[i])
                {
                    Console.WriteLine("Ошибка: запись либо миграция была удалена");
                    return;
                }*/
                Model.Query<Stubs.Migration>().Select("NameMigration")
                                              .Where("Batch", "=", $"{lastBatch}")
                                              .Where("NameMigration", "=", $"{migrations[i].NameMigration}")
                                              .Delete();
                string migrationNameWithoutPath = migrationsNames[i].Replace($"{MigrationsDir}\\", "");
                string migrationNameWithoutDate = Regex.Match(migrationNameWithoutPath, nameRegex).Value;

                string type = $"{MigrationsNamespace}.{ToCamelCase(migrationNameWithoutDate)}, {currentAssembly}";
                Type migrationType = Type.GetType(type);
                ConstructorInfo migrationConstructor = migrationType?.GetConstructor(Type.EmptyTypes);
                dynamic migration = migrationConstructor?.Invoke(null);
                MethodInfo schemaInitMethodInfo = migrationType?.GetMethod("Init");
                schemaInitMethodInfo?.Invoke(migration, null);
                MethodInfo methodInfo = migrationType?.GetMethod(methodName);
                methodInfo.Invoke(migration, null);
                
                Console.WriteLine($"Обратная миграция {migrationNameWithoutPath} успешно выполнена");
            }
        }

        /// <summary>
        /// Преобразует строку в строку формата CamelCase 
        /// </summary>
        /// <param name="str">Изначальная строка</param>
        /// <returns>Результирующая строка</returns>
        private static string ToCamelCase(string str)
        {
            return str.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                .Aggregate(string.Empty, (s1, s2) => s1 + s2);
        }
    }
}
