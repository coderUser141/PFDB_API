using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PFDB.Logging;
using PFDB.ParsingUtility;
using PFDB.WeaponUtility;

Console.Write("");

namespace PFDB
{
	namespace Parsing
	{
		/// <summary>
		/// This class handles the text from the files generated after PyTesseract
		/// </summary>
		public sealed class FileParse : IFileParse
		{
			private string _filetext = string.Empty;
			private PhantomForcesVersion _version;
			private string _filepath = string.Empty;

			/// <summary>
			/// Default constructor.
			/// </summary>
			/// <param name="version">Specifies the version of Phantom Forces for the text provided.</param>
			public FileParse(PhantomForcesVersion version)
			{
				_version = version;
			}

			/// <summary>
			/// Destructor. Writes the file after we are done handling it, so any corrupted words are fixed.
			/// </summary>
			~FileParse(){
				File.WriteAllText( _filepath, _filetext );
			}

			/// <summary>
			/// Reads a file. Throws <see cref="ArgumentNullException"/> if filepath is null, and <see cref="FileNotFoundException"/> if the file does not exist.
			/// </summary>
			/// <param name="filepath">Path to specified file.</param>
			/// <returns>Returns <see cref="string.Empty"/> if the reading failed at all, otherwise returns the text content of the file.</returns>
			/// <exception cref="ArgumentNullException"></exception>
			/// <exception cref="FileNotFoundException"></exception>
			public string FileReader(string filepath)
			{
				if (filepath == null)
				{
					PFDBLogger.LogFatal("File path specified cannot be null.", parameter: nameof(filepath));
					throw new ArgumentNullException(nameof(filepath), "File path specified cannot be null.");
				}
				if (File.Exists(filepath) == false) {
					PFDBLogger.LogFatal($"The path specified at {filepath} does not exist. Ensure that the directory and file exists, then try again.");
					throw new FileNotFoundException($"File not found.", filepath); 
				}
				string output;
				try
				{
					output = File.ReadAllText(filepath);
					_filepath = filepath;
				}
				catch
				{
					output = string.Empty;
				}


				_filetext = output;
				return output;
			}

			/// <summary>
			/// Finds all the statistics in a file.
			/// </summary>
			/// <param name="weaponType">Type of the weapon to be scanned.</param>
			/// <param name="acceptableSpaces">Specifies the acceptable number spaces between words. Default is set to 3.</param>
			/// <param name="acceptableCorruptedWordSpaces">Specifies the acceptable number spaces that a corrupted word can have. Default is set to 3.</param>
			/// <param name="consoleWrite">Set to true to print to the console, false otherwise.</param>
			/// <returns>Returns a list of strings of the statistics of the file. May be empty.</returns>
			public IEnumerable<string> FindAllStatisticsInFile(WeaponType weaponType, int acceptableSpaces = 3, int acceptableCorruptedWordSpaces = 3, bool consoleWrite = false)
			{

				List<string> temp = FindAllStatisticsInFileWithTypes(weaponType, acceptableSpaces, acceptableCorruptedWordSpaces, consoleWrite).Values.ToList();
				
				return temp;
			}

			/// <summary>
			/// Finds all the statistics in a file.
			/// </summary>
			/// <param name="weaponType">Type of the weapon to be scanned.</param>
			/// <param name="acceptableSpaces">Specifies the acceptable number spaces between both words. Default is set to 3.</param>
			/// <param name="acceptableCorruptedWordSpaces">Specifies the acceptable number spaces that a corrupted word can have. Default is set to 3.</param>
			/// <param name="consoleWrite">Set to true to print to the console, false otherwise.</param>
			/// <returns>Returns an <see cref="IDictionary{TKey, TValue}"/> where <c>TKey</c> is <see cref="SearchTargets"/> and it matches a corresponding <c>TValue</c> which has the statistic.</returns>
			public IDictionary<SearchTargets, string> FindAllStatisticsInFileWithTypes(WeaponType weaponType, int acceptableSpaces = 3, int acceptableCorruptedWordSpaces = 3, bool consoleWrite = false)
			{
				IDictionary<SearchTargets, string> temp = new Dictionary<SearchTargets, string>();
				IStatisticParse statisticParse = new StatisticParse(_version, _filetext, acceptableSpaces, acceptableCorruptedWordSpaces, consoleWrite);

				foreach (SearchTargets target in ParsingUtilityClass.GetSearchTargetsForWeapon(weaponType))
				{
					try
					{
						temp.Add(target, statisticParse.FindStatisticInFile(target, weaponType, new List<char>() {  (char)13, (char)10 }));
						_filetext = statisticParse.Filetext; //update, so corrupted words get fixed
					}
					catch(WordNotFoundException ex)
					{
						PFDBLogger.LogInformation($"An exception was raised while searching the file. Internal Message: {ex.Message}", parameter: $"In {_filepath} searching for {target}");

					}
					catch (ArgumentException)
					{
						continue;
					}
					catch(Exception ex)
					{
						PFDBLogger.LogWarning($"An exception was raised while searching the file. Internal Message: {ex.Message}", parameter: $"In {_filepath} searching for {target}");
						//do nothing
					}
				}
				return temp;
			}


		}
	}
}