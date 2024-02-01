// See https://aka.ms/new-console-template for more information
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PFDB.PythonExecutor;

Console.WriteLine();

namespace PFDB
{
	namespace PythonExecutor
	{

		/// <summary>
		/// Python Execution Class
		/// <para>
		/// Note: To compile the python file into a working executable, follow these general steps:
		/// <list type="number">
		/// <item>Ensure Python 3.12 is downloaded</item>
		/// <item>Install 'numpy', 'pytesseract', and 'opencv-python' using pip install</item>
		/// <item>Install 'PyInstaller'</item>
		/// <item>Navigate to the folder containing the python file (impa.py) and run "py -m PyInstaller -path=[path to scripts file, i.e. C:\Users\(youruser)\AppData\Local\Programs\Python\Python312\Scripts] --onefile impa.py"</item>
		/// </list>
		/// </para>
		/// </summary>
		public class PyTesseractExecutor : IPythonExecutor
		{
			/// <summary>
			/// Directory where the Python executable resides
			/// </summary>
			public string programDirectory { get; init; }

			/// <summary>
			/// Directory where the images for reading reside
			/// </summary>
			public string fileDirectory { get; init; }
			/// <summary>
			/// Phantom Forces Version
			/// </summary>
			public string version { get; init; }
			private bool currentDirectoryHasTessbin { get; set; }

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="programDirectory">Directory where the Python executable resides</param>
			/// <param name="fileDirectory">Directory where the images for reading reside</param>
			/// <param name="version">Phantom Forces Version</param>
			/// <param name="currentDirectoryHasTessbin">True if <c>programDirectory</c> has the /tessbin/ folder required to operate.</param>
			public PyTesseractExecutor(string programDirectory, string fileDirectory, string version, bool currentDirectoryHasTessbin)
			{
				if (!programDirectory.EndsWith('\\'))
				{
					programDirectory += '\\';
				}
				this.programDirectory = programDirectory;
				if (!fileDirectory.EndsWith('\\'))
				{
					fileDirectory += '\\';
				}
				this.fileDirectory = fileDirectory;
				this.version = version;
				this.currentDirectoryHasTessbin = currentDirectoryHasTessbin;
			}

			/// <summary>
			/// Compares two file paths and determines a common directory (excludes absolute if a common one is found). Can be used to obfuscate parent root directories when they are not needed.
			/// </summary>
			/// <param name="currentProcessPath">Current directory. Note that this parameter need not be the actual current process directory.</param>
			/// <param name="foreignPath">Foreign directory. Note that this parameter need not be the actual foreign directory.</param>
			/// <returns>A <c>Tuple</c>, with the first item containing the path from the common path to the current directory path, and the second item containing the path from the common path to the foreign directory.</returns>
			/// <exception cref="Exception">Throws exceptions if two paths are unequal when they should be. (illegal case)</exception>
			public static Tuple<string, string> commonExecPath(string currentProcessPath, string foreignPath)
			{
				string tempCurrent = currentProcessPath;
				string tempForeign = foreignPath;
				if (!currentProcessPath.StartsWith(foreignPath) && !foreignPath.StartsWith(currentProcessPath))
				{ //distinct, but has common directory
					for (int i = 0; i < Math.Min(currentProcessPath.Length, foreignPath.Length); ++i)
					{
						if (currentProcessPath[i] != foreignPath[i])
						{
							tempCurrent = tempCurrent.Substring(0, i);
							tempForeign = tempForeign.Substring(0, i);

							if (tempForeign != tempCurrent) throw new Exception($"Something went really wrong: {tempCurrent} should equal {tempForeign}");

							int lastSlashBeforeSubstringC = tempCurrent.LastIndexOf('\\'); //finds last slash (aka last common directory)
							int lastSlashBeforeSubstringF = tempForeign.LastIndexOf('\\');

							if (lastSlashBeforeSubstringC != lastSlashBeforeSubstringF) throw new Exception($"Something went really wrong: {lastSlashBeforeSubstringC} should equal {lastSlashBeforeSubstringF}");

							tempCurrent = tempCurrent.Substring(0, lastSlashBeforeSubstringC); //truncates off last slash
							tempForeign = tempForeign.Substring(0, lastSlashBeforeSubstringF);

							if (tempForeign != tempCurrent) throw new Exception($"Something went really wrong: {tempCurrent} should equal {tempForeign}");

							int lastSlashBeforeSubstringC2 = tempCurrent.LastIndexOf('\\');
							int lastSlashBeforeSubstringF2 = tempForeign.LastIndexOf('\\');

							tempCurrent = currentProcessPath.Substring(lastSlashBeforeSubstringC2);
							tempForeign = foreignPath.Substring(lastSlashBeforeSubstringF2);
							break;
						}
					}
				}
				else //subset, or the same
				{
					tempCurrent = tempCurrent.Substring(0, Math.Min(currentProcessPath.Length, foreignPath.Length));
					tempForeign = tempForeign.Substring(0, Math.Min(currentProcessPath.Length, foreignPath.Length));

					int lastSlashBeforeSubstringC = tempCurrent.LastIndexOf('\\'); //finds last slash (aka last common directory)
					int lastSlashBeforeSubstringF = tempForeign.LastIndexOf('\\');
					if (lastSlashBeforeSubstringC == -1) lastSlashBeforeSubstringC = 0; //couldn't find slash in currentDir, make it beginning of string
					if (lastSlashBeforeSubstringF == -1) lastSlashBeforeSubstringF = 0;

					tempCurrent = tempCurrent.Substring(0, lastSlashBeforeSubstringC); //truncates off last slash
					tempForeign = tempForeign.Substring(0, lastSlashBeforeSubstringF);
					//alternative: tempForeign = tempForeign[..lastSlashBeforeSubstringF];

					int lastSlashBeforeSubstringC2 = tempCurrent.LastIndexOf('\\');
					int lastSlashBeforeSubstringF2 = tempForeign.LastIndexOf('\\');
					if (lastSlashBeforeSubstringC2 == -1) lastSlashBeforeSubstringC2 = 0; //couldn't find slash in currentDir, make it beginning of string
					if (lastSlashBeforeSubstringF2 == -1) lastSlashBeforeSubstringF2 = 0;

					tempCurrent = currentProcessPath.Substring(lastSlashBeforeSubstringC2);
					tempForeign = foreignPath.Substring(lastSlashBeforeSubstringF2);

				}
				return Tuple.Create(tempCurrent, tempForeign);
			}

