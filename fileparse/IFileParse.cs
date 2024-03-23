using PFDB.ParsingUtility;
using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.IO;

namespace PFDB.Parsing
{
	/// <summary>
	/// Interface for defining how to parse a .pfdb file.
	/// </summary>
	public interface IFileParse
	{
		/// <summary>
		/// Reads the file. Throws <see cref="ArgumentNullException"/> if filepath is null, and <see cref="FileNotFoundException"/> if the file does not exist.
		/// </summary>
		/// <param name="filepath">The path to the file.</param>
		/// <returns>A string containing the contents of the file.</returns>
		public string FileReader(string filepath);

		/// <summary>
		/// Finds all the statistics in a file.
		/// </summary>
		/// <param name="weaponType">Type of the weapon to be scanned.</param>
		/// <param name="acceptableSpaces">Specifies the acceptable number spaces between words. Default is set to 3.</param>
		/// <param name="acceptableCorruptedWordSpaces">Specifies the acceptable number spaces that a corrupted word can have. Default is set to 3.</param>
		/// <param name="consoleWrite">Set to true to print to the console, false otherwise.</param>
		/// <returns>Returns a list of strings of the statistics of the file. May be empty.</returns>
		public IEnumerable<string> FindAllStatisticsInFile(WeaponType weaponType, int acceptableSpaces = 3, int acceptableCorruptedWordSpaces = 3, bool consoleWrite = false);

		/// <summary>
		/// Finds all the statistics in a file.
		/// </summary>
		/// <param name="weaponType">Type of the weapon to be scanned.</param>
		/// <param name="acceptableSpaces">Specifies the acceptable number spaces between both words. Default is set to 3.</param>
		/// <param name="acceptableCorruptedWordSpaces">Specifies the acceptable number spaces that a corrupted word can have. Default is set to 3.</param>
		/// <param name="consoleWrite">Set to true to print to the console, false otherwise.</param>
		/// <returns>Returns an <see cref="IDictionary{TKey, TValue}"/> where <c>TKey</c> is <see cref="SearchTargets"/> and it matches a corresponding <c>TValue</c> which has the statistic.</returns>
		public IDictionary<SearchTargets, string> FindAllStatisticsInFileWithTypes(WeaponType weaponType, int acceptableSpaces = 3, int acceptableCorruptedWordSpaces = 3, bool consoleWrite = false);
	}
}
