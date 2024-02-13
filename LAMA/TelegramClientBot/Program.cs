using TelegramClientBot.Models;

namespace TelegramClientBot // Note: actual namespace depends on the project name.
{
    public static class Program
    {
        public static ProgramModel ProgramModel { get; private set; }

        static async Task Main(string[] _)
        {
            try
            {
                using (ProgramModel = new ProgramModel())
                {
                    while (!ProgramModel.IsDisposed) { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}