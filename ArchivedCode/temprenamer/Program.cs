
namespace temprenamer
{
    internal class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine(Environment.CurrentDirectory);
			string path = Environment.CurrentDirectory;
			for (int i = 0; i < 20; ++i)
            {
                for(int j = 0; j < 50; ++j)
                {
					string filename1 = $"{i}_{j}_1.txt";
					string filename2 = $"{i}_{j}_2.txt";
					Console.WriteLine(filename1);
					Console.WriteLine(filename2);
					if (File.Exists($"{path}\\{filename1}") && File.Exists($"{path}\\{filename2}"))
                    {
                        Console.WriteLine($"{filename1} and {filename2} found");
						string file1 = File.ReadAllText(filename1);
						string file2 = File.ReadAllText(filename2);
						file1 = file1.Replace("peter", "Aethelhelm");
						file2 = file2.Replace("peter", "Aethelhelm");
						file1 = $"{file1} {Environment.NewLine} {file2}";
						File.WriteAllText($"{path}\\{i}_{j}.png.pfdb", file1);
					}
					else if(File.Exists(filename1) == false)
					{
						Console.WriteLine($"{filename1} not found");
					}else if(File.Exists(filename2) == false)
					{
						Console.WriteLine($"{filename2} not found");
					}
					else
					{
						Console.WriteLine("none were found");
					}
                }
            }
        }
    }
}