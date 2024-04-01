using PFDB.PythonExecution;
using PFDB.PythonFactory;
using PFDB.WeaponUtility;
using PFDB.Parsing;
using System.Collections.Immutable;
using PFDB.ParsingUtility;
using PFDB.Logging;
using Microsoft.Extensions.Configuration;
using PFDB.Proofreading;

public class ComponentTester
{
	public static void Main(string[] args)
	{

		//PFDBLogger.Setup();

		PFDBLogger logger = new PFDBLogger(".pfdblog");

		PFDBLogger.LogInformation("lalalal", parameter: ["lala"]);
		//logger.ConfigurationRoot.GetValue<string>()

		//Log.Information("peepeepoopoo man");

		/*
		List<IPythonExecutor> list = new List<IPythonExecutor>();
		//get number of weapons from database
		for(int i = 0; i < 19; i++)
		{
			for(int j = 0; j < 4; j++)
			{

				IPythonExecutor temp = new PythonExecutor(OutputDestination.Console | OutputDestination.File)
					.LoadOut(new PythonTesseractExecutable().Construct($"{i}_{j}.png", "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ImageParserForAPI\\version1010", new PhantomForcesVersion("10.1.0"), WeaponUtilityClass.GetWeaponType(i), "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\PyExec\\bin\\Debug\\net8.0", null));

				
				list.Add(temp);
			}
		}


		IPythonExecutionFactory factory = new PythonExecutionFactory<PythonTesseractExecutable>(list);
		//factory.Start();

		*/


		System.Environment.SetEnvironmentVariable("pythonSignalText", "smokin' joe rudeboy", EnvironmentVariableTarget.User);
		
		PythonExecutionFactory<PythonTesseractExecutable> factory2 = new PythonExecutionFactory<PythonTesseractExecutable>(new Dictionary<int, List<int>>()
		{
			{8, new List<int>(){0,1,2,3,4,5,6,7,8,9,10,11,12,13,14 } },
			{9, new List<int>(){0,1,2,3,4,5} },
			{10, new List<int>(){0,1,2,3,4,5} }
		}, new Dictionary<PhantomForcesVersion, string>() { { new PhantomForcesVersion("10.1.0"), "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ImageParserForAPI\\version1010" } }, 
		"C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\PyExec\\bin\\Debug\\net8.0", OutputDestination.File, null
		);
		PythonExecutionFactoryOutput<PythonTesseractExecutable> pythonExecutionFactoryOutput = factory2.Start();
		
		
		
		IFileParse parse = new FileParse(new PhantomForcesVersion("10.1.0"));
		parse.FileReader("C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ComponentTester\\bin\\Debug\\net8.0\\PFDB_outputs\\1010\\0_1.png.pfdb");
		IDictionary<SearchTargets, string> valuePairs = parse.FindAllStatisticsInFileWithTypes(WeaponType.Primary);
		ImmutableSortedDictionary<SearchTargets, string> r = valuePairs.ToImmutableSortedDictionary();
		foreach(SearchTargets p in r.Keys)
		{
			Console.WriteLine(r[p]);
		}

		StatisticProofread proofread = new StatisticProofread(new PhantomForcesVersion("10.1.0"));
		proofread.ApplyRegularExpression(PFDB.StatisticUtility.StatisticOptions.MagazineCapacity, r[SearchTargets.AmmoCapacity]);


		PFDBLogger.LogInformation("Application has finished execution.");
		return;
	}
}
