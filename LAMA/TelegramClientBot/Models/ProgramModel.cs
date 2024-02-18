using TelegramClientBot.Models.DataBase;
using TL;
using SQLite;
using TelegramClientBot.Models.Controllers.TimeTriggers;
using TelegramClientBot.Models.Controllers.Commands.Items;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using TelegramClientBot.Models.Extensions;

namespace TelegramClientBot.Models
{
    /// <summary>
    /// Тело программы.
    /// TO-DO список (* - есть, ? - нету):
    ///     * легкая база данных для хранения некоторых данных;
    ///     * обработчики событий OnMessage от Telegram API;
    ///     * работа с временными тригерами;
    ///     * работа с командными тригерами;
    ///     ? настройка работы с командами (выполнение и автогенерация справочника по командам);
    ///     ? привелегии, роли, уровни доступа для других контактов;
    ///     ? преобразование голоса в текст (а это сложно);
    ///     ? парная работа приложения (клиент + бот, автоответчик, анти-спам бот);
    ///     ? что-то еще...
    /// </summary>
    public class ProgramModel : IDisposable
    {
        #region SQL
        public SQLiteConnection Connection { get; private set; }

        public MessageController MessageController { get; private set; }
        #endregion

        #region Telegram
        public WTelegram.Client Client { get; private set; }
        public TL.User Me { get; set; }

        public readonly Dictionary<long, TL.User> Users = new Dictionary<long, TL.User>();
        public readonly Dictionary<long, TL.ChatBase> Chats = new Dictionary<long, TL.ChatBase>();
        #endregion

        #region Коллекции контроллеров
        public List<TimerTriggerBase> TimeTriggers { get; private set; } = new List<TimerTriggerBase>();
        public List<CommandBase> Commands { get; private set; } = new List<CommandBase>();
        #endregion

        public bool IsDisposed { get { return _isDisposed; } }
        private bool _isDisposed;

        public Task Initialization { get; private set; }


        public ProgramModel()
        {
            Initialization = InitializeAsync();
        }

        /// <summary>
        /// Инициализация программы
        /// </summary>
        private async Task InitializeAsync()
        {
            await InitializateDataBase();
            await InitializateTelegram();

            InitializateTimeTriggers();
            InitializateCommands();
        }

