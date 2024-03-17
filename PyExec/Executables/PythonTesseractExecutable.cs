using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using PFDB.WeaponUtility;


namespace PFDB
{
    namespace PythonExecution
    {
        /// <summary>
        /// Input (consumed) object for <see cref="PythonExecutor"/>, specified for PyTesseract. This object is responsible for calling the Python script directly.
        /// </summary>
        public sealed class PythonTesseractExecutable : IPythonExecutable<IOutput>
		{
			/// <summary>
			/// Name of the file to be read by the Python application.
			/// </summary>
			public string Filename { get; private set; }

			/// <summary>
			/// Directory where the images for reading reside.
			/// </summary>
			public string FileDirectory { get; private set; }

			/// <summary>
			/// WeaponType of the weapon, telling the Python application where to read.
			/// </summary>
			public WeaponType WeaponType { get; private set; }
			
			/// <summary>
			/// Directory where the Python executable resides.
			/// </summary>
			public string ProgramDirectory { get; private set; }

			/// <summary>
			/// Path to "tessbin" folder. If null, "tessbin" folder is assumed to be in the same working directory.
			/// </summary>
			public string? TessbinPath { get; private set; }

            /// <summary>
            /// Command executed by this program.
            /// </summary>
            public string CommandExecuted { get; private set; }

            /// <summary>
            /// Phantom Forces Version. "800" = Version 8.0.0; "1001" = Version 10.0.1, etc.
            /// </summary>
            public PhantomForcesVersion Version { get; private set; }

            private bool _internalExecution = true;
            private bool _untrustedConstruction = true;

            /// <summary>
            /// Unused constructor. Use <see cref="Construct(string, string, PhantomForcesVersion, WeaponType, string)"/> or <see cref="Construct(string, string, PhantomForcesVersion, WeaponType, string, string?)"/> instead.
            /// </summary>
            public PythonTesseractExecutable()
            {
                Filename = string.Empty;
                WeaponType = 0;
                TessbinPath = null;
                FileDirectory = string.Empty;
                Version = new PhantomForcesVersion("8.0.0");
                ProgramDirectory = string.Empty;
                CommandExecuted = string.Empty;
            }

            /// <summary>
            /// Default constructor. <see cref="TessbinPath"/> is assumed to be in the same current working directory. If you need <see cref="TessbinPath"/> to be set to a different directory, use <see cref="Construct(string, string, PhantomForcesVersion, WeaponType, string, string?)"/>.
            /// </summary>
            /// <param name="filename">Name of the file to be read by the Python application.</param>
            /// <param name="fileDirectory">Directory where the images for reading reside.</param>
            /// <param name="weaponType">WeaponType of the weapon, telling the Python application where to read.</param>
            /// <param name="version">Phantom Forces Version. "800" = Version 8.0.0; "1001" = Version 10.0.1, etc.</param>
            /// <param name="programDirectory">Directory where the Python executable resides.</param>
            public IPythonExecutable<IOutput> Construct(string filename, string fileDirectory, PhantomForcesVersion version, WeaponType weaponType, string programDirectory)
            {
                Filename = filename;
                WeaponType = weaponType;
                TessbinPath = null;
                if (TessbinPath != null)
                {
                    if (TessbinPath.EndsWith('\\') == false)
                    {
                        TessbinPath += '\\';
                    }
                }
                if (programDirectory.EndsWith('\\') == false)
                {
                    programDirectory += '\\';
                }
                if (fileDirectory.EndsWith('\\') == false)
                {
                    fileDirectory += '\\';
                }
                FileDirectory = fileDirectory;
                Version = version;
                ProgramDirectory = programDirectory;
                CommandExecuted = string.Empty;
                _untrustedConstruction = false;
                return this;
            }

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="filename">Name of the file to be read by the Python application.</param>
            /// <param name="fileDirectory">Directory where the images for reading reside.</param>
            /// <param name="weaponType">WeaponType of the weapon, telling the Python application where to read.</param>
            /// <param name="version">Phantom Forces Version. "800" = Version 8.0.0; "1001" = Version 10.0.1, etc.</param>
            /// <param name="programDirectory">Directory where the Python executable resides.</param>
            /// <param name="tessbinPath">Path to "tessbin" folder. If null, "tessbin" folder is assumed to be in the same working directory.</param>
            public PythonTesseractExecutable Construct(string filename, string fileDirectory, PhantomForcesVersion version, WeaponType weaponType, string programDirectory, string? tessbinPath )
            {
                Filename = filename;
                WeaponType = weaponType;
                TessbinPath = tessbinPath;
                if (TessbinPath != null)
                {
                    if (TessbinPath.EndsWith('\\') == false)
                    {
                        TessbinPath += '\\';
                    }
                }
                if (programDirectory.EndsWith('\\') == false)
                {
                    programDirectory += '\\';
                }
                if (fileDirectory.EndsWith('\\') == false)
                {
                    fileDirectory += '\\';
                }
                FileDirectory = fileDirectory;
                Version = version;
                ProgramDirectory = programDirectory;
                CommandExecuted = string.Empty;
                _untrustedConstruction = false;
                return this;
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
					pyexecute = new ProcessStartInfo(ProgramDirectory + "impa.exe", $"-c {FileDirectory + Filename} {Convert.ToString((int)WeaponType)} {Version.VersionNumber.ToString()}");
					command.Append(pyexecute.Arguments);
					command = command.Replace(FileDirectory + Filename, "...." + PyUtilityClass.CommonExecutionPath(Environment.ProcessPath ?? "null", FileDirectory + Filename).Item2);
				}
				else
				{
					pyexecute = new ProcessStartInfo(ProgramDirectory + "impa.exe", $"-f {TessbinPath} {FileDirectory + Filename} {Convert.ToString((int)WeaponType)} {Version.VersionNumber.ToString()}");
					command.Append(pyexecute.Arguments);
					command = command.Replace(TessbinPath, "...." + PyUtilityClass.CommonExecutionPath(Environment.ProcessPath ?? "null", TessbinPath).Item2);
				}
				CommandExecuted = command.ToString();
				pyexecute.RedirectStandardOutput = true;
				pyexecute.UseShellExecute = false;
				return pyexecute;
			}

