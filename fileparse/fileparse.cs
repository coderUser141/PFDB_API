// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

//Console.WriteLine(Fileparse.findStatisticInFile("text.txt", );
Console.WriteLine("hi");





/// <summary>
/// This class handles the text from the files generated after PyTesseract
/// </summary>
public static class FileParse
{
	public static void Main(string[] args)
	{
		return;
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
	/// Finds all the indexes of the specified word in the text. Note that the word is matched regardless of uppercase or lowercase.
	/// </summary>
	/// <param name="filetext">Text to be searched.</param>
	/// <param name="word">Word to search for.</param>
	/// <returns>A <see cref="List{T}"/> containing zero-based non-negative integers with indices of where the word was found.</returns>
	private static List<int> indexFinder(string filetext, string word)
	{
		return indexFinderExtended(filetext, word, 0, filetext.Length, StringComparison.CurrentCultureIgnoreCase);
	}

	/// <summary>
	/// Underlying method for indexFinder. Finds all the indexes of the specified word in the text, along with start and end ranges of text that will be searched. Also includes the comparison that will be used to search for the word.
	/// </summary>
	/// <param name="filetext">Text to be searched.</param>
	/// <param name="word">Word to search for.</param>
	/// <param name="start">The start index of where to search within the text. Must be non-negative.</param>
	/// <param name="end">The end index of where to search within the text. Must be non-negative.</param>
	/// <param name="compare">The comparison method to be used. See <seealso cref="StringComparison"/></param>
	/// <returns>A <see cref="List{T}"/> containing zero-based non-negative integers with indices of where the word was found.</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private static List<int> indexFinderExtended(string filetext, string word, int start, int end, StringComparison compare)
	{
		if (start < 0) throw new ArgumentOutOfRangeException(nameof(start), start, "start cannot be negative.");
		if (end < 0) throw new ArgumentOutOfRangeException(nameof(end), end, "end cannot be negative.");
		//filetext = filetext[start..end];
		filetext = filetext.Substring(start, end - start);
		List<int> output = new List<int>();
		for (; filetext.Contains(word, compare);)
		{
			output.Add(filetext.LastIndexOf(word, compare));
			try
			{
				filetext = filetext.Remove(filetext.LastIndexOf(word, compare), word.Length);
			}
			catch (ArgumentOutOfRangeException) { break; }
		}
		return output;
	}

	/// <summary>
	/// Finds all the indexes of the specified letter in the text. Note that the letter is matched regardless of uppercase or lowercase.
	/// </summary>
	/// <param name="filetext">Text to be searched.</param>
	/// <param name="letter">Letter to search for.</param>
	/// <returns>A <see cref="List{T}"/> containing zero-based non-negative integers with indices of where the letter was found.</returns>
	private static List<int> indexFinder(string filetext, char letter)
	{
		return indexFinderExtended(filetext, letter, 0, filetext.Length, StringComparison.CurrentCultureIgnoreCase);
	}

	/// <summary>
	/// Underlying method for indexFinder. Finds all the indexes of the specified letter in the text, along with start and end ranges of text that will be searched. Also includes the comparison that will be used to search for the letter.
	/// </summary>
	/// <param name="filetext">Text to be searched.</param>
	/// <param name="letter">Letter to search for.</param>
	/// <param name="start">The start index of where to search within the text. Must be non-negative.</param>
	/// <param name="end">The end index of where to search within the text. Must be non-negative.</param>
	/// <param name="compare">The comparison method to be used. See <seealso cref="StringComparison"/></param>
	/// <returns>A <see cref="List{T}"/> containing zero-based non-negative integers with indices of where the letter was found.</returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	private static List<int> indexFinderExtended(string filetext, char letter, int start, int end, StringComparison compare)
	{
		if (start < 0) throw new ArgumentOutOfRangeException(nameof(start), start, "start cannot be negative.");
		if (end < 0) throw new ArgumentOutOfRangeException(nameof(end), end, "end cannot be negative.");
		//filetext = filetext[start..end];
		filetext = filetext.Substring(start, end - start);
		List<int> output = new List<int>();
		for (; filetext.Contains(letter, compare);)
		{
			if (filetext.LastIndexOf(letter) == -1) break;
			output.Add(filetext.LastIndexOf(letter));
			try { filetext = filetext.Remove(filetext.LastIndexOf(letter)); }
			catch (ArgumentOutOfRangeException) { break; } //catches filetext.Remove(-1) -> ArgumentOutOfRangeException
		}
		return output;
	}

	/// <summary>
	/// Verifies that the file exists.
	/// </summary>
	/// <param name="filepath">Path to the file specified. Cannot be null, and must point to a specified filepath.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	public static bool fileVerifier(string filepath)
	{
		if(filepath == null)throw new ArgumentNullException(nameof(filepath), "File path specified cannot be null.");
		if(!File.Exists(filepath))throw new FileNotFoundException($"File not found.", filepath);
		return true;
	}

	/// <summary>
	/// Reads a file. Throws <see cref="ArgumentNullException"/> if filepath is null, and <see cref="FileNotFoundException"/> if the file does not exist.
	/// </summary>
	/// <param name="filepath">Path to specified file.</param>
	/// <returns>Returns <see cref="string.Empty"/> if the reading failed at all, otherwise returns the text content of the file.</returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
	public static string fileReader(string filepath)
	{
		string output = string.Empty;
		if (filepath == null) throw new ArgumentNullException(nameof(filepath), "File path specified cannot be null."); 
		if (!File.Exists(filepath)) throw new FileNotFoundException($"File not found.", filepath);
		try
		{
			output = File.ReadAllText(filepath);
		}
		catch
		{
			return string.Empty;
		}
		return output;
	}

    /// <summary>
    /// Searches if two words' index positions are close enough together (specified by <paramref name="bufferSize"/>) <b>and</b> in order. 
    /// The currentPosition is updated within this function to wherever the words have been found.
    /// </summary>
    /// <param name="currentPosition">The current position. This parameter is updated whenever the two words are sufficiently close together.</param>
    /// <param name="inputWord2">The second input word. If the contents of the second input word are unknown, use <see cref="searchTwoWords(ref int, int, int, int, int)"/> (though the contents aren't important anyways lel)</param>
    /// <param name="bufferSize">The buffer size of the locations of the words. Smaller value means higher accuracy but might not be big enough for some cases. Technically, a big enough value here will find <b>all</b> occurences of the first word followed by the second word, so long as they are in order.</param>
    /// <param name="indexI">Index of the first word.</param>
    /// <param name="indexJ">Index of the second word.</param>
    /// <returns>Returns <see cref="true"/> if the two words are close enough together and in order, <see cref="false"/> otherwise.</returns>
    private static bool searchTwoWords(ref int currentPosition, string inputWord2, int bufferSize, int indexI, int indexJ)
	{
		for (int i = 0; i < bufferSize; i++)
		{
			//checks if the first word location matches the second word location
			if (indexI + i == indexJ)
			{
				currentPosition = indexJ + inputWord2.Length;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Searches if two words' index positions are close enough together (specified by <paramref name="bufferSize"/>) <b>and</b> in order. 
	/// The currentPosition is updated within this function to wherever the words have been found.
	/// </summary>
	/// <param name="currentPosition">The current position. This parameter is updated whenever the two words are sufficiently close together.</param>
	/// <param name="inputWord2Length">The length of the second input word.</param>
	/// <param name="bufferSize">The buffer size of the locations of the words. Smaller value means higher accuracy but might not be big enough for some cases. Technically, a big enough value here will find <b>all</b> occurences of the first word followed by the second word, so long as they are in order.</param>
	/// <param name="indexI">Index of the first word.</param>
	/// <param name="indexJ">Index of the second word.</param>
	/// <returns>Returns <see cref="true"/> if the two words are close enough together and in order, <see cref="false"/> otherwise.</returns>
	private static bool searchTwoWords(ref int currentPosition, int inputWord2Length, int bufferSize, int indexI, int indexJ)
	{
		for (int i = 0; i < bufferSize; i++)
		{
			//checks if the first word location matches the second word location
			if (indexI + i == indexJ)
			{
				currentPosition = indexJ + inputWord2Length;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Handles the case when searching for two words.
	/// </summary>
	/// <param name="currentPosition">The current position. Is updated by <see cref="searchTwoWords(ref int, string, int, int, int)"/></param>
	/// <param name="filetext">The text to search through.</param>
	/// <param name="inputWord2">The second word to search for.</param>
	/// <param name="bufferSize">The buffer size of the locations of the words. Smaller value means higher accuracy but might not be big enough for some cases. Technically, a big enough value here will find <b>all</b> occurences of the first word followed by the second word, so long as they are in order.</param>
	/// <param name="indexI">Index of the first word.</param>
	/// <param name="target">The target to search for.</param>
	/// <returns>Returns true if both the words have been matched by <see cref="searchTwoWords(ref int, string, int, int, int)"/>, false otherwise.</returns>
	private static bool twoWordCaseHandler(ref int currentPosition, string filetext, string inputWord2, int bufferSize, int indexI, SearchTargets target)
	{
		bool found = false;
		List<int> inputWord2Locations = new List<int>(indexFinder(filetext, inputWord2));
		//iterates through all second word locations
		foreach (int indexJ in inputWord2Locations)
		{
			switch (target)
			{
				case SearchTargets.Damage:
				case SearchTargets.DamageRange:
					{
						//TODO: investigate if damage and damage range can be separated somehow, and increase readability
						//		while keeping work as low as possible

						break; //breaks out of switch
					}
				case SearchTargets.HeadMultiplier:
				case SearchTargets.LimbMultiplier:
				case SearchTargets.TorsoMultiplier:
					{
						found = searchTwoWords(ref currentPosition, inputWord2, bufferSize + 10, indexI, indexJ);
						break; //breaks out of switch
					}
				default:
					{
						found = searchTwoWords(ref currentPosition, inputWord2, bufferSize, indexI, indexJ);
						break; //breaks out of switch
					}
			}
			if (found) return found ;
		}


		return false;
	}

	/// <summary>
	/// Searches for a statistic within the text from the current location until any character flag(s).
	/// </summary>
	/// <param name="currentPosition">The current position.</param>
	/// <param name="filetext">The text to search through.</param>
	/// <param name="compareChars">A list of characters that stops the reading when encountered.</param>
	/// <returns>A string containing the statistic until the specified character flag. If currentPosition is outside the range of the text, or if there is no character found that matches the specified character flags, this function will return <see cref="string.Empty"/> </returns>
	private static string grabStatisticFromLocation(int currentPosition, string filetext, List<char> compareChars)
	{
		Func<char, List<char>, bool> matchChar = (compare, flags) => { foreach (char k in flags) { if (compare == k) { return true; } } return false; };
		string result = string.Empty;
		try
		{
			for (int j = currentPosition; matchChar(filetext[j], compareChars) == false; j++)
			{
				result += filetext[j];
			}
		}
		catch (IndexOutOfRangeException) { result = string.Empty; }
		return result;
	} 

	/// <summary>
	/// Searches for a target specified within a text. Throws an <see cref="Exception"/> if the first word has not been found.
	/// </summary>
	/// <param name="currentPosition">The current position. This value will be updated by <see cref="twoWordCaseHandler(ref int, string, string, int, int, SearchTargets)"/></param>
	/// <param name="filetext">The text to be searched.</param>
	/// <param name="target">The target to search for.</param>
	/// <returns>A string containing the desired statistic specified.</returns>
	/// <exception cref="Exception"></exception>
	private static string searchForTarget(ref int currentPosition, string filetext, SearchTargets target)
	{
		/*  Algorithm:
		 *  Find locations of the first word, store them in list
		 *  Find locations of the second word (if it exists), and store those in list
		 *  Iterates through each first word location, and checks a certain number of characters ahead to see if any second word locations are there
		 *  If there are no second words, just find all first words
		 */
		const int acceptableSpaces = 3;
		bool found = false;
		string result = string.Empty;
		string inputWord1 = "";
		string? inputWord2 = null;

		//selects input words
		inputWordsSelection(target, ref inputWord1, ref inputWord2);
		List<int> inputWord1Locations = new List<int>(indexFinder(filetext, inputWord1));

		//iterate through all first word locations
		foreach (int indexI in inputWord1Locations)
		{
			//2 word case
			if (inputWord2 != null) found = twoWordCaseHandler(ref currentPosition, filetext, inputWord2, inputWord1.Length + acceptableSpaces + inputWord2.Length, indexI, target);
			if (found) break;

			//1 word case
			//rank, suppression, firerate

			//TODO: verify for all other cases
			if(inputWord1Locations.Count > 0)
			{
				found = true;
				currentPosition = indexI + inputWord1.Length;
				break;
			}

			throw new Exception("There are no words that match the first input word.");
			
		}
		if (found)
		{
			switch (target) {
				case SearchTargets.HeadMultiplier:
					{
						return grabStatisticFromLocation(currentPosition, filetext, new List<char> { 't','T'});
					}
				case SearchTargets.LimbMultiplier:
					{
						return grabStatisticFromLocation(currentPosition, filetext, new List<char> {'=' });
					}
				case SearchTargets.TorsoMultiplier:
					{
						return grabStatisticFromLocation(currentPosition, filetext, new List<char> { 'l', 'L' });
					}
				case SearchTargets.Damage:
					{
						return grabStatisticFromLocation(currentPosition, filetext, new List<char> { 'h', 'H' });
					}
				default:
					{
						//checks for characters 10 (line feed/new line), 12 (form feed/new page), 13 (carriage return) and ends the search there 
						return grabStatisticFromLocation(currentPosition, filetext, new List<char> { (char)10, (char)12, (char)13 });
					}
			}
		}
		return result;

	}

    /// <summary>
    /// Attempts to fix corrupted words. Note that <paramref name="filetext"/> 
    ///		<para>
	///			Steps:
	///			<list type="number">
	///				<item>Iterate through all the first characters of the first word.</item>
	///				<item>Determine if something resembling the first word has been matched:
	///					<list type="bullet">
	///						<item>Skip over spaces</item>
	///						<item>See if <c>i</c> and <c>l</c> match (case-insensitive)</item>
	///					</list>
	///				</item>
	///				<item>Automatically replace detected corrupted words. <b>NOTE:</b> this happens to every word matched.</item>
	///			</list>
    ///		</para>
    /// </summary>
    /// <param name="filetext">Text to be fixed. Note that this parameter is changed and fixed.</param>
    /// <param name="inputWord">Desired word to find and replace with.</param>
    /// <returns>Returns the corrupted word if it has been found, otherwise it returns <see cref="string.Empty"/></returns>
    private static string corruptedWordFixer(ref string filetext, string inputWord)
    {

        //tryhard finding the words
        /*  Iterate through all the first characters of the first word
		 *  Determine if something resembling the first word has been matched
		 *      - Skip over spaces
		 *      - See if i and l match
		 *      - Automatically replace detected corrupted words
		 *      (note: this happens to EVERY word matched)
		 *  Repeat above for the second word
		 *  Repeat above algorithm again, now with fixed words
		 */
        List<int> wordFirstCharLocations = new List<int>(indexFinder(filetext, inputWord.ToString().ToUpperInvariant()[0]));
        StringBuilder tempInputWord = new StringBuilder(inputWord);

		//removes instances of the word before the actual
		string copied = filetext;
        Predicate<int> match = (i) => i > copied.IndexOf("Does the file exist?", StringComparison.CurrentCultureIgnoreCase);
        wordFirstCharLocations.RemoveAll(match); wordFirstCharLocations.TrimExcess();
        string corruptedWord1 = ""; //to replace
        int letterMatch = 0;
        int acceptableSpaces = 3; //margin of error
        int actualSpaces = 0; //number of spaces detected when finding corrupted words
        foreach (int IndexI in wordFirstCharLocations) //through the list of indexes
        {
            //looks for i and l confusion, and matching letters
            //note: does this for every word matched, not just the "desired" one (why not fix the problem while we are at it?)
            for (int i = IndexI; i < IndexI + tempInputWord.Length + acceptableSpaces; i++) //for each char of the file at the chars location
            {
                for (int j = 0; j < tempInputWord.Length; j++)
                {
                    //skips over spaces and increments i only if we are still in the first word "region"
                    if (filetext[i] == 32 && i + 1 < IndexI + tempInputWord.Length + acceptableSpaces)
                    {
                        actualSpaces++;
                        i++; //skips if space
                        continue;
                    }


                    /*testing variables, ideally do not remove
                    //string location = filetext.Substring(i, 20); //1423
                    //char testi = filetext[i];
                    //char testj = tempInputWord1[j];
                    */

                    //looks for matches, and sees if i is in a location where l is, and vice versa
                    if (filetext[i].ToString().ToLower() == tempInputWord[j].ToString().ToLower() || (filetext[i].ToString().ToLower() == "i" && tempInputWord[j].ToString().ToLower() == "l") || (filetext[i].ToString().ToLower() == "l" && tempInputWord[j].ToString().ToLower() == "i"))
                    {
                        letterMatch++;
                        //i don't even know why i assigned index j to be ╚
                        if (letterMatch <= inputWord.Length) tempInputWord[j] = (char)200;
                        break;
                    }
                }
            }

            //look through matching words, and automatically replace them
            if (letterMatch == tempInputWord.Length)
            {
                //find the actual corrupted word
                for (int i = IndexI; i < IndexI + tempInputWord.Length + actualSpaces; i++)
                {
                    corruptedWord1 += filetext[i];
                }
                //replace the whole word with the correct word, and add space padding after
                filetext = filetext.Replace(corruptedWord1, inputWord + " ", StringComparison.CurrentCultureIgnoreCase);
				return corruptedWord1;
            }
        }
		return string.Empty;
    }

    /// <summary>
	/// Finds the statistic specified by <paramref name="target"/>.
	/// </summary>
	/// <param name="filepath">Path to the desired file.</param>
	/// <param name="target">Desired statistic to search for.</param>
	/// <param name="consoleWrite">Set to true to print to the console, false otherwise.</param>
	/// <returns>Returns a string with the specified statistic (along with the statistic name).</returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="FileNotFoundException"></exception>
    public static string findStatisticInFile(string filepath, SearchTargets target, bool consoleWrite)
	{

		string filetext = string.Empty;
		try
		{
            filetext = fileReader(filepath);
		}
		catch(ArgumentNullException e)
		{
			return string.Empty;
		} 
		catch (FileNotFoundException k) 
		{
			return string.Empty;
		}

		if(filetext == string.Empty)return string.Empty;


		string inputWord1 = "";
		string? inputWord2 = null;
		int currentPosition = 0;
		string result = "";

		//selects input words
		inputWordsSelection(target, ref inputWord1, ref inputWord2);

		//i have annotated the crap out of this code because i've come back to it after 9 months and i have no clue what i wrote :')

		//generalize searchAlgorithm for later use below
		try
		{
			result = searchForTarget(ref currentPosition, filetext, target);
		}
		catch
		{
            string corrupted = corruptedWordFixer(ref filetext, inputWord1);
            File.WriteAllText(filepath, filetext);
            if (corrupted != string.Empty && consoleWrite) Console.WriteLine(corrupted + " has been corrected to " + inputWord1 + " ");


            if (inputWord2 != null)
            {
                corrupted = corruptedWordFixer(ref filetext, inputWord2);
                File.WriteAllText(filepath, filetext);
                if (corrupted != string.Empty && consoleWrite) Console.WriteLine(corrupted + " has been corrected to " + inputWord2 + " ");
            }

            result = searchForTarget(ref currentPosition, filetext, target);
        }
		return result;
	}

	/// <summary>
	/// Finds all the statistics in a file.
	/// </summary>
	/// <param name="filepath">Path to the desired file.</param>
	/// <param name="consoleWrite">Set to true to print to the console, false otherwise.</param>
	/// <returns>Returns a list of strings of the statistics of the file. May be empty.</returns>
	public static List<string> findAllStatisticsInFile(string filepath, bool consoleWrite)
	{
		List<string> temp = new List<string>();
		foreach(SearchTargets target in Enum.GetValues(typeof(SearchTargets)))
		{
			try
			{
				temp.Add(findStatisticInFile(filepath, target, consoleWrite));
			}
			catch
			{
				//do nothing
			}
		}
		return temp;
	}

    public static List<Tuple<string,SearchTargets>> findAllStatisticsInFileWithTypes	(string filepath, bool consoleWrite)
    {
        List<string> temp = new List<string>();
        foreach (SearchTargets target in Enum.GetValues(typeof(SearchTargets)))
        {
            try
            {
                temp.Add(findStatisticInFile(filepath, target, consoleWrite));
            }
            catch
            {
                //do nothing
            }
        }
        return temp;
    }

    /// <summary>
    /// Selects words based on target. See <see cref="SearchTargets"/> for the options.
    /// </summary>
    /// <param name="target">Option of statistic target.</param>
    /// <param name="inputWord1">The first word of statistic.</param>
    /// <param name="inputWord2">The second word of statistic.</param>
    public static void inputWordsSelection(SearchTargets target, ref string inputWord1, ref string? inputWord2)
	{

		switch (target)
		{
			case SearchTargets.Rank:
				{ inputWord1 = "rank"; break; }

				//guns
			case SearchTargets.Damage:
				{
					inputWord1 = "damage";
					inputWord2 = "range"; //to be selected AGAINST
					break;
				}
			case SearchTargets.DamageRange:
				{
					inputWord1 = "Damage";
					inputWord2 = "Range"; //to be selected FOR
					break;
				}
			case SearchTargets.Firerate:
				{ inputWord1 = "firerate"; break; }
			case SearchTargets.AmmoCapacity:
				{ inputWord1 = "ammo"; inputWord2 = "capacity"; break; }
			case SearchTargets.HeadMultiplier:
				{ inputWord1 = "head"; inputWord2 = "multiplier"; break; }
			case SearchTargets.TorsoMultiplier:
				{ inputWord1 = "torso"; inputWord2 = "multiplier"; break; }
			case SearchTargets.LimbMultiplier:
				{ inputWord1 = "limb"; inputWord2 = "multiplier"; break; }
			case SearchTargets.MuzzleVelocity:
				{ inputWord1 = "muzzle"; inputWord2 = "velocity"; break; }
			case SearchTargets.Suppression:
				{ inputWord1 = "suppression"; break; }
			case SearchTargets.PenetrationDepth:
				{ inputWord1 = "penetration"; inputWord2 = "depth"; break; }
			case SearchTargets.ReloadTime:
				{ inputWord1 = "reload"; inputWord2 = "time"; break; }
			case SearchTargets.EmptyReloadTime:
				{ inputWord1 = "empty"; inputWord2 = "reload time"; break; }
			case SearchTargets.WeaponWalkspeed:
				{ inputWord1 = "weapon"; inputWord2 = "walkspeed"; break; }
			case SearchTargets.AimingWalkspeed:
				{ inputWord1 = "aiming"; inputWord2 = "walkspeed"; break; }
			case SearchTargets.AmmoType:
				{ inputWord1 = "ammo"; inputWord2 = "type"; break; }
			case SearchTargets.SightMagnification:
				{ inputWord1 = "sight"; inputWord2 = "magnification"; break; }
			case SearchTargets.MinimumTimeToKill:
				{ inputWord1 = "minimum time to"; inputWord2 = "kill"; break; }
			case SearchTargets.HipfireSpreadFactor:
				{ inputWord1 = "hipfire spread"; inputWord2 = "factor"; break; }
			case SearchTargets.HipfireRecoverySpeed:
				{ inputWord1 = "hipfire recovery"; inputWord2 = "speed"; break; }
			case SearchTargets.HipfireSpreadDamping:
				{ inputWord1 = "hipfire spread"; inputWord2 = "damping"; break; }
			case SearchTargets.HipChoke:
				{ inputWord1 = "hip"; inputWord2 = "choke"; break; }
			case SearchTargets.AimChoke:
				{ inputWord1 = "aim"; inputWord2 = "choke"; break; }
			case SearchTargets.EquipSpeed:
				{ inputWord1 = "equip"; inputWord2 = "speed"; break; }
			case SearchTargets.AimModelSpeed:
				{ inputWord1 = "aim model"; inputWord2 = "speed"; break; }
			case SearchTargets.AimMagnificationSpeed:
				{ inputWord1 = "hipfire magnification"; inputWord2 = "speed"; break; }
			case SearchTargets.CrosshairSize:
				{ inputWord1 = "crosshair"; inputWord2 = "size"; break; }
			case SearchTargets.CrosshairSpreadRate:
				{ inputWord1 = "crosshair spread"; inputWord2 = "rate"; break; }
			case SearchTargets.CrosshairRecoverRate:
				{ inputWord1 = "crosshair recover"; inputWord2 = "rate"; break; }
			case SearchTargets.FireModes:
				{ inputWord1 = "fire"; inputWord2 = "modes"; break; }

				//grenades
			case SearchTargets.BlastRadius:
				{ inputWord1 = "blast"; inputWord2 = "radius"; break; }
			case SearchTargets.KillingRadius:
				{ inputWord1 = "killing"; inputWord2 = "radius"; break; }
			case SearchTargets.MaximumDamage:
				{ inputWord1 = "maximum"; inputWord2 = "damage"; break; }
			case SearchTargets.TriggerMechanism:
				{ inputWord1 = "trigger"; inputWord2 = "mechanism"; break; }
			case SearchTargets.SpecialEffects:
				{ inputWord1 = "special"; inputWord2 = "effects"; break; }
			case SearchTargets.StoredCapacity:
				{ inputWord1 = "stored"; inputWord2 = "capacity"; break; }

				//melees
			case SearchTargets.FrontStabDamage:
				{ inputWord1 = "front stab"; inputWord2 = "damage"; break; }
			case SearchTargets.BackStabDamage:
				{ inputWord1 = "back stab"; inputWord2 = "damage"; break; }
			case SearchTargets.MainAttackTime:
				{ inputWord1 = "main attack"; inputWord2 = "time"; break; }
			case SearchTargets.MainAttackDelay:
				{ inputWord1 = "main attack"; inputWord2 = "delay"; break; }
			case SearchTargets.AltAttackTime:
				{ inputWord1 = "alt attack"; inputWord2 = "time"; break; }
			case SearchTargets.AltAttackDelay:
				{ inputWord1 = "alt attack"; inputWord2 = "delay"; break; }
			case SearchTargets.QuickAttackTime:
				{ inputWord1 = "quick attack"; inputWord2 = "time"; break; }
			case SearchTargets.QuickAttackDelay:
				{ inputWord1 = "quick attack"; inputWord2 = "delay"; break; }
			case SearchTargets.Walkspeed:
				{ inputWord1 = "walkspeed"; break; }
		}

	}

}
