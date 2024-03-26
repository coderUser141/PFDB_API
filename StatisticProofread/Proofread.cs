using System;
using System.Text.RegularExpressions;
using PFDB.ParsingUtility;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;

namespace PFDB
{

	namespace Proofreading
	{

		public class StatisticProofread
		{

			private Regex _regexPattern;
			public static void Main(string[] args)
			{

				return;

				//default
				//\d+\.\d+
			}



			private IStatistic _applyRegularExpression (SearchTargets searchTarget, PhantomForcesVersion version)
			{
				switch(searchTarget) {
					case SearchTargets.AmmoCapacity:
						{
							//_regexPattern = (\d+)\x20{0,2}\/\x20{0,2}(\d+)
							//2 capturing groups
							//2 in {0,2} can be tuned to accept n amount of spaces (currently 2)
							break;
						}
					case SearchTargets.Damage:
						{

							//for parsing damage/damage ranges
							break;
						}
					case SearchTargets.DamageRange:
						{

							//for parsing damage/damage ranges
							// (\(\d+\)|\d+\)|\(\d+)
							break;
						}
					case SearchTargets.Firerate:
						{
							// \x20?(\d+\x20?[a-zA-Z])\x20?
							break;
						}
					case SearchTargets.AmmoType:
						{

							break;
						}
					case SearchTargets.FireModes:
						{

							break;
						}
					default:
						{

							break;
						}
				}
				return new Statistic(false, "blah");
			}

			public static Match regex(string text, string pattern)
			{
				//Statistic t = new Statistic();
				//t._needsRevision = true;

				Regex regex = new Regex(@pattern);
				return regex.Match(text);
			}

			public static MatchCollection regexes(string text, string pattern)
			{
				Regex regex = new Regex(@pattern);
				return regex.Matches(text);
			}
		}
	}
}