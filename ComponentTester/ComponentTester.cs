
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using WeaponClasses;
using System.Data.SQLite;
using PFDB.PythonExecutor;
using PFDB.pinvoke;

public class ComponentTester
{
	public static void Main(string[] args)
	{
		PyTesseractExecutable executable = new PyTesseractExecutable("0_0.png", "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ImageParserForAPI\\version1010", WeaponType.Weapon.Primary, "1010", "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ComponentTester\\bin\\Debug\\net8.0", null);
		PythonExecutor uu = new PythonExecutor();
        IOutput out1 = uu.Execute(executable);
		if(out1 is Benchmark t)
		{
			Console.WriteLine(t.OutputString);
		}

        string r = "888.999   55.9";
		Console.WriteLine("gg");
		List<string> list2 = new List<string>(PFDB.fileparse.FileParse.findAllStatisticsInFile("C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\text.txt",true));
		Match match = Proofread.regex(list2[5], @"\d+\.\d+");
		MatchCollection matches = Proofread.regexes(r, @"\d+\.\d+");
		
		if (match.Success)
		{
			Console.WriteLine($"{match.Value} was extracted from {list2[5]}");
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
