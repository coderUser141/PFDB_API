
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
		PFDB.pinvoke.PINVOKE.rankToExperienceDefault(3);
        Console.WriteLine(PFDB.pinvoke.PINVOKE.experienceToRank(PINVOKE.rankToExperienceDefault(213) + PINVOKE.rankToExperienceDefault(173)));


		
        /*
		foreach(List<string> t in listofstr2)
		{
			foreach(string s in t)
			{
				using(SQLiteConnection conn = new SQLiteConnection(@$"Data Source={workingDirectory}\weapon_database.db;Version=3;FailIfMissing=True;"))
				{
					using(SQLiteCommand command = conn.CreateCommand())
					{
						conn.Open();
						command.CommandText = "INSERT INTO version800 (weapon_name,category,category_number) VALUES ('" + s + "','',0); ";
						using(SQLiteDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								//do nothing because it's inserting
							}
						}
						conn.Close();
					}
				}
			}
		}
		/*
		foreach (string t in buildoutputdirectories)
		{
			for(int g = 0; g < listofstr.Count; ++g)
			{
				for (int h = 0; h < listofstr[g].Count; ++h) {
					try {
						File.Copy($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}1.png.txt", $"{workingDirectory}\\{t}\\{g}_{h}_1.txt");
						File.Copy($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}2.png.txt", $"{workingDirectory}\\{t}\\{g}_{h}_2.txt");
                        File.Move($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}1.png.txt", $"{workingDirectory}\\{t}\\{listofstr2[g][h]}_1.txt");
                        File.Move($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}2.png.txt", $"{workingDirectory}\\{t}\\{listofstr2[g][h]}_2.txt");

                    }
					catch
					{
						try
						{
							File.Copy($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}.png.txt", $"{workingDirectory}\\{t}\\{g}_{h}_0.txt");
                            File.Move($"{workingDirectory}\\{t}\\output__{listofstr[g][h]}.png.txt", $"{workingDirectory}\\{t}\\{listofstr2[g][h]}_0.txt");
                        }
						catch
						{
							Console.WriteLine("file not found");
						}
					}
				}
			}
		}*/
        // { "all build outputs v5", "all build outputs v5" };
        List<PyTesseractExecutor> list =
			[
			new PyTesseractExecutor(".", "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ImageParserForAPI\\version902", "902", true),
			new PyTesseractExecutor(".", "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ImageParserForAPI\\version903", "903", true),
			new PyTesseractExecutor(".", "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ImageParserForAPI\\version1001", "1001", true),
			new PyTesseractExecutor(".", "C:\\Users\\Aethelhelm\\source\\repos\\PFDB_API\\ImageParserForAPI\\version1010", "1010", true)
			];
        /*py -m PyInstaller -path=[C:\Users\Aethelhelm\AppData\Local\Programs\Python\Python312\Scripts] --onefile $(SolutionDir)/ImageParserForAPI/impa.py
                        xcopy $(SolutionDir)/ImageParserForAPI/build/impa.exe $(SolutionDir)/ImageParserForAPI \y
                        xcopy $(SolutionDir)/ImageParserForAPI/build/impa.exe $(ProjectDir)/ \y*/
        /*foreach (PyExec exec in list)
        {
            File.WriteAllText(
                $"0_0_{exec.version}.txt",
                exec.exec("0_0.png", 1, null).Item1
                );
            Console.WriteLine($"0_0_{exec.version}.txt");
            File.WriteAllText(
                $"0_1_{exec.version}.txt",
                exec.exec("0_1.png", 1, null).Item1
                );
            Console.WriteLine($"0_1_{exec.version}.txt");

            File.WriteAllText(
                $"7_6_{exec.version}.txt",
                exec.exec("7_6.png", 1, null).Item1
                );
            Console.WriteLine($"7_6_{exec.version}.txt");
            File.WriteAllText(
                $"8_25_{exec.version}.txt",
                exec.exec("8_25.png", 2, null).Item1
                );
            Console.WriteLine($"8_25_{exec.version}.txt");
            File.WriteAllText(
                $"14_0_{exec.version}.txt",
                exec.exec("14_0.png", 3, null).Item1
                );
            Console.WriteLine($"14_0_{exec.version}.txt");
            File.WriteAllText(
                $"17_15_{exec.version}.txt",
                exec.exec("17_15.png", 4, null).Item1
                );
            Console.WriteLine($"17_15_{exec.version}.txt");
        }*/

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
