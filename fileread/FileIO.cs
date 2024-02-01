// See https://aka.ms/new-console-template for more information
using System.IO;
using WeaponClasses;
using System;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine("Hello, World!");
PFDB.fileio.FileIO g = new("802"); //zmtsqf
DateTime st = DateTime.Now;
//g.convertLegacyTextFilesToCurrentTextFiles();
List<List<string>> f = g.readAllFiles();
DateTime en = DateTime.Now;
Console.WriteLine((en - st).TotalMilliseconds / (double)1000);
//g.setAllTextFilenames(WS.versionStrings[g.Version]);
Console.Read();

namespace PFDB
{
	namespace fileio
	{
		/// <summary>
		/// This class defines the input and output handling for files
		/// </summary>
		public class FileIO
		{
			private string version;
			public string Version
			{
				get { return version; }
				set
				{
					int temp;
					try
					{
						temp = Convert.ToInt32(value);
						version = temp.ToString();
					}
					catch
					{
						//do nothing
						version = "0";
					}
				}
			}
			private readonly Dictionary<string, List<List<string>>> legacyWeaponImageFilenames1;
			private readonly Dictionary<string, List<List<string>>> legacyWeaponImageFilenames2;
			//i've removed the following line, because using it can potentially lead to fileparse.FileReader or fileparse.FileVerifier to throw a FileNotFoundException (not good)
			//private Dictionary<string, List<List<string>>> weaponImageFilenames;
			private readonly Dictionary<string, List<List<string>>> numericalWeaponImageFilenames;
			private readonly string directory;

			public FileIO(string version)
			{
				if (WS.versionStrings.ContainsKey(version))
				{
					this.Version = version;
					this.directory = $"output{Version}";
				}
				else
				{
					throw new ArgumentException("The Version you have provided either has not been implemented, or it has never existed in Phantom Forces");
				}
				legacyWeaponImageFilenames1 = new Dictionary<string, List<List<string>>>();
				legacyWeaponImageFilenames2 = new Dictionary<string, List<List<string>>>();
				//i've removed the following line, because using it can potentially lead to fileparse.FileReader or fileparse.FileVerifier to throw a FileNotFoundException (not good)
				//weaponImageFilenames = new Dictionary<string, List<List<string>>>();
				numericalWeaponImageFilenames = new Dictionary<string, List<List<string>>>();
			}

			private bool isLegacy()
			{
				return !(Convert.ToInt32(Version) > 804);
			}


			public void getImageFileNames(bool uppercase)
			{
				int sizeI = WS.versionStrings[Version].Count;
				int sizeJ = 0;
				List<List<string>> tempCategoryStrings2 = new List<List<string>>();
				List<string> tempWeaponStrings2 = new List<string>();

				List<List<string>> tempCategoryStrings1 = new List<List<string>>();
				List<string> tempWeaponStrings1 = new List<string>();


				for (int indexI = 0; indexI < sizeI; ++indexI)
				{
					//clear so we don't duplicate anything
					tempWeaponStrings1.Clear();
					tempWeaponStrings2.Clear();
					sizeJ = WS.versionStrings[Version][indexI].Count;

					for (int indexJ = 0; indexJ < sizeJ; ++indexJ)
					{
						if (!isLegacy())
						{
							tempWeaponStrings1.Add($"{indexI}_{indexJ}.png");
							//tempCategoryStrings2.Add()
						}
						else
						{
							string w = WS.versionStrings[Version][indexI][indexJ]; //all of these are by default lowercase
							if (uppercase) w = w.ToUpper();

							tempWeaponStrings1.Add($"{w}1.png");
							tempWeaponStrings2.Add($"{w}2.png");
						}
					}
					//because tempWeaponStrings1 is passed by reference, we need to copy to keep the data there when we loop around and clear the list
					tempCategoryStrings1.Add(tempWeaponStrings1.ToList());
					tempCategoryStrings2.Add(tempWeaponStrings2.ToList());

				}

				if (!isLegacy())
				{
					numericalWeaponImageFilenames.Add(Version, tempCategoryStrings1);
				}
				else
				{
					legacyWeaponImageFilenames1.Add(Version, tempCategoryStrings1);
					legacyWeaponImageFilenames2.Add(Version, tempCategoryStrings2);
				}
			}

			public void writeOneFile(string filename, string content)
			{
				try
				{
					File.WriteAllText($"{directory}\\{filename}", content);
				}
				catch
				{
					Console.WriteLine($"Writing to {directory}\\{filename} failed.");
				}
			}

			public static void writeOneFile(string directory, string filename, string content)
			{
				try
				{
					File.WriteAllText($"{directory}\\{filename}", content);
				}
				catch
				{
					Console.WriteLine($"Writing to {directory}\\{filename} failed.");
				}
			}

