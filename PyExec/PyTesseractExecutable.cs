// See https://aka.ms/new-console-template for more information
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using PFDB.PythonExecutor;

namespace PFDB
{
    namespace PythonExecutor
    {
        /// <summary>
        /// Input (consumed) object for <see cref="PythonExecutor"/>, specified for PyTesseract.
        /// </summary>
        public class PyTesseractExecutable : IPythonExecutable<IOutput>
		{
			/// <summary>
			/// Name of the file to be read by the Python application.
			/// </summary>
			public string Filename { get; init; }

			/// <summary>
			/// Directory where the images for reading reside.
			/// </summary>
			public string FileDirectory { get; init; }

			/// <summary>
			/// WeaponType of the weapon, telling the Python application where to read.
			/// </summary>
			public WeaponType.Weapon WeaponType { get; init; }

			/// <summary>
			/// Phantom Forces Version. "800" = Version 8.0.0; "1001" = Version 10.0.1, etc.
			/// </summary>
			public string Version { get; init; }
			
			/// <summary>
			/// Directory where the Python executable resides.
			/// </summary>
			public string ProgramDirectory { get; init; }

			/// <summary>
			/// Path to "tessbin" folder. If null, "tessbin" folder is assumed to be in the same working directory.
			/// </summary>
			public string? TessbinPath { get; init; }

            /// <summary>
            /// Command executed by this program.
            /// </summary>
            public string CommandExecuted { get; private set; }
			private bool internalExecution = true;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="filename">Name of the file to be read by the Python application.</param>
            /// <param name="fileDirectory">Directory where the images for reading reside.</param>
            /// <param name="weaponType">WeaponType of the weapon, telling the Python application where to read.</param>
            /// <param name="version">Phantom Forces Version. "800" = Version 8.0.0; "1001" = Version 10.0.1, etc.</param>
            /// <param name="programDirectory">Directory where the Python executable resides.</param>
            /// <param name="tessbinPath">Path to "tessbin" folder. If null, "tessbin" folder is assumed to be in the same working directory.</param>
            public PyTesseractExecutable(string filename, string fileDirectory, WeaponType.Weapon weaponType, string version, string programDirectory, string? tessbinPath)
			{
				Filename = filename;
				WeaponType = weaponType;
				TessbinPath = tessbinPath;
				if (TessbinPath != null)
				{
					if (!TessbinPath.EndsWith('\\'))
					{
						TessbinPath += '\\';
					}
				}
				if (!programDirectory.EndsWith('\\'))
				{
					programDirectory += '\\';
				}
				if (!fileDirectory.EndsWith('\\'))
				{
					fileDirectory += '\\';
				}
				FileDirectory = fileDirectory;
				Version = version;
				ProgramDirectory = programDirectory;
				CommandExecuted = string.Empty;
			}

			/// <summary>
			/// Constructs the <see cref="ProcessStartInfo"/> object depending on if <see cref="TessbinPath"/> is null.
			/// </summary>
			/// <returns>A <see cref="ProcessStartInfo"/> object that can be executed to read the image specified by <see cref="Filename"/></returns>
			public ProcessStartInfo GetProcessStartInfo()
			{
				ProcessStartInfo pyexecute;
				StringBuilder command = new StringBuilder("Command used: ");
				if (TessbinPath == null)
				{
					pyexecute = new ProcessStartInfo(ProgramDirectory + "impa.exe", $"-c {FileDirectory + Filename} {Convert.ToString((int)WeaponType)} {Version}");
					command.Append(pyexecute.Arguments);
					command = command.Replace(FileDirectory + Filename, "...." + PyUtilityClass.commonExecPath(Environment.ProcessPath ?? "null", FileDirectory + Filename).Item2);
				}
				else
				{
					pyexecute = new ProcessStartInfo(ProgramDirectory + "impa.exe", $"-f {TessbinPath} {FileDirectory + Filename} {Convert.ToString((int)WeaponType)} {Version}");
					command.Append(pyexecute.Arguments);
					command = command.Replace(TessbinPath, "...." + PyUtilityClass.commonExecPath(Environment.ProcessPath ?? "null", TessbinPath).Item2);
				}
				CommandExecuted = command.ToString();
				pyexecute.RedirectStandardOutput = true;
				pyexecute.UseShellExecute = false;
				return pyexecute;
			}

			/// <summary>
			/// Checks if the parameters passed through <see cref="PyTesseractExecutable.PyTesseractExecutable(string, string, PFDB.PythonExecutor.WeaponType.Weapon, string, string, string?)"/> are valid.
			/// </summary>
			/// <exception cref="ArgumentException"></exception>
			public void CheckInput()
			{
				internalExecution = false;
				if ((int)WeaponType > 3 || (int)WeaponType < 0)
				{
					throw new ArgumentException("weaponType cannot be greater than 3 or less than 0.");
				}
				if(!File.Exists(ProgramDirectory + "impa.exe"))
				{
					throw new FileNotFoundException($"The application file, specified at {ProgramDirectory + "impa.exe"} does not exist.");
                }
                if (!File.Exists(FileDirectory + Filename))
                {
					throw new FileNotFoundException($"The input file, specified at {FileDirectory + Filename} does not exist.");
                }
            }

			/// <summary>
			/// Executes the Python application
			/// </summary>
			/// <returns></returns>
			public IOutput returnOutput()
            {
                if (internalExecution)
                {
                    return new FailedPythonOutput("The method CheckInputs() has not been called. Do not try to invoke this method directly.");
                }
                Benchmark benchmark = new Benchmark();
				benchmark.StartBenchmark();
				string startTime = benchmark.Start.ToString("dddd, MMMM, yyyy HH:mm:ss:fff");

				ProcessStartInfo pyexecute = GetProcessStartInfo();

				using (Process? execute = Process.Start(pyexecute))
				{
					if (execute != null)
					{
						using (StreamReader reader = execute.StandardOutput)
						{
							string result = reader.ReadToEnd();


							int width = Console.WindowWidth;
							string line = string.Empty;
							for (int i = 0; i < width; ++i)
							{
								line += "_";
							}

							benchmark.StopBenchmark();
							string endTime = benchmark.End.ToString("dddd, MMMM, yyyy HH:mm:ss:fff");
							benchmark.GetElapsedTimeInSeconds(
								$"Elapsed time 1 (s): {benchmark.StopwatchDateTime.TotalSeconds}, Elapsed time 2 (s): {benchmark.StopwatchNormal}{Environment.NewLine}" +
								$"Time start: \t{startTime}{Environment.NewLine}" +
								$"Time end: \t{endTime}{Environment.NewLine}" +
								$"Executed from: {"...." + PyUtilityClass.commonExecPath(FileDirectory + Filename, Environment.ProcessPath ?? "null").Item2}{Environment.NewLine}" +
								$"{CommandExecuted}{Environment.NewLine}Computer Information:{Environment.NewLine}" +
								$"Name: {Environment.MachineName}, Processor Count: {Environment.ProcessorCount}, Page Size: {Environment.SystemPageSize}{Environment.NewLine}" +
								$"Working Set Memory: {Environment.WorkingSet}, .NET Version: {Environment.Version}, Operating System: {Environment.OSVersion}{Environment.NewLine}" +
								$"{line}" +
								$"{Environment.NewLine}{Environment.NewLine}" +
								$"{result}"
								);
							return benchmark;
						}
					}
				}
				benchmark.StopBenchmark();

				return new FailedPythonOutput("Failed.");
				
			}
		}
	}
}