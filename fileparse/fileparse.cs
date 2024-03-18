using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PFDB.WeaponUtility;

Console.Write("");

namespace PFDB
{
    namespace Parsing
    {

        public class WordNotFoundException : Exception
		{
			public WordNotFoundException() { }
			public WordNotFoundException(string message) : base(message) { }
			public WordNotFoundException(string message, Exception inner) : base(message, inner) { }

		}

		/// <summary>
		/// The search targets for statistics
		/// </summary>
		public enum SearchTargets
		{
			Rank,
			//guns
			Damage,
			DamageRange,
			Firerate,
			AmmoCapacity,
			HeadMultiplier,
			TorsoMultiplier,
			LimbMultiplier,
			MuzzleVelocity,
			Suppression,
			PenetrationDepth,
			ReloadTime,
			EmptyReloadTime,
			WeaponWalkspeed,
			AimingWalkspeed,
			AmmoType,
			SightMagnification,
			MinimumTimeToKill,
			HipfireSpreadFactor,
			HipfireRecoverySpeed,
			HipfireSpreadDamping,
			HipChoke,
			AimChoke,
			EquipSpeed,
			AimModelSpeed,
			AimMagnificationSpeed,
			CrosshairSize,
			CrosshairSpreadRate,
			CrosshairRecoverRate,
			FireModes,
			//grenades
			BlastRadius,
			KillingRadius,
			MaximumDamage,
			TriggerMechanism,
			SpecialEffects,
			StoredCapacity,
			//melees
			FrontStabDamage,
			BackStabDamage,
			MainAttackTime,
			MainAttackDelay,
			AltAttackTime,
			AltAttackDelay,
			QuickAttackTime,
			QuickAttackDelay,
			Walkspeed
		}



		/// <summary>
		/// This class handles the text from the files generated after PyTesseract
		/// </summary>
		public sealed class FileParse : IFileParse
		{
			private string _filetext = string.Empty;
			private PhantomForcesVersion _version;

			/// <summary>
			/// Default constructor.
			/// </summary>
			/// <param name="version">Specifies the version of Phantom Forces for the text provided.</param>
			public FileParse(PhantomForcesVersion version)
			{
				_version = version;
			}

			/// <summary>
			/// Reads a file. Throws <see cref="ArgumentNullException"/> if filepath is null, and <see cref="FileNotFoundException"/> if the file does not exist.
			/// </summary>
			/// <param name="filepath">Path to specified file.</param>
			/// <returns>Returns <see cref="string.Empty"/> if the reading failed at all, otherwise returns the text content of the file.</returns>
			/// <exception cref="ArgumentNullException"></exception>
			/// <exception cref="FileNotFoundException"></exception>
			public string fileReader(string filepath)
			{
                if (filepath == null) throw new ArgumentNullException(nameof(filepath), "File path specified cannot be null.");
                if (!File.Exists(filepath)) throw new FileNotFoundException($"File not found.", filepath);
                string output;
                try
                {
                    output = File.ReadAllText(filepath);
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
            /// <param name="acceptableSpaces">Specifies the acceptable number spaces between both words. Default is set to 3.</param>
            /// <param name="acceptableCorruptedWordSpaces">Specifies the acceptable number spaces that a corrupted word can have. Default is set to 3.</param>
            /// <param name="consoleWrite">Set to true to print to the console, false otherwise.</param>
            /// <returns>Returns a list of strings of the statistics of the file. May be empty.</returns>
            public IEnumerable<string> findAllStatisticsInFile(WeaponType weaponType, int acceptableSpaces = 3, int acceptableCorruptedWordSpaces = 3, bool consoleWrite = false)
			{

				List<string> temp = new List<string>();
                StatisticParse statisticParse = new StatisticParse(_version, _filetext, acceptableSpaces, acceptableCorruptedWordSpaces, consoleWrite);
                foreach (SearchTargets target in Enum.GetValues(typeof(SearchTargets)))
				{
					try
					{
						temp.Add(statisticParse.findStatisticInFile(target, weaponType, new List<char>() {  (char)13, (char)10 }));
						_filetext = statisticParse.Filetext; //update, so corrupted words get fixed
					}
					catch
					{
						
						//do nothing
					}
                }
				
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
            public IDictionary<SearchTargets, string> findAllStatisticsInFileWithTypes(WeaponType weaponType, int acceptableSpaces = 3, int acceptableCorruptedWordSpaces = 3, bool consoleWrite = false)
			{
				IDictionary<SearchTargets, string> temp = new Dictionary<SearchTargets, string>();
				IStatisticParse statisticParse = new StatisticParse(_version, _filetext, acceptableSpaces, acceptableCorruptedWordSpaces, consoleWrite);
				foreach (SearchTargets target in Enum.GetValues(typeof(SearchTargets)))
				{
					try
					{
						temp.Add(target, statisticParse.findStatisticInFile(target, weaponType, new List<char>() {  (char)13, (char)10 }));
                        _filetext = statisticParse.Filetext; //update, so corrupted words get fixed
                    }
					catch
					{
						//do nothing
					}
				}
				return temp;
			}


		}
	}
}