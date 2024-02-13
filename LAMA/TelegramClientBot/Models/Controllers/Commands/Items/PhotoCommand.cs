using CommandLine;
using CommandLine.Text;
using TelegramClientBot.Models.Controllers.TimeTriggers;
using TL;
using static SQLite.SQLite3;

namespace TelegramClientBot.Models.Controllers.Commands.Items
{
    [Verb("photo", HelpText = "Задает настройки автоматического переключения аватара")]
    public sealed class PhotoOptions : CommandOptionsBase
    {
        [Option('e', "enabled", HelpText = "Задает состояние автоматической смены аватара", Required = false)]
        public bool Enabled { get; set; }

        [Option('o', "ordered", HelpText = "Задает смену аватара последовательно", Required = false)]
        public bool Ordered { get; set; }
    }

    public class PhotoCommand : CommandBase
    {
        private PhotoTrigger PhotoTrigger { get; set; }

        public override CommandOptionsBase Options { get { return new PhotoOptions(); } }

        public PhotoCommand(PhotoTrigger photoTrigger) : base()
        {
            PhotoTrigger = photoTrigger;
            Desctiprion = "Команда для проверки даты на сервере, на котором работает бот";
        }



        public override void Run(Message message)
        {
            var args = CommandBase.ExtractArgs(message);

            var parser = new Parser(x => x.HelpWriter = null);

            //Parser.Default.ParseArguments<PhotoOptions>(args)
            //    .WithParsed<PhotoOptions>(AutoSwitchOptionsParsed)
            //    .WithNotParsed(HandleParseError);
        }

        public void run(string[] args)
        {
            var parser = new Parser(x => x.HelpWriter = null);

            var parserResult = parser.ParseArguments<PhotoOptions>(args);
            parserResult.WithParsed<PhotoOptions>(o => AutoSwitchOptionsParsed(o, parserResult))
                .WithNotParsed(e => DisplayHelp(parserResult));
        }

        private void AutoSwitchOptionsParsed<T>(PhotoOptions options, ParserResult<T> result)
        {
            DisplayHelp(result);
            //if (!string.IsNullOrEmpty(options.Timer))
            //{
            //    try
            //    {
            //        var timer = TimeSpan.ParseExact(options.Timer ?? string.Empty, "hh:mm:ss", null);
            //        PhotoTrigger.TimeTrigger = timer;
            //    }
            //    catch (FormatException e) 
            //    {
            //        SendException(exception: e);
            //    }
            //}

            //to-do
        }

        private void HandleParseError(IEnumerable<CommandLine.Error> enumerable)
        {
            //DisplayHelp(enumerable);
        }

        private void SendException(IEnumerable<CommandLine.Error>? parserErrors = null, Exception? exception = null)
        {

        }
        /*private async void RunOptions(PhotoCommandOptions options)
        {
            if (options.Random == true)
            {
                var program = Program.ProgramModel;
                var getUserPhotos = await program.Client.Photos_GetUserPhotos(program.Me);
                
                var randomize = getUserPhotos.photos.Where(x => x.ID != program.Me.photo.photo_id).PickRandom();
                
                if (randomize != null)
                {
                    var response = await program.Client.Photos_UpdateProfilePhoto(randomize);
                    if (response != null)
                    {
                        program.Me.photo.photo_id = response.photo.ID;
                    }
                }
            }
        }*/
    }
}
