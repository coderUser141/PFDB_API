using System;
using System.IO;
using System.Collections.Generic;


namespace WeaponClasses
{


    /// <summary>
    /// Defines a weapon object.
    /// </summary>
    public class Weapon
    {
        /// <summary>
        /// Name of the weapon.
        /// </summary>
        protected string name;
        /// <summary>
        /// Rank of the weapon.
        /// </summary>
        protected int rank;
        /// <summary>
        /// If the weapon has a rank.
        /// </summary>
        protected bool hasRank;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the weapon.</param>
        /// <param name="hasRank">If the weapon has a rank.</param>
        /// <param name="rank">If the previous parameter is true, the rank of the weapon. Otherwise, the rank is 0.</param>
        public Weapon(string name, bool hasRank, int rank)
        {
            this.name = name;
            if (hasRank && rank > 0)
            {
                this.rank = rank;
            }
            else
            {
                this.rank = 0;
            }
            this.hasRank = hasRank;
        }

        /// <summary>
        /// Name getter/setter.
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// Rank getter/setter.
        /// </summary>
        public int Rank { get { return rank; } set { rank = value; } }
        /// <summary>
        /// HasRank getter/setter.
        /// </summary>
        public bool HasRank { get { return hasRank; } set { hasRank = value; } }


    }

    /// <summary>
    /// Creates objects for defining carried weapons' attributes (melees, guns).
    /// </summary>
    public class Carried
    {

        private double limbMultipler,
                            torsoMultiplier,
                            headMultiplier,
                            walkspeed;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="limbMultipler">Limb multiplier of the weapon.</param>
        /// <param name="torsoMultiplier">Torso multiplier of the weapon.</param>
        /// <param name="headMultiplier">Head multiplier of the weapon.</param>
        /// <param name="walkspeed">Walkspeed of the weapon.</param>
        public Carried(double limbMultipler, double torsoMultiplier, double headMultiplier, double walkspeed)
        {
            this.limbMultipler = limbMultipler;
            this.torsoMultiplier = torsoMultiplier;
            this.headMultiplier = headMultiplier;
            this.walkspeed = walkspeed;
        }

        /// <summary>
        /// LimbMultipler getter/setter.
        /// </summary>
        public double LimbMultiplier { get { return limbMultipler; } set { limbMultipler = value; } }
        /// <summary>
        /// TorsoMultiplier getter/setter.
        /// </summary>
        public double TorsoMultiplier { get { return torsoMultiplier; } set { torsoMultiplier = value; } }
        /// <summary>
        /// HeadMultiplier getter/setter.
        /// </summary>
        public double HeadMultiplier { get { return headMultiplier; } set { headMultiplier = value; } }
        /// <summary>
        /// WalkSpeed getter/setter.
        /// </summary>
        public double WalkSpeed { get { return walkspeed; } set { walkspeed = value; } }
    }

    /// <summary>
    /// Creates objects for defining ranged weapons' attributes (grenades, guns).
    /// </summary>
    public class Ranged
    {
        private double range1,
                        range2,
                        range1Damage,
                        range2Damage;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="range1">Close range in studs.</param>
        /// <param name="range2">Far range in studs.</param>
        /// <param name="range1Damage">Range 1's damage.</param>
        /// <param name="range2Damage">Range 2's damage.</param>
        public Ranged(double range1, double range2, double range1Damage, double range2Damage)
        {
            this.range1 = range1;
            this.range2 = range2;
            this.range1Damage = range1Damage;
            this.range2Damage = range2Damage;
        }

        /// <summary>
        /// Damage for a given weapon and range.
        /// </summary>
        /// <param name="weapon">The weapon to be used for calculations.</param>
        /// <param name="range">The distance to the target.</param>
        /// <returns>The damage at a given range.</returns>
        public static double damageFunction(Ranged weapon, double range)
        {
            double range1 = weapon.Range1;
            double range2 = weapon.Range2;
            double range1damage = weapon.Range1Damage;
            double range2damage = weapon.Range2Damage;

            if (range <= range1 && range >= 0)
            {
                return range1damage;
            }
            else if (range >= range2)
            {
                return range2damage;
            }
            else if (range <= range2 && range >= range1)
            {
                double slope = (range2damage - range1damage) / (range2 - range1);
                double yintercept = range2damage - (range2 * slope);
                return range * slope + yintercept;
            }
            else
            {
                //range was below 0; impossible case
                throw new ArgumentException("Range cannot be a negative value.", nameof(range));
            }
            //gun.RangedAttributes.Range1Damage
        }

        /// <summary>
        /// General calculator for how many hits it takes to kill an enemy from full health.
        /// </summary>
        /// <param name="rangedAttributes">The <see cref="Ranged.Ranged(double, double, double, double)">rangedAttributes</see> of the weapon.</param>
        /// <param name="multiplier">The multiplier associated with the weapon.</param>
        /// <param name="range">The distance to the target.</param>
        /// <returns>The number of hits that kills an enemy from full health.</returns>
        /// <exception cref="OutOfMemoryException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static int lethalityCalculatorGeneral(Ranged rangedAttributes, double multiplier, double range)
        {
            if (rangedAttributes.Range2Damage > 0 && range >= 0 && multiplier > 0)
            {

                for (int i = 0; i < 50; i++)
                {
                    double outt = Ranged.damageFunction(rangedAttributes, range) * multiplier * i;
                    if (outt >= 100)
                    {
                        return i;
                    }

                    if (i > 25) break; //to break the loop

                }
                throw new OutOfMemoryException("A value passed prevented the loop from returning a value (This might also happen if something takes more than 25 hits to kill, in which case, contact me and I'll fix the code.)");

            }
            throw new ArgumentException("No value can be equal to zero.", "Either multiplier or a value in the Ranged object passed.");

        }

        /// <summary>
        /// General calculator for how many hits it takes to kill an enemy from full health.
        /// </summary>
        /// <param name="multiplier">The multiplier associated with the weapon.</param>
        /// <param name="range">The distance to the target.</param>
        /// <returns>The number of hits that kills an enemy from full health.</returns>
        /// <exception cref="OutOfMemoryException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public int lethalityCalculatorGeneral(double multiplier, double range)
        {
            if (range2Damage > 0 && range >= 0 && multiplier > 0)
            {

                for (int i = 0; i < 50; i++)
                {
                    double outt = Ranged.damageFunction(this, range) * multiplier * i;
                    if (outt >= 100)
                    {
                        return i;
                    }

                    if (i > 25) break; //to break the loop

                }
                throw new OutOfMemoryException("A value passed prevented the loop from returning a value (This might also happen if something takes more than 25 hits to kill, in which case, contact me and I'll fix the code.)");

            }
            throw new ArgumentException("No value can be equal to zero.", "Either multiplier or a value in the Ranged object passed.");

        }

        /// <summary>
        /// Gun calculator for how many shots it takes to kill an enemy.
        /// </summary>
        /// <param name="conversion">The <c>Conversion</c> object to be used for the calculations.</param>
        /// <param name="range">The distance to the target.</param>
        /// <returns>A Tuple containing the shots required to kill for limb, torso and head hits respectively.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="OutOfMemoryException"></exception>
        public static Tuple<int, int, int> lethalityCalculator(Conversion conversion, double range)
        {
            int limb = 0;
            int torso = 0;
            int head = 0;
            try
            {
                limb = lethalityCalculatorGeneral(conversion.RangedAttributes, conversion.CarriedAttributes.LimbMultiplier, range);
                torso = lethalityCalculatorGeneral(conversion.RangedAttributes, conversion.CarriedAttributes.TorsoMultiplier, range);
                head = lethalityCalculatorGeneral(conversion.RangedAttributes, conversion.CarriedAttributes.HeadMultiplier, range);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.Message, e);
            }
            catch (OutOfMemoryException f)
            {
                throw new OutOfMemoryException(f.Message, f);
            }
            return Tuple.Create(limb, torso, head);
        }


