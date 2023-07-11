namespace DateDumperApp
{
    internal interface ICsvService
    {
        void Add(string csvPath, string data);
    }
    internal class CsvService : ICsvService
    {
        public void Add(string csvPath, string data)
        {
            File.WriteAllText(csvPath, data);
        }
    }
}
