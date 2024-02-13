using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using TL;
using WTelegram;

namespace TelegramClientBot
{
    public static class Commands
    {
        //public static List<Function> Functions = commandList();
        //static List<Function> commandList()
        //{
        //    return new List<Function>()
        //    {
        //        new Function(help, $"Возвращает список доступных комманд."),
        //        new Function(save, $"Сохраняет текущий диалог у бота."),
        //        new Function(test, $"Это тестовая функция."),
        //        new Function(date, $"Возвращает системную время и дату бота."),
        //        /*
        //        new Function(list, $"Возвращает список пользователей по критериям.{Environment.NewLine}" +
        //        $"          -o - только онлайн пользователи.{Environment.NewLine}" +
        //        $"          -f - только оффлайн пользователи."),*/
        //    };
        //}

        //public static void save(Message replyMessage, Command command)
        //{
        //    var peerId = replyMessage.from_id.ID;
        //    var chatData = Program.ChatsCollection.FirstOrDefault(x => x.Key == peerId).Value ?? null;

        //    if (chatData != null)
        //    {
        //        chatData.SaveMessages();
        //    }
        //    else
        //    {
        //        chatData = new ChatData(peerId);
        //        //chatData.History = 
        //    }
        //}

        //public static void test(Message replyMessage, Command command)
        //{
        //    trySendMessage(replyMessage, "Тестовое сообщение, возвращаемое тестовой функцией.");

        //    return;
        //}

        //public static void help(Message replyMessage, Command command)
        //{
        //    var messages = Functions.ConvertAll(x =>
        //    {
        //        return $"/{x.Name} - {x.Description}";
        //    });
        //    var message = $"Список доступных команд{Environment.NewLine}{string.Join(Environment.NewLine, messages.ToArray())}";

        //    trySendMessage(replyMessage, message);

        //    return;
        //}

        //public static void date(Message replyMessage, Command command)
        //{
        //    trySendMessage(replyMessage, DateTime.Now.ToString());
        //    return;
        //}

        //public static void list(Message replyMessage, Command command)
        //{
        //    var isGroup = true;
        //    return;
        //}

        //public static async void trySendMessage(Message replyMessage, string responseMessage)
        //{
        //    if (responseMessage.StartsWith('/')) throw new Exception("Нельзя отправлять сообщения со слешем");

        //    var user = Program.Users.FirstOrDefault(x => x.Key == replyMessage.Peer.ID).Value ?? null;
        //    var group = Program.Chats.FirstOrDefault(x => x.Key == replyMessage.Peer.ID && Program.AllowedChatIDs.Any(y => x.Key == y)).Value ?? null;
        //    IPeerInfo? peer = user != null ? user : group != null ? group : null;

        //    if (peer != null)
        //    {
        //        await Program.Client.SendMessageAsync(peer.ToInputPeer(), responseMessage);
        //    }
        //}
    }

    public class Function
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Flags { get; set; }
        public Delegate Delegate { get; set; }

        public Function(Delegate del, string desc, string[]? flags = null)
        {
            Delegate = del;
            Name = Delegate.Method.Name;
            Description = desc;
            Flags = flags ?? Array.Empty<string>();
        }

        public override string ToString()
        {
            return $"/{this.Name}\t-\t{this.Description}{Environment.NewLine}" +
                $"";
        }
    }

    public class Flag
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Flag(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public class Command
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public IEnumerable<string> Flags { get; set; }


        public Command(string name, string value, IEnumerable<string> flags)
        {
            Name = name;
            Value = value;
            Flags = flags;
        }
    }
}
