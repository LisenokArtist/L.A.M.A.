using CommandLine;
using CommandLine.Text;
using TL;

namespace TelegramClientBot.Models.Controllers.Commands.Items
{
    public interface CommandOptionsBase
    {

        //public HelpText GetUsage(string s)
        //{
        //    return HelpText.AutoBuild<CommandOptionsBase>(this,
        //        e => HelpText.DefaultParsingErrorsHandler<CommandOptionsBase>(this, e));
        //}
    }

    public abstract class CommandBase
    {
        /// <summary>
        /// Название функции
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Описание функции
        /// </summary>
        public string Desctiprion { get; init; } = string.Empty;

        public CommandBase(string name = "")
        {
            Name = name != "" ? name : this.GetType().Name.Replace("Command", string.Empty);
        }

        public abstract void Run(Message message);

        public abstract CommandOptionsBase Options { get; }

        public static string[]? ExtractArgs(Message msg)
        {
            return msg.message.Split(' ').Skip(1).ToArray();
        }

        public static void DisplayHelp<T>(ParserResult<T> result)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AddDashesToOption = false;
                h.AdditionalNewLineAfterOption = false;
                h.Heading = "Myapp 2.0.0-beta"; //change header
                h.Copyright = "Copyright (c) 2019 Global.com"; //change copyright text
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            string str = helpText;
            Console.WriteLine(helpText);
        }
    }
}
