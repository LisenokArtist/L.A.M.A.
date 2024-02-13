using SQLite;

namespace TelegramClientBot.Models.DataBase
{
    /// <summary>
    /// Базовый класс для работы с таблицами базы данных.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseController<T>
    {
        public SQLiteConnection _connection { get; private set; }

        public BaseController(SQLiteConnection connection)
        {
            _connection = connection;
            InitializateTable();
        }

        /// <summary>
        /// Инициализирует (или создает если отсутствует) таблицу с свойствами (колонками) как у объекта типа T
        /// </summary>
        internal void InitializateTable() { _connection.CreateTable<T>(); }

        /// <summary>
        /// Абстрактный метод добавления данных в базу.
        /// </summary>
        /// <param name="obj"></param>
        public abstract void Add(object obj);

        /// <summary>
        /// Абстрактный метод получения данных из базы.
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public abstract IEnumerable<T> Get(int[] Ids);

        /// <summary>
        /// Метод обновления множеств значений в базе.
        /// </summary>
        /// <param name="values"></param>
        public void Update(IEnumerable<object> values)
        {
            _connection.UpdateAll(values);
        }
    }
}
