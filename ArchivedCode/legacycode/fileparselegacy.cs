// See https://aka.ms/new-console-template for more information
using System.Text;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

Console.WriteLine("Hello, World!");
namespace PFDB.Parsing
{
    namespace Legacy
    {
        /// <summary>
        /// Legacy code for fileparse. Avoid using.
        /// </summary>
        [Obsolete("Use PFDB.Parsing")]
        static class FileParseLegacy
        {
            /// <summary>
            /// Finds all the indexes of the specified word in the text. Note that the word is matched regardless of uppercase or lowercase.
            /// </summary>
            /// <param name="filetext">Text to be searched.</param>
            /// <param name="word">Word to search for.</param>
            /// <returns>A List containing the indices of the word.</returns>
            private static List<int> indexFinder(string filetext, string word)
            {
                List<int> output = new();
                for (; filetext.Contains(word, StringComparison.CurrentCultureIgnoreCase);)
                {
                    output.Add(filetext.LastIndexOf(word, StringComparison.CurrentCultureIgnoreCase));
                    try
                    {
                        filetext = filetext.Remove(filetext.LastIndexOf(word, StringComparison.CurrentCultureIgnoreCase), word.Length);
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
            /// <returns>A List containing the indices of the letter.</returns>
            private static List<int> indexFinder(string filetext, char letter)
            {
                List<int> output = new();
                for (; filetext.Contains(letter, StringComparison.CurrentCultureIgnoreCase);)
                {
                    if (filetext.LastIndexOf(letter) == -1) break;
                    output.Add(filetext.LastIndexOf(letter));
                    try { filetext = filetext.Remove(filetext.LastIndexOf(letter)); }
                    catch (ArgumentOutOfRangeException) { break; } //catches filetext.Remove(-1) -> ArgumentOutOfRangeException
                }
                return output;
            }

            /// <summary>
            /// Verifies that the file exists, and reads from the file.
            /// </summary>
            /// <param name="filepath">Filepath to check.</param>
            /// <returns>A string containing the results of the file specified.</returns>
            /// <exception cref="ArgumentNullException"></exception>
            /// <exception cref="FileNotFoundException"></exception>
            [Obsolete("Use PFDB.Parsing")]
            public static string fileReader(string filepath)
            {
                if (filepath == null) throw new ArgumentNullException(nameof(filepath), "File path specified cannot be null.");
                if (!File.Exists(filepath)) throw new FileNotFoundException($"File not found.", filepath);
                return File.ReadAllText(filepath);
            }


            public enum LegacySearchTargets
            {
                Version,
                Rank,
                //guns
                Damage,
                Firerate,
                AmmoCapacity,
                FireModes,
                HeadMultiplier,
                TorsoMultiplier,
                LimbMultiplier,
                DamageRange,
                MuzzleVelocity,
                PenetrationDepth,
                Suppression,
                ReloadTime,
                EmptyReloadTime,
                WeaponWalkspeed,
                AimingWalkspeed,
                AmmoType,
                //grenade
                BlastRadius,
                KillingRadius,
                MaximumDamage,
                TriggerMechanism,
                SpecialEffects,
                StoredCapacity,
                //melees
                BladeLength,
                FrontStabDamage,
                BackStabDamage,
                Walkspeed
            }

            private static void inputWordsSelectionLegacy(LegacySearchTargets target, ref string inputWord1, ref string? inputWord2)
            {
                switch (target)
                {
                    case LegacySearchTargets.Rank:
                        { inputWord1 = "rank"; break; }
                    case LegacySearchTargets.Damage:
                        {
                            inputWord1 = "damage";
                            inputWord2 = "range"; //to be selected AGAINST
                            break;
                        }
                    case LegacySearchTargets.Firerate:
                        { inputWord1 = "firerate"; break; }
                    case LegacySearchTargets.AmmoCapacity:
                        { inputWord1 = "ammo"; inputWord2 = "capacity"; break; }
                    case LegacySearchTargets.FireModes:
                        { inputWord1 = "fire"; inputWord2 = "modes"; break; }
                    case LegacySearchTargets.HeadMultiplier:
                        { inputWord1 = "head"; inputWord2 = "multiplier"; break; }
                    case LegacySearchTargets.TorsoMultiplier:
                        { inputWord1 = "torso"; inputWord2 = "multiplier"; break; }
                    case LegacySearchTargets.LimbMultiplier:
                        { inputWord1 = "limb"; inputWord2 = "multiplier"; break; }
                    case LegacySearchTargets.DamageRange:
                        {
                            inputWord1 = "Damage";
                            inputWord2 = "Range"; //to be selected FOR
                            break;
                        }
                    case LegacySearchTargets.MuzzleVelocity:
                        { inputWord1 = "muzzle"; inputWord2 = "velocity"; break; }
                    case LegacySearchTargets.PenetrationDepth:
                        { inputWord1 = "penetration"; inputWord2 = "depth"; break; }
                    case LegacySearchTargets.Suppression:
                        { inputWord1 = "suppression"; break; }
                    case LegacySearchTargets.ReloadTime:
                        { inputWord1 = "reload"; inputWord2 = "time"; break; }
                    case LegacySearchTargets.EmptyReloadTime:
                        { inputWord1 = "empty"; inputWord2 = "reload time"; break; }
                    case LegacySearchTargets.WeaponWalkspeed:
                        { inputWord1 = "weapon"; inputWord2 = "walkspeed"; break; }
                    case LegacySearchTargets.AimingWalkspeed:
                        { inputWord1 = "aiming"; inputWord2 = "walkspeed"; break; }
                    case LegacySearchTargets.AmmoType:
                        { inputWord1 = "ammo"; inputWord2 = "type"; break; }
                    case LegacySearchTargets.BlastRadius:
                        { inputWord1 = "blast"; inputWord2 = "radius"; break; }
                    case LegacySearchTargets.KillingRadius:
                        { inputWord1 = "killing"; inputWord2 = "radius"; break; }
                    case LegacySearchTargets.TriggerMechanism:
                        { inputWord1 = "trigger"; inputWord2 = "mechanism"; break; }
                    case LegacySearchTargets.SpecialEffects:
                        { inputWord1 = "special"; inputWord2 = "effects"; break; }
                    case LegacySearchTargets.StoredCapacity:
                        { inputWord1 = "stored"; inputWord2 = "capacity"; break; }
                    case LegacySearchTargets.BladeLength:
                        { inputWord1 = "blade"; inputWord2 = "length"; break; }
                    case LegacySearchTargets.FrontStabDamage:
                        { inputWord1 = "front stab"; inputWord2 = "damage"; break; }
                    case LegacySearchTargets.BackStabDamage:
                        { inputWord1 = "back stab"; inputWord2 = "damage"; break; }
                    case LegacySearchTargets.Walkspeed:
                        { inputWord1 = "walkspeed"; break; }
                    case LegacySearchTargets.Version:
                        { inputWord1 = "version"; break; }
                    case LegacySearchTargets.MaximumDamage:
                        { inputWord1 = "maximum"; inputWord2 = "damage"; break; }
                }
            }


            [Obsolete("Use PFDB.Parsing")]
            public static string findStatisticInFileLegacy(string filepath, LegacySearchTargets targets, bool consoleWrite)
            {
                string filetext = fileReader(filepath);

                string inputWord1 = "";
                string? inputWord2 = null;

                //selects input words
                inputWordsSelectionLegacy(targets, ref inputWord1, ref inputWord2);

                //i have annotated the crap out of this code because i've come back to it after 9 months and i have no clue what i wrote :')

                /*  Algorithm:
                 *  Find locations of the first word, store them in list
                 *  Find locations of the second word (if it exists), and store those in list
                 *  Iterates through each first word location, and checks a certain number of characters ahead to see if any second word locations are there
                 *  If there are no second words, just find all first words
                 */

                //finds locations of the first word
                List<int> inputWord1Locations = new(indexFinder(filetext, inputWord1));
                List<int> inputWord2Locations = new();
                if (inputWord2 != null)
                {
                    inputWord2Locations = indexFinder(filetext, inputWord2);
                }

                List<int> firstWordFirstCharLocations = new();
                List<int> secondWordFirstCharLocations = new();

                bool found = false;
                int currentPosition = 0;
                int foundCounter = 0;
                string result = "";
                Action whenFound = () => { found = true; foundCounter++; };

                firstWordFirstCharLocations = indexFinder(filetext, inputWord1.ToString().ToUpperInvariant()[0]);
                if (inputWord2 != null) secondWordFirstCharLocations = indexFinder(filetext, inputWord2.ToString().ToUpperInvariant()[0]);

                Predicate<int> match = (i) => i > filetext.IndexOf("Does the file exist?", StringComparison.CurrentCultureIgnoreCase);
                firstWordFirstCharLocations.RemoveAll(match); firstWordFirstCharLocations.TrimExcess();
                secondWordFirstCharLocations.RemoveAll(match); secondWordFirstCharLocations.TrimExcess();

                int acceptableSpaces = 3; //margin of error
                int actualSpaces = 0; //number of spaces detected when finding corrupted words
                string corruptedWord1 = ""; //to replace
                string corruptedWord2 = ""; //to replace
                int letterMatch = 0;
                StringBuilder tempInputWord1 = new(inputWord1);

                //generalize searchAlgorithm for later use below
                Action search = () =>
                {
                    //iterate through all first word locations
                    foreach (int indexI in inputWord1Locations)
                    {
                        //2 word case
                        if (inputWord2 != null)
                        {
                            //iterates through all second word locations
                            foreach (int indexJ in inputWord2Locations)
                            {
                                //for this specific case, search for both damage and range, and select against 
                                if (targets == LegacySearchTargets.Damage)
                                {
                                    if (filetext.Substring(indexI, inputWord1.Length + inputWord2.Length + acceptableSpaces + 1).Contains(inputWord2))
                                    {
                                        //do nothing, this is for damage range
                                        //Console.WriteLine("found damage + range");
                                    }
                                    else
                                    {
                                        //Console.WriteLine("found only damage");
                                        whenFound();
                                        currentPosition = indexI + inputWord1.Length;
                                        break;
                                    }
                                }
                                else
                                { //general case
                                    for (int i = 0; i < inputWord1.Length + inputWord2.Length + acceptableSpaces; i++)
                                    {
                                        //checks if the first word location matches the second word location
                                        if (indexI + i == indexJ)
                                        {
                                            whenFound();
                                            currentPosition = indexJ + inputWord2.Length;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        //1 word case
                        else
                        {
                            //rank, suppression, firerate
                            whenFound();
                            currentPosition = indexI + inputWord1.Length;
                            break;
                        }
                    }
                };

                search();

                //if the words have been matched
                if (found)
                {
                    //checks for characters 10 (line feed/new line), 12 (form feed/new page), 13 (carriage return) and ends the search there 
                    for (int j = currentPosition; filetext[j] != 10 && filetext[j] != 12 && filetext[j] != 13; j++)
                    {
                        result += filetext[j];
                    }
                }
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
                else
                {
                    foreach (int IndexI in firstWordFirstCharLocations) //through the list of indexes
                    {
                        //looks for i and l confusion, and matching letters
                        //note: does this for every word matched, not just the "desired" one (why not fix the problem while we are at it?)
                        for (int i = IndexI; i < IndexI + tempInputWord1.Length + acceptableSpaces; i++) //for each char of the file at the chars location
                        {
                            for (int j = 0; j < tempInputWord1.Length; j++)
                            {
                                //skips over spaces and increments i only if we are still in the first word "region"
                                if (filetext[i] == 32 && i + 1 < IndexI + tempInputWord1.Length + acceptableSpaces)
                                {
                                    actualSpaces++;
                                    i++; //skips if space
                                    continue;
                                }
                                //testing variables, ideally do not remove
                                //string location = filetext.Substring(i, 20); //1423
                                //char testi = filetext[i];
                                //char testj = tempInputWord1[j];

                                //looks for matches, and sees if i is in a location where l is, and vice versa
                                if (filetext[i].ToString().ToLower() == tempInputWord1[j].ToString().ToLower() || (filetext[i].ToString().ToLower() == "i" && tempInputWord1[j].ToString().ToLower() == "l") || (filetext[i].ToString().ToLower() == "l" && tempInputWord1[j].ToString().ToLower() == "i"))
                                {
                                    letterMatch++;
                                    //i don't even know why i assigned index j to be ╚
                                    if (letterMatch <= inputWord1.Length) tempInputWord1[j] = (char)200;
                                    break;
                                }
                            }
                        }

                        //look through matching words, and automatically replace them
                        if (letterMatch == tempInputWord1.Length)
                        {
                            //find the actual corrupted word
                            for (int i = IndexI; i < IndexI + tempInputWord1.Length + actualSpaces; i++)
                            {
                                corruptedWord1 += filetext[i];
                            }
                            //replace the whole word with the correct word, and add space padding after
                            filetext = filetext.Replace(corruptedWord1, inputWord1 + " ", StringComparison.CurrentCultureIgnoreCase);
                            //write to the file
                            File.WriteAllText(filepath, filetext);
                            //display change
                            Console.WriteLine(corruptedWord1 + " has been corrected to " + inputWord1 + " ");
                            break;
                        }
                        //reset everything
                        letterMatch = 0; actualSpaces = 0;
                        tempInputWord1 = new(inputWord1);
                    }
                    //reset if failed
                    letterMatch = 0; actualSpaces = 0;

                    //same thing as above, but with word 2
                    if (inputWord2 != null)
                    {
                        StringBuilder tempInputWord2 = new(inputWord2);
                        foreach (int IndexJ in secondWordFirstCharLocations) //through the list of indexes
                        {
                            for (int i = IndexJ; i < IndexJ + tempInputWord2.Length + acceptableSpaces; i++) //for each char of the file at the chars location
                            {
                                for (int j = 0; j < tempInputWord2.Length; j++)
                                {
                                    if (filetext[i] == 32 && i + 1 < IndexJ + tempInputWord2.Length + acceptableSpaces)
                                    {
                                        actualSpaces++;
                                        i++; //skips if space
                                        continue;
                                    }
                                    //string location = filetext.Substring(i, 10); //1423
                                    //char testi = filetext[i];
                                    //char testj = tempInputWord2[j];
                                    if (filetext[i].ToString().ToLower() == tempInputWord2[j].ToString().ToLower() || (filetext[i].ToString().ToLower() == "i" && tempInputWord2[j].ToString().ToLower() == "l") || (filetext[i].ToString().ToLower() == "l" && tempInputWord2[j].ToString().ToLower() == "i"))
                                    {
                                        letterMatch++;
                                        if (letterMatch <= inputWord2.Length) tempInputWord2[j] = (char)200;
                                        break;
                                    }
                                }
                            }
                            if (letterMatch == tempInputWord2.Length)
                            {
                                for (int i = IndexJ; i < IndexJ + tempInputWord2.Length + actualSpaces; i++)
                                {
                                    corruptedWord2 += filetext[i];
                                }

                                filetext = filetext.Replace(corruptedWord2, inputWord2 + " ", StringComparison.CurrentCultureIgnoreCase);
                                File.WriteAllText(filepath, filetext);
                                Console.WriteLine(corruptedWord2 + " has been corrected to " + inputWord2 + " ");
                                break;
                            }
                            letterMatch = 0; actualSpaces = 0;
                            tempInputWord2 = new(inputWord2);
                        }
                    }

                    inputWord1Locations = indexFinder(filetext, inputWord1);
                    if (inputWord2 != null)
                    {
                        inputWord2Locations = indexFinder(filetext, inputWord2);
                    }
                    //redo the algorithm
                    search();

                    if (found)
                    {
                        for (int j = currentPosition; filetext[j] != 10 && filetext[j] != 12 && filetext[j] != 13; j++)
                        {
                            result += filetext[j];
                        }
                    }
                }
                //Console.WriteLine(inputWord1 + inputWord2 ?? "");
                if (consoleWrite) Console.WriteLine(result);
                return result;
            }

            [Obsolete("Use PFDB.Parsing")]
            public static Task<string> findStatisticInFileAsyncLegacy(string filepath, LegacySearchTargets targets)
            {
                string filetext = fileReader(filepath);

                string inputWord1 = "";
                string? inputWord2 = null;

                //selects input words
                inputWordsSelectionLegacy(targets, ref inputWord1, ref inputWord2);

                //i have annotated the crap out of this code because i've come back to it after 9 months and i have no clue what i wrote :')

                /*  Algorithm:
                 *  Find locations of the first word, store them in list
                 *  Find locations of the second word (if it exists), and store those in list
                 *  Iterates through each first word location, and checks a certain number of characters ahead to see if any second word locations are there
                 *  If there are no second words, just find all first words
                 */

                //finds locations of the first word
                List<int> inputWord1Locations = new(indexFinder(filetext, inputWord1));
                List<int> inputWord2Locations = new();
                if (inputWord2 != null)
                {
                    inputWord2Locations = indexFinder(filetext, inputWord2);
                }

                List<int> firstWordFirstCharLocations = new();
                List<int> secondWordFirstCharLocations = new();

                bool found = false;
                int currentPosition = 0;
                int foundCounter = 0;
                string result = "";
                Action whenFound = () => { found = true; foundCounter++; };

                firstWordFirstCharLocations = indexFinder(filetext, inputWord1.ToString().ToUpperInvariant()[0]);
                if (inputWord2 != null) secondWordFirstCharLocations = indexFinder(filetext, inputWord2.ToString().ToUpperInvariant()[0]);

                Predicate<int> match = (i) => i > filetext.IndexOf("Does the file exist?", StringComparison.CurrentCultureIgnoreCase);
                firstWordFirstCharLocations.RemoveAll(match); firstWordFirstCharLocations.TrimExcess();
                secondWordFirstCharLocations.RemoveAll(match); secondWordFirstCharLocations.TrimExcess();

                int acceptableSpaces = 3; //margin of error
                int actualSpaces = 0; //number of spaces detected when finding corrupted words
                string corruptedWord1 = ""; //to replace
                string corruptedWord2 = ""; //to replace
                int letterMatch = 0;
                StringBuilder tempInputWord1 = new(inputWord1);

                //generalize searchAlgorithm for later use below
                Action search = () =>
                {
                    //iterate through all first word locations
                    foreach (int indexI in inputWord1Locations)
                    {
                        //2 word case
                        if (inputWord2 != null)
                        {
                            //iterates through all second word locations
                            foreach (int indexJ in inputWord2Locations)
                            {
                                //for this specific case, search for both damage and range, and select against 
                                if (targets == LegacySearchTargets.Damage)
                                {
                                    if (filetext.Substring(indexI, inputWord1.Length + inputWord2.Length + acceptableSpaces + 1).Contains(inputWord2))
                                    {
                                        //do nothing, this is for damage range
                                        //Console.WriteLine("found damage + range");
                                    }
                                    else
                                    {
                                        //Console.WriteLine("found only damage");
                                        whenFound();
                                        currentPosition = indexI + inputWord1.Length;
                                        break;
                                    }
                                }
                                else
                                { //general case
                                    for (int i = 0; i < inputWord1.Length + inputWord2.Length + acceptableSpaces; i++)
                                    {
                                        //checks if the first word location matches the second word location
                                        if (indexI + i == indexJ)
                                        {
                                            whenFound();
                                            currentPosition = indexJ + inputWord2.Length;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        //1 word case
                        else
                        {
                            //rank, suppression, firerate
                            whenFound();
                            currentPosition = indexI + inputWord1.Length;
                            break;
                        }
                    }
                };

                search();

                //if the words have been matched
                if (found)
                {
                    //checks for characters 10 (line feed/new line), 12 (form feed/new page), 13 (carriage return) and ends the search there 
                    for (int j = currentPosition; filetext[j] != 10 && filetext[j] != 12 && filetext[j] != 13; j++)
                    {
                        result += filetext[j];
                    }
                }
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
                else
                {
                    foreach (int IndexI in firstWordFirstCharLocations) //through the list of indexes
                    {
                        //looks for i and l confusion, and matching letters
                        //note: does this for every word matched, not just the "desired" one (why not fix the problem while we are at it?)
                        for (int i = IndexI; i < IndexI + tempInputWord1.Length + acceptableSpaces; i++) //for each char of the file at the chars location
                        {
                            for (int j = 0; j < tempInputWord1.Length; j++)
                            {
                                //skips over spaces and increments i only if we are still in the first word "region"
                                if (filetext[i] == 32 && i + 1 < IndexI + tempInputWord1.Length + acceptableSpaces)
                                {
                                    actualSpaces++;
                                    i++; //skips if space
                                    continue;
                                }
                                //testing variables, ideally do not remove
                                //string location = filetext.Substring(i, 20); //1423
                                //char testi = filetext[i];
                                //char testj = tempInputWord1[j];

                                //looks for matches, and sees if i is in a location where l is, and vice versa
                                if (filetext[i].ToString().ToLower() == tempInputWord1[j].ToString().ToLower() || (filetext[i].ToString().ToLower() == "i" && tempInputWord1[j].ToString().ToLower() == "l") || (filetext[i].ToString().ToLower() == "l" && tempInputWord1[j].ToString().ToLower() == "i"))
                                {
                                    letterMatch++;
                                    //i don't even know why i assigned index j to be ╚
                                    if (letterMatch <= inputWord1.Length) tempInputWord1[j] = (char)200;
                                    break;
                                }
                            }
                        }

                        //look through matching words, and automatically replace them
                        if (letterMatch == tempInputWord1.Length)
                        {
                            //find the actual corrupted word
                            for (int i = IndexI; i < IndexI + tempInputWord1.Length + actualSpaces; i++)
                            {
                                corruptedWord1 += filetext[i];
                            }
                            //replace the whole word with the correct word, and add space padding after
                            filetext = filetext.Replace(corruptedWord1, inputWord1 + " ", StringComparison.CurrentCultureIgnoreCase);
                            //write to the file
                            File.WriteAllText(filepath, filetext);
                            //display change
                            Console.WriteLine(corruptedWord1 + " has been corrected to " + inputWord1 + " ");
                            break;
                        }
                        //reset everything
                        letterMatch = 0; actualSpaces = 0;
                        tempInputWord1 = new(inputWord1);
                    }
                    //reset if failed
                    letterMatch = 0; actualSpaces = 0;

                    //same thing as above, but with word 2
                    if (inputWord2 != null)
                    {
                        StringBuilder tempInputWord2 = new(inputWord2);
                        foreach (int IndexJ in secondWordFirstCharLocations) //through the list of indexes
                        {
                            for (int i = IndexJ; i < IndexJ + tempInputWord2.Length + acceptableSpaces; i++) //for each char of the file at the chars location
                            {
                                for (int j = 0; j < tempInputWord2.Length; j++)
                                {
                                    if (filetext[i] == 32 && i + 1 < IndexJ + tempInputWord2.Length + acceptableSpaces)
                                    {
                                        actualSpaces++;
                                        i++; //skips if space
                                        continue;
                                    }
                                    //string location = filetext.Substring(i, 10); //1423
                                    //char testi = filetext[i];
                                    //char testj = tempInputWord2[j];
                                    if (filetext[i].ToString().ToLower() == tempInputWord2[j].ToString().ToLower() || (filetext[i].ToString().ToLower() == "i" && tempInputWord2[j].ToString().ToLower() == "l") || (filetext[i].ToString().ToLower() == "l" && tempInputWord2[j].ToString().ToLower() == "i"))
                                    {
                                        letterMatch++;
                                        if (letterMatch <= inputWord2.Length) tempInputWord2[j] = (char)200;
                                        break;
                                    }
                                }
                            }
                            if (letterMatch == tempInputWord2.Length)
                            {
                                for (int i = IndexJ; i < IndexJ + tempInputWord2.Length + actualSpaces; i++)
                                {
                                    corruptedWord2 += filetext[i];
                                }

                                filetext = filetext.Replace(corruptedWord2, inputWord2 + " ", StringComparison.CurrentCultureIgnoreCase);
                                File.WriteAllText(filepath, filetext);
                                Console.WriteLine(corruptedWord2 + " has been corrected to " + inputWord2 + " ");
                                break;
                            }
                            letterMatch = 0; actualSpaces = 0;
                            tempInputWord2 = new(inputWord2);
                        }
                    }

                    inputWord1Locations = indexFinder(filetext, inputWord1);
                    if (inputWord2 != null)
                    {
                        inputWord2Locations = indexFinder(filetext, inputWord2);
                    }
                    //redo the algorithm
                    search();

                    if (found)
                    {
                        for (int j = currentPosition; filetext[j] != 10 && filetext[j] != 12 && filetext[j] != 13; j++)
                        {
                            result += filetext[j];
                        }
                    }
                }
                //Console.WriteLine(inputWord1 + inputWord2 ?? "");
                //if (consoleWrite) Console.WriteLine(result);
                return Task.FromResult(result);
            }

            [Obsolete("Use PFDB.Parsing")]
            public static void findStatisticInFileReplaceLegacy(string filepath, LegacySearchTargets targets, string replacedString, string replacementString, bool missing, bool invalid, bool consoleWrite)
            {
                string filetext = fileReader(filepath);

                string inputWord1 = "";
                string? inputWord2 = null;

                inputWordsSelectionLegacy(targets, ref inputWord1, ref inputWord2);

                List<int> inputWord1Locations = new(indexFinder(filetext, inputWord1));
                List<int> inputWord2Locations = new();

                bool found = false;
                int currentPosition = 0;
                int foundCounter = 0;
                string result = "";
                Action whenFound = () => { found = true; foundCounter++; };

                if (inputWord2 != null)
                {
                    inputWord2Locations = indexFinder(filetext, inputWord2);
                }
                //targets == SearchTargets.LimbMultiplier || targets == SearchTargets.WeaponWalkspeed || targets == SearchTargets.AimingWalkspeed
                foreach (int indexI in inputWord1Locations)
                {
                    if (inputWord2 != null) //2 words
                    {
                        foreach (int indexJ in inputWord2Locations)
                        {
                            if (targets == LegacySearchTargets.Damage)
                            {
                                //special
                                if (filetext.Substring(indexI, inputWord1.Length + inputWord2.Length + 4).Contains(inputWord2))
                                {
                                    //Console.WriteLine("found damage + range");
                                }
                                else
                                {
                                    //Console.WriteLine("found only damage");
                                    whenFound();
                                    currentPosition = indexI + inputWord1.Length;
                                    break;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < inputWord1.Length + inputWord2.Length + 3; i++)
                                {
                                    if (indexI + i == indexJ)
                                    {
                                        //Console.WriteLine("found!! at: " + indexJ.ToString());
                                        whenFound();
                                        currentPosition = indexJ + inputWord2.Length;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else //1 word
                    {
                        //rank, suppression, firerate
                        found = true;
                        currentPosition = indexI + inputWord1.Length;
                        break;
                    }
                }

                if (found)
                {
                    if (filetext.Contains(replacedString) && filetext.IndexOf(replacedString) < 10 + currentPosition && invalid)
                    {

                        File.WriteAllText(filepath, filetext.Replace(replacedString, " " + replacementString, StringComparison.CurrentCultureIgnoreCase));

                    }
                    else if (missing)
                    {
                        File.WriteAllText(filepath, filetext.Insert(currentPosition, " " + replacementString));
                    }
                }
                if (consoleWrite) Console.WriteLine(replacedString + " is now " + replacementString + " (file location: " + filepath + ")");
            }

        }
    }
}