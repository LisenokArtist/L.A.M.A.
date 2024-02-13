using SQLite;
using TelegramClientBot.Models.Tables;
using TL;

namespace TelegramClientBot.Models.DataBase
{
    public class MessageController : BaseController <MessageModel>
    {
        public MessageController(SQLiteConnection connection) : base(connection) { }

        public override void Add(object obj)
        {
            switch (obj)
            {
                case Message m:
                    {
                        var messageModel = new MessageModel
                        {
                            Id = m.ID,
                            FromId = m.From?.ID,
                            PeerId = m.Peer?.ID,
                            Message = m.message,
                            Date = m.Date,
                        };
                        _connection.Insert(messageModel);
                        break;
                    }

                case MessageService ms:
                    {
                        var messageModel = new MessageModel
                        {
                            Id = ms.ID,
                            FromId = ms.From?.ID,
                            PeerId = ms.Peer?.ID,
                            Message = ms.action.GetType().Name[13..],
                            Date = ms.Date,
                        };
                        _connection.Insert(messageModel);
                        break;
                    }

                case MessageActionSetMessagesTTL mas:
                    {
                        break;
                    }
                    
                default: break;
            }
        }

        public override IEnumerable<MessageModel> Get(int[] Ids)
        {
            List<MessageModel> result = _connection.Query<MessageModel>($"SELECT * FROM MessagesTable WHERE {string.Join(" OR ", Ids.Select(x => $"Id={x}"))}");
            return result;
        }
    }

}
