using PFDB.WeaponUtility;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections.Immutable;
using System.Diagnostics;
using PFDB.Logging;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Runtime.CompilerServices;
using System.Xml.Schema;


namespace PFDB.SQLite
{
	/// <summary>
	/// Defines a class that interfaces with SQL databases, and retrieves data from them. This data is meant to be used downstream for the API, and allows quicker access than manually running impa(.py).
	/// </summary>
	public static class WeaponTable
	{
		//path to the database
		private static string _databasePath = "weapon_database.db";


		private static string[] _categories = ["weaponName", "category", "categoryNumber", "categoryShorthand", "versionRankTieBreaker", "rank", "uniqueWeaponID"];
		private enum _categoryNames
		{
			WeaponName, Category, CategoryNumber, CategoryShorthand, VersionRankTieBreaker, Rank, UniqueWeaponID
		}
		private static string _tableName = "cumulativeChanges";


		//unpopulated fields
		private static IDictionary<PhantomForcesVersion, HashSet<WeaponIdentification>>
						_weaponIDCache
			= new Dictionary<PhantomForcesVersion, HashSet<WeaponIdentification>>();
		private static IDictionary<PhantomForcesVersion, Dictionary<Categories, int>> _weaponCountsCache = new Dictionary<PhantomForcesVersion, Dictionary<Categories, int>>();
		private static IEnumerable<PhantomForcesVersion> _listOfVersions = new List<PhantomForcesVersion>();

		/// <summary>
		/// Defines a dictionary that maps a Phantom Forces version with a collection of weapon IDs for the particular version (also includes category and weapon numbers).
		/// </summary>
		public static Dictionary<PhantomForcesVersion,HashSet<WeaponIdentification>>
		WeaponIDCache
		{
			get
			{
				return (Dictionary<PhantomForcesVersion,HashSet<WeaponIdentification>>)_weaponIDCache;
			}
		}


		/// <summary>
		/// Defines a dictionary that maps a Phantom Forces version to a dictionary that maps each category (in the specified version) to the number of weapons in the specified category.
		/// </summary>
		public static Dictionary<
						PhantomForcesVersion,
						Dictionary<Categories, int>>
			WeaponCounts
		{
			get
			{
				return (Dictionary<
						PhantomForcesVersion,
						Dictionary<Categories, int>>)_weaponCountsCache;
			}
		}


		/// <summary>
		/// Lists all the versions in the database. Do not remove .ToList(), as it makes a copy.
		/// </summary>
		public static List<PhantomForcesVersion> ListOfVersions { get { return _listOfVersions.ToList(); } }

		/// <summary>
		/// Initializes and populates all fields within this class for requests. Slow time to execute, so it is best for this to be called during initialization.
		/// </summary>
		/// <returns></returns>
		public static (bool success, Stopwatch stopwatch) InitializeEverything()
		{
			bool success = false;
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				PFDBLogger.LogInformation("Starting retrieval of list of versions");
				_setUpListOfVersionsInDatabase();
				PFDBLogger.LogInformation("Finished retrieval of list of versions");
				PFDBLogger.LogDebug("_listOfVersions populated");

				PFDBLogger.LogInformation("Starting cumulative tables setup and weapon identifications caching");
				_generateIndividualCumulativeChangesTablesUpToSpecificVersion(null);
				PFDBLogger.LogInformation("Finished cumulative tables setup and weapon identifications caching");
				PFDBLogger.LogDebug("_weaponIDCache populated (with weaponNames)");

				PFDBLogger.LogInformation("Starting weapon counts caching");
				_getWeaponCountsForEveryVersion();
				PFDBLogger.LogInformation("Finished weapon counts caching");
				PFDBLogger.LogDebug("_weaponCountsCache populated");
				success = true;
			}
			catch (SQLiteException exception)
			{
				PFDBLogger.LogFatal("Unable to interface with SQLite database. Check if it exists and try again.", exception.Message);
			}
			catch (Exception exception)
			{
				PFDBLogger.LogError("An error occured while interfacing with the SQLite database.", exception.Message);
			}
			stopwatch.Stop();

			PFDBLogger.LogInformation($"Stopwatch elapsed milliseconds: {stopwatch.ElapsedMilliseconds}", parameter: stopwatch);
			return (success, stopwatch);
		}


