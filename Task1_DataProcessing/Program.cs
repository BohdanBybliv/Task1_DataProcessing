namespace Task1_DataProcessing
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Watcher watcher = new Watcher();
            
            await watcher.Start();
        }
    }
}