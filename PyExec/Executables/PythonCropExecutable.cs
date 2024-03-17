using System;
using System.Diagnostics;
using System.IO;
using PFDB.WeaponUtility;

namespace PFDB
{
    namespace PythonExecution
    {
        /// <summary>
        /// Input (consumed) object for <see cref="PythonExecutor"/>, specified for image cropping. This object is responsible for calling the Python script directly.
        /// </summary>
        public sealed class PythonCropExecutable : IPythonExecutable<IOutput>
        {

            /// <summary>
            /// Phantom Forces Version. "800" = Version 8.0.0; "1001" = Version 10.0.1, etc.
            /// </summary>
            public PhantomForcesVersion Version { get; private set; }

            /// <summary>
            /// WeaponType of the weapon, telling the Python application where to read.
            /// </summary>
            public WeaponType WeaponType { get; private set; }

            /// <summary>
            /// Directory where the images for reading reside.
            /// </summary>
            public string FileDirectory { get; private set; }

            /// <summary>
            /// Name of the file to be read by the Python application.
            /// </summary>
            public string Filename { get; private set; }

            /// <summary>
            /// Directory where the Python executable resides.
            /// </summary>
            public string ProgramDirectory { get; private set; }

            private bool _internalExecution = true;
            private bool _untrustedConstruction = true;


            /// <summary>
            /// Unused constructor.
            /// </summary>
            public PythonCropExecutable()
            {
                Filename = string.Empty;
                WeaponType = 0;
                FileDirectory = string.Empty;
                Version = new PhantomForcesVersion("8.0.0");
                ProgramDirectory = string.Empty;
            }

            /// <summary>
            /// Constructs the executable.
            /// </summary>
            /// <param name="filename">Name of the file to be read by the Python application.</param>
            /// <param name="fileDirectory">Directory where the images for reading reside.</param>
            /// <param name="weaponType">WeaponType of the weapon, telling the Python application where to read.</param>
            /// <param name="version">Phantom Forces Version. "800" = Version 8.0.0; "1001" = Version 10.0.1, etc.</param>
            /// <param name="programDirectory">Directory where the Python executable resides.</param>
            public IPythonExecutable<IOutput> Construct(string filename, string fileDirectory, PhantomForcesVersion version, WeaponType weaponType, string programDirectory)
            {
                if (!programDirectory.EndsWith('\\'))
                {
                    programDirectory += '\\';
                }
                if (!fileDirectory.EndsWith('\\'))
                {
                    fileDirectory += '\\';
                }
                FileDirectory = fileDirectory;
                Filename = filename;
                Version = version;
                this.WeaponType = weaponType;
                ProgramDirectory = programDirectory;
                _untrustedConstruction = false;
                return this;
            }

            /// <summary>
            /// Checks if the parameters passed through <see cref="PythonCropExecutable.Construct(string, string, PhantomForcesVersion, WeaponType, string)"/> are valid.
            /// </summary>
            /// <exception cref="ArgumentException"></exception>
            public void CheckInput()
            {
                PythonAggregateException aggregateException = new PythonAggregateException();
                _internalExecution = false;
                if (File.Exists(ProgramDirectory + "impa.exe") == false)
                {
                    //this shouldn't be logged, the factory ideally should catch and log it
                    aggregateException.exceptions.Add(new FileNotFoundException($"The application file, specified at {ProgramDirectory + "impa.exe"} does not exist.", ProgramDirectory + "impa.exe"));
                    //throw new FileNotFoundException($"The application file, specified at {ProgramDirectory + "impa.exe"} does not exist.");
                }
                if (File.Exists(FileDirectory + Filename) == false)
                {
                    //this shouldn't be logged, the factory ideally should catch and log it
                    aggregateException.exceptions.Add(new FileNotFoundException($"The input file, specified at {FileDirectory + Filename} does not exist.", FileDirectory + Filename));
                }

                if(aggregateException.exceptions.Count == 1)
                {
                    throw aggregateException.exceptions[0];
                }else if(aggregateException.exceptions.Count > 1)
                {
                    throw aggregateException;
                }
                else
                {
                    return;
                }
            }

            /// <summary>
			/// Constructs the <see cref="ProcessStartInfo"/> object.
			/// </summary>
			/// <returns>A <see cref="ProcessStartInfo"/> object that can be executed to read the image specified by <see cref="Filename"/></returns>
            public ProcessStartInfo GetProcessStartInfo()
            {
                ProcessStartInfo pyexecute;
                pyexecute = new ProcessStartInfo(ProgramDirectory + "impa.exe", string.Format("{0} {1} {2} {3}", "-w", FileDirectory + Filename, Convert.ToString(WeaponType), Version.VersionNumber.ToString()));
                pyexecute.RedirectStandardOutput = true;
                pyexecute.UseShellExecute = false;
                return pyexecute;
            }

            /// <summary>
            /// Returns the output string of the Python application crop subroutine.
            /// </summary>
            /// <returns>Output string from Python application.</returns>
            public IOutput ReturnOutput()
            {
                if (_internalExecution || _untrustedConstruction)
                {
                    //this shouldn't be logged, the factory ideally should catch and log it
                    return new FailedPythonOutput("The methods Construct() and CheckInput() have not been called. Do not try to invoke this method directly.");
                }
                ProcessStartInfo pyexecute = GetProcessStartInfo();
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
                                command = command.Replace(FileDirectory + Filename, "...." + PyUtilityClass.CommonExecutionPath(Environment.ProcessPath ?? "null", FileDirectory + Filename).Item2);
                                int width = Console.WindowWidth;
                                string line = string.Empty;
                                for (int i = 0; i < width; ++i)
                                {
                                    line += "_";
                                }

                                PythonOutput finalOutput = new PythonOutput(
                                    $"Executed from: {"...." + PyUtilityClass.CommonExecutionPath(FileDirectory + Filename, Environment.ProcessPath ?? "null").Item2}{Environment.NewLine}" +
                                    $"{command}{Environment.NewLine}Computer Information:{Environment.NewLine}" +
                                    $"Name: {Environment.MachineName}, Processor Count: {Environment.ProcessorCount}, Page Size: {Environment.SystemPageSize}{Environment.NewLine}" +
                                    $"Working Set Memory: {Environment.WorkingSet}, .NET Version: {Environment.Version}, Operating System: {Environment.OSVersion}{Environment.NewLine}" +
                                    $"{line}" +
                                    $"{Environment.NewLine}{Environment.NewLine}" +
                                $"{result}");
                                return finalOutput;
                            }
                        }
                    }
                }
                //this shouldn't be logged, the factory ideally should catch and log it
                return new FailedPythonOutput("Failed.");
            }
        }
    }
}