		/// <summary>
		/// Builds a connection string for the weapon database. Assumes that the database is in the current working directory if <c>directory</c> is null.
		/// </summary>
		/// <param name="directory">Optional. Assigned to null by default, meaning that it will look for the database in the current directory.</param>
		/// <returns>A connection stringbuilder from where the connection string can be obtained.</returns>
		public static SQLiteConnectionStringBuilder _getConnectionString(string? directory = null)
		{
			string actualpath = directory ?? Directory.GetCurrentDirectory();
			char slash = Directory.Exists("/usr/bin") ? '/' : '\\';
			SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder($@"Data Source={actualpath}{slash}{_databasePath};Version=3;FailIfMissing=True;");
			return builder;
		}

		/// <summary>
		/// Template class boilerplate that executes an SQL non-querying statement (no values are returned).
		/// </summary>
		/// <param name="commandText">The SQL statement to execute.</param>
		/// <returns>The number of rows affected by the SQL statement.</returns>
		private static int _executeNonQuery(string commandText)
		{
			PFDBLogger.LogDebug($"Executing non-query. Command text: {commandText}", parameter: commandText);
			Stopwatch stopwatch = Stopwatch.StartNew();
			int t = 0;
			using (SQLiteConnection conn = new SQLiteConnection(_getConnectionString().ConnectionString))
			{
				using (SQLiteCommand cmd = conn.CreateCommand())
				{
					conn.Open();
					cmd.CommandText = commandText;
					t = cmd.ExecuteNonQuery();
					conn.Close();
				}
			}
			stopwatch.Stop();
			PFDBLogger.LogDebug($"Elapsed milliseconds for non-query: {stopwatch.ElapsedMilliseconds}", parameter: stopwatch);
			return t;
		}

		//public static void _cumulativeChangesTableSetup()

		/// <summary>
		/// Verifies if then specified version is contained within the database.
		/// </summary>
		/// <param name="version">The version to verify if it is in the database.</param>
		/// <returns>True if the version is found within the database, false otherwise.</returns>
		public static bool VerifyVersionIsInDatabase(PhantomForcesVersion version)
		{
			return _listOfVersions.Contains(version);
		}

		/// <summary>
		/// Verifies if then specified version is contained within the database.
		/// </summary>
		/// <param name="version">The version to verify if it is in the database.</param>
		/// <returns>True if the version is found within the database, false otherwise.</returns>
		public static bool VerifyVersionIsInDatabase(int version)
		{
			return _listOfVersions.Where(x => x.VersionNumber == version).Any();
		}

		/// <summary>
		/// Gets the list of versions that are contained within the database.
		/// </summary>
		/// <returns>A list containing integers, derived from <see cref="PhantomForcesVersion.VersionNumber"/>.</returns>
		public static IEnumerable<int> GetListOfVersionIntegersInDatabase()
		{
			return _listOfVersions.Select(x => x.VersionNumber);
		}

