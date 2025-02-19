using PFDB.WeaponUtility;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections.Immutable;
using System.Diagnostics;
using PFDB.Logging;


namespace PFDB.SQLite
{
	/// <summary>
	/// Defines a class that interfaces with SQL databases, and retrieves data from them. This data is meant to be used downstream for the API, and allows quicker access than manually running impa(.py).
	/// </summary>
	public static class WeaponTable
	{
		private static string _databasePath = "weapon_database.db";
		private static string[] _categories = ["weaponName", "category", "categoryNumber","categoryShorthand", "versionRankTieBreaker", "rank", "uniqueWeaponID"];
		private enum _categoryNames
		{
			WeaponName, Category, CategoryNumber, CategoryShorthand, VersionRankTieBreaker, Rank, UniqueWeaponID
		}
		private static string _tableName = "cumulativeChanges";
		private static IDictionary<PhantomForcesVersion,HashSet<(WeaponIdentification weaponID, int categoryNumber, int weaponNumber)>> _weaponIDCache = new Dictionary<PhantomForcesVersion,HashSet<(WeaponIdentification, int, int)>>();
		private static IDictionary<PhantomForcesVersion,Dictionary<Categories, int>> _weaponCountsCache = new Dictionary<PhantomForcesVersion, Dictionary<Categories, int>>();
		private static IEnumerable<PhantomForcesVersion> _listOfVersions = new List<PhantomForcesVersion>();

		/// <summary>
		/// Defines a dictionary that maps a Phantom Forces version with a collection of weapon IDs for the particular version (also includes category and weapon numbers).
		/// </summary>
		public static Dictionary<PhantomForcesVersion, HashSet<(WeaponIdentification weaponID, int categoryNumber, int weaponNumber)>> WeaponIDCache { get { return (Dictionary<PhantomForcesVersion, HashSet<(WeaponIdentification weaponID, int categoryNumber, int weaponNumber)>>)_weaponIDCache; } }
		/// <summary>
		/// Defines a dictionary that maps a Phantom Forces version to a dictionary that maps each category (in the specified version) to the number of weapons in the specified category.
		/// </summary>
		public static Dictionary<PhantomForcesVersion, Dictionary<Categories,int>> WeaponCounts { get { return (Dictionary<PhantomForcesVersion, Dictionary<Categories, int>>)_weaponCountsCache; } }
		/// <summary>
		/// Lists all the versions in the database.
		/// </summary>
		public static List<PhantomForcesVersion> ListOfVersions { get { return _listOfVersions.ToList(); } }

		/// <summary>
		/// Initializes and populates all fields within this class for requests. Slow time to execute, so it is best for this to be called during initialization.
		/// </summary>
		/// <returns></returns>
		public static Stopwatch InitializeEverything()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			PFDBLogger.LogInformation("Starting cumulative table table setup");
			_cumulativeChangesTableSetup();
			PFDBLogger.LogInformation("Finished cumulative table table setup");
			PFDBLogger.LogInformation("Starting weapon counts");
			_getWeaponCountsForEveryVersion();
			PFDBLogger.LogInformation("Finished weapon counts");
			PFDBLogger.LogInformation("Starting weapon identifications");
			_getWeaponIdentificationsForEveryVersion();
			PFDBLogger.LogInformation("Finished weapon identifications");
			stopwatch.Stop();
			
			PFDBLogger.LogInformation($"Stopwatch elapsed milliseconds: {stopwatch.ElapsedMilliseconds}", parameter: stopwatch);
			return stopwatch;
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

		/// <summary>
		/// Sets up the cumulative changes table.
		/// Deletes the previous table if it exists, then inserts records from the very first recorded version available (version 8.0.0).
		/// </summary>
		private static void _cumulativeChangesTableSetup()
		{

			PFDBLogger.LogInformation($"Executing cumulative table setup.");
			Stopwatch stopwatch = Stopwatch.StartNew();
			_executeNonQuery($"DROP TABLE IF EXISTS {_tableName};");
			_executeNonQuery(
				$"CREATE TABLE {_tableName} (\n" +
					$"\t\"{_categories[(int)_categoryNames.WeaponName]}\" TEXT,\n" +
					$"\t\"{_categories[(int)_categoryNames.Category]}\" TEXT,\n" +
					$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\" INTEGER,\n" +
					$"\t\"{_categories[(int)_categoryNames.CategoryShorthand]}\" TEXT,\n" +
					$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\" INTEGER,\n" +
					$"\t\"{_categories[(int)_categoryNames.Rank]}\" INTEGER,\n" +
					$"\t\"{_categories[(int)_categoryNames.UniqueWeaponID]}\" INTEGER\n" +
				$");");
			_executeNonQuery(
				$"INSERT INTO {_tableName} (\n" +
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
				"FROM version800");
			stopwatch.Stop();
			PFDBLogger.LogInformation($"Elapsed milliseconds for cumulative table setup.", parameter: stopwatch);
		}

