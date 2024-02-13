using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using TL;

namespace TelegramClientBot
{
    static class ClientModel
    {
        static WTelegram.Client Client { get; set; }
        static User My;
        static readonly Dictionary<long, User> Users = new();
        static readonly Dictionary<long, ChatBase> Chats = new();

        static string? Config(string what)
        {
            switch (what)
            {
                case "api_id": return "29293269";
                case "api_hash": return "6fde20d780d858bc58ab959c4bf152d0";
                case "phone_number": return "+79514978054";
                case "verification_code": Console.Write("Code: "); return Console.ReadLine();
                default: return null;
            }
        }

        /*static async Task Main(string[] _)
        {
            Client = new WTelegram.Client(Config);
            using (Client)
            {
                Client.OnUpdate += Client_OnUpdate;
                My = await Client.LoginUserIfNeeded();
                Users[My.id] = My;
                var dialogs = await Client.Messages_GetAllDialogs();
                dialogs.CollectUsersChats(Users, Chats);
            }
        }*/

        private static async Task Client_OnUpdate(IObject arg)
        {
            Updates updates = (Updates)arg;
            foreach (var update in updates.UpdateList)
            {
                switch (update)
                {
                    case UpdateNewMessage unm: break;
                }
            }
        }

        private static async void TryParseComma(string message)
        {
            string pattern = @"[/]\w+";
            RegexOptions options = RegexOptions.Multiline;
            var regex = new Regex(pattern, options);
            var isComma = regex.IsMatch(message);

            if (isComma)
            {
                await Client.SendMessageAsync(My.ToInputPeer(), "Это сообщение распознано как комманда");
            }
        }
    }
}
