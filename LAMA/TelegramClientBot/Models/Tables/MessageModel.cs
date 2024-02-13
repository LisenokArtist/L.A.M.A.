namespace TelegramClientBot.Models.Tables
{
    [SQLite.Table("MessagesTable")]
    public class MessageModel : BaseItem
    {
        public long? Id { get { return _Id; } set { _Id = value; } }
        private long? _Id;

        public long? FromId { get { return _fromId; } set { _fromId = value; } }
        private long? _fromId;

        public long? PeerId { get { return _peerId; } set { _peerId = value; } }
        private long? _peerId;

        public string? Message { get { return _message; } set { _message = value; } }
        private string? _message;

        public DateTime? Date { get { return _date; } set { _date = value; } }
        private DateTime? _date;

        public bool IsDeleted { get { return _isDeleted; } set { _isDeleted = value; } }
        private bool _isDeleted = false;
    }
}
