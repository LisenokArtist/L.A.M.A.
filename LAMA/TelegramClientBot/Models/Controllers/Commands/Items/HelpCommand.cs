using CommandLine;
using CommandLine.Text;
using TL;

namespace TelegramClientBot.Models.Controllers.Commands.Items
{
    public sealed class HelpOptions : CommandOptionsBase
    {
        [Option('i', "info", Required = false, HelpText = "Отображает описание для указанной команды")]
        public string CommandName { get; set; }
    }

    public class HelpCommand : CommandBase
    {
        public override CommandOptionsBase Options { get { return new HelpOptions(); } }

        public HelpCommand() : base()
        {
            //var a = CommandLine.Text.HelpText.AutoBuild()
            //a.Settings = Parser.Default.ParseArguments<HelpOptions>().Value;
        }

        public override void Run(Message message)
        {
            var args = CommandBase.ExtractArgs(message);

            Parser.Default.ParseArguments<HelpOptions>(args)
                .WithParsed<HelpOptions>(HelpOptionsParsed)
                .WithNotParsed(HandleParseError);
        }

        private void HelpOptionsParsed(HelpOptions options)
        {
            if (string.IsNullOrEmpty(options.CommandName))
            {

            }
            else
            {

            }
        }

        private void HandleParseError(IEnumerable<CommandLine.Error> enumerable)
        {
            //throw new NotImplementedException();
        }


    }
}
