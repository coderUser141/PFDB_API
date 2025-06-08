using PFDB.PythonExecution;
using PFDB.PythonFactory;
using PFDB.WeaponUtility;
using PFDB.Logging;
using Serilog;
using PFDB.PythonExecutionUtility;
using PFDB.WeaponStructure;
using PFDB.SQLite;
using System.Collections.Immutable;
using PFDB.ParsingUtility;
using PFDB.PythonTesting;
using PFDB.PythonFactoryUtility;
using System.ComponentModel.Design;

public class ComponentTester
{

	/*
	 *	test -> test capabilities of various components
	 *			(supported: PyExec, FileParse)
	 *			(planned: SQLiteHandler)
	 *			
	 *	build -> builds specific versions
	 *			(supported: existing versions)
	 *			(plan to build all)
	 *	
	 *	inventory -> shows inventory
	 *			(planned: images, text)
	 *	
	 *	
	 *	
	 */

	public enum Operations
	{
		Help = 0,
		Test = 1,
		Build = 2,
		Inventory = 3,
		ManualProofread = 4


	}

	public static void displayHelp()
	{
		//ConsoleColor initial = Console.BackgroundColor;
		//Console.BackgroundColor = ConsoleColor.DarkRed;
		Console.WriteLine("help message");
		//Console.BackgroundColor = initial;
	}

	public static void Main(string[] args)
	{

		Operations operation = Operations.Help;

		if (args.Length == 0)
		{
			displayHelp();
			return;
		}

		Console.WriteLine(args[0]);


		switch (args[0].ToLowerInvariant())
		{
			case "--help":
			case "help":
			{
				operation = Operations.Help;
				displayHelp();
				break;
			}
			case "test":
			{
				operation = Operations.Test;
				break;
			}
			case "build":
			{
				operation = Operations.Build;
				break;
			}
			case "inventory":
			{
				operation = Operations.Inventory;
				break;
			}
			case "proofread":
			{
				operation = Operations.ManualProofread;
				break;
			}
			default:
			{
				operation = Operations.Help;
				break;
			}
		}

		Console.WriteLine(operation);

		PFDBLogger logger = new PFDBLogger(".pfdblog");

		switch (operation) {
			case Operations.Help:
				{
					displayHelp();
					break;
				}
			case Operations.Test:
				{
					break;
				}
			case Operations.Build:
				{
					break;
				}
			case Operations.Inventory:
				{
					break;
				}
			case Operations.ManualProofread:
				{
					break;
				}

		}
		
		/*
		if(test){
			int score = 0;
			if(ParseTesting.Test())score++;

				//pythonProgramPath = args[1]
				//imageBasePath = args[2]

			if (PythonTest.Test(args[1], args[2], null)) score++;
			if(score >= 2){
				Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}");
				PythonTest.TestingOutput("All tests", score >= 2, "2", 2.ToString());
				PFDBLogger.LogInformation("Tests have passed successfully!");
				Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}");
			}
		}

		if(build){
			Console.WriteLine("building");
			//buildAllVersions()
			if(args.Length == 3){
				//full build

				//pythonProgramPath = args[1]
				//imageBasePath = args[2]
				
				buildAllVersions(args[2], args[1], null);
			}else if(args.Length == 4){
				//specific version build 

				//pythonProgramPath = args[1]
				//imageBasePath = args[2]
				
				buildSpecificVersion(args[2], args[1], null, new PhantomForcesVersion(args[3]));
			}else if(args.Length > 4){

			}
			
		}
		*/
		Log.Logger.Information("Application end. Logging has finished.");
		return;
	}

	public static bool Test()
	{
		return true;
	}

	public static void inventory()
	{
		//list all weapons from database
		//and sees if they exist
	}


	public static void buildAllVersions(string imageBasePath, string pythonProgramPath, string? tessbinPath)
	{
		if (Directory.Exists(imageBasePath) == false)
		{
			PFDBLogger.LogError($"Directory path was not found: {imageBasePath}");
			return;
			//throw new DirectoryNotFoundException($"Directory path was not found: {imageBasePath}");
		}
		IDictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>> list = new Dictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>>();
		IDictionary<PhantomForcesVersion, string> versionAndPathPairs = new Dictionary<PhantomForcesVersion, string>();

		foreach (PhantomForcesVersion version in WeaponTable.ListOfVersions)
		{
			IDictionary<Categories, int> weaponCounts = WeaponTable.WeaponCounts[version]; //maximum number of weapons in the category
			Dictionary<Categories, List<int>> weaponNumbers = new Dictionary<Categories, List<int>>();
			foreach (Categories category in weaponCounts.Keys)
			{
				List<int> tempList = new List<int>();
				for (int i = 0; i < weaponCounts[category]; ++i)
				{
					tempList.Add(i);
				}
				weaponNumbers.Add(category, tempList);
			}
			list.Add(version, weaponNumbers);
			versionAndPathPairs.Add(version, $"{imageBasePath}{PyUtilityClass.slash}version{version.VersionNumber}{PyUtilityClass.slash}");
		}
		PythonExecutionFactory<PythonTesseractExecutable> factory = new PythonExecutionFactory<PythonTesseractExecutable>(list, versionAndPathPairs, pythonProgramPath, OutputDestination.Console | OutputDestination.File, tessbinPath);
		IPythonExecutionFactoryOutput factoryOutput = factory.Start();
		PFDBLogger.LogWarning("The following files are missing:");
		foreach (string str in factoryOutput.MissingFiles)
		{
			Console.WriteLine(str);
		}

	}

	public static void buildSpecificVersion(string imageBasePath, string pythonProgramPath, string? tessbinPath, PhantomForcesVersion version){
		//verify path
		//string sourcePath = "/mnt/bulkdata/Programming/PFDB/PFDB_API/textOutputsByVersion/version1001";
		//PhantomForcesVersion version = new PhantomForcesVersion(10,0,1);
		

		if(Directory.Exists(imageBasePath) == false){
			PFDBLogger.LogError($"Directory path was not found: {imageBasePath}");
			return;
			//throw new DirectoryNotFoundException($"Directory path was not found: {imageBasePath}");
		}

		
		IDictionary<Categories, int> weaponCounts = WeaponTable.WeaponCounts[version]; //maximum number of weapons in the category
		Dictionary<Categories, List<int>> weaponNumbers = new Dictionary<Categories, List<int>>();
		foreach(Categories category in weaponCounts.Keys){
			List<int> tempList = new List<int>();
			for(int i = 0; i < weaponCounts[category]; ++i){
				tempList.Add(i);
			}
			weaponNumbers.Add(category, tempList);
		}
		IDictionary<PhantomForcesVersion, string> versionAndPathPairs = new Dictionary<PhantomForcesVersion, string>
		{
			{ version, $"{imageBasePath}{PyUtilityClass.slash}version{version.VersionNumber}{PyUtilityClass.slash}" }
		};
		PythonExecutionFactory<PythonTesseractExecutable> factory = 
		new PythonExecutionFactory<PythonTesseractExecutable>(new Dictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>>{{version, weaponNumbers}}, versionAndPathPairs, pythonProgramPath, OutputDestination.Console | OutputDestination.File, tessbinPath);
		IPythonExecutionFactoryOutput factoryOutput = factory.Start();
		PFDBLogger.LogWarning("The following files are missing:");
		foreach(string str in factoryOutput.MissingFiles){
			Console.WriteLine(str);
		}


		//verify number of images
		
	}






}


