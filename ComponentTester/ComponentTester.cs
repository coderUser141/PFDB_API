using PFDB.PythonExecution;
using PFDB.PythonFactory;
using PFDB.WeaponUtility;
using PFDB;

public class ComponentTester
{
	public static void Main(string[] args)
	{
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


		IPythonExecutionFactory factory2 = new PythonExecutionFactory<PythonTesseractExecutable>(new Dictionary<int, List<int>>()
		{
			{0, new List<int>(){1,2,3,4,5,6,7 } },
			{1, new List<int>(){3,4} }, 
			{2, new List<int>(){2,3} }, 
			{3, new List<int>(){1} }
		}, new Dictionary<PhantomForcesVersion, string>() { { new PhantomForcesVersion("10.1.0"), "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ImageParserForAPI\\version1010" } }, 
		"C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\PyExec\\bin\\Debug\\net8.0", OutputDestination.Console | OutputDestination.File, null
		);
		//factory2.Start();


	}
}