			/// <summary>
			/// Executes the Python Executable. List of weaponTypes:
			/// <list type="bullet">
			/// <item>1 = primary</item>
			/// <item>2 = secondary</item>
			/// <item>3 = grenade</item>
			/// <item>4 = melee</item>
			/// </list>
			/// </summary>
			/// <param name="filename">File name of the image to be read.</param>
			/// <param name="weaponType">Type of weapon that the image is reading.</param>
			/// <param name="tessbinpath">Path to /tessbin/ folder. Unused if <see cref="currentDirectoryHasTessbin"/> is true.</param>
			/// <returns>A Tuple, with the first item containing the result; the second containing the first stopwatch time (in milliseconds) and the third containing the second stopwatch time (in milliseconds). Note that the second and third items return -1.0 when the function fails.</returns>
			/// <exception cref="ArgumentNullException"></exception>
			/// <exception cref="ArgumentException"></exception>
			public Tuple<string, double, double> execute(string filename, int weaponType, string? tessbinpath)
			{
				if (weaponType > 3 || weaponType < 0)
				{
					throw new ArgumentException("weaponType cannot be greater than 3 or less than 0.");
				}
				if (tessbinpath != null)
				{
					if (!tessbinpath.EndsWith("\\"))
					{
						tessbinpath += "\\";
					}
				}
				DateTime start = DateTime.Now;
				string startTime = start.ToString("dddd, MMMM, yyyy HH:mm:ss:fff");
				Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

				ProcessStartInfo pyexecute;
				if (currentDirectoryHasTessbin)
				{
					pyexecute = new ProcessStartInfo(programDirectory + "impa.exe", string.Format("{0} {1} {2} {3}", "-c", fileDirectory + filename, Convert.ToString(weaponType), version));
				}
				else
				{
					if (tessbinpath == null)
					{
						throw new ArgumentNullException(nameof(tessbinpath), "If currentDirectoryHasTessbin is false, tessbinpath parameter cannot be empty.");
					}
					pyexecute = new ProcessStartInfo(programDirectory + "impa.exe", string.Format("{0} {1} {2} {3} {4}", "-f", tessbinpath, fileDirectory + filename, Convert.ToString(weaponType), version));
				}
				pyexecute.RedirectStandardOutput = true;
				pyexecute.UseShellExecute = false;

				//
				if (pyexecute != null)
				{
					using (Process? execute = Process.Start(pyexecute))
					{
						if (execute != null)
						{
							using (StreamReader reader = execute.StandardOutput)
							{
								string result = reader.ReadToEnd();

								DateTime end = DateTime.Now;
								string endTime = end.ToString("dd, MMMM, yyyy HH:mm:ss:fff");
								string command = "Command used: " + pyexecute.Arguments;
								command = command.Replace(fileDirectory + filename, "...." + commonExecPath(Environment.ProcessPath ?? "null", fileDirectory + filename).Item2);
								if (tessbinpath != null) command = command.Replace(tessbinpath, "...." + commonExecPath(Environment.ProcessPath ?? "null", tessbinpath).Item2);
								watch.Stop();
								string elapsedTime = Convert.ToString(((double)watch.ElapsedMilliseconds) / ((double)1000)) + " seconds";
								int width = Console.WindowWidth;
								string line = string.Empty;
								for (int i = 0; i < width; ++i)
								{
									line += "_";
								}

								string finalOutput = $"Time start: \t{startTime}{Environment.NewLine}" +
									$"Time end: \t{endTime}{Environment.NewLine}" +
									$"Elapsed time (1): {elapsedTime}, Elapsed time (2): {(end - start).TotalSeconds}{Environment.NewLine}" +
									$"Executed from: {"...." + commonExecPath(fileDirectory + filename, Environment.ProcessPath ?? "null").Item2}{Environment.NewLine}" +
									$"{command}{Environment.NewLine}Computer Information:{Environment.NewLine}" +
									$"Name: {Environment.MachineName}, Processor Count: {Environment.ProcessorCount}, Page Size: {Environment.SystemPageSize}{Environment.NewLine}" +
									$"Working Set Memory: {Environment.WorkingSet}, .NET Version: {Environment.Version}, Operating System: {Environment.OSVersion}{Environment.NewLine}" +
									$"{line}" +
									$"{Environment.NewLine}{Environment.NewLine}" +
									$"{result}";

								return Tuple.Create(finalOutput, (end - start).TotalSeconds, ((double)watch.ElapsedMilliseconds) / ((double)1000));
							}
						}
					}
				}

				return Tuple.Create("failed", -1.0, -1.0);
			}

