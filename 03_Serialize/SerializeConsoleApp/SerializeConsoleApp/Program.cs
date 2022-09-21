using System.Xml.Serialization;

namespace Serializer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serUtil = new SerializeUtil();

            List<int> numbers = GenerateRandomNums();
            Console.WriteLine(new String('-', 30) 
                + "\nList is Generated! - " + string.Join(',', numbers) + '\n' 
                + new String('-', 30));
            serUtil.Serialize(numbers, @"C:\Users\pi623\Desktop\ser.xml");
        }
        private static List<int> GenerateRandomNums()
        {
            List<int> nums = new();
            Random random = new Random();
            int size = random.Next(1, 10);
            for (int idx = 0; idx < size; idx++)
            {
                nums.Add(random.Next(1, 100));
            }
            size = default(int);
            return nums;
        }
    }
    class SerializeUtil
    {
        public bool Logging { get; set; }
        public SerializeUtil()
        {
            this.Logging = true;
        }
        public void Serialize<T>(T target, string filename)
        {
            var ser = new XmlSerializer(typeof(T));

            using (var sw = new StreamWriter(filename))
            {
                ser.Serialize(sw, target);
                if (IsDuplicated(filename))
                {
                    Console.Write("this file name is duplicated, do you want to overwrite? ");
                    string? ans = Console.ReadLine();
                    if (string.IsNullOrEmpty(ans) || ans.StartsWith("n", StringComparison.OrdinalIgnoreCase))
                        return;
                }
                if (Logging)
                {
                    LogWithTime($"Complete to serialize '{typeof(T).Name} - {filename.Split('\\').Last()}'");
                }
            }
            bool IsDuplicated(string filename)
            {
                string[] files = Directory.GetFiles(string.Join('\\', filename.Split('\\')[0..^1]));
                if (files.Any(o => o.Equals(filename)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public void Deserialize<T>(string filename, out T? target)
        {
            target = default;
            var ser = new XmlSerializer(typeof(object));
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                target = (T)ser.Deserialize(fs);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            if (Logging)
            {
                if (target == null)
                {
                    LogWithTime($"File is null '{typeof(T).Name} - {nameof(target)}'");
                }
                else
                {
                    LogWithTime($"Complete to Deserialize '{typeof(T).Name} - {nameof(target)}'");
                }
            }
        }
        private void LogWithTime(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss:fff")}] {message}");
        }
    }
}