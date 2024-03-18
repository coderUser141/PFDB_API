using PFDB.WeaponUtility;
using System.Collections.Generic;
using System.Text;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Linq;

namespace PFDB
{
	namespace Parsing
	{

		public sealed class StatisticParse : IStatisticParse
		{

			private PhantomForcesVersion _version;
			private List<IIndexSearch> _wordLocationSearchers = new List<IIndexSearch>();
			private string _filetext = string.Empty;
			private List<string> _inputWordList = new List<string>();

			private bool _consoleWrite;
			private SearchTargets _searchTarget = 0;
			private int _acceptableSpaces;
			private int _acceptableCorruptedWordSpaces; //margin of error

			public string Filetext { get { return _filetext; } }

			public StatisticParse(PhantomForcesVersion version, string text, int acceptableSpaces = 3, int acceptableCorruptedWordSpaces = 3, bool consoleWrite = false)
			{

				_version = version;
				_consoleWrite = consoleWrite;
				_acceptableCorruptedWordSpaces = acceptableCorruptedWordSpaces;
				_acceptableSpaces = acceptableSpaces;
				_filetext = text;
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
				List<int> wordFirstCharLocations = (List<int>)
													new IndexSearch(_filetext, inputWord.ToUpperInvariant()[0].ToString())
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

			private List<string> lister(params string[] words)
			{
				return words.ToList();
			}

			/// <summary>
			/// Selects words based on target. See <see cref="SearchTargets"/> for the options.
			/// </summary>
			private void inputWordsSelection()
			{
				_inputWordList.Clear();

				switch (_searchTarget)
				{
					case SearchTargets.Rank:
						{
							_inputWordList.Add("rank"); break; 
						}

					//guns
					case SearchTargets.Damage:
						{
							//the space is intentional, i may have named something DamageInfo and i'm too lazy to change the python script rn, maybe a todo tbh
							_inputWordList.AddRange(lister(["damage "]));
							//to be selected AGAINST
							break;
						}
					case SearchTargets.DamageRange:
						{
							_inputWordList.AddRange(lister(["damage", "range"]));
							//to be selected FOR
							break;
						}
					case SearchTargets.Firerate:
						{ _inputWordList.AddRange(lister(["firerate"])); break; }
					case SearchTargets.AmmoCapacity:
						{ _inputWordList.AddRange(lister(["ammo", "capacity"])); ; break; }
					case SearchTargets.HeadMultiplier:
						{ _inputWordList.AddRange(lister(["head", "multiplier"])); break; }
					case SearchTargets.TorsoMultiplier:
						{ _inputWordList.AddRange(lister(["torso", "multiplier"]));  break; }
					case SearchTargets.LimbMultiplier:
						{ _inputWordList.AddRange(lister(["limb", "multiplier"])); break; }
					case SearchTargets.MuzzleVelocity:
						{ _inputWordList.AddRange(lister(["muzzle", "velocity"])); break; }
					case SearchTargets.Suppression:
						{ _inputWordList.AddRange(lister(["suppression"])); ; break; }
					case SearchTargets.PenetrationDepth:
						{ _inputWordList.AddRange(lister(["penetration", "depth"])); break; }
					case SearchTargets.ReloadTime:
						{ _inputWordList.AddRange(lister(["reload", "time"])); break; }
					case SearchTargets.EmptyReloadTime:
						{ _inputWordList.AddRange(lister(["empty", "reload", "time"])); break; }
					case SearchTargets.WeaponWalkspeed:
						{ _inputWordList.AddRange(lister(["weapon", "walkspeed"])); break; }
					case SearchTargets.AimingWalkspeed:
						{ _inputWordList.AddRange(lister(["aiming", "walkspeed"])); break; }
					case SearchTargets.AmmoType:
						{ _inputWordList.AddRange(lister(["ammo", "type"])); break; }
					case SearchTargets.SightMagnification:
						{ _inputWordList.AddRange(lister(["sight", "magnification"])); break; }
					case SearchTargets.MinimumTimeToKill:
						{ _inputWordList.AddRange(lister(["minimum", "time", "to", "kill"])); break; }
					case SearchTargets.HipfireSpreadFactor:
						{ _inputWordList.AddRange(lister(["hipfire", "spread", "factor"])); break; }
					case SearchTargets.HipfireRecoverySpeed:
						{ _inputWordList.AddRange(lister(["hipfire", "recovery", "speed"])); break; }
					case SearchTargets.HipfireSpreadDamping:
						{ _inputWordList.AddRange(lister(["hipfire", "spread", "damping"])); break; }
					case SearchTargets.HipChoke:
						{ _inputWordList.AddRange(lister(["hip", "choke"])); break; }
					case SearchTargets.AimChoke:
						{ _inputWordList.AddRange(lister(["aim", "choke"])); break; }
					case SearchTargets.EquipSpeed:
						{ _inputWordList.AddRange(lister(["equip", "speed"])); break; }
					case SearchTargets.AimModelSpeed:
						{ _inputWordList.AddRange(lister(["aim", "model", "speed"])); break; }
					case SearchTargets.AimMagnificationSpeed:
						{ _inputWordList.AddRange(lister(["aim", "magnification", "speed"])); break; }
					case SearchTargets.CrosshairSize:
						{ _inputWordList.AddRange(lister(["crosshair", "size"])); break; }
					case SearchTargets.CrosshairSpreadRate:
						{ _inputWordList.AddRange(lister(["crosshair", "spread", "rate"])); break; }
					case SearchTargets.CrosshairRecoverRate:
						{ _inputWordList.AddRange(lister(["crosshair", "recover", "rate"])); break; }
					case SearchTargets.FireModes:
						{ _inputWordList.AddRange(lister(["fire", "modes"])); break; }

					//grenades
					case SearchTargets.BlastRadius:
						{ _inputWordList.AddRange(lister(["blast", "radius"])); break; }
					case SearchTargets.KillingRadius:
						{ _inputWordList.AddRange(lister(["killing", "radius"])); break; }
					case SearchTargets.MaximumDamage:
						{ _inputWordList.AddRange(lister(["maximum", "damage"])); break; }
					case SearchTargets.TriggerMechanism:
						{ _inputWordList.AddRange(lister(["trigger", "mechanism"])); break; }
					case SearchTargets.SpecialEffects:
						{ _inputWordList.AddRange(lister(["special", "effects"])); break; }
					case SearchTargets.StoredCapacity:
						{ _inputWordList.AddRange(lister(["stored", "capacity"])); break; }

					//melees
					case SearchTargets.FrontStabDamage:
						{ _inputWordList.AddRange(lister(["front", "stab", "damage"])); break; }
					case SearchTargets.BackStabDamage:
						{ _inputWordList.AddRange(lister(["back", "stab", "damage"])); break; }
					case SearchTargets.MainAttackTime:
						{ _inputWordList.AddRange(lister(["main", "attack", "time"])); break; }
					case SearchTargets.MainAttackDelay:
						{ _inputWordList.AddRange(lister(["main", "attack", "delay"])); break; }
					case SearchTargets.AltAttackTime:
						{ _inputWordList.AddRange(lister(["alt", "attack", "time"])); break; }
					case SearchTargets.AltAttackDelay:
						{ _inputWordList.AddRange(lister(["alt", "attack", "delay"])); break; }
					case SearchTargets.QuickAttackTime:
						{ _inputWordList.AddRange(lister(["quick", "attack", "time"])); break; }
					case SearchTargets.QuickAttackDelay:
						{ _inputWordList.AddRange(lister(["quick", "attack", "delay"])); break; }
					case SearchTargets.Walkspeed:
						{ _inputWordList.AddRange(lister(["walkspeed"])); break; }
				}

				_wordLocationSearchers.Clear();
				foreach(string word in _inputWordList)
				{
					_wordLocationSearchers.Add(new IndexSearch(_filetext, word));
				}

			}

            /// <summary>
            /// Searches if two words' index positions are close enough together <b>and</b> in order. 
            /// </summary>
            /// <param name="firstWordOrCharSearcher">The <see cref="IIndexSearch"/> implementation that searches for the first character or first word.</param>
            /// <param name="secondWordSearcher">The <see cref="IIndexSearch"/> implementation that searches for the second word.</param>
            /// <returns>A <see cref="IEnumerable{int}"/> that contains all the locations of the first word/character where the words are close enough and in order.</returns>
            private IEnumerable<int> wordProximityChecker(IIndexSearch firstWordOrCharSearcher, IIndexSearch secondWordSearcher)
			{
				List<int> result = new List<int>();
				foreach (int i in firstWordOrCharSearcher.ListOfIndices)
				{
					foreach (int j in secondWordSearcher.ListOfIndices)
					{
						if (i + firstWordOrCharSearcher.Word?.Length + _acceptableSpaces > j && i < j)
						{
							result.Add(i);
						}
					}
				}
				return result;
			}

			/// <summary>
			/// Searches if two words' index positions are close enough together <b>and</b> in order. 
			/// </summary>
			/// <param name="firstWordOrCharLocations">The <see cref="IIndexSearch"/> implementation that searches for the first character or first word.</param>
			/// <param name="secondWordLocations">The <see cref="IIndexSearch"/> implementation that searches for the second word.</param>
			/// <param name="word">The first word. The content doesn't matter, just the length matters.</param>
			/// <returns>A <see cref="IEnumerable{int}"/> that contains all the locations of the first word/character where the words are close enough and in order.</returns>
			private IEnumerable<int> wordProximityChecker(IEnumerable<int> firstWordOrCharLocations, IEnumerable<int> secondWordLocations, string word)
			{
				List<int> result = new List<int>();
				foreach (int i in firstWordOrCharLocations)
				{
					foreach (int j in secondWordLocations)
					{
						if (i + word.Length + _acceptableSpaces > j && i < j)
						{
							result.Add(i);
						}
					}
				}
				return result;
			}

			/// <summary>
			/// Searches the text to find locations where statistics are likely to be.
			/// </summary>
			/// <returns>An <see cref="IEnumerable{int}"/> containing the position of the first character of the first word.</returns>
			/// <exception cref="Exception"></exception>
			private IEnumerable<int> grabStatisticLocations()
			{
				IEnumerable<int> ints;
				if(_wordLocationSearchers.Last().ListOfIndices.Count > 0)
				{
					if (_wordLocationSearchers.Count == 1)
						return _wordLocationSearchers[0].ListOfIndices;

					
					/*
					 * offset since ints starts at original location, but we need an offset to make sure we aren't
					 * too far away from the next word
					 */
                    int offset = 0;
                    if (_wordLocationSearchers.Count > 2)
                    {
                        offset = _inputWordList[0].Length + _inputWordList[1].Length;
                    }
					
					//temporary list
                    List<int> tempInts = new List<int>();

                    if (_wordLocationSearchers.All(x => x.ListOfIndices.Count > 0))
					{
						//2+ word case, check the first two words
						ints = wordProximityChecker(_wordLocationSearchers[0], _wordLocationSearchers[1]);
						tempInts.AddRange(ints);

						//add offset to each item
                        for (int j = 0; j < tempInts.ToList().Count; ++j)
                        {
                            tempInts[j] += offset;
                        }

                        //if 2 words, this is ignored
                        for (int i = 2; i < _wordLocationSearchers.Count; ++i)
						{
							//add offset for every new word encountered
                            tempInts = wordProximityChecker(tempInts, _wordLocationSearchers[i].ListOfIndices, _wordLocationSearchers[i].Word ?? _inputWordList[i]).ToList();
                            offset += _inputWordList[i].Length;
                            for (int j = 0; j < tempInts.ToList().Count; ++j)
                            {
                                tempInts[j] += _inputWordList[i].Length;
                            }
                        }

						//remove offset to give us original positions
                        for (int j = 0; j < tempInts.ToList().Count; ++j)
                        {
                            tempInts[j] -= offset;
                        }


                        return tempInts;
					}

					//modified case

					IIndexSearch firstWordFirstCharacterLocations = new IndexSearch(_filetext, _wordLocationSearchers[0].Word?[0].ToString());
					IIndexSearch secondWordFirstCharacterLocations = new IndexSearch(_filetext, _wordLocationSearchers[0].Word?[0].ToString());

                    //2+ word case, check the first two words
                    ints = wordProximityChecker(firstWordFirstCharacterLocations, secondWordFirstCharacterLocations);
                    tempInts.AddRange(ints);

                    //add offset to each item
                    for (int j = 0; j < tempInts.ToList().Count; ++j)
                    {
                        tempInts[j] += offset;
                    }

                    //if 2 words, this is ignored
                    for (int i = 2; i < _wordLocationSearchers.Count; ++i)
                    {
						//add offset for every new word encountered
						IIndexSearch firstCharacterLocations = new IndexSearch(_filetext, (_wordLocationSearchers[i].Word ?? _inputWordList[i])[0].ToString());
                        tempInts = wordProximityChecker(tempInts, _wordLocationSearchers[i].ListOfIndices, _wordLocationSearchers[i].Word ?? _inputWordList[i]).ToList();
                        offset += _inputWordList[i].Length;
                        for (int j = 0; j < tempInts.ToList().Count; ++j)
                        {
                            tempInts[j] += _inputWordList[i].Length;
                        }
                    }

                    //remove offset to give us original positions
                    for (int j = 0; j < tempInts.ToList().Count; ++j)
                    {
                        tempInts[j] -= offset;
                    }
                    return tempInts;
                    

                }
				else
				{
					throw new Exception("no words were found");
					//unhandled for now, todo
				}
			}


			public string findStatisticInFile(SearchTargets target, WeaponType weaponType, IEnumerable<char> endings)
			{
				_inputWordList.Clear();
				_wordLocationSearchers.Clear();

				for(int i = 0; i < _wordLocationSearchers.Count; ++i)
				{
					_wordLocationSearchers[i] = new IndexSearch("", null);
				}


				if ((target <= SearchTargets.FireModes) == false && weaponType != WeaponType.Primary && weaponType != WeaponType.Secondary)
				{
					//return; //error
					throw new ArgumentException($"The SearchTarget specified does not match the WeaponType specified.");
				}
				else if (target >= SearchTargets.BlastRadius && target <= SearchTargets.StoredCapacity && weaponType != WeaponType.Grenade)
				{
					//return; //error
					throw new ArgumentException($"The SearchTarget specified does not match the WeaponType specified.");
				}
				else if (target >= SearchTargets.FrontStabDamage && weaponType != WeaponType.Melee)
				{
					//return; //error
					throw new ArgumentException($"The SearchTarget specified does not match the WeaponType specified.");
				}

				if (_filetext == "") //return; // error
					throw new ArgumentException("text was empty");

				_searchTarget = target;


				//todo: use inputWordsSelection to generate IIndexSearch DONE
				inputWordsSelection();

				//for reload time case
                IEnumerable<int> statisticNonGrataLocations = new List<int>();


                StringBuilder result = new StringBuilder();
                List<char> list = new List<char>(endings);
                list.AddRange(new List<char>() { (char)13, (char)10 });

                foreach (IIndexSearch word in _wordLocationSearchers)
                {
                    if (word.isEmpty())
                    {
                        corruptedWordFixer(word.Word);
                        word.Search();
                    }
                }

                switch (target)
				{

					//one word cases
					case SearchTargets.Firerate:
					case SearchTargets.Walkspeed:
						{
							if (_wordLocationSearchers[0].isEmpty())
							{
								corruptedWordFixer(_inputWordList[0]);
								_wordLocationSearchers[0].Search();
								if (_wordLocationSearchers[0].isEmpty()) //return; //error
									throw new WordNotFoundException($"{_inputWordList[0]} was not found anywhere in the text with one-word case.");
							}
							break;
						}


					//special cases
					case SearchTargets.Damage:
						{
                            if (_wordLocationSearchers[0].isEmpty())
                            {
                                corruptedWordFixer(_inputWordList[0]);
                                _wordLocationSearchers[0].Search();
                                if (_wordLocationSearchers[0].isEmpty()) //return; //error
                                    throw new WordNotFoundException($"{_inputWordList[0]} was not found anywhere in the text with one-word case.");
                            }

							//if legacy version, eliminate damage range instances
							if (_version.VersionNumber < 900) 
							{
								_inputWordList.Clear();
								_wordLocationSearchers.Clear();
								_inputWordList.AddRange(lister(["damage", "range"]));
								foreach (string word in _inputWordList)
								{
									_wordLocationSearchers.Add(new IndexSearch(_filetext, word));
								}
								try
								{
                                    statisticNonGrataLocations = grabStatisticLocations();
								}
								finally
								{
									_inputWordList.Clear();
									_wordLocationSearchers.Clear();
									inputWordsSelection();
								}
							}
                            break;
						}

                    case SearchTargets.Suppression:
                        {
                            if (_wordLocationSearchers[0].isEmpty())
                            {
                                corruptedWordFixer(_inputWordList[0]);
                                _wordLocationSearchers[0].Search();
                                if (_wordLocationSearchers[0].isEmpty()) //return; //error
                                    throw new WordNotFoundException($"{_inputWordList[0]} was not found anywhere in the text with one-word case.");
                            }
                            _inputWordList.Clear();
                            _wordLocationSearchers.Clear();
                            _inputWordList.AddRange(lister(["suppression", "range"]));
                            foreach (string word in _inputWordList)
                            {
                                _wordLocationSearchers.Add(new IndexSearch(_filetext, word));
                            }
                            try
                            {
                                statisticNonGrataLocations = grabStatisticLocations();
                            }
                            finally
                            {
                                _inputWordList.Clear();
                                _wordLocationSearchers.Clear();
                                inputWordsSelection();
                            }
                            break;
						}
                    case SearchTargets.Rank:
                        {
                            if (_wordLocationSearchers[0].isEmpty())
                            {
                                corruptedWordFixer(_inputWordList[0]);
                                _wordLocationSearchers[0].Search();
                                if (_wordLocationSearchers[0].isEmpty()) //return; //error
                                    throw new WordNotFoundException($"{_inputWordList[0]} was not found anywhere in the text with one-word case.");
                            }
                            IIndexSearch rankinfoSearch = new IndexSearch(_filetext, "RankInfo");
                            _wordLocationSearchers[0].RemoveFromList(rankinfoSearch.ListOfIndices);
                            break;
                        }
                    case SearchTargets.ReloadTime:
						{

							_inputWordList.Clear();
							_wordLocationSearchers.Clear();
							_inputWordList.AddRange(lister(["empty", "reload", "time"]));
                            foreach (string word in _inputWordList)
                            {
                                _wordLocationSearchers.Add(new IndexSearch(_filetext, word));
                            }
							try
							{
                                statisticNonGrataLocations = grabStatisticLocations();
							}
							finally
							{
								_inputWordList.Clear();
								_wordLocationSearchers.Clear();
								inputWordsSelection();
							}
							break;
						}

					//case where we have 2+ words
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


							foreach(IIndexSearch word in _wordLocationSearchers)
							{
								if (word.isEmpty())
								{
									corruptedWordFixer(word.Word);
									word.Search();
								}
							}

							if (_wordLocationSearchers.Last().ListOfIndices.Count > 0) goto Success;

							throw new WordNotFoundException($"None of the {_inputWordList.Count} words were found.");

						}
				}

				Success:

				List<int> locations = new List<int>(); //dummy list
				try
                {
                    if (_version.VersionNumber >= 900 && (_searchTarget == SearchTargets.DamageRange || _searchTarget == SearchTargets.Damage)) goto DamageRangeSkip; //skip because we expect it not to be there
                    locations = grabStatisticLocations().ToList();
                }
				catch
				{
					throw new WordNotFoundException("None of the two words were found.");
					//return; //error, no words were found
				}

            DamageRangeSkip:

                int temp = 0;
				if (_searchTarget == SearchTargets.ReloadTime)
				{
					foreach(int u in locations) {
						foreach (int t in statisticNonGrataLocations)
						{
							//Console.WriteLine($"empty position {t}, reload position {u}, t forward {t + "empty".Length + _acceptableSpaces}, t backward {t - "empty".Length - _acceptableSpaces}");
							//if "empty" is close enough to "reload time", we can conclude that it's not what we want and we should remove it
							if ((t < u && t + "empty".Length + _acceptableSpaces > u) || 
								(t > u && t - "empty".Length - _acceptableSpaces < u))
							{
								temp = u;
							}
						}
                    }
                    locations.RemoveAll(x => x == temp);
                } 
				else if(_searchTarget == SearchTargets.Suppression)
				{
                    foreach (int u in locations)
                    {
                        foreach (int t in statisticNonGrataLocations)
                        {
                            //if "suppression range" is close enough to "suppression", we can conclude that it's not what we want and we should remove it
                            if ((t < u && t + "suppression".Length + _acceptableSpaces > u) || (t == u) ||
                                (t > u && t - "suppression".Length - _acceptableSpaces < u))
                            {
                                temp = u;
                            }
                        }
                    }
                    locations.RemoveAll(x => x == temp);
                }
                else if (_searchTarget == SearchTargets.Damage)
                {
                    foreach (int u in locations)
                    {
                        foreach (int t in statisticNonGrataLocations)
                        {
                            //if "damage range" is close enough to "damage", we can conclude that it's not what we want and we should remove it
                            if ((t < u && t + "damage".Length + _acceptableSpaces > u) || (t == u) ||
                                (t > u && t - "damage".Length - _acceptableSpaces < u))
                            {
                                temp = u;
                            }
                        }
                    }
                    locations.RemoveAll(x => x == temp);
                }else if((_searchTarget == SearchTargets.Damage ||  _searchTarget == SearchTargets.DamageRange) && _version.VersionNumber >= 900)
				{
					IIndexSearch indexSearch = new IndexSearch(_filetext, "index ");
					locations.AddRange(indexSearch.ListOfIndices);
				}



				foreach (int location in locations)
				{
					int i = location;
					StringBuilder r = new StringBuilder(string.Empty); 
					for(; list.Contains(_filetext[i]) == false && i < _filetext.Length;++i)
					{
						//technically this try block can be removed, but its safer to leave it for now, im sick of debugging strange errors
						try
						{
							r.Append(_filetext[i]);
						}
						catch (ArgumentOutOfRangeException)
						{
							break;
						}
					}
					result.Append(r);
                    result.Append('\t');
                }




                return result.ToString();


			}


		}
	}
}