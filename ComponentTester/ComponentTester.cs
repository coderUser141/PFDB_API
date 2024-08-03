using PFDB.PythonExecution;
using PFDB.PythonFactory;
using PFDB.WeaponUtility;
using PFDB.Logging;
using PFDB.PythonExecutionUtility;
using PFDB.WeaponStructure;
using PFDB.SQLite;
using System.Collections.Immutable;

public class ComponentTester
{
	public static void Main(string[] args)
	{

		PFDBLogger logger = new PFDBLogger(".pfdblog");
		//WeaponTable.InitializeEverything();
		PFDBLogger.LogInformation("lalalal", parameter: ["lala"]);

		//IPythonExecutable<IOutput> pythonExecutable = new PythonTesseractExecutable<Benchmark>();

		//System.Environment.SetEnvironmentVariable("pythonSignalText", "smokin' joe rudeboy", EnvironmentVariableTarget.User);

		PhantomForcesVersion version902 = new PhantomForcesVersion("9.0.2");
		
		IPhantomForcesDataModel model = new PhantomForcesDataModel(PhantomForcesDataModel.GetWeaponCollection(version902));

		//PythonExecutionFactory<PythonTesseractExecutable> factory2 = new PythonExecutionFactory<PythonTesseractExecutable>(weaponNumbers, new Dictionary<PhantomForcesVersion, string>() { { version902, "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ImageParserForAPI\\version902" } }, 
		//"C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\PyExec\\bin\\Debug\\net8.0", OutputDestination.File, null
		//);

		//factory2.Start();

		//IPhantomForcesDataModel phantomForcesDataModel = new PhantomForcesDataModel(PhantomForcesDataModel.GetWeaponCollection(new PhantomForcesVersion("10.0.1")));

		PFDBLogger.LogInformation("Application has finished execution.");
		return;
	}
}
