using CommandLine;
using TL;

namespace TelegramClientBot.Models.Controllers.Commands.Items
{
    public sealed class DateOptions : CommandOptionsBase
    {
        [Option('f', "format", Required = false, HelpText = "Выводит результат в указанном формате")]
        public string Format { get; set; } = string.Empty;
    }

    public class DateCommand : CommandBase
    {
        public DateCommand() : base()
        {
            Desctiprion = "Команда для проверки даты на сервере, на котором работает бот";
        }

        public override CommandOptionsBase Options { get { return new DateOptions(); } }

        public override void Run(Message message)
        {

        }




        /*public override object Run(params string[] args)
        {
            var options = Parser.Default.ParseArguments<DateCommandOptions>(args);

            if (options is Parsed<DateCommandOptions> res)
            {
                if (res.Value.Format != null)
                {
                    return DateTime.Now.ToString(res.Value.Format);
                }

                return DateTime.Now.ToString();
            }
            else
            {
                var notParsed = (NotParsed<DateCommandOptions>)options;
                var notParsedResult = from i in notParsed.Errors
                                      let e = i as UnknownOptionError
                                      select $"{e.Tag} {e.Token}";
                var r = string.Join(Environment.NewLine, notParsedResult);
                return r;
            }
        }*/
    }
}
