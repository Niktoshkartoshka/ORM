using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ORM
{
    /// <summary>
    /// Файл конфигураций
    /// </summary>
    public abstract class NktOrm
    {
        /// <summary>
        /// Название строки подключения по умолчанию
        /// </summary>
        private static string _defaultConnectionName;

        /// <summary>
        /// Возможные строки подключения
        /// </summary>
        private static Dictionary<string, SqlConnection> _connections = new();

        /// <summary>
        /// Позволяет получить название строки подключения, которое установлено по умолчанию
        /// </summary>
        /// <returns>Название строки подключения по умолчанию</returns>
        public static string GetDefaultConnectionName() => _defaultConnectionName;

        /// <summary>
        /// Позволяет сохранить строку подключения
        /// </summary>
        /// <param name="name">Название строки подключения</param>
        /// <param name="connectionString">Строка подключения</param>
        /// <param name="isDefault">Флаг установки подключения по умолчанию</param>
        public static void SetConnection(string name, string connectionString, bool isDefault = false)
        {
            if (_connections.ContainsKey(name))
            {
                throw new Exception("Соединение с названием " + name + " уже существует");
            }
            if (_connections.Count == 0 || isDefault)
            {
                _defaultConnectionName = name;
            }
            SqlConnection connection = new(connectionString);
            _connections.Add(name, connection);
        }

        /// <summary>
        /// Позволяет получить строку подключения по названию
        /// </summary>
        /// <param name="name">Название строки подключения</param>
        /// <returns>Строки подключения с указанным названием</returns>
        public static SqlConnection GetConnection(string name)
        {
            try
            {
                return _connections[name];
            }
            catch (Exception)
            {
                throw new Exception ("Соединение с названием " + name + " не существует");
            }
        }
    }
}
