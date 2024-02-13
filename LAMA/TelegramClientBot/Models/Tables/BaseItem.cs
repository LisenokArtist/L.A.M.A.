namespace TelegramClientBot.Models.Tables
{
    public class BaseItem
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int PK { get; set; }
    }
}