        /// <summary>
        /// Range1 getter/setter.
        /// </summary>
        public double Range1 { get { return range1; } set { range1 = value; } }
        /// <summary>
        /// Range2 getter/setter.
        /// </summary>
        public double Range2 { get { return range2; } set { range2 = value; } }
        /// <summary>
        /// DamageRange1 getter/setter.
        /// </summary>
        public double Range1Damage { get { return range1Damage; } set { range1Damage = value; } }
        /// <summary>
        /// DamageRange2 getter/setter.
        /// </summary>
        public double Range2Damage { get { return range2Damage; } set { range2Damage = value; } }

    }

    /// <summary>
    /// Class defining a list of <c>FireMode</c> objects.
    /// </summary>
    public class FireModeList
    {
        private List<FireMode> modes = new();

        /// <summary>
        /// <c>FireModeList</c> indexer
        /// </summary>
        /// <param name="index">Index of the <c>FireModeList</c>.</param>
        /// <returns></returns>
        public FireMode this[int index]
        {
            get { return modes[index]; }
            set { modes[index] = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="firemodes"><see cref="ParseFireModeStringIterator(List{string})">See parameters.</see></param>
        public FireModeList(List<string> firemodes)
        {
            ParseFireModeStringIterator(firemodes);
        }

        /// <summary>
        /// Contrsuctor for adding existing <c>FireMode</c> objects.
        /// </summary>
        /// <param name="firemodes">The <c>FireMode</c> objects to be added.</param>
        public FireModeList(List<FireMode> firemodes)
        {
            foreach (FireMode r in firemodes)
            {
                modes.Add(r);
            }
        }

        /// <summary>
        /// Parses a string into a <c>FireMode</c> object. Case-insensitive. Does not accept any special characters or spaces.
        /// <example>
        /// Example of formatting:
        /// <code>
        /// a600 -> automatic 600rpm
        /// s750 -> semiautomatic 750rpm
        /// s40la0 -> semiautomatic 40rpm, lever action
        /// s300sh8 -> semiautomatic 300 rpm, shotgun with 8 pellets
        /// b400bb0 -> burst (double) 400rpm
        /// </code>
        /// Code formatting: mode,firerate(rpm),special modes,pellets(for shotguns)
        /// <code>
        /// mode = (a|auto|automatic), (s|semi|semiautomatic), (b|burst)
        /// firerate = Any integer. Non-negative.
        /// special modes = For burst: (b = InstantBurst,bb = DoubleBurst,bbb = TripleBurst); 
        ///                 For semiautomatic: (la|lever|leveraction = LeverAction, 
        ///                                     ps|pump|pumpshotgun = PumpShotgun,
        ///                                     sh|shotgun = Shotgun)
        /// pellets = Any integer, but required to include and can be set to 0. Non-negative.
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="firemodes">A string specified in the format above. Case-insensitive. Does not accept any special characters or spaces.</param>
        /// <returns><c>FireMode</c> object referring to Automatic, Semiautomatic</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static FireMode ParseFireModeString(string firemodes)
        {


            string str = firemodes;

            string mode = "";
            int firerate = 0;
            string specialFlags = "";
            int pellets = 0;

            List<string> resultInt = new();
            List<string> resultString = new();
            int count = 0; //count for numbers
            int count1 = 0; //count for letters
            string output = " ";
            try
            {
                //iterates through the individual string and parses
                for (int i = 0; i < str.Length; i++)
                {
                    output = (count + ", " + count1 + ", " + i + " ");
                    File.AppendAllText("test.log", output);

                    if (Convert.ToInt32(str[i]) > 64 && Convert.ToInt32(str[i]) < 91)
                    { //if uppercase, turns into lowercase
                        str = str.Replace(str[i], (char)(Convert.ToInt32(str[i]) + 32));
                    }

                    if (Convert.ToInt32(str[i]) > 47 && Convert.ToInt32(str[i]) < 58)
                    { //detects number, increments count

                        count++;
                        if (i + 1 == str.Length)
                        { //detects if the end of the string has been reached and adds previous whole number
                            resultInt.Add(str.Substring(i - count + 1, count));
                            output = string.Concat(str.AsSpan(i - count + 1, count), " ");
                            File.AppendAllText("test.log", output);
                        }

                        if (i > 0)
                        { //avoids outofboundexception
                            if (Convert.ToInt32(str[i - 1]) > 96 && Convert.ToInt32(str[i - 1]) < 123)
                            { //detects if previous character is a letter and adds the whole word before into the list
                                resultString.Add(str.Substring(i - count1, count1));
                                output = string.Concat(str.AsSpan(i - count, count), " ");

                                File.AppendAllText("test.log", output);
                            }
                        }

                        count1 = 0; //reset letter counter
                    }
                    else
                    {

                        if (i > 0)
                        { //avoids outofboundexception
                            if (Convert.ToInt32(str[i - 1]) > 47 && Convert.ToInt32(str[i - 1]) < 58)
                            { //detects if previous character is a number and adds the whole number before into the list
                                resultInt.Add(str.Substring(i - count, count));
                                output = string.Concat(str.AsSpan(i - count, count), " ");

                                File.AppendAllText("test.log", output);
                            }
                        }

                        if (Convert.ToInt32(str[i]) > 96 && Convert.ToInt32(str[i]) < 123)
                        { //detects letter, increments count
                            count1++;
                            if (i + 1 == str.Length)
                            { //detects if the end of the string has been reached and adds previous whole word
                                resultString.Add(str.Substring(i - count + 1, count));
                                output = string.Concat(str.AsSpan(i - count + 1, count), " ");
                                File.AppendAllText("test.log", output);
                            }
                        }

                        count = 0; //reset number counter
                    }
                }

                firerate = (resultInt[0].Length != 0) ? Convert.ToInt32(resultInt[0]) : 0;
                mode = (resultString[0].Length != 0) ? resultString[0] : "";
            }
            catch (NullReferenceException e)
            {
                throw new NullReferenceException("String cannot be null", e);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException(nameof(firemodes), firemodes, "String must have both numbers and letters, no spaces.");
            }


            try
            {
                specialFlags = (resultString[1].Length != 0) ? resultString[1] : "";
            }
            catch (ArgumentOutOfRangeException)
            {

                specialFlags = "";
            }

            try
            {
                pellets = (resultInt[1].Length != 0) ? Convert.ToInt32(resultInt[1]) : 0;
            }
            catch (ArgumentOutOfRangeException)
            {
                pellets = 0;
            }

            char firstCharacterMode = mode[0];


            if ((mode.Contains("automatic") || str.Contains("auto")) || firstCharacterMode == 'a')
            {
                return new FireMode(firerate, "Automatic", false, "", (specialFlags.Length != 0), specialFlags, 0);
                //modes.Add(Automatic); //Automatic SemiAutomatic Burst | BoltAction LeverAction PumpShotgun Shotgun TripleBurst DoubleBurst InstantBurst
            }
            else if ((mode.Contains("semiautomatic") || mode.Contains("semi") || mode.Contains("semi-automatic")) || firstCharacterMode == 's')
            {
                if (specialFlags.Contains("boltaction") || specialFlags.Contains("bolt") || specialFlags.Contains("ba"))
                {
                    return new FireMode(firerate, "SemiAutomatic", false, "", true, "BoltAction", 0);
                    //modes.Add(BoltAction);
                }
                else if (specialFlags.Contains("leveraction") || specialFlags.Contains("lever") || specialFlags.Contains("la"))
                {
                    return new FireMode(firerate, "SemiAutomatic", false, "", true, "LeverAction", 0);
                    //modes.Add(LeverAction);
                }
                else if (specialFlags.Contains("pumpshotgun") || specialFlags.Contains("pump") || specialFlags.Contains("ps"))
                {
                    return new FireMode(firerate, "SemiAutomatic", false, "", true, "PumpShotgun", pellets);
                    //modes.Add(PumpShotgun);
                }
                else if ((specialFlags.Contains("shotgun") || specialFlags.Contains("sh")) && !(specialFlags.Contains("pumpshotgun")))
                {
                    return new FireMode(firerate, "SemiAutomatic", false, "", true, "Shotgun", pellets);
                    //modes.Add(Shotgun);
                }


                return new FireMode(firerate, "SemiAutomatic", false, "", (specialFlags.Length != 0), specialFlags, 0);
                //modes.Add(SemiAutomatic);

            }
            else if ((mode.Contains("burst") || firstCharacterMode == 'b'))
            {
                if (specialFlags.Contains("bbbbb"))
                {
                    return new FireMode(firerate, "Burst", true, "IIIII", true, "QuintupleBurst", 0);
                    //modes.Add(TripleBurst);
                }
                else
                if (specialFlags.Contains("bbbb"))
                {
                    return new FireMode(firerate, "Burst", true, "IIII", true, "QuadrupleBurst", 0);
                    //modes.Add(TripleBurst);
                }
                else
                if (specialFlags.Contains("bbb"))
                {
                    return new FireMode(firerate, "Burst", true, "III", true, "TripleBurst", 0);
                    //modes.Add(TripleBurst);
                }
                else if (specialFlags.Contains("bb"))
                {

                    return new FireMode(firerate, "Burst", true, "II", true, "DoubleBurst", 0);
                    //modes.Add(DoubleBurst);
                }
                else if (specialFlags.Contains('b'))
                {
                    return new FireMode(firerate, "Burst", true, "I", true, "InstantBurst", pellets);
                    //modes.Add(InstantBurst);
                }



            }
            throw new ArgumentException("The value provided did not meet the criteria specified.", nameof(firemodes));
        }

        /// <summary>
        /// Iterates through the string array of firemodes.
        /// </summary>
        /// <param name="firemodes">An array of strings as specified in the <c>ParseFireModeString</c> method as <see cref="ParseFireModeString(string)">seen here</see>.</param>
        /// <exception cref="NullReferenceException"></exception>
        public void ParseFireModeStringIterator(List<string> firemodes)
        {
            try
            {
                foreach (string str in firemodes)
                {
                    modes.Add(ParseFireModeString(str));

                }
            }
            catch (NullReferenceException e)
            {
                throw new NullReferenceException("firemodes cannot be null.", e);
            }
        }


        /// <summary>
        /// Searches the list for a specific <c>FireMode</c>.
        /// </summary>
        /// <param name="firemode">The <c>FireMode</c> to search with.</param>
        /// <returns>True if the list contains the FireMode, false otherwise.</returns>
        public bool searchList(FireMode firemode)
        {
            if (modes.Contains(firemode))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Searches the list for a specific <c>FireMode</c>'s index.
        /// </summary>
        /// <param name="firemode">The <c>FireMode</c> to search with.</param>
        /// <returns>Index of item if it is found, otherwise -1.</returns>
        public int indexOfFireMode(FireMode firemode)
        {
            if (modes.Contains(firemode))
            {
                return modes.IndexOf(firemode);
            }
            else
            {
                return -1;
            }
        }


        /// <summary>
        /// Adds element to the end of the <c>FireModeList</c> list
        /// </summary>
        /// <param name="firemode">The <c>FireMode</c> to add.</param>
        /// <returns></returns>
        public bool addFireMode(FireMode firemode)
        {
            modes.Add(firemode);
            return searchList(firemode);
        }


        /// <summary>
        /// Adds new default firemode.
        /// </summary>
        /// <param name="firemode">The <c>FireMode</c> to insert.</param>
        public void addNewDefaultFiremode(FireMode firemode)
        {
            modes.Insert(0, firemode);
        }


        /// <summary>
        /// Searches through the list of <c>FireMode</c> objects for a specific property.
        /// </summary>
        /// <param name="mode">String of the mode desired. Modes include: <code>Automatic SemiAutomatic Burst | BoltAction LeverAction PumpShotgun Shotgun TripleBurst DoubleBurst InstantBurst</code></param>
        /// <param name="firerate">Firerate of the mode desired. Non-negative.</param>
        /// <param name="pellets">Pellets of the mode desired. Non-negative.</param>
        /// <returns>If the list has the specified properties</returns>
        public bool searchListProperties(string mode, int firerate, int pellets)
        {
            if (firerate < 0 || pellets < 0) throw new ArgumentException("Neither integer parameter can be negative.");
            //Automatic SemiAutomatic Burst | BoltAction LeverAction PumpShotgun Shotgun TripleBurst DoubleBurst InstantBurst
            foreach (FireMode firemode in modes)
            {
                if (mode.ToLower() == firemode.Mode.ToLower() || (mode.ToLower().Contains("semi") && firemode.Mode.ToLower() == "semiautomatic") || firemode.Pellets == pellets || firemode.Firerate == firerate)
                {
                    return true;
                }
                else
                {
                    if (mode.ToLower() == firemode.SpecialMode.ToLower() && firemode.Special)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Deletes a <c>FireMode</c> at a specified index.
        /// </summary>
        /// <param name="index">Index must be not equal to 0, and must also be greater than 0.</param>
        public void deleteFireMode(int index)
        {
            if (index != 0 && index > 0)
            {
                modes.RemoveAt(index);
            }
        }

        /// <summary>
        /// Deletes a range of <c>FireMode</c> objects.
        /// </summary>
        /// <param name="index">The starting index of the range to delete. Non-negative, non-zero.</param>
        /// <param name="count">The count of items to delete. Non-negative, non-zero.</param>
        /// <exception cref="ArgumentException"></exception>
        public void deleteFireModeRange(int index, int count)
        {
            if (index != 0 && index > 0 && count != 0 && count > 0)
            {
                modes.RemoveRange(index, count);

            }
            else if (index <= 0)
            {
                throw new ArgumentException("Cannot be negative.", nameof(index));
            }
            else
            {
                throw new ArgumentException("Cannot be negative.", nameof(count));
            }
        }

        /// <summary>
        /// Gets the count of <c>FireMode</c> objects.
        /// </summary>
        /// <returns>The count of <c>FireMode</c> objects.</returns>
        public int FireModeCount()
        {
            return modes.Count;
        }


        /// <summary>
        /// Returns a list of <c>FireMode</c> objects.
        /// </summary>
        /// <returns>List of <c>FireMode</c> objects contained in <c>FireModeList</c>.</returns>
        public List<FireMode> getFireModes()
        {
            return modes;
        }

    }

    /// <summary>
    /// Class that defines firemodes for guns.
    /// </summary>
    public class FireMode
    {


        private double firerate;
        private string mode;
        private bool burst;
        private string burstMode;
        private bool special;
        private string specialMode;
        private int pellets;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="firerate">Firerate of the gun in rounds per minute (RPM).</param>
        /// <param name="mode">Firing mode name, e.g. <c>(Automatic|Semiautomatic|Burst)</c>.</param>
        /// <param name="burst">Specifies if the firemode has burst capabilities.</param>
        /// <param name="burstMode">If the previous value is true, specifies if the firemode has InstantBurst("I"), DoubleBurst("II"), or TripleBurst("III").</param>
        /// <param name="special">Specifies if the firemode has special modes.</param>
        /// <param name="specialMode">If the previous value is true, specifies if the firemode has any special modes.</param>
        /// <param name="pellets">If the firemode is of a shotgun, specifies the number of pellets.</param>
        public FireMode(double firerate, string mode, bool burst, string burstMode, bool special, string specialMode, int pellets)
        {
            this.firerate = firerate;
            this.mode = mode;
            if (burst)
            {
                this.burstMode = burstMode;
            }
            else
            {
                this.burstMode = "";
            }

            if (special)
            {
                this.specialMode = specialMode;
            }
            else
            {
                this.specialMode = "";
            }

            this.burst = burst;
            this.special = special;
            this.pellets = pellets;
        }
        /// <summary>
        /// Firerate getter/setter.
        /// </summary>
        public double Firerate { get { return firerate; } set { firerate = value; } }
        /// <summary>
        /// Mode getter/setter.
        /// </summary>
        public string Mode { get { return mode; } set { mode = value; } }
        /// <summary>
        /// Burst getter/setter.
        /// </summary>
        public bool Burst { get { return burst; } set { burst = value; } }
        /// <summary>
        /// BurstMode getter/setter.
        /// </summary>
        public string BurstMode { get { return burstMode; } set { burstMode = value; } }
        /// <summary>
        /// Special getter/setter.
        /// </summary>
        public bool Special { get { return special; } set { special = value; } }
        /// <summary>
        /// SpecialMode getter/setter.
        /// </summary>
        public string SpecialMode { get { return specialMode; } set { specialMode = value; } }
        /// <summary>
        /// Pellets getter/setter.
        /// </summary>
        public int Pellets { get { return pellets; } set { pellets = value; } }


    }

    /// <summary>
    /// Defines a list of <c>Conversion</c> objects.
    /// </summary>
    public class ConversionList
    {

        private List<Conversion> conversions = new();

        /// <summary>
        /// Indexer for the list.
        /// </summary>
        /// <param name="index">Index of the list to be accessed.</param>
        /// <returns>Index of the list to be accessed.</returns>
        public Conversion this[int index]
        {
            get
            {
                return conversions[index];
            }
            set
            {
                conversions[index] = value;
            }
        }

        /// <summary>
        /// Adds a new <c>Conversion</c> object to the end of the list.
        /// </summary>
        /// <param name="conversion">The <c>Conversion</c> object to add.</param>
        public void addConversion(Conversion conversion)
        {
            conversions.Add(conversion);
        }

        /// <summary>
        /// Default <c>Conversion</c> object getter/setter.
        /// </summary>
        public Conversion DefaultConversion { get { return conversions[0]; } set { conversions[0] = value; } }

        /// <summary>
        /// Removes the <c>Conversion</c> object at the specified index of the list.
        /// </summary>
        /// <param name="index">The index of the list to be removed.</param>
        public void removeConversion(int index)
        {
            try
            {
                conversions.RemoveAt(index);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException(e.Message, e);
            }
        }

        /// <summary>
        /// Inserts a new <c>Conversion</c> object.
        /// </summary>
        /// <param name="index">Index of the intended <c>Conversion</c> object insertion.</param>
        /// <param name="conversion">The <c>Conversion</c> object to be inserted.</param>
        public void insertConversion(Conversion conversion, int index)
        {
            conversions.Insert(index, conversion);
        }

        /// <summary>
        /// Gets the length of the list.
        /// </summary>
        /// <returns>The count of elements in the list.</returns>
        public int conversionsCount()
        {
            return conversions.Count;

        }

        /// <summary>
        /// Gets the list of <c>Converison</c> objects.
        /// </summary>
        /// <returns>The list of the conversions.</returns>
        public List<Conversion> getConversions()
        {
            return conversions;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="defaultConversion">The default <c>Conversion</c> object of the gun. See <see cref="Conversion.Conversion(string, int, int, List{string}, Carried, Ranged, double, double, double, double, double, double)"/></param>
        public ConversionList(Conversion defaultConversion)
        {
            addConversion(defaultConversion);

        }

    }

    /// <summary>
    /// Defines a conversion for a gun.
    /// </summary>
    public class Conversion
    {
        private string attachment;
        private string conversionName;
        private bool ammoConversion;


        private FireModeList fireModes;
        private Carried carriedAttributes;
        private Ranged rangedAttributes;
        private string caliber;
        private int ammoCapacity;
        private int magazineCapacity;
        private double penetration;
        private double muzzleVelocity;
        private double aimingWalkspeed;
        private double reloadSpeed;
        private double emptyReloadSpeed;
        private double suppression;

        /// <summary>
        /// DO NOT USE THIS OVERLOAD
        /// </summary>
        /// <param name="armourPiercing"></param>
        /// <param name="hollowPoint"></param>
        /// <param name="carriedAttributes"></param>
        /// <param name="rangedAttributes"></param>
        /// <param name="penetration"></param>
        /// <param name="suppression"></param>
        public Conversion(bool armourPiercing, bool hollowPoint, Carried carriedAttributes, Ranged rangedAttributes, double penetration, double suppression)
        {
            if (hollowPoint)
            {
                this.carriedAttributes = new Carried(carriedAttributes.LimbMultiplier, carriedAttributes.TorsoMultiplier / carriedAttributes.TorsoMultiplier, carriedAttributes.HeadMultiplier * (1 / 1.2), carriedAttributes.WalkSpeed);
                this.rangedAttributes = new Ranged(10.5529 * Math.Log(rangedAttributes.Range1, 2.61), rangedAttributes.Range2 * 0.9, rangedAttributes.Range1Damage * 1.2, rangedAttributes.Range2Damage / 1.2);
                this.penetration = penetration * 0.5;
                this.suppression = suppression * 1.1;
            }
            else if (armourPiercing)
            {
                this.carriedAttributes = new Carried(carriedAttributes.LimbMultiplier, carriedAttributes.TorsoMultiplier, carriedAttributes.HeadMultiplier, carriedAttributes.WalkSpeed);
                this.rangedAttributes = new Ranged(rangedAttributes.Range1 * 0.5, rangedAttributes.Range2, rangedAttributes.Range1Damage, rangedAttributes.Range2Damage);
                this.penetration = penetration * 1.5;
                this.suppression = suppression * 0.5;
            }
        }

        /// <summary>
        /// Constructor for armour piercing / hollow point rounds.
        /// </summary>
        /// <param name="armourPiercing">If the gun has armour piercing rounds. Mutually exclusive with <c>hollowPoint</c>.</param>
        /// <param name="hollowPoint">If the gun has hollow point rounds. Mutually exclusive with <c>armourPiercing</c>.</param>
        /// <param name="gun"><c>Gun</c> object to pass through.</param>
        /// <exception cref="ArgumentException"></exception>
        public Conversion(bool armourPiercing, bool hollowPoint, Gun gun)
        {

            if (gun.DefaultAmmoCapacity <= 0)
            {
                throw new ArgumentException("Cannot be zero nor negative.", nameof(ammoCapacity));
            }
            else if (gun.DefaultMagazineCapacity <= 0)
            {
                throw new ArgumentException("Cannot be zero nor negative.", nameof(magazineCapacity));
            }
            else if (gun.DefaultReloadSpeed <= 0)
            {
                throw new ArgumentException("Cannot be zero nor negative.", nameof(reloadSpeed));
            }
            else if (gun.DefaultEmptyReloadSpeed <= 0)
            {
                throw new ArgumentException("Cannot be zero nor negative.", nameof(emptyReloadSpeed));
            }
            else if (gun.DefaultAimingWalkspeed <= 0 && !gun.DefaultCaliber.Contains("20x82mm")) //exclude ntw
            {
                throw new ArgumentException("Cannot be zero nor negative.", nameof(aimingWalkspeed));
            }
            if (hollowPoint)
            {
                this.carriedAttributes = new Carried(gun.DefaultCarriedAttributes.LimbMultiplier, gun.DefaultCarriedAttributes.TorsoMultiplier / gun.DefaultCarriedAttributes.TorsoMultiplier, gun.DefaultCarriedAttributes.HeadMultiplier * (1 / 1.2), gun.DefaultCarriedAttributes.WalkSpeed);
                this.rangedAttributes = new Ranged(10.5529 * Math.Log(gun.DefaultRangedAttributes.Range1, 2.61), gun.DefaultRangedAttributes.Range2 * 0.9, gun.DefaultRangedAttributes.Range1Damage * 1.2, gun.DefaultRangedAttributes.Range2Damage / 1.2);
                penetration = gun.DefaultPenetration * 0.5;
                suppression = gun.DefaultSuppression * 1.1;


                attachment = "Hollow Point";
                ammoConversion = true;
                conversionName = gun.Name + " Hollow Point";

                //pass through params
                fireModes = gun.DefaultFireModes;
                caliber = gun.DefaultCaliber;
                aimingWalkspeed = gun.DefaultAimingWalkspeed;
                emptyReloadSpeed = gun.DefaultEmptyReloadSpeed;
                reloadSpeed = gun.DefaultReloadSpeed;
                muzzleVelocity = gun.DefaultMuzzleVelocity;
                ammoCapacity = gun.DefaultAmmoCapacity;
                magazineCapacity = gun.DefaultMagazineCapacity;
            }
            else if (armourPiercing)
            {
                this.carriedAttributes = new Carried(gun.DefaultCarriedAttributes.LimbMultiplier, gun.DefaultCarriedAttributes.TorsoMultiplier, gun.DefaultCarriedAttributes.HeadMultiplier, gun.DefaultCarriedAttributes.WalkSpeed);
                this.rangedAttributes = new Ranged(gun.DefaultRangedAttributes.Range1 * 0.5, gun.DefaultRangedAttributes.Range2, gun.DefaultRangedAttributes.Range1Damage, gun.DefaultRangedAttributes.Range2Damage);
                penetration = gun.DefaultPenetration * 1.5;
                suppression = gun.DefaultSuppression * 0.5;

                attachment = "Armour Piercing";
                ammoConversion = true;
                conversionName = gun.Name + " Armour Piercing";

                //pass through params
                fireModes = gun.DefaultFireModes;
                caliber = gun.DefaultCaliber;
                aimingWalkspeed = gun.DefaultAimingWalkspeed;
                emptyReloadSpeed = gun.DefaultEmptyReloadSpeed;
                reloadSpeed = gun.DefaultReloadSpeed;
                muzzleVelocity = gun.DefaultMuzzleVelocity;
                ammoCapacity = gun.DefaultAmmoCapacity;
                magazineCapacity = gun.DefaultMagazineCapacity;
            }
        }

        /// <summary>
        /// For adding a new conversion with <b>different firemodes</b> from the original gun. 
        /// </summary>
        /// <param name="name">Name of the gun which has the conversion. (You can pass <c>[Gun].Name</c>)</param>
        /// <param name="attachment">Attachment required for conversion.</param>
        /// <param name="conversionName">The name of the gun if the conversion affects the gun's name. If left blank, this will be <c>name + " " + attachment</c>.</param>
        /// <param name="ammoConversion">If the conversion is an ammo conversion.</param>
        /// <param name="caliber">The caliber of the gun with the conversion.</param>
        /// <param name="ammoCapacity">The ammo capacity of the gun with the conversion.</param>
        /// <param name="magazineCapacity">The magazine capacity of the gun with the conversion.</param>
        /// <param name="firemodes">The <see cref="FireModeList.ParseFireModeStringIterator(List{string})">firemodes</see> of the gun with the conversion.</param>
        /// <param name="carriedAttributes">The <see cref="Carried">carriedAttributes</see> of the gun with the conversion.</param>
        /// <param name="rangedAttributes">The <see cref="Ranged">rangedAttributes</see> of the gun with the conversion.</param>
        /// <param name="penetration">The penetration depth of the gun with the conversion.</param>
        /// <param name="muzzleVelocity">The muzzle velocity of the gun with the conversion.</param>
        /// <param name="reloadSpeed">The reload speed of the gun with the conversion.</param>
        /// <param name="emptyReloadSpeed">The empty reload speed of the gun with the conversion.</param>
        /// <param name="suppression">The suppression of the gun with the conversion.</param>
        /// <param name="aimingWalkspeed">The aiming walkspeed of the gun with the conversion.</param>
        /// <exception cref="ArgumentException"></exception>
        public Conversion(string name, string attachment, string conversionName, bool? ammoConversion, string caliber, int ammoCapacity, int magazineCapacity, List<string> firemodes, Carried carriedAttributes, Ranged rangedAttributes, double penetration, double muzzleVelocity, double reloadSpeed, double emptyReloadSpeed, double suppression, double aimingWalkspeed)
        { //new conversions
            this.attachment = attachment;
            this.ammoConversion = ammoConversion ?? false;
            if (conversionName == "")
            {
                this.conversionName = name + " " + attachment;
            }
            else
            {
                this.conversionName = conversionName;
            }


            this.caliber = caliber;
            this.ammoCapacity = ammoCapacity;
            this.magazineCapacity = magazineCapacity;
            fireModes = new FireModeList(firemodes);
            this.carriedAttributes = carriedAttributes;
            this.rangedAttributes = rangedAttributes;
            this.penetration = penetration;
            this.muzzleVelocity = muzzleVelocity;
            this.reloadSpeed = reloadSpeed;
            this.emptyReloadSpeed = emptyReloadSpeed;
            this.suppression = suppression;
            this.aimingWalkspeed = aimingWalkspeed;


            if (ammoCapacity <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(ammoCapacity));
            }
            else if (magazineCapacity <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(magazineCapacity));
            }
            else if (reloadSpeed <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(reloadSpeed));
            }
            else if (emptyReloadSpeed <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(emptyReloadSpeed));
            }
            else if (aimingWalkspeed <= 0 && !caliber.Contains("20x82mm") && !conversionName.Contains("ntw", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("Cannot be null.", nameof(aimingWalkspeed));
            }
        }

        /// <summary>
        /// For adding a new conversion with the <b>same firemodes</b> as the original gun. 
        /// </summary>
        /// <param name="name">Name of the gun which has the conversion. (You can pass <c>[Gun].Name</c>)</param>
        /// <param name="attachment">Attachment required for conversion.</param>
        /// <param name="conversionName">The name of the gun if the conversion affects the gun's name. If left blank, this will be <c>name + " " + attachment</c>.</param>
        /// <param name="ammoConversion">If the conversion is an ammo conversion.</param>
        /// <param name="caliber">The caliber of the gun with the conversion.</param>
        /// <param name="ammoCapacity">The ammo capacity of the gun with the conversion.</param>
        /// <param name="magazineCapacity">The magazine capacity of the gun with the conversion.</param>
        /// <param name="firemodes">The firemodes of the gun with the conversion.</param>
        /// <param name="carriedAttributes">The <see cref="Carried">carriedAttributes</see> of the gun with the conversion.</param>
        /// <param name="rangedAttributes">The <see cref="Ranged">rangedAttributes</see> of the gun with the conversion.</param>
        /// <param name="penetration">The penetration depth of the gun with the conversion.</param>
        /// <param name="muzzleVelocity">The muzzle velocity of the gun with the conversion.</param>
        /// <param name="reloadSpeed">The reload speed of the gun with the conversion.</param>
        /// <param name="emptyReloadSpeed">The empty reload speed of the gun with the conversion.</param>
        /// <param name="suppression">The suppression of the gun with the conversion.</param>
        /// <param name="aimingWalkspeed">The aiming walkspeed of the gun with the conversion.</param>
        /// <exception cref="ArgumentException"></exception>
        public Conversion(string name, string attachment, string conversionName, bool? ammoConversion, string caliber, int ammoCapacity, int magazineCapacity, FireModeList firemodes, Carried carriedAttributes, Ranged rangedAttributes, double penetration, double muzzleVelocity, double reloadSpeed, double emptyReloadSpeed, double suppression, double aimingWalkspeed)
        { //new conversions (with same firemodes)
            this.attachment = attachment;
            this.ammoConversion = ammoConversion ?? false;
            if (conversionName == "")
            {
                this.conversionName = name + " " + attachment;
            }
            else
            {
                this.conversionName = conversionName;
            }

            this.caliber = caliber;
            this.ammoCapacity = ammoCapacity;
            this.magazineCapacity = magazineCapacity;
            fireModes = firemodes;
            this.carriedAttributes = carriedAttributes;
            this.rangedAttributes = rangedAttributes;
            this.penetration = penetration;
            this.muzzleVelocity = muzzleVelocity;
            this.reloadSpeed = reloadSpeed;
            this.emptyReloadSpeed = emptyReloadSpeed;
            this.suppression = suppression;
            this.aimingWalkspeed = aimingWalkspeed;

            if (ammoCapacity <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(ammoCapacity));
            }
            else if (magazineCapacity <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(magazineCapacity));
            }
            else if (reloadSpeed <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(reloadSpeed));
            }
            else if (emptyReloadSpeed <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(emptyReloadSpeed));
            }
            else if (aimingWalkspeed <= 0 && !caliber.Contains("20x82mm") && !conversionName.Contains("ntw", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("Cannot be null.", nameof(aimingWalkspeed));
            }
        }

        /// <summary>
        /// For adding the default conversion for a gun. 
        /// </summary>
        /// <param name="caliber">The caliber of the gun with the default conversion.</param>
        /// <param name="ammoCapacity">The ammo capacity of the gun with the default conversion.</param>
        /// <param name="magazineCapacity">The magazine capacity of the gun with the default conversion.</param>
        /// <param name="firemodes">The firemodes of the gun with the default conversion.</param>
        /// <param name="carriedAttributes">The <see cref="Carried">carriedAttributes</see> of the gun with the default conversion.</param>
        /// <param name="rangedAttributes">The <see cref="Ranged">rangedAttributes</see> of the gun with the default conversion.</param>
        /// <param name="penetration">The penetration depth of the gun with the default conversion.</param>
        /// <param name="muzzleVelocity">The muzzle velocity of the gun with the default conversion.</param>
        /// <param name="reloadSpeed">The reload speed of the gun with the default conversion.</param>
        /// <param name="emptyReloadSpeed">The empty reload speed of the gun with the default conversion.</param>
        /// <param name="suppression">The suppression of the gun with the default conversion.</param>
        /// <param name="aimingWalkspeed">The aiming walkspeed of the gun with the default conversion.</param>
        /// <exception cref="ArgumentException"></exception>
        public Conversion(string caliber, int ammoCapacity, int magazineCapacity, List<string> firemodes, Carried carriedAttributes, Ranged rangedAttributes, double penetration, double muzzleVelocity, double reloadSpeed, double emptyReloadSpeed, double suppression, double aimingWalkspeed)
        { //default

            this.caliber = caliber;
            this.ammoCapacity = ammoCapacity;
            this.magazineCapacity = magazineCapacity;
            fireModes = new FireModeList(firemodes);
            this.carriedAttributes = carriedAttributes;
            this.rangedAttributes = rangedAttributes;
            this.penetration = penetration;
            this.muzzleVelocity = muzzleVelocity;
            this.reloadSpeed = reloadSpeed;
            this.emptyReloadSpeed = emptyReloadSpeed;
            this.suppression = suppression;
            this.aimingWalkspeed = aimingWalkspeed;

            if (ammoCapacity <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(ammoCapacity));
            }
            else if (magazineCapacity <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(magazineCapacity));
            }
            else if (reloadSpeed <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(reloadSpeed));
            }
            else if (emptyReloadSpeed <= 0)
            {
                throw new ArgumentException("Cannot be null.", nameof(emptyReloadSpeed));
            }
            else if (aimingWalkspeed <= 0 && !caliber.Contains("20x82mm")) //exclude ntw
            {
                throw new ArgumentException("Cannot be null.", nameof(aimingWalkspeed));
            }

        }

        /// <summary>
        /// Attachment getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public string Attachment { get { return attachment; } set { attachment = value; } }

        /// <summary>
        /// ConversionName getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public string ConversionName { get { return conversionName; } set { conversionName = value; } }

        /// <summary>
        /// AmmoConversion getter/setter.  See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public bool AmmoConversion { get { return ammoConversion; } set { ammoConversion = value; } }


        /// <summary>
        /// FireModes getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public FireModeList FireModes { get { return fireModes; } set { fireModes = value; } }

        /// <summary>
        /// CarriedAttributes getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public Carried CarriedAttributes { get { return carriedAttributes; } set { carriedAttributes = value; } }

        /// <summary>
        /// RangedAttributes getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public Ranged RangedAttributes { get { return rangedAttributes; } set { rangedAttributes = value; } }

        /// <summary>
        /// Caliber getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public string Caliber { get { return caliber; } set { caliber = value; } }

        /// <summary>
        /// AmmoCapacity getter/setter. See<see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public int AmmoCapacity { get { return ammoCapacity; } set { ammoCapacity = value; } }

        /// <summary>
        /// MagazineCapacity getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public int MagazineCapacity { get { return magazineCapacity; } set { magazineCapacity = value; } }

        /// <summary>
        /// Penetration getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double Penetration { get { return penetration; } set { penetration = value; } }

        /// <summary>
        /// MuzzleVelocity getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double MuzzleVelocity { get { return muzzleVelocity; } set { muzzleVelocity = value; } }

        /// <summary>
        /// AimingWalkspeed getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double AimingWalkspeed { get { return aimingWalkspeed; } set { aimingWalkspeed = value; } }

        /// <summary>
        /// ReloadSpeed getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double ReloadSpeed { get { return reloadSpeed; } set { reloadSpeed = value; } }

        /// <summary>
        /// EmptyReloadSpeed getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double EmptyReloadSpeed { get { return emptyReloadSpeed; } set { emptyReloadSpeed = value; } }

        /// <summary>
        /// Suppression getter/setter. See <see cref="Conversion.Conversion(string, string, string, bool?, string, int, int, FireModeList, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double Suppression { get { return suppression; } set { suppression = value; } }
    }

    /// <summary>
    /// Defines a gun.
    /// </summary>
    public class Gun : Weapon
    {

        private ConversionList conversions;

        private FireModeList defaultFireModes;
        private Carried defaultCarriedAttributes;
        private Ranged defaultRangedAttributes;
        private string defaultCaliber;
        private int defaultAmmoCapacity;
        private int defaultMagazineCapacity;
        private double defaultPenetration;
        private double defaultMuzzleVelocity;
        private double aimingWalkspeed;
        private double defaultReloadSpeed;
        private double defaultEmptyReloadSpeed;
        private double defaultSuppression;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the gun.</param>
        /// <param name="hasRank">If the gun has a rank.</param>
        /// <param name="rank">The rank of the gun.</param>
        /// <param name="defaultGun">The <c>Conversion</c> object defining the default gun. <see cref="Conversion.Conversion(string, int, int, List{string}, Carried, Ranged, double, double, double, double, double, double)"></see></param>
        /// <param name="hasArmourPiercing">If the gun supports armour piercing rounds.</param>
        /// <param name="hasHollowPoint">If the gun supports hollow point rounds</param>
        public Gun(string name, bool hasRank, int rank, Conversion defaultGun, bool hasArmourPiercing, bool hasHollowPoint) : base(name, hasRank, rank)
        {

            defaultCaliber = defaultGun.Caliber;
            defaultAmmoCapacity = defaultGun.AmmoCapacity;
            defaultMagazineCapacity = defaultGun.MagazineCapacity;
            defaultFireModes = defaultGun.FireModes;
            defaultCarriedAttributes = defaultGun.CarriedAttributes;
            defaultRangedAttributes = defaultGun.RangedAttributes;
            defaultPenetration = defaultGun.Penetration;
            defaultMuzzleVelocity = defaultGun.MuzzleVelocity;
            defaultReloadSpeed = defaultGun.ReloadSpeed;
            defaultEmptyReloadSpeed = defaultGun.EmptyReloadSpeed;
            defaultSuppression = defaultGun.Suppression;
            aimingWalkspeed = defaultGun.AimingWalkspeed;

            conversions = addNewConversionList(defaultGun);
            if (hasArmourPiercing)
            {
                conversions.addConversion(new Conversion(true, false, this));
            }
            if (hasHollowPoint)
            {
                conversions.addConversion(new Conversion(false, true, this));
            }
        }

        /// <summary>
        /// Constructor for SQL database reading.
        /// </summary>
        /// <param name="name">Name of the gun.</param>
        /// <param name="hasRank">If the gun has a rank.</param>
        /// <param name="rank">The gun's rank.</param>
        /// <param name="conversions">List of conversions as a <c>ConversionList</c> object.</param>
        public Gun(string name, bool hasRank, int rank, ConversionList conversions) : base(name, hasRank, rank)
        {
            defaultCaliber = conversions.DefaultConversion.Caliber;
            defaultAmmoCapacity = conversions.DefaultConversion.AmmoCapacity;
            defaultMagazineCapacity = conversions.DefaultConversion.MagazineCapacity;
            defaultFireModes = conversions.DefaultConversion.FireModes;
            defaultCarriedAttributes = conversions.DefaultConversion.CarriedAttributes;
            defaultRangedAttributes = conversions.DefaultConversion.RangedAttributes;
            defaultPenetration = conversions.DefaultConversion.Penetration;
            defaultMuzzleVelocity = conversions.DefaultConversion.MuzzleVelocity;
            defaultReloadSpeed = conversions.DefaultConversion.ReloadSpeed;
            defaultEmptyReloadSpeed = conversions.DefaultConversion.EmptyReloadSpeed;
            defaultSuppression = conversions.DefaultConversion.Suppression;
            aimingWalkspeed = conversions.DefaultConversion.AimingWalkspeed;
            this.conversions = conversions;
        }


        /// <summary>
        /// Weapon placeholder constructor.
        /// </summary>
        /// <param name="name">Name of the gun.</param>
        /// <param name="hasRank">If the gun has a rank.</param>
        /// <param name="rank">The rank of the gun, if any.</param>
        public Gun(string name, bool hasRank, int rank) : base(name, hasRank, rank)
        {
        }

        private static ConversionList addNewConversionList(Conversion defaultConversion)
        {
            return new ConversionList(defaultConversion);
        }




        //public 
        /// <summary>
        /// Conversions getter/setter.
        /// </summary>
        public ConversionList Conversions { get { return conversions; } set { conversions = value; } }


        /// <summary>
        /// FireModes getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int,List{string}, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public FireModeList DefaultFireModes { get { return defaultFireModes; } set { defaultFireModes = value; } }

        /// <summary>
        /// CarriedAttributes getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int, List{string}, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public Carried DefaultCarriedAttributes { get { return defaultCarriedAttributes; } set { defaultCarriedAttributes = value; } }

        /// <summary>
        /// RangedAttributes getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int,List{string}, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public Ranged DefaultRangedAttributes { get { return defaultRangedAttributes; } set { defaultRangedAttributes = value; } }

        /// <summary>
        /// Caliber getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int,List{string}, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public string DefaultCaliber { get { return defaultCaliber; } set { defaultCaliber = value; } }

        /// <summary>
        /// AmmoCapacity getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int,List{string}, Carried, Ranged, double, double, double, double, double, double)"/> 
        /// </summary>
        public int DefaultAmmoCapacity { get { return defaultAmmoCapacity; } set { defaultAmmoCapacity = value; } }

        /// <summary>
        /// MagazineCapacity getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int, List{string}, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public int DefaultMagazineCapacity { get { return defaultMagazineCapacity; } set { defaultMagazineCapacity = value; } }

        /// <summary>
        /// Penetration getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int, List{string}, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double DefaultPenetration { get { return defaultPenetration; } set { defaultPenetration = value; } }

        /// <summary>
        /// MuzzleVelocity getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int, List{string}, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double DefaultMuzzleVelocity { get { return defaultMuzzleVelocity; } set { defaultMuzzleVelocity = value; } }

        /// <summary>
        /// AimingWalkspeed getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int, List{string}, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double DefaultAimingWalkspeed { get { return aimingWalkspeed; } set { aimingWalkspeed = value; } }

        /// <summary>
        /// ReloadSpeed getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int, List{string}, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double DefaultReloadSpeed { get { return defaultReloadSpeed; } set { defaultReloadSpeed = value; } }

        /// <summary>
        /// EmptyReloadSpeed getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int, List{string}, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double DefaultEmptyReloadSpeed { get { return defaultEmptyReloadSpeed; } set { defaultEmptyReloadSpeed = value; } }

        /// <summary>
        /// Suppression getter/setter. See <see cref="Gun.Gun(string, bool, int, Conversion, bool, bool)"/> and <see cref="Conversion.Conversion(string, int, int, List{string}, Carried, Ranged, double, double, double, double, double, double)"/>
        /// </summary>
        public double DefaultSuppression { get { return defaultSuppression; } set { defaultSuppression = value; } }




    }

    /// <summary>
    /// Defines a melee weapon.
    /// </summary>
    public class Melee : Weapon
    {
        private double frontStabDamage;
        private double backStabDamage;
        private double bladeLength;
        private Carried carriedAttributes;

        /// <summary>
        /// Weapon placeholder constructor.
        /// </summary>
        /// <param name="name">Name of the melee.</param>
        /// <param name="hasRank">If the melee has a rank.</param>
        /// <param name="rank">The rank of the melee, if any.</param>
        public Melee(string name, bool hasRank, int rank) : base(name, hasRank, rank)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the melee.</param>
        /// <param name="hasRank">If the melee has a rank.</param>
        /// <param name="rank">The rank of the melee.</param>
        /// <param name="frontStabDamage">The front stab damage of the melee.</param>
        /// <param name="backStabDamage">The back stab damage of the melee.</param>
        /// <param name="bladeLength">The blade length of the melee.</param>
        /// <param name="carriedAttributes">The <see cref="Carried.Carried(double, double, double, double)">carriedAttributes</see> of the melee.</param>
        public Melee(string name, bool hasRank, int rank, double frontStabDamage, double backStabDamage, double bladeLength, Carried carriedAttributes) : base(name, hasRank, rank)
        {
            this.frontStabDamage = frontStabDamage;
            this.backStabDamage = backStabDamage;
            this.bladeLength = bladeLength;
            this.carriedAttributes = carriedAttributes;
        }

        /// <summary>
        /// FrontStabDamage getter/setter. See <see cref="Melee.Melee(string, bool, int, double, double, double, Carried)"/>
        /// </summary>
        public double FrontStabDamage { get { return frontStabDamage; } set { frontStabDamage = value; } }

        /// <summary>
        /// BackStabDamage getter/setter. See <see cref="Melee.Melee(string, bool, int, double, double, double, Carried)"/>
        /// </summary>
        public double BackStabDamage { get { return backStabDamage; } set { backStabDamage = value; } }

        /// <summary>
        /// BladeLength getter/setter. See <see cref="Melee.Melee(string, bool, int, double, double, double, Carried)"/>
        /// </summary>
        public double BladeLength { get { return bladeLength; } set { bladeLength = value; } }

        /// <summary>
        /// CarriedAttributes getter/setter. See <see cref="Melee.Melee(string, bool, int, double, double, double, Carried)"/>
        /// </summary>
        public Carried CarriedAttributes { get { return carriedAttributes; } set { carriedAttributes = value; } }


    }

    /// <summary>
    /// Defines a grenade.
    /// </summary>
    public class Grenade : Weapon
    {
        private bool fuse;
        private double fuseTime; //depends on above bool
        private bool special;
        private string specialMode; //depends on above bool
        private int storedCapacity;
        private double blastRadius;
        private double killRadius;
        private double maximumDamage;


        /// <summary>
        /// Weapon placeholder constructor.
        /// </summary>
        /// <param name="name">Name of the grenade.</param>
        /// <param name="hasRank">If the grenade has a rank.</param>
        /// <param name="rank">The rank of the grenade, if any.</param>
        public Grenade(string name, bool hasRank, int rank) : base(name, hasRank, rank)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the grenade.</param>
        /// <param name="hasRank">If the grenade has a rank.</param>
        /// <param name="rank">The rank of the grenade.</param>
        /// <param name="fuse">If the grenade has a fuse.</param>
        /// <param name="fuseTime">The fuse time of the grenade.</param>
        /// <param name="special">If the grenade has a special property.</param>
        /// <param name="specialMode">The special property of the grenade.</param>
        /// <param name="storedCapacity">The stored capacity of the grenades.</param>
        /// <param name="blastRadius">The radius that is guaranteed to deal minimum damage.</param>
        /// <param name="killRadius">The radius that is guaranteed to deal lethal (100) damage.</param>
        /// <param name="maximumDamage">The maximum damage the grenade can deal.</param>
        public Grenade(string name, bool hasRank, int rank, bool fuse, double fuseTime, bool special, string specialMode, int storedCapacity, double blastRadius, double killRadius, double maximumDamage) : base(name, hasRank, rank)
        {
            this.fuse = fuse;
            this.fuseTime = fuseTime;
            this.special = special;
            this.specialMode = specialMode;
            this.storedCapacity = storedCapacity;
            this.blastRadius = blastRadius;
            this.killRadius = killRadius;
            this.maximumDamage = maximumDamage;
        }

        /// <summary>
        /// Fuse getter/setter. See <see cref="Grenade.Grenade(string, bool, int, bool, double, bool, string, int, double, double, double)"></see>
        /// </summary>
        public bool Fuse { get { return fuse; } set { fuse = value; } }

        /// <summary>
        /// FuseTime getter/setter. See <see cref="Grenade.Grenade(string, bool, int, bool, double, bool, string, int, double, double, double)"></see>
        /// </summary>
        public double FuseTime { get { return fuseTime; } set { fuseTime = value; } }

        /// <summary>
        /// Special getter/setter. See <see cref="Grenade.Grenade(string, bool, int, bool, double, bool, string, int, double, double, double)"></see>
        /// </summary>
        public bool Special { get { return special; } set { special = value; } }

        /// <summary>
        /// SpecialMode getter/setter. See <see cref="Grenade.Grenade(string, bool, int, bool, double, bool, string, int, double, double, double)"></see>
        /// </summary>
        public string SpecialMode { get { return specialMode; } set { specialMode = value; } }

        /// <summary>
        /// StoredCapacity getter/setter. See <see cref="Grenade.Grenade(string, bool, int, bool, double, bool, string, int, double, double, double)"></see>
        /// </summary>
        public int StoredCapacity { get { return storedCapacity; } set { storedCapacity = value; } }

        /// <summary>
        /// BlastRadius getter/setter. See <see cref="Grenade.Grenade(string, bool, int, bool, double, bool, string, int, double, double, double)"></see>
        /// </summary>
        public double BlastRadius { get { return blastRadius; } set { blastRadius = value; } }

        /// <summary>
        /// KillRadius getter/setter. See <see cref="Grenade.Grenade(string, bool, int, bool, double, bool, string, int, double, double, double)"></see>
        /// </summary>
        public double KillRadius { get { return killRadius; } set { killRadius = value; } }

        /// <summary>
        /// RangedAttributes getter/setter. See <see cref="Grenade.Grenade(string, bool, int, bool, double, bool, string, int, double, double, double)"></see>
        /// </summary>
        public double MaximumDamage { get { return maximumDamage; } set { maximumDamage = value; } }

    }

    /// <summary>
    /// Defines a category containing a list of <c>Weapon</c> objects. Example: Battle Rifles, PDWs, Pistols, etc.
    /// </summary>
    public class Category//<Weapon> //: ICategory
    {
        private Dictionary<int, Weapon> weaponList = new();
        private string name;
        /*
        public Category(ICategory lCategory = null)
        {
            LCategory = lCategory; 
        }
        public ICategory LCategory { get; set; }
        */

        /// <summary>
        /// Indexer for the list.
        /// </summary>
        /// <param name="index">The index to access.</param>
        /// <returns>The value of the specified index of the list.</returns>
        public Weapon this[int index]
        {
            get
            {
                return weaponList[index];
            }
            set
            {
                weaponList[index] = value;
            }
        }

        /// <summary>
        /// Name of the category.
        /// </summary>
        public string Name { get { return name; } set { name = value; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="weapon">First <c>Weapon</c> object to be added, if any.</param>
        /// <param name="categoryName">Name of the list.</param>
        public Category(Weapon? weapon, string categoryName)
        {
            if (weapon != null) addWeapon(weapon);
            this.name = categoryName;
        }

        //generates id from weapon rank
        /// <summary>
        /// Generates an ID for a <c>Weapon</c>.
        /// </summary>
        /// <param name="weapon"><c>Weapon</c> to process.</param>
        /// <returns>ID generated.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int IDGenerator(Weapon weapon)
        {
            if (weapon.Rank < 0)
            {
                throw new ArgumentException("Rank cannot be negative.", nameof(weapon));
            }
            // TODO handle melees with no ranks

            return weapon.Rank * 10;
        }

        //matches a key and value pair (give a key, get a value; give a value, get a key)
        /// <summary>
        /// Matches a key and value.
        /// </summary>
        /// <param name="key">Key that returns its matched value. If not passed, passes the key of the value searched.</param>
        /// <param name="value">Value that returns its matched key. If not passed, passes the value of the key searched.</param>
        /// <returns>True if a match is found, false otherwise.</returns>
        public bool matchKeyValue(ref int key, ref Weapon value)
        {
            if (weaponList.ContainsKey(key))
            {
                value = weaponList[key];
                return true;
            }
            else if (weaponList.ContainsValue(value))
            {
                for (int t = 0; t < weaponList.Count; t++)
                {
                    if (weaponList[t] == value)
                    {
                        value = weaponList[t];
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }

        }

        //if the list has a key
        /// <summary>
        /// Determines if the list has a desired key.
        /// </summary>
        /// <param name="key">The key to be searched.</param>
        /// <returns>If the list has the desired key.</returns>
        public bool hasKey(int key)
        {
            foreach (int r in weaponList.Keys)
            {
                if (r == key) return true;
            }
            return false;
        }

        //if the list has a weapon
        /// <summary>
        /// Determines if the list has a desired <c>Weapon</c>.
        /// </summary>
        /// <param name="weapon">The <c>Weapon</c> to be searched.</param>
        /// <returns>If the list has the desired <c>Weapon</c>.</returns>
        public bool hasWeapon(Weapon weapon)
        {
            foreach (Weapon weapon1 in weaponList.Values)
            {
                if (weapon1 == weapon) return true;
            }
            return false;
        }

        //give a weapon, get its id
        /// <summary>
        /// Searches for an <c>Weapon</c>'s ID in the keys of the list.
        /// </summary>
        /// <param name="weapon">The <c>Weapon</c> to search with.</param>
        /// <returns>ID of the <c>Weapon</c> in the list, if it exists. Returns -1 otherwise.</returns>
        public int IDLookup(Weapon weapon)
        {
            foreach (int id in weaponList.Keys)
            {
                if (weaponList[id] == weapon)
                {
                    return id;
                }
            }
            return -1;
        }

        //get a weapon by looking up its rank 
        /// <summary>
        /// Searches for a weapon by looking up its rank.
        /// </summary>
        /// <param name="rank">The rank to search with.</param>
        /// <returns>The <c>Weapon</c>, if found. Throws an exception otherwise.</returns>
        /// <exception cref="Exception"></exception>
        public Weapon weaponLookupByRank(int rank)
        {
            foreach (Weapon weapon in weaponList.Values)
            {
                if (weapon.Rank == rank)
                {
                    return weapon;
                }
            }
            throw new Exception("Weapon not found.");
        }

        //get a weapon by looking up its id
        /// <summary>
        /// Searches for a <c>Weapon</c> by passing its ID.
        /// </summary>
        /// <param name="id">The ID to search with.</param>
        /// <returns>The <c>Weapon</c>, if found.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Weapon weaponLookupByID(int id)
        {
            Weapon output;
            if (id < 0) throw new ArgumentException("ID cannot be negative.", nameof(id));
            try
            {
                output = weaponList[id];

            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException(nameof(id), e.Message);
            }
            return output;
        }

        //adds a weapon to the list
        /// <summary>
        /// Adds a new <c>Weapon</c> to the list.
        /// </summary>
        /// <param name="weapon">The <c>Weapon</c> object to be added.</param>
        /// <returns>True if succeeded, false otherwise.</returns>
        public bool addWeapon(Weapon weapon)
        {
            int er = 2;
            Func<Weapon, int> deleg = x => IDGenerator(x) != -1 ? IDGenerator(weapon) : er;
            if (deleg(weapon) == er) return false;
            try
            {
                weaponList.Add(deleg(weapon), weapon);
            }
            catch (ArgumentException)
            {
                try
                {
                    weaponList.Add(deleg(weapon) + 1, weapon);
                }
                catch (ArgumentException)
                {
                    weaponList.Add(deleg(weapon) + 2, weapon);

                }
            }
            return true;
        }

        private int meleeID = 0;

        /// <summary>
        /// Adds a new <c>Weapon</c> to the list.
        /// </summary>
        /// <param name="weapon">The <c>Weapon</c> object to be added.</param>
        /// <returns>True if succeeded, false otherwise.</returns>
        public bool addMelee(Weapon weapon, int id)
        {
            int er = 2;
            Func<Weapon, int> deleg = x => IDGenerator(x) != -1 ? IDGenerator(weapon) : er;
            if (deleg(weapon) == er) return false;
            try
            {
                weaponList.Add(id, weapon);
            }
            catch (ArgumentException)
            {
                weaponList.Add(id + 1, weapon);
            }
            return true;
        }

        /// <summary>
        /// Searches for a melee by looking up its rank.
        /// </summary>
        /// <param name="name">The name to search with.</param>
        /// <returns>The <c>Weapon</c>, if found. Throws an exception otherwise.</returns>
        /// <exception cref="Exception"></exception>
        public Weapon meleeLookupByName(string name)
        {
            foreach (Weapon weapon in weaponList.Values)
            {
                if (weapon.Name.Contains(name))
                {
                    return weapon;
                }
            }
            throw new Exception("Weapon not found.");
        }

        /// <summary>
        /// Removes <c>Weapon</c> object from the list by its rank.
        /// </summary>
        /// <param name="rank">The rank of the weapon to remove. Non-negative.</param>
        /// <returns>True if succeeded, false otherwise.</returns>
        /// <exception cref="ArgumentException"></exception>
        public bool removeWeaponByRank(int rank)
        {
            if (rank < 0) throw new ArgumentException("Cannot be negative.", nameof(rank));
            return weaponList.Remove(IDLookup(weaponLookupByRank(rank)));
        }

        /// <summary>
        /// Removes <c>Weapon</c> object from the list by its ID.
        /// </summary>
        /// <param name="id">The ID of the weapon to remove. Non-negative.</param>
        /// <returns>True if succeeded, false otherwise.</returns>
        /// <exception cref="ArgumentException"></exception>
        public bool removeWeaponByID(int id)
        {
            if (id < 0) throw new ArgumentException("Cannot be negative.", nameof(id));
            return weaponList.Remove(id);
        }

        /// <summary>
        /// Gets the amount of <c>Weapon</c> objects in the list.
        /// </summary>
        /// <returns>The count of <c>Weapon</c> objects.</returns>
        public int weaponCount()
        {
            return weaponList.Count;
        }

        /// <summary>
        /// Returns the Dictionary list.
        /// </summary>
        /// <returns>The Dictionary list.</returns>
        public Dictionary<int, Weapon> getKeyValuePairs()
        {
            return weaponList;
        }

        /// <summary>
        /// Returns a <c>Weapon</c> list
        /// </summary>
        /// <returns>A list containing the <c>Weapon</c> objects in the list.</returns>
        public List<Weapon> getWeapons()
        {
            List<Weapon> output = new();
            foreach (Weapon weapon in weaponList.Values)
            {
                output.Add(weapon);
            }
            return output;
        }



    }


    /// <summary>
    /// Defines a list of <c>Category</c> objects.
    /// </summary>
    public class Class
    {
        private List<Category> categoryList = new();
        private string name;

        /// <summary>
        /// Indexer for the list.
        /// </summary>
        /// <param name="index">The index to be accessed.</param>
        /// <returns>The <c>Category</c> object at the index.</returns>
        public Category this[int index]
        {
            get
            {
                return categoryList[index];
            }
            set
            {
                categoryList[index] = value;
            }
        }


        /// <summary>
        /// Name of the class.
        /// </summary>
        public string Name { get { return name; } set { name = value; } }

#nullable enable
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="firstCategory">The first category to be added to the list, if any.</param>
        /// <param name="name">Name of the list.</param>
        public Class(Category? firstCategory, string name)
        {
            if (firstCategory != null) addCategory(firstCategory);
            this.name = name;
        }
#nullable disable

        /// <summary>
        /// Returns the list of <c>Category</c> objects.
        /// </summary>
        /// <returns>List of <c>Category</c> objects.</returns>
        public List<Category> getCategoryList()
        {
            return categoryList;
        }

        /// <summary>
        /// Adds a <c>Category</c> object to the list.
        /// </summary>
        /// <param name="category"><c>Category</c> object to be added.</param>
        public void addCategory(Category category)
        {
            categoryList.Add(category);
        }

        /// <summary>
        /// Removes a <c>Category</c> object to the list.
        /// </summary>
        /// <param name="category"><c>Category</c> object to be removed.</param>
        public void removeCategory(Category category)
        {
            categoryList.Remove(category);
        }

        /// <summary>
        /// Determines if the list has a <c>Category</c> object.
        /// </summary>
        /// <param name="category"><c>Category</c> object to search.</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool hasCategory(Category category)
        {
            return categoryList.Contains(category);
        }

        /// <summary>
        /// Gets the length of th
        /// </summary>
        /// <returns></returns>
        public int categoryCount()
        {
            return categoryList.Count;
        }

    }

    

    public static class WS
    {
        //order of strings: assault rifles, personal defense weapons (PDWs), light 
        public static Dictionary<string, List<List<string>>> versionStrings = new Dictionary<string, List<List<string>>>() {
            { 
                "800", 
                new List<List<string>>(){
                    new List<string>()
                    {
                        "ak12","an94","as-val","scar-l","aug-a1","m16a4","g36","m16a1","m16a3","type-20","aug-a2","k2","famas-f1",
                "ak47","aug-a3","l85a2","hk416","ak74","akm","ak103","tar-21","type-88","m231","c7a2","stg-44","g11k2"
                    },new List<string>()
                    {
                        "mp5k","ump45","g36c","mp7","mac10","p90","colt-mars","mp5","colt-smg-633","l2a3","mp5sd","mp10","m3a1",
                "mp510","uzi","aug-a3-para-xs","k7","aks74u","ppsh-41","fal-para-shorty","kriss-vector","pp-19-bizon","mp40",
                "x95-smg","tommy-gun","rama-1130"
                    },new List<string>()
                    {
                        "colt-lmg","m60","aug-hbar","mg36","rpk12","l86-lsw","rpk","hk21e","hamr-iar","rpk74","mg3kws"
                    }, new List<string>()
                    {
                        "intervention","model-700","dragunov-svu","aws","bfg-50","awm","trg-42","mosin-nagant","dragunov-svds",
                "m1903","k14","hecate-ii","ft300","m107","steyr-scout","wa2000","ntw-20"
                    }, new List<string>()
                    {
                        "m14","beowulf-ecr","scar-h","ak12br","g3a3","ag-3","hk417","henry-45-70","fal-5000"
                    }, new List<string>()
                    {
                        "m4a1","g36k","m4","l22","scar-pdw","aku12","groza-1","ots-126","ak12c","honey-badger","k1a","sr-3m","groza-4",
                "mc51","fal-5063-para","1858-carbine","ak105","jury","kac-srr","gyrojet-carbine","c8a2","x95r","hk51b",
                "can-cannon"
                    }, new List<string>()
                    {
                        "mk11","sks","sl-8","vss-vintorez","msg90","m21","beowulf-tcr","sa58-spr","scar-ssr"
                    }, new List<string>()
                    {
                        "ksg-12","model-870","dbv12","ks-23m","saiga-12","stevens-db","e-gun","aa-12","spas-12","dt11","usas-12"
                    }, new List<string>()
                    {
                        "g17","m9","m1911a1","desert-eagle-l5","g21","g23","m45a1","g40","kg-99","g50","five-seven","zip-22","gi-m1",
                "hardballer","izhevsk-pb","makarov-pm","gb-22","desert-eagle-xix","automag-iii","gyrojet-mark-i","gsp",
                "grizzly","m2011","alien","af2011-a1"
                    }, new List<string>()
                    {
                        "g18c","93r","pp-2000","tec-9","micro-uzi","skorpion-vz61","asmi","mp1911","arm-pistol"
                    }, new List<string>()
                    {
                        "mp412-rex","mateba-6","1858-new-army","redhawk-44","judge","executioner"
                    }, new List<string>()
                    {
                        "super-shorty","sfg-50","m79-thumper","advanced-coilgun","sawed-off","saiga-12u","obrez","sass-308"
                    }, new List<string>()
                    {
                        "m67-frag", "mk-2-frag", "m24-stick", "m26-frag", "m560-mini", "v40-mini", "roly-hg"
                    }, new List<string>()
                    {
                        "dynamite-3", "dynamite", "rgd-5-he","semtex", "pb-grenade", "bundle-charge"
                    },
                    new List<string>()
                    {
                        "t-13 impact","rgn udzs","rgo udzs"
                    },new List<string>()
                    {
                        "cleaver", "tanzanite-blade", "war-fan", "nata-hatchet",
                "mekleth", "karambit", "krampus-kukri", "trench-knife", "knife", "tactical-spatula", "hunting-knife",
                "tanto", "entrencher", "ritual-sickle", "kama", "key", "ice-pick", "machete", "tomahawk", "pocket-knife",
                "havoc-blade", "cutter", "jason", "bridal-brandisher", "darkheart", "streiter", "balisong", "kommando",
                "linked-sword", "classic-knife", "jkey"
                    }, new List<string>()
                    {
                        "zircon-trident", "nordic-war-axe", "world-buster", "noobslayer", "hattori", "chosen-one", "reaper",
                "zero-cutter", "naginata", "training-bayonet", "longsword", "fire-axe", "harvester", "zweihander"
                    }, new List<string>()
                    {
                        "asp-baton", "toy-gun", "maglite-club", "crowbar", "mjolnir", "keyboard", "fumelee", "candy-cane",
                "bare-fists", "tanzanite-pick", "stick-grenade", "bloxy", "holiday-tea", "trench-mace", "clonker",
                "nightstick", "stun-gun", "uchiwa", "fixer", "brass-knuckle", "cricket-bat", "frying-pan", "arm-cannon",
                "starlis-funpost"
                    }, new List<string>()
                    {
                        "paddle", "cursed-shinai", "banjo", "stylis-brush", "kanabo", "stopper", "the-axe", "void-staff",
                "morning-star", "present", "crane"
                    }
                }
            },
            {
                "801",
                new List<List<string>>() {
                    new List<string>()
                    {
                        "ak12","an94","as-val","scar-l","aug-a1","m16a4","g36","m16a1","m16a3","type-20","aug-a2","k2","famas-f1",
                "ak47","aug-a3","l85a2","hk416","ak74","akm","ak103","tar-21","type-88","m231","c7a2","stg-44","g11k2"
                    },new List<string>()
                    {
                        "mp5k","ump45","g36c","mp7","mac10","p90","colt-mars","mp5","colt-smg-633","l2a3","mp5sd","mp10","m3a1",
                "mp510","uzi","aug-a3-para-xs","k7","aks74u","ppsh-41","fal-para-shorty","kriss-vector","pp-19-bizon","mp40",
                "x95-smg","tommy-gun","rama-1130","bwc9-a","five-0"
                    },new List<string>()
                    {
                        "colt-lmg","m60","aug-hbar","mg36","rpk12","l86-lsw","rpk","hk21e","hamr-iar","rpk74","mg3kws","mgv-176","stoner-96"
                    }, new List<string>()
                    {
                        "intervention","model-700","dragunov-svu","aws","bfg-50","awm","trg-42","mosin-nagant","dragunov-svds",
                "m1903","k14","hecate-ii","ft300","m107","steyr-scout","wa2000","ntw-20"
                    }, new List<string>()
                    {
                        "m14","beowulf-ecr","scar-h","ak12br","g3a3","ag-3","hk417","henry-45-70","fal-5000"
                    }, new List<string>()
                    {
                        "m4a1","g36k","m4","l22","scar-pdw","aku12","groza-1","ots-126","ak12c","honey-badger","k1a","sr-3m","groza-4",
                "mc51","fal-5063-para","1858-carbine","ak105","jury","kac-srr","gyrojet-carbine","c8a2","x95r","hk51b",
                "can-cannon"
                    }, new List<string>()
                    {
                        "mk11","sks","sl-8","vss-vintorez","msg90","m21","beowulf-tcr","sa58-spr","scar-ssr"
                    }, new List<string>()
                    {
                        "ksg-12","model-870","dbv12","ks-23m","saiga-12","stevens-db","e-gun","aa-12","spas-12","dt11","usas-12"
                    }, new List<string>()
                    {
                        "g17","m9","m1911a1","desert-eagle-l5","g21","g23","m45a1","g40","kg-99","g50","five-seven","zip-22","gi-m1",
                "hardballer","izhevsk-pb","makarov-pm","gb-22","desert-eagle-xix","automag-iii","gyrojet-mark-i","gsp",
                "grizzly","m2011","alien","af2011-a1"
                    }, new List<string>()
                    {
                        "g18c","93r","pp-2000","tec-9","micro-uzi","skorpion-vz61","asmi","mp1911","arm-pistol"
                    }, new List<string>()
                    {
                        "mp412-rex","mateba-6","1858-new-army","redhawk-44","judge","executioner"
                    }, new List<string>()
                    {
                        "super-shorty","sfg-50","m79-thumper","advanced-coilgun","sawed-off","saiga-12u","obrez","sass-308"
                    }, new List<string>()
                    {
                        "m67-frag", "mk-2-frag", "m24-stick", "m26-frag", "m560-mini", "v40-mini", "roly-hg"
                    }, new List<string>()
                    {
                        "dynamite-3", "dynamite", "rgd-5-he","semtex", "pb-grenade", "bundle-charge"
                    },
                    new List<string>()
                    {
                        "t-13 impact","rgn udzs","rgo udzs"
                    },new List<string>()
                    {
                        "cleaver", "tanzanite-blade", "war-fan", "nata-hatchet",
                "mekleth", "karambit", "krampus-kukri", "trench-knife", "knife", "tactical-spatula", "hunting-knife",
                "tanto", "entrencher", "ritual-sickle", "kama", "key", "ice-pick", "machete", "tomahawk", "pocket-knife",
                "havoc-blade", "cutter", "jason", "bridal-brandisher", "darkheart", "streiter", "balisong", "kommando",
                "linked-sword", "classic-knife", "jkey"
                    }, new List<string>()
                    {
                        "zircon-trident", "nordic-war-axe", "world-buster", "noobslayer", "hattori", "chosen-one", "reaper",
                "zero-cutter", "naginata", "training-bayonet", "longsword", "fire-axe", "harvester", "zweihander"
                    }, new List<string>()
                    {
                        "asp-baton", "toy-gun", "maglite-club", "crowbar", "mjolnir", "keyboard", "fumelee", "candy-cane",
                "bare-fists", "tanzanite-pick", "stick-grenade", "bloxy", "holiday-tea", "trench-mace", "clonker",
                "nightstick", "stun-gun", "uchiwa", "fixer", "brass-knuckle", "cricket-bat", "frying-pan", "arm-cannon",
                "starlis-funpost"
                    }, new List<string>()
                    {
                        "paddle", "cursed-shinai", "banjo", "stylis-brush", "kanabo", "stopper", "the-axe", "void-staff",
                "morning-star", "present", "crane"
                    }
                }
            },
            {
                "802",
                new List<List<string>>() {
                    new List<string>()
                    {
                        "ak12","an94","as-val","scar-l","aug-a1","m16a4","g36","m16a1","m16a3","type-20","aug-a2","k2","famas-f1",
                "ak47","aug-a3","l85a2","hk416","ak74","akm","ak103","tar-21","type-88","m231","c7a2","stg-44","g11k2"
                    },new List<string>()
                    {
                        "mp5k","ump45","g36c","mp7","mac10","p90","colt-mars","mp5","colt-smg-633","l2a3","mp5sd","mp10","m3a1",
                "mp510","uzi","aug-a3-para-xs","k7","aks74u","ppsh-41","fal-para-shorty","kriss-vector","pp-19-bizon","mp40",
                "x95-smg","tommy-gun","rama-1130","bwc9-a","five-0"
                    },new List<string>()
                    {
                        "colt-lmg","m60","aug-hbar","mg36","rpk12","l86-lsw","rpk","hk21e","hamr-iar","rpk74","mg3kws","mgv-176","stoner-96","mg42"
                    }, new List<string>()
                    {
                        "intervention","model-700","dragunov-svu","aws","bfg-50","awm","trg-42","mosin-nagant","dragunov-svds",
                "m1903","k14","hecate-ii","ft300","m107","steyr-scout","wa2000","ntw-20"
                    }, new List<string>()
                    {
                        "m14","beowulf-ecr","scar-h","ak12br","g3a3","ag-3","hk417","henry-45-70","fal-5000"
                    }, new List<string>()
                    {
                        "m4a1","g36k","m4","l22","scar-pdw","aku12","groza-1","ots-126","ak12c","honey-badger","k1a","sr-3m","groza-4",
                "mc51","fal-5063-para","1858-carbine","ak105","jury","kac-srr","gyrojet-carbine","c8a2","x95r","hk51b",
                "can-cannon"
                    }, new List<string>()
                    {
                        "mk11","sks","sl-8","vss-vintorez","msg90","m21","beowulf-tcr","sa58-spr","scar-ssr"
                    }, new List<string>()
                    {
                        "ksg-12","model-870","dbv12","ks-23m","saiga-12","stevens-db","e-gun","aa-12","spas-12","dt11","usas-12"
                    }, new List<string>()
                    {
                        "g17","m9","m1911a1","desert-eagle-l5","g21","g23","m45a1","g40","kg-99","g50","five-seven","zip-22","gi-m1",
                "hardballer","izhevsk-pb","makarov-pm","gb-22","desert-eagle-xix","automag-iii","gyrojet-mark-i","gsp",
                "grizzly","m2011","alien","af2011-a1"
                    }, new List<string>()
                    {
                        "g18c","93r","pp-2000","tec-9","micro-uzi","skorpion-vz61","asmi","mp1911","arm-pistol"
                    }, new List<string>()
                    {
                        "mp412-rex","mateba-6","1858-new-army","redhawk-44","judge","executioner"
                    }, new List<string>()
                    {
                        "super-shorty","sfg-50","m79-thumper","advanced-coilgun","sawed-off","saiga-12u","obrez","sass-308"
                    }, new List<string>()
                    {
                        "m67-frag", "mk-2-frag", "m24-stick", "m26-frag", "m560-mini", "v40-mini", "roly-hg"
                    }, new List<string>()
                    {
                        "dynamite-3", "dynamite", "rgd-5-he","semtex", "pb-grenade", "bundle-charge"
                    },
                    new List<string>()
                    {
                        "t-13 impact","rgn udzs","rgo udzs"
                    }, new List<string>()
                    {
                        "cleaver", "tanzanite-blade", "war-fan", "nata-hatchet",
                "mekleth", "karambit", "krampus-kukri", "trench-knife", "knife", "tactical-spatula", "hunting-knife",
                "tanto", "entrencher", "ritual-sickle", "kama", "key", "ice-pick", "machete", "tomahawk", "pocket-knife",
                "havoc-blade", "cutter", "jason", "bridal-brandisher", "darkheart", "streiter", "balisong", "kommando",
                "linked-sword", "classic-knife", "jkey"
                    }, new List<string>()
                    {
                        "zircon-trident", "nordic-war-axe", "world-buster", "noobslayer", "hattori", "chosen-one", "reaper",
                "zero-cutter", "naginata", "training-bayonet", "longsword", "fire-axe", "harvester", "zweihander"
                    }, new List<string>()
                    {
                        "asp-baton", "toy-gun", "maglite-club", "crowbar", "mjolnir", "keyboard", "fumelee", "candy-cane",
                "bare-fists", "tanzanite-pick", "stick-grenade", "bloxy", "holiday-tea", "trench-mace", "clonker",
                "nightstick", "stun-gun", "uchiwa", "fixer", "brass-knuckle", "cricket-bat", "frying-pan", "arm-cannon",
                "starlis-funpost"
                    }, new List<string>()
                    {
                        "paddle", "cursed-shinai", "banjo", "stylis-brush", "kanabo", "stopper", "the-axe", "void-staff",
                "morning-star", "present", "crane"
                    }
                }

            },
            {
                "902",
                new List<List<string>>() {
                    new List<string>()
                    {
                        "ak12","an94","as-val","scar-l","aug-a1","m16a4","g36","m16a1","m16a3","type-20","aug-a2","k2","famas-f1",
                "ak47","aug-a3","l85a2","hk416","ak74","akm","ak103","tar-21","type-88","m231","c7a2","stg-44","g11k2"
                    },new List<string>()
                    {
                        "mp5k","ump45","g36c","mp7","mac10","p90","colt-mars","mp5","colt-smg-633","l2a3","mp5sd","mp10","m3a1",
                "mp510","uzi","aug-a3-para-xs","k7","aks74u","ppsh-41","fal-para-shorty","kriss-vector","pp-19-bizon","mp40",
                "x95-smg","tommy-gun","rama-1130","bwc9-a","five-0"
                    },new List<string>()
                    {
                        "colt-lmg","m60","aug-hbar","mg36","rpk12","l86-lsw","rpk","hk21e","hamr-iar","rpk74","mg3kws","mgv-176","stoner-96","chainsaw","mg42"
                    }, new List<string>()
                    {
                        "intervention","model-700","dragunov-svu","aws","bfg-50","awm","trg-42","mosin-nagant","dragunov-svds",
                "m1903","k14","hecate-ii","ft300","m107","steyr-scout","wa2000","ntw-20"
                    }, new List<string>()
                    {
                        "m14","beowulf-ecr","scar-h","ak12br","g3a3","ag-3","hk417","henry-45-70","fal-5000"
                    }, new List<string>()
                    {
                        "m4a1","g36k","m4","l22","scar-pdw","aku12","groza-1","ots-126","ak12c","honey-badger","k1a","sr-3m","groza-4",
                "mc51","fal-5063-para","1858-carbine","ak105","jury","kac-srr","gyrojet-carbine","c8a2","x95r","hk51b",
                "can-cannon"
                    }, new List<string>()
                    {
                        "mk11","sks","sl-8","vss-vintorez","msg90","m21","beowulf-tcr","sa58-spr","scar-ssr"
                    }, new List<string>()
                    {
                        "ksg-12","model-870","dbv12","ks-23m","saiga-12","stevens-db","e-gun","aa-12","spas-12","dt11","usas-12"
                    }, new List<string>()
                    {
                        "g17","m9","m1911a1","m17","desert-eagle-l5","g21","g23","m45a1","g40","kg-99","g50","five-seven","zip-22","gi-m1",
                "hardballer","izhevsk-pb","makarov-pm","gb-22","desert-eagle-xix","automag-iii","gyrojet-mark-i","gsp",
                "grizzly","m2011","alien","af2011-a1"
                    }, new List<string>()
                    {
                        "g18c","93r","pp-2000","tec-9","micro-uzi","skorpion-vz61","asmi","mp1911","arm-pistol"
                    }, new List<string>()
                    {
                        "mp412-rex","mateba-6","1858-new-army","redhawk-44","judge","executioner"
                    }, new List<string>()
                    {
                        "super-shorty","sfg-50","m79-thumper","advanced-coilgun","sawed-off","saiga-12u","obrez","sass-308"
                    }, new List<string>()
                    {
                        "m67-frag", "mk-2-frag", "m24-stick", "m26-frag", "m560-mini", "v40-mini", "roly-hg"
                    }, new List<string>()
                    {
                        "dynamite-3", "dynamite", "rgd-5-he","semtex", "pb-grenade", "bundle-charge"
                    },
                    new List<string>()
                    {
                        "t-13 impact","rgn udzs","rgo udzs"
                    },new List<string>()
                    {
                        "cleaver", "tanzanite-blade", "war-fan", "nata-hatchet",
                "mekleth", "karambit", "krampus-kukri", "trench-knife", "knife", "tactical-spatula", "hunting-knife",
                "tanto", "entrencher", "ritual-sickle", "kama", "key", "ice-pick", "machete", "tomahawk", "pocket-knife",
                "havoc-blade", "cutter", "jason", "bridal-brandisher", "darkheart", "streiter", "balisong", "kommando",
                "linked-sword", "classic-knife", "jkey"
                    }, new List<string>()
                    {
                        "zircon-trident", "nordic-war-axe", "world-buster", "noobslayer", "hattori", "chosen-one", "reaper",
                "zero-cutter", "naginata", "training-bayonet", "longsword", "fire-axe", "harvester", "zweihander"
                    }, new List<string>()
                    {
                        "asp-baton", "toy-gun", "maglite-club", "crowbar", "mjolnir", "keyboard", "fumelee", "candy-cane",
                "bare-fists","sparkler", "tanzanite-pick", "stick-grenade", "bloxy", "holiday-tea", "trench-mace", "clonker",
                "nightstick", "stun-gun", "uchiwa", "fixer", "brass-knuckle", "cricket-bat", "frying-pan", "arm-cannon",
                "starlis-funpost"
                    }, new List<string>()
                    {
                        "paddle", "cursed-shinai", "banjo", "stylis-brush", "kanabo", "stopper", "the-axe", "void-staff",
                "morning-star", "present", "crane"
                    }
                }
            },
            {
                "903",
                new List<List<string>>()
                {
                    new List<string>()
                    {
                        "ak12","an94","as-val","scar-l","aug-a1","m16a4","g36","m16a1","m16a3","type-20","aug-a2","k2","famas-f1",
                "ak47","aug-a3","l85a2","hk416","ak74","akm","ak103","tar-21","type-88","m231","c7a2","stg-44","g11k2"
                    },new List<string>()
                    {
                        "mp5k","ump45","g36c","mp7","mac10","p90","colt-mars","mp5","colt-smg-633","l2a3","mp5sd","mp10","m3a1",
                "mp510","uzi","aug-a3-para-xs","k7","aks74u","ppsh-41","fal-para-shorty","kriss-vector","pp-19-bizon","mp40",
                "x95-smg","tommy-gun","rama-1130","bwc9-a","five-0"
                    },new List<string>()
                    {
                        "colt-lmg","m60","aug-hbar","mg36","rpk12","l86-lsw","rpk","hk21e","hamr-iar","rpk74","mg3kws","m1918a2","mgv-176","stoner-96","chainsaw","mg42"
                    }, new List<string>()
                    {
                        "intervention","model-700","dragunov-svu","aws","bfg-50","awm","trg-42","mosin-nagant","dragunov-svds",
                "m1903","k14","hecate-ii","ft300","m107","steyr-scout","wa2000","ntw-20"
                    }, new List<string>()
                    {
                        "m14","beowulf-ecr","scar-h","ak12br","g3a3","ag-3","hk417","henry-45-70","fal-5000","hcar"
                    }, new List<string>()
                    {
                        "m4a1","g36k","m4","l22","scar-pdw","aku12","groza-1","ots-126","ak12c","honey-badger","k1a","sr-3m","groza-4",
                "mc51","fal-5063-para","1858-carbine","ak105","jury","kac-srr","gyrojet-carbine","c8a2","x95r","hk51b",
                "can-cannon"
                    }, new List<string>()
                    {
                        "mk11","sks","sl-8","vss-vintorez","msg90","m21","beowulf-tcr","sa58-spr","scar-ssr"
                    }, new List<string>()
                    {
                        "ksg-12","model-870","dbv12","ks-23m","saiga-12","stevens-db","e-gun","aa-12","spas-12","dt11","usas-12"
                    }, new List<string>()
                    {
                        "g17","m9","m1911a1","m17","desert-eagle-l5","g21","g23","m45a1","g40","kg-99","g50","five-seven","zip-22","gi-m1",
                "hardballer","izhevsk-pb","makarov-pm","gb-22","desert-eagle-xix","automag-iii","gyrojet-mark-i","gsp",
                "grizzly","m2011","alien","af2011-a1"
                    }, new List<string>()
                    {
                        "g18c","93r","pp-2000","tec-9","micro-uzi","skorpion-vz61","asmi","mp1911","arm-pistol"
                    }, new List<string>()
                    {
                        "mp412-rex","mateba-6","1858-new-army","redhawk-44","judge","executioner"
                    }, new List<string>()
                    {
                        "super-shorty","sfg-50","m79-thumper","advanced-coilgun","sawed-off","saiga-12u","obrez","sass-308"
                    }, new List<string>()
                    {
                        "m67-frag", "mk-2-frag", "m24-stick", "m26-frag", "m560-mini", "v40-mini", "roly-hg"
                    }, new List<string>()
                    {
                        "dynamite-3", "dynamite", "rgd-5-he","semtex", "pb-grenade", "bundle-charge"
                    }, 
                    new List<string>()
                    {
                        "t-13 impact","rgn udzs","rgo udzs"
                    },
                    new List<string>()
                    {
                        "cleaver", "tanzanite-blade", "war-fan", "nata-hatchet",
                "mekleth", "karambit", "krampus-kukri", "trench-knife", "knife", "tactical-spatula", "hunting-knife",
                "tanto", "entrencher", "ritual-sickle", "kama", "key", "ice-pick", "machete", "tomahawk", "pocket-knife",
                "havoc-blade", "cutter", "jason", "bridal-brandisher", "darkheart", "streiter", "balisong", "kommando",
                "linked-sword", "classic-knife", "jkey"
                    }, new List<string>()
                    {
                        "zircon-trident", "nordic-war-axe", "world-buster", "noobslayer", "hattori", "chosen-one", "reaper",
                "zero-cutter", "naginata", "training-bayonet", "longsword", "fire-axe", "harvester", "zweihander"
                    }, new List<string>()
                    {
                        "asp-baton", "toy-gun", "maglite-club", "crowbar", "mjolnir", "keyboard", "fumelee", "candy-cane",
                "bare-fists","sparkler", "tanzanite-pick", "stick-grenade", "bloxy", "holiday-tea", "trench-mace", "clonker",
                "nightstick", "stun-gun", "uchiwa", "fixer", "brass-knuckle", "cricket-bat", "frying-pan", "arm-cannon",
                "starlis-funpost"
                    }, new List<string>()
                    {
                        "paddle", "cursed-shinai", "banjo", "stylis-brush", "kanabo", "stopper", "the-axe", "void-staff",
                "morning-star", "present", "crane"
                    }
                }
            }
        
        
        };
        
        //limited: boxy buster (pistol)

        /*
         ohbt: flame of olympia, pacific fm, slay bells, sleigh bells, the countdown
         ohbe: gospell blade, icemourne
         thbt: scl-s3-drastic, warhammer, zircon-slamsickle
         */

        //special thanks to: Kanako#9096, Fork#2067, and シノン△#1231
        //now for.k, and sinonzarokku
        //huge shoutout to hackurtoaster#7938

    }
}