            /// <summary>
            /// Checks if the parameters passed through <see cref="PythonTesseractExecutable.Construct(string, string, PhantomForcesVersion, WeaponType, string, string?)"/> are valid.
            /// </summary>
            /// <exception cref="ArgumentException"></exception>
			/// <exception cref="DirectoryNotFoundException"></exception>
			/// <exception cref="FileNotFoundException"></exception>
			/// <exception cref="PythonAggregateException"></exception>
            public void CheckInput()
			{
				_internalExecution = false;
				PythonAggregateException aggregateException = new PythonAggregateException();
				if ((int)WeaponType > 4 || (int)WeaponType < 1)
                {
                    //this shouldn't be logged, the factory ideally should catch and log it
                    aggregateException.exceptions.Add(new ArgumentException("weaponType cannot be greater than 3 or less than 0."));
				}
				if (TessbinPath == null)
				{
					if (!Directory.Exists($"{Directory.GetCurrentDirectory()}\\tessbin\\"))
                    {
                        //this shouldn't be logged, the factory ideally should catch and log it
                        aggregateException.exceptions.Add(new DirectoryNotFoundException($"The tessbin path specified at {Directory.GetCurrentDirectory()}\\tessbin\\ does not exist. Ensure that the directory exists, then try again."));
					}
				}
				else
				{
                    if (!Directory.Exists(TessbinPath + "\\tessbin\\"))
                    {
                        //this shouldn't be logged, the factory ideally should catch and log it
                        aggregateException.exceptions.Add(new DirectoryNotFoundException($"The tessbin path specified at {TessbinPath}\\tessbin\\ does not exist. Ensure that the directory exists, then try again."));
                    }
                }
				if(!File.Exists(ProgramDirectory + "impa.exe"))
                {
                    //this shouldn't be logged, the factory ideally should catch and log it
                    aggregateException.exceptions.Add(new FileNotFoundException($"The application file, specified at {ProgramDirectory + "impa.exe"} does not exist."));
                }
                if (!File.Exists(FileDirectory + Filename))
                {
                    //this shouldn't be logged, the factory ideally should catch and log it
                    aggregateException.exceptions.Add(new FileNotFoundException($"The input file, specified at {FileDirectory + Filename} does not exist."));
                }


                if (aggregateException.exceptions.Count == 1)
                {
                    throw aggregateException.exceptions[0];
                }
                else if (aggregateException.exceptions.Count > 1)
                {
                    throw aggregateException;
                }
                else
                {
                    return;
                }
            }

			/// <summary>
			/// Executes the Python application
			/// </summary>
			/// <returns></returns>
			public IOutput ReturnOutput()
            {
                if (_internalExecution || _untrustedConstruction)
                {
                    //this shouldn't be logged, the factory ideally should catch and log it
                    return new FailedPythonOutput("The methods Construct() and CheckInputs() have not been called. Do not try to invoke this method directly.");
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

							string line = string.Empty;
							for (int i = 0; i < Console.WindowWidth; ++i)
							{
								line += "_";
							}

							benchmark.StopBenchmark();
							string endTime = benchmark.End.ToString("dddd, MMMM, yyyy HH:mm:ss:fff");
							benchmark.GetElapsedTimeInSeconds(result);
							return benchmark;
						}
					}
				}
				benchmark.StopBenchmark();

                //this shouldn't be logged, the factory ideally should catch and log it
                return new FailedPythonOutput("Failed.");
				
			}

        }
	}
}