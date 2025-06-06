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

public class ComponentTester
{

	public static void displayHelp(){
		Console.WriteLine("help message");
	}

	public static void Main(string[] args)
	{
		bool test = false;
		bool build = false;

		switch (args.Length){
			case 0:{
				displayHelp();
				break;
			}
			case 1:{
				string argument = args[0].Trim();
				if(argument.Contains('-') && argument.Contains("--") == false){
					//single dash commands: -b, -h, -t

					Console.WriteLine("single");
					if(args[0].Contains('h')){
						displayHelp();
						break;
					}

					


				}else if(argument.Contains("--")){
					//double dash commands: --help, --build, --test
					Console.WriteLine("double");
					if(args[0].Contains("help")){
						displayHelp();
						break;
					}
				}
				break;
			}
			case 2:
			{

				break;
			}
			case 3:{
				string argument = args[0];
				if(argument.Contains('-') && argument.Contains("--") == false){
					if(args[0].Contains('b')){
						Console.WriteLine("build");
						build = true;
					}
					if(args[0].Contains('t')){
						Console.WriteLine("test");
						test = true;
					}
				}else if(argument.Contains("--")){
					if(args[0].Contains("build")){
						Console.WriteLine("build");
						build = true;
					} else if(args[0].Contains("test")){
						Console.WriteLine("test");
						test = true;
					}
				}
				break;
			}
			case 4:{
				
				string argument = args[0];
				if(argument.Contains('-') && argument.Contains("--") == false){
					if(args[0].Contains('b')){
						Console.WriteLine("build");
						build = true;
					}
				}else if(argument.Contains("--")){

				}
				break;
			}

		}
		PFDBLogger logger = new PFDBLogger(".pfdblog");

		// nothing -> help
		// -h or --help ->  help
		// -t = test
		// -b = build

		// --tesseract = path to executable
		// --imageBasePath


		if (test || build){
			WeaponTable.InitializeEverything();
		}

		Console.WriteLine($"Has test been found? {test}");

		if(test){
			int score = 0;
			if(ParseTesting.Test())score++;
			/*
				pythonProgramPath = args[1]
				imageBasePath = args[2]
			*/
			if(PythonTest.Test(args[1],args[2], null))score++;
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
				/*
					pythonProgramPath = args[1]
					imageBasePath = args[2]
				*/
				buildAllVersions(args[2], args[1], null);
			}else if(args.Length == 4){
				//specific version build 
				/*
					pythonProgramPath = args[1]
					imageBasePath = args[2]

				*/
				buildSpecificVersion(args[2], args[1], null, new PhantomForcesVersion(args[3]));
			}else if(args.Length > 4){

			}
			
		}

		Log.Logger.Information("Application end. Logging has finished.");
		return;
	}

	public static void buildAllVersions(string imageBasePath, string pythonProgramPath, string? tessbinPath){
		if(Directory.Exists(imageBasePath) == false){
			PFDBLogger.LogError($"Directory path was not found: {imageBasePath}");
			return;
			//throw new DirectoryNotFoundException($"Directory path was not found: {imageBasePath}");
		}
		IDictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>> list = new Dictionary<PhantomForcesVersion, Dictionary<Categories, List<int>>>();
		IDictionary<PhantomForcesVersion, string> versionAndPathPairs = new Dictionary<PhantomForcesVersion, string>();

		foreach(PhantomForcesVersion version in WeaponTable.ListOfVersions){
			IDictionary<Categories, int> weaponCounts = WeaponTable.WeaponCounts[version]; //maximum number of weapons in the category
			Dictionary<Categories, List<int>> weaponNumbers = new Dictionary<Categories, List<int>>();
			foreach(Categories category in weaponCounts.Keys){
				List<int> tempList = new List<int>();
				for(int i = 0; i < weaponCounts[category]; ++i){
					tempList.Add(i);
				}
				weaponNumbers.Add(category, tempList);
			}
			list.Add(version,weaponNumbers);
			versionAndPathPairs.Add(version, $"{imageBasePath}{PyUtilityClass.slash}version{version.VersionNumber}{PyUtilityClass.slash}");
		}
		PythonExecutionFactory<PythonTesseractExecutable> factory = new PythonExecutionFactory<PythonTesseractExecutable>(list, versionAndPathPairs, pythonProgramPath, OutputDestination.Console | OutputDestination.File, tessbinPath);
		IPythonExecutionFactoryOutput factoryOutput = factory.Start();
		PFDBLogger.LogWarning("The following files are missing:");
		foreach(string str in factoryOutput.MissingFiles){
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


