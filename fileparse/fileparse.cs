using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PFDB.WeaponUtility;


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


		public sealed class IndexSearch : IIndexSearch
		{
			public string Text { get; init; }
			public string? Word { get; init; }
			public StringComparison StringComparisonMethod { get; init; }
			public List<int> ListOfIndices { get; init; }

			public IndexSearch(string text, string? word)
			{
				ListOfIndices = new List<int>();
				Text = text;
				Word = word;
				StringComparisonMethod = StringComparison.InvariantCultureIgnoreCase;
				Search(); //automatically searches
			}

			public IndexSearch(string text, string? word, StringComparison stringComparisonMethod)
			{
				ListOfIndices = new List<int>();
				Text = text;
				Word = word;
				StringComparisonMethod = stringComparisonMethod;
				Search(); //automatically searches
			}

			public bool isEmpty()
			{
				return ListOfIndices.Count == 0;
			}

			public List<int> Search()
			{
				if (Word == null) return ListOfIndices; //early return if null
				string _filetext = Text;
				bool _isChar = Word.Length == 1;
				for (; //if we are searching for a single character, use char overload for String.Contains()
					(_isChar) ? _filetext.Contains(Word[0], StringComparisonMethod) : _filetext.Contains(Word, StringComparisonMethod);
					)
				{
					try
					{
						ListOfIndices.Add(
							(_isChar) ? _filetext.LastIndexOf(Word[0]) : _filetext.LastIndexOf(Word, StringComparisonMethod)
							);
						_filetext = (_isChar) ? _filetext.Remove(Word[0]) : _filetext.Remove(_filetext.LastIndexOf(Word, StringComparisonMethod), Word.Length);
					}
					catch (ArgumentOutOfRangeException) { break; }
				}
				return ListOfIndices;
			}
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

		public sealed class StatisticParse
		{
			
			private PhantomForcesVersion _version;
			private int _currentPosition;
			private IIndexSearch _firstWordLocationSearcher;
			private IIndexSearch _secondWordLocationSearcher;
			private string _filetext = string.Empty;
			private string _inputWord1 = string.Empty;
			private string? _inputWord2 = null;
			private bool _consoleWrite;
			private SearchTargets _searchTarget = 0;
			private int _acceptableSpaces;
			private int _acceptableCorruptedWordSpaces; //margin of error

			public StatisticParse(PhantomForcesVersion version, int acceptableSpaces = 3, int acceptableCorruptedWordSpaces = 3, bool consoleWrite = false)
			{
				_firstWordLocationSearcher = new IndexSearch("", null);
				_secondWordLocationSearcher = new IndexSearch("", null);
				_version = version;
				_consoleWrite = consoleWrite;
				_acceptableCorruptedWordSpaces = acceptableCorruptedWordSpaces;
				_acceptableSpaces = acceptableSpaces;
				_filetext = string.Empty;
				//_currentPosition = 0;
			}

			

			/// <summary>
			/// Attempts to fix corrupted words.
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
			/// <param name="inputWord">Desired word to find and replace with.</param>
			/// <returns>Returns the corrupted word if it has been found, otherwise it returns <see cref="string.Empty"/></returns>
			private string corruptedWordFixer(string? inputWord)
			{
				if (inputWord == null) return string.Empty;
				List<int> wordFirstCharLocations = new IndexSearch(_filetext, inputWord.ToUpperInvariant()[0].ToString())
														.Search();
				StringBuilder tempInputWord = new StringBuilder(inputWord);

				/////////////// double check this line, it may change, i might remove it ////////////////////
				wordFirstCharLocations.RemoveAll((i) => i > _filetext.IndexOf("Does the file exist?", StringComparison.CurrentCultureIgnoreCase)); 
				wordFirstCharLocations.TrimExcess();

				string corruptedWord1 = ""; //to replace
				int letterMatch = 0;
				int actualSpaces = 0; //number of spaces detected when finding corrupted words
				foreach (int IndexI in wordFirstCharLocations) //through the list of indexes
				{
					/*
					 * looks for i and l confusion, and matching letters
					 * note: does this for every word matched, not just the "desired" one (why not fix the problem while we are at it?)
					 * 
					 * these two loops below start from the index in the loop above
					 * the first loop starts from the index, and goes until the length of the word + buffer
					 * the second loop starts from 0 and goes until the length of the word
					 * we have a buffer, that way if there are spaces, the corrupted word (with spaces) can be replaced later
					 * inside the innermost loop, there is a check to see if we have encountered a space, and if so, we can skip past
					 * 		that character, but log it as a space. we do not want to check if that is part of the word since we have
					 * 		not put spaces in the word definitions.
					 * this way, when we get to the actual check later (to see if the lowercase versions of the letters are the same),
					 * 		we are actually comparing letters, and not spaces. if there is a match, we increment letterMatch
					 * when letterMatch equals the length of the word, we can then replace it with the correct word.
					 */

					for (int i = IndexI; i < IndexI + tempInputWord.Length + _acceptableCorruptedWordSpaces; i++) //for each char of the file at the chars location
					{
						for (int j = 0; j < tempInputWord.Length; j++) //through the word's character length
						{
							//skips over spaces and increments i only if we are still in the first word "region", otherwise is ignored
							if (_filetext[i] == 32 && i + 1 < IndexI + tempInputWord.Length + _acceptableSpaces)
							{
								actualSpaces++; //logs the number of spaces encountered
								i++; 
								continue; //skips innermost loop if there is a space
							}

							/*testing variables, ideally do not remove
							//string location = filetext.Substring(i, 20); //1423
							//char testi = filetext[i];
							//char testj = tempInputWord1[j];
							*/

							//looks for matches, and sees if i is in a location where l is, and vice versa
							if (_filetext[i].ToString().ToLower() == tempInputWord[j].ToString().ToLower() ||
								(_filetext[i].ToString().ToLower() == "i" && tempInputWord[j].ToString().ToLower() == "l") ||
								(_filetext[i].ToString().ToLower() == "l" && tempInputWord[j].ToString().ToLower() == "i"))
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
							corruptedWord1 += _filetext[i];
						}
						//replace the whole word with the correct word, and add space padding after
						_filetext = _filetext.Replace(corruptedWord1, inputWord + " ", StringComparison.CurrentCultureIgnoreCase);
						return corruptedWord1;
					}
				}
				return string.Empty;
			}

			/// <summary>
			/// Selects words based on target. See <see cref="SearchTargets"/> for the options.
			/// </summary>
			private void inputWordsSelection()
			{

				switch (_searchTarget)
				{
					case SearchTargets.Rank:
						{ _inputWord1 = "rank"; break; }

					//guns
					case SearchTargets.Damage:
						{
							_inputWord1 = "damage";
							_inputWord2 = "range"; //to be selected AGAINST
							break;
						}
					case SearchTargets.DamageRange:
						{
							_inputWord1 = "Damage";
							_inputWord2 = "Range"; //to be selected FOR
							break;
						}
					case SearchTargets.Firerate:
						{ _inputWord1 = "firerate"; break; }
					case SearchTargets.AmmoCapacity:
						{ _inputWord1 = "ammo"; _inputWord2 = "capacity"; break; }
					case SearchTargets.HeadMultiplier:
						{ _inputWord1 = "head"; _inputWord2 = "multiplier"; break; }
					case SearchTargets.TorsoMultiplier:
						{ _inputWord1 = "torso"; _inputWord2 = "multiplier"; break; }
					case SearchTargets.LimbMultiplier:
						{ _inputWord1 = "limb"; _inputWord2 = "multiplier"; break; }
					case SearchTargets.MuzzleVelocity:
						{ _inputWord1 = "muzzle"; _inputWord2 = "velocity"; break; }
					case SearchTargets.Suppression:
						{ _inputWord1 = "suppression"; break; }
					case SearchTargets.PenetrationDepth:
						{ _inputWord1 = "penetration"; _inputWord2 = "depth"; break; }
					case SearchTargets.ReloadTime:
						{ _inputWord1 = "reload"; _inputWord2 = "time"; break; }
					case SearchTargets.EmptyReloadTime:
						{ _inputWord1 = "empty"; _inputWord2 = "reload time"; break; }
					case SearchTargets.WeaponWalkspeed:
						{ _inputWord1 = "weapon"; _inputWord2 = "walkspeed"; break; }
					case SearchTargets.AimingWalkspeed:
						{ _inputWord1 = "aiming"; _inputWord2 = "walkspeed"; break; }
					case SearchTargets.AmmoType:
						{ _inputWord1 = "ammo"; _inputWord2 = "type"; break; }
					case SearchTargets.SightMagnification:
						{ _inputWord1 = "sight"; _inputWord2 = "magnification"; break; }
					case SearchTargets.MinimumTimeToKill:
						{ _inputWord1 = "minimum time to"; _inputWord2 = "kill"; break; }
					case SearchTargets.HipfireSpreadFactor:
						{ _inputWord1 = "hipfire spread"; _inputWord2 = "factor"; break; }
					case SearchTargets.HipfireRecoverySpeed:
						{ _inputWord1 = "hipfire recovery"; _inputWord2 = "speed"; break; }
					case SearchTargets.HipfireSpreadDamping:
						{ _inputWord1 = "hipfire spread"; _inputWord2 = "damping"; break; }
					case SearchTargets.HipChoke:
						{ _inputWord1 = "hip";	_inputWord2 = "choke"; break; }
					case SearchTargets.AimChoke:
						{ _inputWord1 = "aim"; _inputWord2 = "choke"; break; }
					case SearchTargets.EquipSpeed:
						{ _inputWord1 = "equip"; _inputWord2 = "speed"; break; }
					case SearchTargets.AimModelSpeed:
						{ _inputWord1 = "aim model"; _inputWord2 = "speed"; break; }
					case SearchTargets.AimMagnificationSpeed:
						{ _inputWord1 = "hipfire magnification"; _inputWord2 = "speed"; break; }
					case SearchTargets.CrosshairSize:
						{ _inputWord1 = "crosshair"; _inputWord2 = "size"; break; }
					case SearchTargets.CrosshairSpreadRate:
						{ _inputWord1 = "crosshair spread"; _inputWord2 = "rate"; break; }
					case SearchTargets.CrosshairRecoverRate:
						{ _inputWord1 = "crosshair recover"; _inputWord2 = "rate"; break; }
					case SearchTargets.FireModes:
						{ _inputWord1 = "fire"; _inputWord2 = "modes"; break; }

					//grenades
					case SearchTargets.BlastRadius:
						{ _inputWord1 = "blast"; _inputWord2 = "radius"; break; }
					case SearchTargets.KillingRadius:
						{ _inputWord1 = "killing"; _inputWord2 = "radius"; break; }
					case SearchTargets.MaximumDamage:
						{ _inputWord1 = "maximum"; _inputWord2 = "damage"; break; }
					case SearchTargets.TriggerMechanism:
						{ _inputWord1 = "trigger"; _inputWord2 = "mechanism"; break; }
					case SearchTargets.SpecialEffects:
						{ _inputWord1 = "special"; _inputWord2 = "effects"; break; }
					case SearchTargets.StoredCapacity:
						{ _inputWord1 = "stored";_inputWord2 = "capacity"; break; }

					//melees
					case SearchTargets.FrontStabDamage:
						{ _inputWord1 = "front stab"; _inputWord2 = "damage"; break; }
					case SearchTargets.BackStabDamage:
						{ _inputWord1 = "back stab"; _inputWord2 = "damage"; break; }
					case SearchTargets.MainAttackTime:
						{ _inputWord1 = "main attack"; _inputWord2 = "time"; break; }
					case SearchTargets.MainAttackDelay:
						{ _inputWord1 = "main attack";	_inputWord2 = "delay"; break; }
					case SearchTargets.AltAttackTime:
						{ _inputWord1 = "alt attack"; _inputWord2 = "time"; break; }
					case SearchTargets.AltAttackDelay:
						{ _inputWord1 = "alt attack"; _inputWord2 = "delay"; break; }
					case SearchTargets.QuickAttackTime:
						{ _inputWord1 = "quick attack"; _inputWord2 = "time"; break; }
					case SearchTargets.QuickAttackDelay:
						{ _inputWord1 = "quick attack"; _inputWord2 = "delay"; break; }
					case SearchTargets.Walkspeed:
						{ _inputWord1 = "walkspeed"; break; }
				}

			}

            /// <summary>
            /// Searches if two words' index positions are close enough together <b>and</b> in order. 
            /// </summary>
            /// <param name="firstWordOrCharSearcher">The <see cref="IIndexSearch"/> implementation that searches for the first character or first word.</param>
            /// <param name="secondWordSearcher">The <see cref="IIndexSearch"/> implementation that searches for the second word.</param>
			/// <returns>A <see cref="IEnumerable{int}"/> that contains all the locations where the words are close enough and in order.</returns>
            private IEnumerable<int> wordProximityChecker(IIndexSearch firstWordOrCharSearcher, IIndexSearch secondWordSearcher)
			{
				List<int> result = new List<int>();
				foreach(int i in firstWordOrCharSearcher.ListOfIndices)
				{
					foreach(int j in secondWordSearcher.ListOfIndices)
					{
						if(i + _inputWord1.Length + _acceptableSpaces > j && i < j)
						{
							result.Add(i);
						}
					}
				}
				return result;
			}
			
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			/// <exception cref="Exception"></exception>
			private IEnumerable<int> grabStatisticLocations()
			{
				IEnumerable<int> ints;

				if (_firstWordLocationSearcher.ListOfIndices.Count == 0 && _secondWordLocationSearcher.ListOfIndices.Count > 0)
				{
					//assume modified case
					IIndexSearch firstWordFirstCharLocations = new IndexSearch(_filetext, _inputWord1[0].ToString());
					ints = wordProximityChecker(firstWordFirstCharLocations, _secondWordLocationSearcher);

				}else if(_firstWordLocationSearcher.ListOfIndices.Count > 0 && _secondWordLocationSearcher.ListOfIndices.Count == 0)
				{
					//assume one word case
					ints = _firstWordLocationSearcher.ListOfIndices;
				}else if(_firstWordLocationSearcher.ListOfIndices.Count > 0 && _secondWordLocationSearcher.ListOfIndices.Count > 0)
				{
					//assume two word case
					ints = wordProximityChecker(_firstWordLocationSearcher, _secondWordLocationSearcher);
				}
				else
				{
					throw new Exception("no words were found");
					//unhandled for now, todo
				}

				return ints;
			}

			
			private void findStatisticInFileRemake(string filepath, SearchTargets target, WeaponType weaponType, IEnumerable<char> endings)
			{
				if((target <= SearchTargets.FireModes) == false && weaponType != WeaponType.Primary && weaponType != WeaponType.Secondary)
				{
					return; //error
				}else if(target >= SearchTargets.BlastRadius && target <= SearchTargets.StoredCapacity && weaponType != WeaponType.Grenade)
				{
					return; //error
				}else if(target >= SearchTargets.FrontStabDamage && weaponType != WeaponType.Melee)
				{
					return; //error
				}

				try
				{
					_filetext = FileParse.fileReader(filepath);
				}
				catch
				{
					return; //error, log
				}

				if (_filetext == "") return; // error

				//todo: potential to add functionality to read directly from a string or object

				_searchTarget = target;


				//todo: use inputWordsSelection to generate IIndexSearch
				inputWordsSelection();

				IIndexSearch firstWordLocationSearcher = new IndexSearch(_filetext, _inputWord1);
				IIndexSearch secondWordLocationSearcher = new IndexSearch(_filetext, _inputWord2);

				
				

				switch (target)
				{

						//one word cases
					case SearchTargets.Rank:
					case SearchTargets.Firerate:
					case SearchTargets.Suppression:
					case SearchTargets.Walkspeed:
						{
							if (firstWordLocationSearcher.isEmpty())
							{
								corruptedWordFixer(_inputWord1);
								firstWordLocationSearcher.Search();
								if (firstWordLocationSearcher.isEmpty()) return; //error
							}
							_firstWordLocationSearcher = firstWordLocationSearcher;
							_secondWordLocationSearcher = secondWordLocationSearcher;
							break;
						}

						//special cases
					case SearchTargets.Damage:
						{
							
							break;
						}
					case SearchTargets.DamageRange:
						{

							break;
						}

						//case where we have two words
					default:
						{
							/*
							if (firstWordLocationSearcher.isEmpty())
							{
								corruptedWordFixer(_inputWord1);
								firstWordLocationSearcher.Search();
								if (firstWordLocationSearcher.isEmpty())
								{
									if (secondWordLocationSearcher.isEmpty())
									{
										corruptedWordFixer(_inputWord2);
										secondWordLocationSearcher.Search();
										if (secondWordLocationSearcher.isEmpty())
										{
											return; //error
										}
										else
										{
											_firstWordLocationSearcher = firstWordLocationSearcher;
											_secondWordLocationSearcher = secondWordLocationSearcher;
											//modified grabstatistic
										}
									}
								}
							}

							secondWordLocationSearcher.Search();
							if (secondWordLocationSearcher.isEmpty())
							{
								corruptedWordFixer(_inputWord2);
								secondWordLocationSearcher.Search();
								if (secondWordLocationSearcher.isEmpty())
								{
									return; //error
								}
							}*/

							if (firstWordLocationSearcher.isEmpty() == false)
								goto FirstWordFound; //that one moment when goto is actually useful :D
							
							corruptedWordFixer(_inputWord1);

							if (firstWordLocationSearcher.isEmpty() == false)
								goto FirstWordFound;

							_firstWordLocationSearcher = firstWordLocationSearcher;
							_secondWordLocationSearcher = secondWordLocationSearcher;

						FirstWordFound:
							if (secondWordLocationSearcher.isEmpty() == false)
								goto SecondWordFound;

							corruptedWordFixer(_inputWord2);

							if (secondWordLocationSearcher.isEmpty() == false)
								goto SecondWordFound;

							return; //error

						SecondWordFound:

							_firstWordLocationSearcher = firstWordLocationSearcher;
							_secondWordLocationSearcher = secondWordLocationSearcher;

							//grab statistic like normal
							break;
						}
				}

				IEnumerable<int> locations; //dummy list
				try
				{
					locations = grabStatisticLocations();
				}
				catch
				{
					return; //error, no words were found
				}
				




			}


		}
		/*
		private class LegacyStatisticParse
		{
			

			/*
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
				IndexSearch indexSearch = new IndexSearch(filetext, inputWord2);
				indexSearch.Search();
				List<int> inputWord2Locations = indexSearch.ListOfIndices;
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
					if (found) return found;
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
		/*
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
					if (inputWord1Locations.Count > 0)
					{
						found = true;
						currentPosition = indexI + inputWord1.Length;
						break;
					}

					throw new Exception("There are no words that match the first input word.");

				}
				if (found)
				{
					switch (target)
					{
						case SearchTargets.HeadMultiplier:
							{
								return grabStatisticFromLocation(currentPosition, filetext, new List<char> { 't', 'T' });
							}
						case SearchTargets.LimbMultiplier:
							{
								return grabStatisticFromLocation(currentPosition, filetext, new List<char> { '=' });
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
				catch (ArgumentNullException)
				{
					return string.Empty;
				}
				catch (FileNotFoundException)
				{
					return string.Empty;
				}

				if (filetext == string.Empty) return string.Empty;


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
		}*/


		/// <summary>
		/// This class handles the text from the files generated after PyTesseract
		/// </summary>
		public  class FileParse
		{


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
			/// Finds all the statistics in a file.
			/// </summary>
			/// <param name="filepath">Path to the desired file.</param>
			/// <param name="consoleWrite">Set to true to print to the console, false otherwise.</param>
			/// <returns>Returns a list of strings of the statistics of the file. May be empty.</returns>
			public static List<string> findAllStatisticsInFile(string filepath, bool consoleWrite)
			{
				List<string> temp = new List<string>();
				foreach (SearchTargets target in Enum.GetValues(typeof(SearchTargets)))
				{
					try
					{
						//temp.Add(findStatisticInFile(filepath, target, consoleWrite));
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
			/// <param name="filepath">Path to the desired file.</param>
			/// <param name="consoleWrite">Set to true to print to the console, false otherwise.</param>
			/// <returns>Returns a list of tuples, containing both strings of the statistics and the type of statistics (defined by <see cref="SearchTargets"/>) from the file. May be empty.</returns>
			public static List<Tuple<string, SearchTargets>> findAllStatisticsInFileWithTypes(string filepath, bool consoleWrite)
			{
				List<Tuple<string, SearchTargets>> temp = new List<Tuple<string, SearchTargets>>();
				//List<string> temp = new List<string>();
				foreach (SearchTargets target in Enum.GetValues(typeof(SearchTargets)))
				{
					try
					{
						//temp.Add(Tuple.Create(findStatisticInFile(filepath, target, consoleWrite), target));
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