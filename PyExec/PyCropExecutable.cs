using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB
{
    namespace PythonExecutor
    {
        /// <summary>
        /// Input (consumed) object for <see cref="PythonExecutor"/>, specified for image cropping.
        /// </summary>
        public class PyCropExecutable : IPythonExecutable<IOutput>
        {

            /// <summary>
            /// Phantom Forces Version. "800" = Version 8.0.0; "1001" = Version 10.0.1, etc.
            /// </summary>
            public string Version { get; init; }

            /// <summary>
            /// WeaponType of the weapon, telling the Python application where to read.
            /// </summary>
            public WeaponType.Weapon WeaponType { get; init; }

            /// <summary>
            /// Directory where the images for reading reside.
            /// </summary>
            public string FileDirectory { get; init; }

            /// <summary>
            /// Name of the file to be read by the Python application.
            /// </summary>
            public string Filename { get; init; }

            /// <summary>
            /// Directory where the Python executable resides.
            /// </summary>
            public string ProgramDirectory { get; init; }

            private bool internalExecution = true;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="filename">Name of the file to be read by the Python application.</param>
            /// <param name="fileDirectory">Directory where the images for reading reside.</param>
            /// <param name="weaponType">WeaponType of the weapon, telling the Python application where to read.</param>
            /// <param name="version">Phantom Forces Version. "800" = Version 8.0.0; "1001" = Version 10.0.1, etc.</param>
            /// <param name="programDirectory">Directory where the Python executable resides.</param>
            public PyCropExecutable(string filename, string fileDirectory, string version, WeaponType.Weapon weaponType, string programDirectory)
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
            }

            /// <summary>
            /// Checks if the parameters passed through <see cref="PyCropExecutable.PyCropExecutable(string, string, string, WeaponType.Weapon, string)"/> are valid.
            /// </summary>
            /// <exception cref="ArgumentException"></exception>
            public void CheckInput()
            {
                internalExecution = false;
                if (!File.Exists(ProgramDirectory + "impa.exe"))
                {
                    throw new FileNotFoundException($"The application file, specified at {ProgramDirectory + "impa.exe"} does not exist.");
                }
                if (!File.Exists(FileDirectory + Filename))
                {
                    throw new FileNotFoundException($"The input file, specified at {FileDirectory + Filename} does not exist.");
                }
            }

            /// <summary>
			/// Constructs the <see cref="ProcessStartInfo"/> object.
			/// </summary>
			/// <returns>A <see cref="ProcessStartInfo"/> object that can be executed to read the image specified by <see cref="Filename"/></returns>
            public ProcessStartInfo GetProcessStartInfo()
            {
                ProcessStartInfo pyexecute;
                pyexecute = new ProcessStartInfo(ProgramDirectory + "impa.exe", string.Format("{0} {1} {2} {3}", "-w", FileDirectory + Filename, Convert.ToString(WeaponType), Version));
                pyexecute.RedirectStandardOutput = true;
                pyexecute.UseShellExecute = false;
                return pyexecute;
            }

            /// <summary>
            /// Returns the output string of the Python application crop subroutine.
            /// </summary>
            /// <returns>Output string from Python application.</returns>
            public IOutput returnOutput()
            {
                if (internalExecution)
                {
                    return new FailedPythonOutput("The method CheckInputs() has not been called. Do not try to invoke this method directly.");
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
                                command = command.Replace(FileDirectory + Filename, "...." + PyUtilityClass.commonExecPath(Environment.ProcessPath ?? "null", FileDirectory + Filename).Item2);
                                int width = Console.WindowWidth;
                                string line = string.Empty;
                                for (int i = 0; i < width; ++i)
                                {
                                    line += "_";
                                }

                                PythonOutput finalOutput = new PythonOutput(
                                    $"Executed from: {"...." + PyUtilityClass.commonExecPath(FileDirectory + Filename, Environment.ProcessPath ?? "null").Item2}{Environment.NewLine}" +
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
                return new FailedPythonOutput("Failed.");
            }
        }
    }
}
