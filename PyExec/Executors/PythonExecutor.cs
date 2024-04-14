// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Text;
using System.Threading;
using PFDB.Logging;


namespace PFDB
{
	namespace PythonExecution
	{

		/// <summary>
		/// Specifies the output destination. Use <c><see cref="OutputDestination.Console"/> | <see cref="OutputDestination.File"/></c> to specify both outputs.
		/// </summary>
		public enum OutputDestination
		{
			/// <summary>
			/// Output to the console.
			/// </summary>
			Console = 1,
			/// <summary>
			/// Output to a file. The file is generated in <code>{currentWorkingDirectory}/</code>
			/// </summary>
			File
		}

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
		public class PythonExecutor : IPythonExecutor
		{

			/// <summary>
			/// Signal state for <see cref="PythonFactory.IPythonExecutionFactory.Start()"/>.
			/// </summary>
			public ManualResetEvent manualEvent { get; set; }

			/// <summary>
			/// Output destination for the enclosed <see cref="IOutput"/> object.
			/// </summary>
			public OutputDestination Destination { get; init; }

			/// <summary>
			/// Output of the enclosed <see cref="IOutput"/> object.
			/// </summary>
			public IOutput Output { get; private set; }

			/// <summary>
			/// Input for this enclosing class (<see cref="PythonExecutor"/>). Will execute and return in <see cref="Output"/> as a class implementing <see cref="IOutput"/>.
			/// </summary>
			public IPythonExecutable<IOutput> Input { get; private set; }

			/// <summary>
			/// Default constructor.
			/// </summary>
			/// <param name="destination">Output destination for the enclosed <see cref="IOutput"/> object.</param>
			public PythonExecutor(OutputDestination destination)
			{
				Input = new InitExecutable(); Output = new PythonOutput("");
				Destination = destination;
				manualEvent = new ManualResetEvent(false); //signal for PythonExecutionFactory
			}

			/// <summary>
			/// Loads an input class implementing <see cref="IPythonExecutable{IOutput}"/>.
			/// </summary>
			/// <param name="input">Input for this enclosing class (<see cref="PythonExecutor"/>). Will execute and return in <see cref="Output"/> as a class implementing <see cref="IOutput"/>.</param>
			/// <exception cref="ArgumentException"></exception>
			public void Load(IPythonExecutable<IOutput> input)
			{
				//edge case where IPythonExecutable is loaded with a FailedPythonOutput type
				if(input is IPythonExecutable<FailedPythonOutput>)
				{
					throw new ArgumentException("Input cannot be of FailedPythonOutput type.");
				}
				Input = input;
			}

			/// <summary>
			/// Loads an input class implementing <see cref="IPythonExecutable{IOutput}"/>.
			/// Note: <see cref="IOutput"/> type specifier in parameter <paramref name="input"/> cannot be of <see cref="FailedPythonOutput"/> type.
			/// </summary>
			/// <param name="input">Input for this enclosing class (<see cref="PythonExecutor"/>). Will execute and return in <see cref="Output"/> as a class implementing <see cref="IOutput"/>.</param>
			/// <returns>The same object for chaining.</returns>
			/// <exception cref="ArgumentException"></exception>
			public IPythonExecutor LoadOut(IPythonExecutable<IOutput> input)
			{
				//edge case where IPythonExecutable is loaded with a FailedPythonOutput type
				if (input is IPythonExecutable<FailedPythonOutput>)
				{
					throw new ArgumentException("Input cannot be of FailedPythonOutput type.");
				}
				Input = input;
				return this;
			}

			/// <summary>
			/// Executes the Python Executable.
			/// Populates <see cref="Output"/> when finished.
			/// </summary>
			/// <exception cref="ArgumentNullException"></exception>
			/// <exception cref="ArgumentException"></exception>
			public void Execute(object? bs)
			{
				try
				{
					Input.CheckInput();
				}catch(PythonAggregateException ex)
				{
					StringBuilder builder = new StringBuilder();
					foreach(SystemException exception in ex.exceptions)
					{
						builder.Append($"Exception: {exception.GetType()} ||| {exception.Message}{Environment.NewLine}");
					}
					Output = new FailedPythonOutput(builder.ToString());
				}catch(Exception ex)
				{
					Output = new FailedPythonOutput(ex.Message);
				}
				
				Output = Input.ReturnOutput();
				//Console.WriteLine((int)Destination | (int)OutputDestination.File);
				if (((int)Destination & (int)OutputDestination.File) == (int)OutputDestination.File)
				{
					//Console.WriteLine(Directory.GetCurrentDirectory() ?? "null folder");

					if (!Directory.Exists($"{Directory.GetCurrentDirectory()}\\PFDB_outputs\\"))
					{
						Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\PFDB_outputs\\");
						PFDBLogger.LogInformation($"Creating directory at {Directory.GetCurrentDirectory()}\\PFDB_outputs\\ because it did not exist.");
					}
					if (!Directory.Exists($"{Directory.GetCurrentDirectory()}\\PFDB_log\\"))
					{
						Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\PFDB_log\\");
						PFDBLogger.LogInformation($"Creating directory at {Directory.GetCurrentDirectory()}\\PFDB_log\\ because it did not exist.");
					}
					if (!Directory.Exists($"{Directory.GetCurrentDirectory()}\\PFDB_outputs\\{Input.WeaponID.Version.VersionNumber}")) {
						Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\PFDB_outputs\\{Input.WeaponID.Version.VersionNumber}");
						PFDBLogger.LogInformation($"Creating directory at {Directory.GetCurrentDirectory()}\\PFDB_outputs\\{Input.WeaponID.Version.VersionNumber} because it did not exist.");
					}
					if (!Directory.Exists($"{Directory.GetCurrentDirectory()}\\PFDB_log\\{Input.WeaponID.Version.VersionNumber}")) {
						Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\PFDB_log\\{Input.WeaponID.Version.VersionNumber}");
						PFDBLogger.LogInformation($"Creating directory at {Directory.GetCurrentDirectory()}\\PFDB_log\\{Input.WeaponID.Version.VersionNumber} because it did not exist.");
					}

					File.WriteAllText($"{Directory.GetCurrentDirectory()}\\PFDB_outputs\\{Input.WeaponID.Version.VersionNumber}\\{Input.Filename}.pfdb", Output.OutputString);
					File.WriteAllText($"{Directory.GetCurrentDirectory()}\\PFDB_log\\{Input.WeaponID.Version.VersionNumber}\\{Input.Filename}.pfdblog",
						$"Filename: {Input.Filename} {Environment.NewLine}" +
						$"Program Directory: {Input.ProgramDirectory} {Environment.NewLine}" +
						((Output is Benchmark benchmark) ? $"Elapsed time by DateTime (s): { benchmark.StopwatchDateTime.TotalSeconds}, Elapsed time by Stopwatch (s): { benchmark.StopwatchNormal.ElapsedMilliseconds / (double)1000}{Environment.NewLine}": "") +
						((Input is PythonTesseractExecutable inputpyt) ? ($"PF Version: {inputpyt.WeaponID.Version.VersionNumber} {Environment.NewLine}" +
						$"Weapon Type: {inputpyt.WeaponType} {Environment.NewLine}" +
						$"Command Executed: {inputpyt.CommandExecuted} {Environment.NewLine}" +
						$"FileDirectory {inputpyt.FileDirectory} {Environment.NewLine}") : "")
						);
				}
				if (((int)Destination & (int)OutputDestination.Console) == (int)OutputDestination.Console)
				{
					Console.WriteLine(Output?.OutputString);
				}
				manualEvent.Set();
			}
		}
	}
}