		/// <summary>
		/// Gets the list of versions that are contained within the database. Updates <see cref="_listOfVersions"/> and thus <see cref="ListOfVersions"/> with the results.
		/// </summary>
		/// <returns>A list of <see cref="PhantomForcesVersion"/> indicating the versions.</returns>
		private static IEnumerable<PhantomForcesVersion> _setUpListOfVersionsInDatabase()
		{
			List<PhantomForcesVersion> ints = new List<PhantomForcesVersion>();
			using (SQLiteConnection conn = new SQLiteConnection(_getConnectionString().ConnectionString))
			{
				using (SQLiteCommand cmd = conn.CreateCommand())
				{
					conn.Open();
					Stopwatch stopwatch = Stopwatch.StartNew();
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE sqlite_master.type = \"table\";";
					Regex parser = new Regex(@"\d+$");
					using (SQLiteDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							string line = reader.GetString(0);
							Match currentLineMatcher = parser.Match(line);
							if (currentLineMatcher.Success)
							{
								try
								{
									ints.Add(new PhantomForcesVersion(currentLineMatcher.Value));
								}
								catch
								{
									continue;
								}
							}
						}
					}
					stopwatch.Stop();
					conn.Close();

				}
			}
			ints.Sort(); //NEVER REMOVE THIS LINE
			_listOfVersions = ints;
			return ints;
		}

		/// <summary>
		/// Gets a dictionary of categories with their associated number of weapons in the category. Note that this depends on the Phantom Forces version specified.
		/// </summary>
		/// <param name="version">The version to retreive the results from.</param>
		/// <returns>A dictionary of categories with their associated number of weapons in the category.</returns>
		/// <exception cref="SQLiteException"></exception>
		private static IDictionary<Categories, int> _getWeaponCount(PhantomForcesVersion version)
		{
			try
			{
				using (SQLiteConnection conn = new SQLiteConnection(_getConnectionString().ConnectionString))
				{
					using (SQLiteCommand cmd = conn.CreateCommand())
					{
						conn.Open();
						cmd.CommandText = $"SELECT " +
								$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
								$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
							$"COUNT(DISTINCT \"{_categories[(int)_categoryNames.WeaponName]}\") \n" +
							$"FROM \"version{version.VersionNumber}cumulative\" " +
							$"GROUP BY \"{_categories[(int)_categoryNames.CategoryNumber]}\"" +
							$"ORDER BY \"{_categories[(int)_categoryNames.CategoryNumber]}\" + 0;";
						using (SQLiteDataReader reader = cmd.ExecuteReader())
						{
							IDictionary<Categories, int> tempDictionary = new Dictionary<Categories, int>();
							while (reader.Read())
							{
								int category = reader.GetInt32(1);
								if (category < 0 || category >= 19)
								{
									throw new SQLiteException("Category number was outside the acceptable number limits");
								}
								//Console.WriteLine(reader.GetInt32(1) + ", " + reader.GetInt32(2));
								tempDictionary.Add((Categories)category, reader.GetInt32(2));
							}
							_weaponCountsCache.TryAdd(version, tempDictionary.ToDictionary());
						}
						conn.Close();
					}
				}
			}
			catch (SQLiteException error)
			{
				PFDBLogger.LogError($"An exception happened during the execution of the SQLite command. Most likely the table was not set up properly. Internal error message: {error.Message}");
				throw new Exception("An error occured.");
			}
			return _weaponCountsCache[version];
		}

		/// <summary>
		/// Returns a complete list of weapon counts for every version. Comprises of a dictionary for each version and associated <see cref="Dictionary{TKey, TValue}"/> where <c>TKey</c> is <see cref="PhantomForcesVersion"/> and <c>TValue</c> is <see cref="int"/>.
		/// </summary>
		/// <returns>A complete list of weapon counts for every version</returns>
		private static IDictionary<PhantomForcesVersion, Dictionary<Categories, int>> _getWeaponCountsForEveryVersion()
		{
			try
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				using (SQLiteConnection conn = new SQLiteConnection(_getConnectionString().ConnectionString))
				{
					using (SQLiteCommand cmd = conn.CreateCommand())
					{
						conn.Open();
						foreach (PhantomForcesVersion version in _listOfVersions)
						{
							cmd.CommandText = $"SELECT " +
									$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
									$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
								$"COUNT(DISTINCT \"{_categories[(int)_categoryNames.WeaponName]}\") \n" +
								$"FROM \"version{version.VersionNumber}cumulative\" " +
								$"GROUP BY \"{_categories[(int)_categoryNames.CategoryNumber]}\"" +
								$"ORDER BY \"{_categories[(int)_categoryNames.CategoryNumber]}\" + 0;";
							using (SQLiteDataReader reader = cmd.ExecuteReader())
							{
								IDictionary<Categories, int> tempDictionary = new Dictionary<Categories, int>();
								while (reader.Read())
								{
									int category = reader.GetInt32(1);
									if (category < 0 || category >= 19)
									{
										throw new SQLiteException("Category number was outside the acceptable number limits");
									}
									//Console.WriteLine(reader.GetInt32(1) + ", " + reader.GetInt32(2));
									tempDictionary.Add((Categories)category, reader.GetInt32(2));
								}
								_weaponCountsCache.TryAdd(version, tempDictionary.ToDictionary());
							}
						}
						conn.Close();
					}
				}
			}
			catch (SQLiteException error)
			{
				PFDBLogger.LogError($"An exception happened during the execution of the SQLite command. Most likely the table was not set up properly. Internal error message: {error.Message}");
				throw new Exception("An error occured.");
			}
			return _weaponCountsCache;
		}


		//private static IEnumerable<(WeaponIdentification weaponID, int categoryNumber, int weaponNumber)> _getWeaponIdentifications(PhantomForcesVersion version)
		//private static IDictionary<PhantomForcesVersion, HashSet<(WeaponIdentification weaponID, int categoryNumber, int weaponNumber)>> _getWeaponIdentificationsForEveryVersion()
		//private static WeaponIdentification _getWeaponIdentification(PhantomForcesVersion version, int categoryNumber, int weaponNumber)

		private static void _generateIndividualCumulativeChangesTablesUpToSpecificVersion(PhantomForcesVersion? targetVersion)
		{
			//if targetVersion == null, use maximum 



			/*
			 *
			 *	Some background: the tables named "version800", "version802", etc. all have weapons that change with that version.
			 *	I used to use a single cumulative table that accumulated all the changes until the present version, but that had its limitations.
			 *	It would rewrite itself if you wished to access a different cumulative version, so I have decided to make a cumulative table for each version.
			 *
			 *	High level overview + goals:
			 *	1. Create cumulative changes table. This table will be a cache when working. (scrapped)
			 *	2. Create cumulative tables for a specific version, i.e. version800cumulative
			 *	3. Clean each cumulative table and rebuild it
			 *	4. Generate weapon IDs on each table 
			 *	5. Dump data from versioned cumulative table at user command
			 *	6. idk something
			 *	
			 */


			IEnumerable<int> versionInts = GetListOfVersionIntegersInDatabase();

			Stopwatch stopwatch = Stopwatch.StartNew();

			StringBuilder cleanText = new StringBuilder(string.Empty); //cleans individual cumulative changes tables
			Dictionary<int, string> versionAndCommandPairs = new Dictionary<int, string>();
			StringBuilder deletionCommandText = new StringBuilder(string.Empty); //removes duplicate named entries from the cumulative changes tables (usually modified weapons) and inserts the new 

			/*
				Usually the command looks like:

				DELETE FROM cumulativeChanges 
				WHERE cumulativeChanges.weaponName 
				IN (
					SELECT cumulativeChanges.weaponName 
					FROM cumulativeChanges 
					INNER JOIN version1001 
					ON (cumulativeChanges.weaponName = version1001.weaponName) 
					WHERE cumulativeChanges.weaponName IS NOT NULL AND version1001.weaponName IS NOT NULL 
				);INSERT INTO cumulativeChanges (
					"weaponName",
					"category",
					"categoryNumber",
					"categoryShorthand",
					"versionRankTieBreaker",
					"rank"
				) SELECT 
					"weaponName",
					"category",
					"categoryNumber",
					"categoryShorthand",
					"versionRankTieBreaker",
					"rank"
				FROM version1001;
			*/


			List<PhantomForcesVersion> listOfVersions = _listOfVersions.ToList();
			listOfVersions.Sort();
			PhantomForcesVersion maximumVersion = targetVersion ?? listOfVersions.Last();
			PFDBLogger.LogInformation(maximumVersion.VersionNumber.ToString());

			foreach (int limitVersion in versionInts.Where(x => x <= maximumVersion.VersionNumber && x > 800))
			{
				foreach (int currentVersion in versionInts.Where(x => x <= limitVersion))
				{
					deletionCommandText.Append(
						$"DELETE FROM version{currentVersion}cumulative\n" +
						$"WHERE version{currentVersion}cumulative.{_categories[(int)_categoryNames.WeaponName]} \n" +
						$"IN (\n" +
							$"\tSELECT version{currentVersion}cumulative.{_categories[(int)_categoryNames.WeaponName]} \n" +
							$"\tFROM version{currentVersion}cumulative \n" +
							$"\tINNER JOIN version{currentVersion} \n" +
							$"\tON (version{currentVersion}cumulative.{_categories[(int)_categoryNames.WeaponName]} = version{currentVersion}.{_categories[(int)_categoryNames.WeaponName]}) \n" +
							$"\tWHERE version{currentVersion}cumulative.{_categories[(int)_categoryNames.WeaponName]} IS NOT NULL AND version{currentVersion}.{_categories[(int)_categoryNames.WeaponName]} IS NOT NULL \n" +
						$");" +
						$"INSERT INTO version{currentVersion}cumulative (\n" +
							$"\t\"{_categories[(int)_categoryNames.WeaponName]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryShorthand]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Rank]}\"\n" +
						$") SELECT \n" +
							$"\t\"{_categories[(int)_categoryNames.WeaponName]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryShorthand]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Rank]}\"\n" +
						$"FROM version{currentVersion};\n"
					);
				}
			}

			foreach (int currentVersion in versionInts.Where(x => x <= maximumVersion.VersionNumber && x >= 800))
			{
				cleanText.Append(
					$"DROP TABLE IF EXISTS version{currentVersion}cumulative;" +
					$"CREATE TABLE version{currentVersion}cumulative (\n" +
						$"\t\"{_categories[(int)_categoryNames.WeaponName]}\" TEXT,\n" +
						$"\t\"{_categories[(int)_categoryNames.Category]}\" TEXT,\n" +
						$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\" INTEGER,\n" +
						$"\t\"{_categories[(int)_categoryNames.CategoryShorthand]}\" TEXT,\n" +
						$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\" INTEGER,\n" +
						$"\t\"{_categories[(int)_categoryNames.Rank]}\" INTEGER,\n" +
						$"\t\"{_categories[(int)_categoryNames.UniqueWeaponID]}\" INTEGER\n" +
					$");" +
					$"INSERT INTO version{currentVersion}cumulative (\n" +
						$"\t\"{_categories[(int)_categoryNames.WeaponName]}\",\n" +
						$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
						$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
						$"\t\"{_categories[(int)_categoryNames.CategoryShorthand]}\",\n" +
						$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\",\n" +
						$"\t\"{_categories[(int)_categoryNames.Rank]}\"\n" +
					$") SELECT \n" +
						$"\t\"{_categories[(int)_categoryNames.WeaponName]}\",\n" +
						$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
						$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
						$"\t\"{_categories[(int)_categoryNames.CategoryShorthand]}\",\n" +
						$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\",\n" +
						$"\t\"{_categories[(int)_categoryNames.Rank]}\"\n" +
					"FROM version800;"
				);
			}
			cleanText.Append(deletionCommandText);

			//the SQL command has been prepared to set up the tables up to this point


			Dictionary<PhantomForcesVersion, HashSet<WeaponIdentification>> weaponKeyDictionary = new Dictionary<PhantomForcesVersion, HashSet<WeaponIdentification>>();


			using (SQLiteConnection connection = new SQLiteConnection(_getConnectionString(null).ConnectionString))
			{
				using (SQLiteCommand command = connection.CreateCommand())
				{
					connection.Open();
					command.CommandText = cleanText.ToString();

					Stopwatch stopwatch1 = Stopwatch.StartNew();
					command.ExecuteNonQuery();
					stopwatch1.Stop();
					PFDBLogger.LogStopwatch(stopwatch1, $"Clean duration");


					string updateCommandText = string.Empty;
					foreach (PhantomForcesVersion currentVersion in _listOfVersions.Where(x => x <= maximumVersion && x >= new PhantomForcesVersion("8.0.0")))
					{
						int currentVersionNumber = currentVersion.VersionNumber;
						stopwatch1.Restart();
						command.CommandText =
						"SELECT \n" +
							$"\t\"{_categories[(int)_categoryNames.WeaponName]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryShorthand]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Rank]}\"\n" +
						$"FROM version{currentVersionNumber}cumulative;\n";

						PFDBLogger.LogDebug(command.CommandText);
						HashSet<WeaponIdentification> currentVersionRowResults = new HashSet<WeaponIdentification>();

						using (SQLiteDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								//Console.WriteLine($"{currentVersionNumber}, {reader.GetInt32((int)_categoryNames.CategoryNumber)}, {reader.GetInt32((int)_categoryNames.VersionRankTieBreaker)}, {reader.GetInt32((int)_categoryNames.Rank)}");
								currentVersionRowResults.Add(
									new WeaponIdentification(
										currentVersion,
										(Categories)reader.GetInt32((int)_categoryNames.CategoryNumber),
										reader.GetInt32((int)_categoryNames.Rank),
										reader.GetInt32((int)_categoryNames.VersionRankTieBreaker),
										reader.GetString((int)_categoryNames.WeaponName)
									)
								);
							}
						}
						stopwatch1.Stop();
						PFDBLogger.LogStopwatch(stopwatch1, $"Read for version {currentVersionNumber} duration");

						weaponKeyDictionary.Add(currentVersion, currentVersionRowResults);

						foreach (WeaponIdentification currentRowResults in weaponKeyDictionary[currentVersion])
						{
							updateCommandText += $"UPDATE version{currentVersionNumber}cumulative SET {_categories[(int)_categoryNames.UniqueWeaponID]} = {currentRowResults.ID} WHERE version{currentVersionNumber}cumulative.{_categories[(int)_categoryNames.WeaponName]} = \"{currentRowResults.WeaponName}\";";

						}
					}
					stopwatch1.Restart();
					command.CommandText = updateCommandText;
					command.ExecuteNonQuery();
					stopwatch1.Stop();
					PFDBLogger.LogStopwatch(stopwatch1, $"Update duration");
					PFDBLogger.LogDebug(command.CommandText);



					connection.Close();
				}
			}

			stopwatch.Stop();
			PFDBLogger.LogStopwatch(stopwatch, $"Total duration");
			_weaponIDCache = weaponKeyDictionary;



		}


	}
}