        #region Инициализация основных компонентов
        /// <summary>
        /// Инициализация базы данных.
        /// </summary>
        private Task InitializateDataBase()
        {
            Connection = new SQLiteConnection(new SQLiteConnectionString(Path.Combine(Environment.CurrentDirectory, "DataBase.db")));
            MessageController = new MessageController(Connection);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Инициализация клиента Telegram.
        /// </summary>
        private async Task InitializateTelegram()
        {
            Client = new WTelegram.Client(Config);
            Client.FloodRetryThreshold = 120;
            Me = await Client.LoginUserIfNeeded();

            Client.OnUpdate += OnUpdate;
        }
        
        /// <summary>
        /// Инициализация триггеров времени
        /// </summary>
        private void InitializateTimeTriggers()
        {
            var timeTriggers = new List<TimerTriggerBase>
            {
                new PhotoTrigger(new TimeSpan(1, 0, 0)),
            };

            timeTriggers.ForEach(x =>
            {
                x.OnTimeTriggered += OnTimeTriggered;
                x.Run();
            });

            TimeTriggers = timeTriggers;
        }

        /// <summary>
        /// Инициализация триггеров комманд
        /// </summary>
        private void InitializateCommands()
        {
            var commands = new List<CommandBase>()
            {
                new HelpCommand(),
                new PhotoCommand((PhotoTrigger)TimeTriggers.Single(x => x is PhotoTrigger)),
            };

            Commands = commands;
        }
        #endregion

        #region Обработчик событий триггера времени
        private void OnTimeTriggered(TimerTriggerBase @base)
        {
            switch (@base)
            {
                case PhotoTrigger pt: OnPhotoTrigger(pt); break;
            }
        }

        private async void OnPhotoTrigger(PhotoTrigger pt)
        {
            var photosData = await Client.Photos_GetUserPhotos(Me);

            if (photosData.photos.Length > 1)
            {
                if (pt.Ordered)
                {
                    var nextPhoto = photosData.photos.SkipWhile(x => x.ID != Me.photo.photo_id).Skip(1).FirstOrDefault();
                    if (nextPhoto != null)
                    {
                        await OnPhotoTriggerResponse(pt, nextPhoto);
                    }
                }
                else
                {
                    var randomize = photosData.photos.Where(x => x.ID != Me.photo.photo_id).PickRandom();
                    if (randomize != null)
                    {
                        await OnPhotoTriggerResponse(pt, randomize);
                    }
                }
            }
        }
        
        private async Task OnPhotoTriggerResponse(PhotoTrigger photoTrigger, PhotoBase photo)
        {
            try
            {
                var response = await Client.Photos_UpdateProfilePhoto(photo);

                if (response != null)
                {
                    Me.photo.photo_id = response.photo.ID;
                }

                if (photoTrigger.TemporalTimeTrigger != null)
                {
                    photoTrigger.TimeTrigger = (TimeSpan)photoTrigger.TemporalTimeTrigger;
                    photoTrigger.TemporalTimeTrigger = null;
                }
            }
            catch (TL.RpcException rpc) //FLOOD_WAIT_X
            {
                photoTrigger.TemporalTimeTrigger = photoTrigger.TimeTrigger;
                photoTrigger.TimeTrigger = new TimeSpan(0, 0, rpc.X);
            }
        }
        #endregion

        #region Обработчики событий ТГ
        /// <summary>
        /// Основной обработчик события.
        /// </summary>
        /// <param name="u">Класс с информацией о событиях.</param>
        private Task OnUpdate(UpdatesBase u)
        {
            foreach (var upd in u.UpdateList)
            {
                switch (upd)
                {
                    case UpdateNewMessage unm: OnUpdateNewMessage(unm.message); break;
                    case UpdateEditMessage uem: OnUpdateEditMessage(uem.message); break;
                    case UpdateDeleteMessages udm: OnUpdateDeleteMessages(udm); break;
                    case UpdateReadHistoryInbox urhi: OnUpdateReadHistoryInbox(urhi); break;
                    case UpdateUserStatus uus: OnUpdateUserStatus(uus); break;
                    default: OnUpdateDefault(upd); break;
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Обработчик события "новое сообщение".
        /// Сохраняет копию сообщения в базу.
        /// Если сообщение начинается со слеша "/", выполняет команду.
        /// </summary>
        /// <param name="message"></param>
        private void OnUpdateNewMessage(MessageBase message)
        {
            //Разрешать сохранение и выполнение комманд только от пользователей.
            bool isUserPeer = message.Peer is PeerUser;
            if (isUserPeer)
            {
                MessageController.Add(message);

                var isCommand = message is Message mc && mc.message.StartsWith('/');
                if (isCommand) OnCommandMessage(message);
            }
        }

        /// <summary>
        /// Выполняет команду если сообщение начинается со слеша "/".
        /// </summary>
        /// <param name="message"></param>
        private void OnCommandMessage(MessageBase message)
        {
            //Комманды доступны только мне
            if (message.Peer.ID != Me.id)
                return;

            if (message is Message m)
            {
                var commandName = m.message.Split(' ')[0].Remove(0, 1);
                var command = Commands.FirstOrDefault(x => x.Name.ToLower() == commandName);
                command?.Run(m);
            }
        }

        /// <summary>
        /// Обработчик события "сообщение изменено".
        /// Сохраняет текстовое сообщение в базу.
        /// </summary>
        /// <param name="mb"></param>
        private void OnUpdateEditMessage(MessageBase mb)
        {
            MessageController.Add(mb);

            return;
        }

        /// <summary>
        /// Обработчик события "сообщение удалено".
        /// Выполняет поиск в базе сообщений по его идентификатору и помечает запись как "удалено".
        /// </summary>
        /// <param name="udm"></param>
        private void OnUpdateDeleteMessages(UpdateDeleteMessages udm)
        {
            var queryMessages = MessageController.Get(udm.messages);
            foreach (var queryMessage in queryMessages)
            {
                queryMessage.IsDeleted = true;
            }
            MessageController.Update(queryMessages);
        }

        /// <summary>
        /// Обработчик события "сообщение прочитано"
        /// </summary>
        /// <param name="urhi"></param>
        private void OnUpdateReadHistoryInbox(UpdateReadHistoryInbox urhi)
        {

        }

        /// <summary>
        /// Обработчик события "статус контакта обновлен".
        /// </summary>
        /// <param name="uus"></param>
        private void OnUpdateUserStatus(UpdateUserStatus uus)
        {
            if (uus.user_id == Me.id) return;

            return;
        }

        /// <summary>
        /// Обработчик всех остальных событий
        /// </summary>
        /// <param name="upd"></param>
        private void OnUpdateDefault(Update upd)
        {
            return;
        }
        #endregion

        public void Dispose()
        {
            if (_isDisposed) return;

            Client.Dispose();

            Connection.Close();
            Connection.Dispose();

            _isDisposed = true;
        }

        /// <summary>
        /// Метод получения настроек для подключения к Telegram API.
        /// </summary>
        public static string? Config(string what)
        {
            /// Используйте следующую структуру файла secrets.json для настроек:
            /// {
            ///   "TGConfigurations": {
            ///     "api_id": "xxx",
            ///     "api_hash": "xxx",
            ///     "phone_number": "xxx"
            ///   }
            /// }
            var builder = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();
            var sections = builder.GetSection("TGConfigurations");

            switch (what)
            {
                case "api_id": return sections.GetSection("api_id").Value;
                case "api_hash": return sections.GetSection("api_hash").Value;
                case "phone_number": return sections.GetSection("phone_number").Value;
                case "verification_code": Console.Write("Code: "); return Console.ReadLine();
                default: return null;
            }
        }
    }
}
