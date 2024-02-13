namespace TelegramClientBot.Models.Tables
{
    public enum UserPermissionLevel
    {
        Null,
        Base,
        Extended,
        Moderator,
        Admin
    }

    public class ContactModel : BaseItem
    {
        /// <summary>
        /// Идентификатор контакта
        /// </summary>
        public long? Id { get { return _id; } set { _id = value; } }
        private long? _id;

        /// <summary>
        /// Уровень доступа
        /// </summary>
        public int? PermissionLevel { get { return _permissionLevel; } set { _permissionLevel = value; } }
        private int? _permissionLevel;


        /// <summary>
        /// Контакт принадлежит телеграму
        /// </summary>
        public bool? IsTelegram { get { return _isTelegram; } set { _isTelegram = value; } }
        private bool? _isTelegram;


        /// <summary>
        /// Дата добавления
        /// </summary>
        public DateTime? AddedDate { get { return _addedDate; } set { _addedDate = value; } }
        private DateTime? _addedDate;
    }
}