			public void writeAllFiles(List<List<string>> weaponoutputs)
			{
				Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff"));
				Directory.CreateDirectory(directory);

				int sizeI = weaponoutputs.Count;
				if (sizeI != WS.versionStrings[Version].Count) throw new ArgumentException(
					$"Incorrect size matching for outer List. Expected: {WS.versionStrings[Version].Count}, Actual: {sizeI}");


				for (int indexI = 0; indexI < sizeI; indexI++)
				{
					int sizeJ = weaponoutputs[indexI].Count;
					if (sizeJ != WS.versionStrings[Version][indexI].Count) throw new ArgumentException(
						$"Incorrect size matching for outer List. Expected: {WS.versionStrings[Version][indexI].Count}, Actual: {sizeJ}");
					for (int indexJ = 0; indexJ < sizeJ; indexJ++)
					{
						writeOneFile(directory, $"{Version}_{indexI}_{indexJ}.txt", $"{weaponoutputs[indexI][indexJ]}");
					}
				}

				Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff"));
			}

			public static string readOneTextFile(string directory, string filename)
			{
				if (!File.Exists($"{directory}\\{filename}")) throw new FileNotFoundException($"{directory}\\{filename} was not found;");
				return File.ReadAllText($"{directory}\\{filename}");
			}

			public string readOneTextFile(string filename)
			{
				if (!File.Exists($"{directory}\\{filename}")) throw new FileNotFoundException($"{directory}\\{filename} was not found;");
				return File.ReadAllText($"{directory}\\{filename}");
			}

			public void convertLegacyTextFilesToCurrentTextFiles()
			{
				List<List<string>> tempCategoryStrings = new List<List<string>>();
				List<string> tempWeaponStrings = new List<string>();


				int sizeI = WS.versionStrings[Version].Count;
				for (int indexI = 0; indexI < sizeI; indexI++)
				{
					int sizeJ = WS.versionStrings[Version][indexI].Count;
					for (int indexJ = 0; indexJ < sizeJ; indexJ++)
					{
						try
						{
							string first = readOneTextFile($"output__{WS.versionStrings[Version][indexI][indexJ].ToUpper()}1.png.txt");
							string second = readOneTextFile($"output__{WS.versionStrings[Version][indexI][indexJ].ToUpper()}2.png.txt");
							writeOneFile($"{indexI}_{indexJ}.txt", $"{first}{Environment.NewLine}{second}");
							//Console.WriteLine($"SUCCESS: output__{WS.versionStrings[Version][indexI][indexJ].ToUpper()}1.png.txt");
							//Console.WriteLine($"SUCCESS: output__{WS.versionStrings[Version][indexI][indexJ].ToUpper()}2.png.txt");
						}
						catch
						{
							//Console.WriteLine($"FAIL: output__{WS.versionStrings[Version][indexI][indexJ].ToUpper()}1.png.txt");
							//Console.WriteLine($"FAIL: output__{WS.versionStrings[Version][indexI][indexJ].ToUpper()}2.png.txt");
							try
							{
								string content = readOneTextFile($"output__{WS.versionStrings[Version][indexI][indexJ].ToUpper()}.png.txt");
								writeOneFile($"{indexI}_{indexJ}.txt", content);
								//Console.WriteLine($"SUCCESS: output__{WS.versionStrings[Version][indexI][indexJ].ToUpper()}.png.txt");
							}
							catch
							{
								//Console.WriteLine($"FAIL: output__{WS.versionStrings[Version][indexI][indexJ].ToUpper()}.png.txt");
							}
							//do nothing
						}
					}
				}
			}

			public List<List<string>> readAllFiles()
			{
				if (isLegacy()) convertLegacyTextFilesToCurrentTextFiles();

				List<List<string>> tempCategoryContents = new List<List<string>>();
				List<string> tempWeaponContents = new List<string>();

				int sizeI = WS.versionStrings[Version].Count;
				for (int indexI = 0; indexI < sizeI; indexI++)
				{
					int sizeJ = WS.versionStrings[Version][indexI].Count;
					for (int indexJ = 0; indexJ < sizeJ; indexJ++)
					{
						try
						{
							tempWeaponContents.Add(readOneTextFile($"{indexI}_{indexJ}.txt"));
						}
						catch
						{
							Console.WriteLine($"{indexI}_{indexJ}.txt failed.");
						}
					}
					tempCategoryContents.Add(tempWeaponContents.ToList());
					tempWeaponContents.Clear();
				}
				return tempCategoryContents;
			}


		}
	}
}