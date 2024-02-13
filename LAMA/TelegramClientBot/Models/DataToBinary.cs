using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramClientBot.Models
{
    public class DataToBinary
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DataToBinary(int id, string text)
        {
            Id = id;
            Text = text;
        }

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

        public static void Save(DataToBinary[] datas, string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
            using (var writer = new BinaryWriter(fs, Encoding.UTF8))
            {
                writer.Write(datas.Length);  //Int32
                foreach (DataToBinary data in datas)
                {
                    byte[] idBytes = BitConverter.GetBytes(data.Id);
                    byte[] textBytes = GetBytes(data.Text);
                    int recordLength = idBytes.Length + textBytes.Length;
                    writer.Write(recordLength); //Размер записи
                    writer.Write(idBytes); //Id
                    writer.Write(textBytes.Length); //Размер записи текста
                    writer.Write(textBytes); //Text
                }
            }
        }

        public static DataToBinary[] Load(string fileName)
        {
            DataToBinary[] datas = null;
            FileStream fs = new FileStream(fileName, FileMode.Open);
            using (var reader = new BinaryReader(fs, Encoding.UTF8))
            {
                int arraySize = reader.ReadInt32();  //Int32
                datas = new DataToBinary[arraySize];

                for (int i = 0; i < arraySize; i++)
                {
                    int recordSize = reader.ReadInt32();
                    int id = reader.ReadInt32();
                    int sizeOfText = reader.ReadInt32();
                    string text = GetString(reader.ReadBytes(sizeOfText));

                    datas[i] = new DataToBinary(id, text);
                }
            }

            return datas;
        }
    }
}
