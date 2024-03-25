using System;
using System.Text.RegularExpressions;
using PFDB.ParsingUtility;
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
				//for parsing damage/damage ranges
				//(\(\d+\)|\d+\)|\(\d+)

				//default
				//\d+\.\d+
			}



			private void _applyRegularExpression (SearchTargets searchTarget, PhantomForcesVersion version)
			{
				switch(searchTarget) {
					case SearchTargets.AmmoCapacity:
						{
							//_regexPattern = 
							break;
						}
					case SearchTargets.Damage:
						{

							break;
						}
					case SearchTargets.DamageRange:
						{

							break;
						}
					case SearchTargets.Firerate:
						{

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