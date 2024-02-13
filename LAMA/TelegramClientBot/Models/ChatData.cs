using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramClientBot;
using WTelegram;


namespace TL
{
    public class Dialog
    {
        public string Directory
        {
            get { return $"{Environment.CurrentDirectory}\\Dialogs\\{Id}"; }
        }

        public long Id { get; }

        /// <summary>
        /// Настройки пользователя
        /// </summary>
        public UserPreferences Preferences { get; } = new();


        public Dialog(long id)
        {
            Id = id;
        }

        private string GetDateTimeLocale(DateTime dateTime = new DateTime())
        {
            return dateTime.ToString("yyyy.MM");
        }

        public void AddMessages(TL.MessageBase[] messages)
        {
            FileStream fileStream = new FileStream($"{Directory}\\{Id}-{GetDateTimeLocale()}.dat", FileMode.OpenOrCreate);
            using (var writer = new BinaryWriter(fileStream, Encoding.UTF8))
            {
                writer.BaseStream.Position = writer.BaseStream.Length;
                
                foreach (DialogMessage message in messages)
                {
                    writer.Write(message.ID);
                    writer.Write(message.Date.ToString());
                    writer.Write(message.ToString());
                }
            }
        }

        /*public DialogMessage FindMessages(DateTime dateTime, string dateFormat = "yyyy.MM")
        {
            var files = new DirectoryInfo($"{Directory}").GetFiles();
            var filteredFiles = from file in files
                                where file.Extension.ToLower() == ".dat"
                                select file;

            foreach (var file in filteredFiles)
            {

                FileStream fs = new FileStream(file.FullName, FileMode.Open);
                using (var reader = new BinaryReader(fs, Encoding.UTF8))
                {
                    while (reader.PeekChar() > -1)
                    {
                        int Id = reader.ReadInt32();
                        DateTime date = DateTime.Parse(reader.ReadString());
                        string message = reader.ReadString();

                        if (date.ToString(dateFormat) == dateTime.ToString(dateFormat))
                        {
                            return DialogMessage
                        }
                    }
                }
            }

            return null;
        }*/

        private static byte[] GetBytes(string text)
        {
            byte[] bytes = new byte[text.Length * sizeof(char)];
            System.Buffer.BlockCopy(text.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }

    public class DialogMessage : MessageBase
    {
        public DialogMessage() : base()
        {

        }
    }



    public class ChatData
    {
        public string Path
        {
            get { return $"{Environment.CurrentDirectory}\\Archive\\{ID}"; }
        }

        public long ID { get; }


        public Dictionary<int, MessageBase> History { get; set; } = new();

        private bool IsSaved { get; set; }

        public UserPreferences Preferences { get; set; } = new UserPreferences();


        public ChatData(long id)
        {
            ID = id;

            LoadMessages();
        }

        public void LoadMessages()
        {
            // Проверим директорию в которой хранятся наши файлы.
            DirectoryInfo folder = new DirectoryInfo(Path);
            if (!folder.Exists) folder.Create();

            return;
        }

        public void SaveMessages()
        {
            WriteToJsonFile();
        }

        private void WriteToJsonFile(bool append = false)
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(this);
                writer = new StreamWriter(Path, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        
    }

    public class UserPreferences
    {
        public bool CanUseCommands { get; set; } = true;

        public UserPreferences() { }
    }
}