		/// <summary>
		/// Updates the unique weapon identifier for each weapon in the entire database.
		/// </summary>
		private static void _updateAllVersions()
		{
			IEnumerable<int> ints = GetListOfVersionIntegersInDatabase();
			foreach (int version in ints)
			{
				List<(string weaponName, int categoryNumber, int versionRankTieBreaker, int rank)> weaponNames = new List<(string, int, int, int)>();
				using (SQLiteConnection conn = new SQLiteConnection(_getConnectionString().ConnectionString))
				{
					using (SQLiteCommand cmd = conn.CreateCommand())
					{
						conn.Open();
						cmd.CommandText = 
							"SELECT \n" +
								$"\t\"{_categories[(int)_categoryNames.WeaponName]}\",\n" +
								$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
								$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
								$"\t\"{_categories[(int)_categoryNames.CategoryShorthand]}\",\n" +
								$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\",\n" +
								$"\t\"{_categories[(int)_categoryNames.Rank]}\"\n" +
							$"FROM version{version} \n";
						using (SQLiteDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								weaponNames.Add(
									(reader.GetString((int)_categoryNames.WeaponName),
									reader.GetInt32((int)_categoryNames.CategoryNumber),
									reader.GetInt32((int)_categoryNames.VersionRankTieBreaker),
									reader.GetInt32((int)_categoryNames.Rank)));
							}
						}
						conn.Close();
					}
				}
				_executeNonQuery($"ALTER TABLE version{version} ADD \"{_categories[(int)_categoryNames.UniqueWeaponID]}\" INTEGER;");
				foreach ((string weaponName, int categoryNumber, int versionRankTieBreaker, int rank) g in weaponNames)
				{
					WeaponIdentification id = new WeaponIdentification(new PhantomForcesVersion(version.ToString()), (Categories)g.Item2, g.Item4, g.Item3);
					_executeNonQuery($"UPDATE version{version} SET {_categories[(int)_categoryNames.UniqueWeaponID]} = {id.ID} WHERE version{version}.{_categories[(int)_categoryNames.WeaponName]} = \"{g.Item1}\"");
				}
			}
		}

		/// <summary>
		/// Updates the cumulative changes table. 
		/// <para>Consider if we have a version (like version 8.0.2). First, this function will delete duplicate entries from the table whose weapon name matches a weapon name from the current version. Therefore, the previous version's (8.0.0) entries will be overwritten by the current version's entries (8.0.2).</para>
		/// <para>Finally, all the records will be altered to include the unique weapon identifier.</para>
		/// </summary>
		/// <param name="currentVersion">The version to generate the cumulative table for.</param>
		public static void UpdateCumulativeChangesTable(PhantomForcesVersion currentVersion)
		{

			PFDBLogger.LogInformation($"Executing cumulative table update. Version: {currentVersion.VersionString}", parameter: currentVersion);
			Stopwatch stopwatch = Stopwatch.StartNew();
			IEnumerable<int> ints = GetListOfVersionIntegersInDatabase();
			foreach(int version in ints.Where(x => x <= currentVersion.VersionNumber && x > 800))
			{
				_executeNonQuery(
					$"DELETE FROM {_tableName} \n" +
					$"WHERE {_tableName}.{_categories[(int)_categoryNames.WeaponName]} \n" +
					$"IN (\n" +
						$"\tSELECT {_tableName}.{_categories[(int)_categoryNames.WeaponName]} \n" +
						$"\tFROM {_tableName} \n" +
						$"\tINNER JOIN version{version} \n" +
						$"\tON ({_tableName}.{_categories[(int)_categoryNames.WeaponName]} = version{version}.{_categories[(int)_categoryNames.WeaponName]}) \n" +
						$"\tWHERE {_tableName}.{_categories[(int)_categoryNames.WeaponName]} IS NOT NULL AND version{version}.{_categories[(int)_categoryNames.WeaponName]} IS NOT NULL \n" +
					$");");
				_executeNonQuery(
					$"INSERT INTO {_tableName} (\n" +
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
					$"FROM version{version}");
			}
			List<(string weaponName, int categoryNumber, int versionRankTieBreaker, int rank)> weaponNames = new List<(string, int, int, int)>();
			using (SQLiteConnection conn = new SQLiteConnection(_getConnectionString().ConnectionString))
			{
				using (SQLiteCommand cmd = conn.CreateCommand())
				{
					conn.Open();
					cmd.CommandText =
						"SELECT \n" +
							$"\t\"{_categories[(int)_categoryNames.WeaponName]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryShorthand]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Rank]}\"\n" +
						$"FROM {_tableName} \n";
					using (SQLiteDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							weaponNames.Add(
								(reader.GetString((int)_categoryNames.WeaponName),
								reader.GetInt32((int)_categoryNames.CategoryNumber),
								reader.GetInt32((int)_categoryNames.VersionRankTieBreaker),
								reader.GetInt32((int)_categoryNames.Rank)));
						}
					}
					conn.Close();
				}
			}

