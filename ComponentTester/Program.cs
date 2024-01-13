
using System.Text.RegularExpressions;

public class ComponentTester
{
    public static void Main(string[] args)
    {
        string r = "888.999   55.9";
        Console.WriteLine("gg");
        List<string> list = new List<string>(FileParse.findAllStatisticsInFile("C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\text.txt",true));
        Match match = Proofread.regex(list[5], @"\d+\.\d+");
        MatchCollection matches = Proofread.regexes(r, @"\d+\.\d+");
        if (match.Success)
        {
            Console.WriteLine($"{match.Value} was extracted from {list[5]}");
        }
        else
        {
            Console.WriteLine("g");
        }
        foreach(Match g in matches)
        {
            Console.WriteLine($"{g.Value} was extracted from {r})");
        }
    }
}
