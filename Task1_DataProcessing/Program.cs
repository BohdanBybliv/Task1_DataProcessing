namespace Task1_DataProcessing
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using (Watcher watcher = new Watcher())
            {
                watcher.Start();
                Console.WriteLine("Press 'r' to reset, 's' to stop and save log.");
                string? input = Console.ReadLine();
                if (input == "r") watcher.Reset();
                else if (input == "s") watcher.Stop();
                Console.ReadKey();
            }
        }
    }
}