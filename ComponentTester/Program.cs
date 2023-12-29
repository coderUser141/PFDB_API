
using System.Text.RegularExpressions;

public class ComponentTester
{
    public static void Main(string[] args)
    {
        
        Console.WriteLine("gg");
        List<string> list = new List<string>(FileParse.findAllStatisticsInFile("C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\text.txt",true));
        Match match = Proofread.regex(list[4], @"\d+\.+\d+");
        if (match.Success)
        {
            Console.WriteLine($"{match.Value} was extracted from {list[5]}");
        }
        else
        {
            Console.WriteLine("g");
        }
    }
}
