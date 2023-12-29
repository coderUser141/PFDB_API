
namespace temprenamer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            for (int i = 15; i >= 12; --i)
            {
                for(int j = 0; j < 50; ++j)
                {
                    Console.WriteLine($"{i}_{j}.png");
                    if (File.Exists($"{i}_{j}.png"))
                    {
                        Console.WriteLine($"{i}_{j}.png found");
                        File.Move($"{i}_{j}.png", $"{(i + 3)}_{j}.png");
                    }
                    else
                    {
                        Console.WriteLine($"{i}_{j}.png not found, skipping");
                        break;
                    }
                }
            }
        }
    }
}