using PFDB.PythonExecution;
using PFDB.PythonFactory;
using PFDB.WeaponUtility;
using PFDB.Parsing;
using System.Collections.Immutable;
using PFDB.ParsingUtility;
using PFDB.Logging;
using Microsoft.Extensions.Configuration;

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
			{14, new List<int>(){0,1,2 } }
		}, new Dictionary<PhantomForcesVersion, string>() { { new PhantomForcesVersion("9.0.2"), "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ImageParserForAPI\\version902" } }, 
		"C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\PyExec\\bin\\Debug\\net8.0", OutputDestination.File, null
		);
		//PythonExecutionFactoryOutput<PythonTesseractExecutable> pythonExecutionFactoryOutput = factory2.Start();
		
		
		
		IFileParse parse = new FileParse(new PhantomForcesVersion("10.1.0"));
		parse.FileReader("C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ComponentTester\\bin\\Debug\\net8.0\\PFDB_outputs\\1010\\0_1.png.pfdb");
		IDictionary<SearchTargets, string> valuePairs = parse.FindAllStatisticsInFileWithTypes(WeaponType.Primary);
		ImmutableSortedDictionary<SearchTargets, string> r = valuePairs.ToImmutableSortedDictionary();
		foreach(SearchTargets p in r.Keys)
		{
			//Console.WriteLine(r[p]);
			
		}


		PFDBLogger.LogInformation("Application has finished execution.");
		return;
	}
}