			foreach ((string weaponName, int categoryNumber, int versionRankTieBreaker, int rank) g in weaponNames)
			{
				WeaponIdentification id = new WeaponIdentification(currentVersion, (Categories)g.Item2, g.Item4, g.Item3);
				_executeNonQuery($"UPDATE {_tableName} SET {_categories[(int)_categoryNames.UniqueWeaponID]} = {id.ID} WHERE {_tableName}.{_categories[(int)_categoryNames.WeaponName]} = \"{g.Item1}\"");
			}
			stopwatch.Stop();
			PFDBLogger.LogInformation($"Elapsed milliseconds for cumulative table update.", parameter: stopwatch);
		}

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
		private static IEnumerable<PhantomForcesVersion> _getListOfVersionsInDatabase()
		{
			List<PhantomForcesVersion> ints = new List<PhantomForcesVersion>();
			using (SQLiteConnection conn = new SQLiteConnection(_getConnectionString().ConnectionString))
			{
				using (SQLiteCommand cmd = conn.CreateCommand())
				{
					conn.Open();
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE sqlite_master.type = \"table\";";
					Regex parser = new Regex(@"\d+");
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
				}
			}
			ints.Sort();
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

			_cumulativeChangesTableSetup();
			UpdateCumulativeChangesTable(version);
			using (SQLiteConnection conn = new SQLiteConnection(_getConnectionString().ConnectionString))
			{
				using (SQLiteCommand cmd = conn.CreateCommand())
				{
					conn.Open();
					cmd.CommandText = $"SELECT " +
							$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
						$"COUNT(DISTINCT \"{_categories[(int)_categoryNames.WeaponName]}\") \n" +
						$"FROM \"{_tableName}\" " +
						$"GROUP BY \"{_categories[(int)_categoryNames.CategoryNumber]}\"" +
						$"ORDER BY \"{_categories[(int)_categoryNames.CategoryNumber]}\" + 0;";
					using (SQLiteDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							int category = reader.GetInt32(1);
							if (category < 0 || category >= 19)
							{
								throw new SQLiteException("Category number was outside the acceptable number limits");
							}

							_weaponCountsCache.TryAdd(version,new Dictionary<Categories, int>() { { (Categories)category, reader.GetInt32(2) } }.ToDictionary());
						}
					}
				}
			}
			return _weaponCountsCache[version];
		}

		/// <summary>
		/// Returns a complete list of weapon counts for every version. Comprises of a dictionary for each version and associated <see cref="Dictionary{TKey, TValue}"/> where <c>TKey</c> is <see cref="PhantomForcesVersion"/> and <c>TValue</c> is <see cref="int"/>.
		/// </summary>
		/// <returns>A complete list of weapon counts for every version</returns>
		private static IDictionary<PhantomForcesVersion,Dictionary<Categories,int>> _getWeaponCountsForEveryVersion()
		{
			IDictionary<PhantomForcesVersion, Dictionary<Categories, int>> result = new Dictionary<PhantomForcesVersion, Dictionary<Categories, int>>();
			foreach (PhantomForcesVersion version in _listOfVersions.Any() ? _listOfVersions : _getListOfVersionsInDatabase())
			{
				result.TryAdd(version, _getWeaponCount(version).ToDictionary());
			}
			
			_weaponCountsCache = result;
			return result;
		}

		/// <summary>
		/// Returns a collection of tuples, where each tuple has a unique weapon identifier, its associated category number and weapon number (for each category).
		/// <para>Only returns for a single version of Phantom Forces.</para>
		/// </summary>
		/// <param name="version">The Phantom Forces version that contains the associated list of tuples.</param>
		/// <returns>A collection of tuples that associates a unique weapon identifier with the category number and weapon number.</returns>
		private static IEnumerable<(WeaponIdentification weaponID, int categoryNumber, int weaponNumber)> _getWeaponIdentifications(PhantomForcesVersion version)
		{

			UpdateCumulativeChangesTable(version);
			HashSet<(WeaponIdentification weaponID, int categoryNumber, int weaponNumber)> weapons = new HashSet<(WeaponIdentification, int, int)>();
			using (SQLiteConnection conn = new SQLiteConnection(_getConnectionString().ConnectionString))
			{
				using (SQLiteCommand cmd = conn.CreateCommand())
				{
					conn.Open();
					cmd.CommandText = "SELECT\n" +
							$"\t\"{_categories[(int)_categoryNames.WeaponName]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryShorthand]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Rank]}\" \n" +
						$"FROM {_tableName} \n" +
						$"ORDER BY \"{_categories[(int)_categoryNames.CategoryNumber]}\", \n" +
							$"\t\"{_categories[(int)_categoryNames.Rank]}\" + 0, \n" +
							$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\" + 0;";
					using (SQLiteDataReader reader = cmd.ExecuteReader())
					{
						int counter = 0;
						int category = 0;
						//we can use counter here because we already ordered it
						while (reader.Read())
						{
							weapons.Add(
								(new WeaponIdentification(version, (Categories)reader.GetInt32((int)_categoryNames.CategoryNumber), reader.GetInt32((int)_categoryNames.Rank), reader.GetInt32((int)_categoryNames.VersionRankTieBreaker)), category, counter)
								);
							if (category == reader.GetInt32((int)_categoryNames.CategoryNumber))
							{
								counter++;
							}
							else
							{
								counter = 0;
							}
							category = reader.GetInt32((int)_categoryNames.CategoryNumber);
							
						}
					}
				}
			}
			_weaponIDCache.TryAdd(version,weapons);
			return _weaponIDCache[version];
			
		}

		/// <summary>
		/// Gets weapon identifications and their associated category number and weapon number for each version in the database.
		/// </summary>
		/// <returns>A dictionary associating a <see cref="PhantomForcesVersion"/> with a result from <see cref="_getWeaponIdentifications(PhantomForcesVersion)"/>.</returns>
		private static IDictionary<PhantomForcesVersion,HashSet<(WeaponIdentification weaponID, int categoryNumber, int weaponNumber)>> _getWeaponIdentificationsForEveryVersion()
		{
			var result = new Dictionary<PhantomForcesVersion, HashSet<(WeaponIdentification weaponID, int categoryNumber, int weaponNumber)>>();
			foreach(PhantomForcesVersion version in _listOfVersions.Any() ? _listOfVersions : _getListOfVersionsInDatabase())
			{
				result.TryAdd(version, new HashSet<(WeaponIdentification weaponID, int categoryNumber, int weaponNumber)>(_getWeaponIdentifications(version)));
			}
			_weaponIDCache = result;
			return _weaponIDCache;
		}

		/// <summary>
		/// Gets a single unique weapon identifier for a weapon given its Phantom Forces version, its category number, and its weapon number.
		/// </summary>
		/// <param name="version">The Phantom Forces version that the weapon is contained within.</param>
		/// <param name="categoryNumber">The category number of the desired weapon.</param>
		/// <param name="weaponNumber">The weapon number of the desired weapon.</param>
		/// <returns>A <see cref="WeaponIdentification"/> object that is associated with the weapon.</returns>
		private static WeaponIdentification _getWeaponIdentification(PhantomForcesVersion version, int categoryNumber, int weaponNumber)
		{
			_cumulativeChangesTableSetup();
			UpdateCumulativeChangesTable(version);

			int rank = 0;
			int rankTieBreaker = 0;
			string weaponName= string.Empty;

			using (SQLiteConnection conn = new SQLiteConnection(_getConnectionString().ConnectionString))
			{
				using(SQLiteCommand cmd = conn.CreateCommand())
				{
					conn.Open();
					cmd.CommandText = "SELECT\n" +
							$"\t\"{_categories[(int)_categoryNames.WeaponName]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Category]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryNumber]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.CategoryShorthand]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\",\n" +
							$"\t\"{_categories[(int)_categoryNames.Rank]}\" \n" + 
						$"FROM {_tableName} \n" +
						$"ORDER BY \"{_categories[(int)_categoryNames.CategoryNumber]}\", \n" +
							$"\t\"{_categories[(int)_categoryNames.Rank]}\" + 0, \n" +
							$"\t\"{_categories[(int)_categoryNames.VersionRankTieBreaker]}\" + 0;";
					using (SQLiteDataReader reader = cmd.ExecuteReader())
					{
						int counter = 0;
						while (reader.Read())
						{
							int category = reader.GetInt32(2);

							//Console.WriteLine(reader.GetString(0));
							//Console.WriteLine($"{reader.GetString(1)}, \t{category}, \t{reader.GetString(3)}, \t{reader.GetInt32(4)}, \t{reader.GetInt32(5)}, \t{reader.GetString(0)}");

							if(category == categoryNumber)
							{
								weaponName = reader.GetString((int)_categoryNames.WeaponName);
								if(counter == weaponNumber)
								{
									rank = reader.GetInt32((int)_categoryNames.Rank);
									rankTieBreaker = reader.GetInt32((int)_categoryNames.VersionRankTieBreaker);
									//Console.WriteLine(reader.GetString(0));
									break;
								}
								counter++;
							}
						}
					}
					conn.Close();
				}
			}
			return new WeaponIdentification(version, (Categories)categoryNumber, rank, rankTieBreaker, weaponName);
		}

	}
}