			public string crop(string filename, int weaponType)
			{
				ProcessStartInfo pyexecute;
				pyexecute = new ProcessStartInfo(programDirectory + "impa.exe", string.Format("{0} {1} {2} {3}", "-w", fileDirectory + filename, Convert.ToString(weaponType), version));
				pyexecute.RedirectStandardOutput = true;
				pyexecute.UseShellExecute = false;
				if (pyexecute != null)
				{
					using (Process? execute = Process.Start(pyexecute))
					{
						if (execute != null)
						{
							using (StreamReader reader = execute.StandardOutput)
							{
								string result = reader.ReadToEnd();

								string command = "Command used: " + pyexecute.Arguments;
								command = command.Replace(fileDirectory + filename, "...." + commonExecPath(Environment.ProcessPath ?? "null", fileDirectory + filename).Item2);
								int width = Console.WindowWidth;
								string line = string.Empty;
								for (int i = 0; i < width; ++i)
								{
									line += "_";
								}

								string finalOutput =
									$"Executed from: {"...." + commonExecPath(fileDirectory + filename, Environment.ProcessPath ?? "null").Item2}{Environment.NewLine}" +
									$"{command}{Environment.NewLine}Computer Information:{Environment.NewLine}" +
									$"Name: {Environment.MachineName}, Processor Count: {Environment.ProcessorCount}, Page Size: {Environment.SystemPageSize}{Environment.NewLine}" +
									$"Working Set Memory: {Environment.WorkingSet}, .NET Version: {Environment.Version}, Operating System: {Environment.OSVersion}{Environment.NewLine}" +
									$"{line}" +
									$"{Environment.NewLine}{Environment.NewLine}" +
								$"{result}";
								return finalOutput;
							}
						}
					}
				}
				return string.Empty;
			}

		}

		public class Benchmark : IOutput{
            public string OutputString { get; init; }
            public double StopwatchNormal { get; set; }
			public double StopwatchDateTime { get; set; }
            public Benchmark(string outputString)
			{
				OutputString = outputString;
			}
		}

		public interface IOutput
		{

			public string OutputString
			{
				get;
			}




		}

		public interface IPythonExecutor
		{

			/// <summary>
			/// Directory where the Python executable resides
			/// </summary>
			public string programDirectory { get; init; }

			/// <summary>
			/// Directory where the images for reading reside
			/// </summary>
			public string fileDirectory { get; init; }
			/// <summary>
			/// Phantom Forces Version
			/// </summary>
			public string version { get; init; }
		}
	}
}