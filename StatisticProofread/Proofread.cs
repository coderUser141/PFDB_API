using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using PFDB.ParsingUtility;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;

namespace PFDB
{

	namespace Proofreading
	{

		public class StatisticProofread
		{
			private WeaponIdentification _WID;
			public WeaponIdentification WeaponID { get { return _WID; } }
			//private Regex _regexPattern;
			private static Dictionary<StatisticOptions, Regex> regexDictionary = new Dictionary<StatisticOptions, Regex>()
			{
							//2 in {0,2} can be tuned to accept n amount of spaces (currently 2)
				{StatisticOptions.MagazineCapacity, new Regex(@"(\d+)\x20{0,2}\/\x20{0,2}\d+") },
				{StatisticOptions.ReserveCapacity, new Regex(@"\d+\x20{0,2}\/\x20{0,2}(\d+)") },
				{StatisticOptions.Damage, new Regex(@"(\d+\.?\d*)\n") },
				{StatisticOptions.DamageRange, new Regex(@"(\(\d+\)|\d+\)|\(\d+)") },
				{StatisticOptions.Firerate, new Regex(@"\x20?(\d+\x20?[a-zA-Z]?)\x20?") },
				{StatisticOptions.AmmoType, new Regex(@"(5\.56x45mm|5\.45x39mm|\.45\x20{0,2}ACP|9x19mm|7\.62x39mm|7\.62x51mm|9x18mm|12\x20{0,2}gauge|\.22\x20{0,2}Long\x20{0,2}Rifle|\.50\x20{0,2}BMG)") },
				{StatisticOptions.FireModes, new Regex(@"\|\x20{0,2}(AUTO|SEMI|[Il]{0,6})") }

			};


			public static void Main(string[] args)
			{

				return;

			}

			
			public static SearchTargets StatisticOptionToSearchTarget(StatisticOptions option)
			{
				//IEnumerable<string> list = Enum.GetNames<StatisticOptions>();
				SearchTargets outparam;
				bool found = Enum.TryParse(option.ToString(), true, out outparam);
				if (found) return outparam;

				if (new List<string>() { StatisticOptions.ReserveCapacity.ToString(), StatisticOptions.MagazineCapacity.ToString(), StatisticOptions.TotalAmmoCapacity.ToString() }.Contains(option.ToString())) return SearchTargets.AmmoCapacity;

				throw new ArgumentException("The argument was invalid");
			}

			
			public static StatisticOptions SearchTargetToStatisticOption(SearchTargets target)
			{
				StatisticOptions outparam;
				bool found = Enum.TryParse(target.ToString(), true, out outparam);
				if (found) return outparam;
				throw new ArgumentException("The argument was invalid");
			}

			public StatisticProofread(WeaponIdentification weaponID)
			{
				_WID = weaponID;
			}


			public IStatistic ApplyRegularExpression (StatisticOptions statisticTarget, string inputString)
			{
				IStatistic statistic;
				switch (statisticTarget)
				{
					case StatisticOptions.AmmoType:
					case StatisticOptions.MagazineCapacity:
					case StatisticOptions.ReserveCapacity:
						{
							//_regexPattern = new Regex(@"\d+\x20{0,2}\/\x20{0,2}(\d+)");
							Match match = regexDictionary[statisticTarget].Match(inputString);
							string statisticInputString = _regexStringVerifier(match, inputString);
							statistic = new SingleStatistic(match.Success == false, statisticInputString, _WID, statisticTarget);
							break;
						}
					case StatisticOptions.DamageRange: //i realized that the handling is the exact same
					case StatisticOptions.Damage:
						{
							//for parsing damage/damage ranges
							MatchCollection matchCollection;
							List<string> strings = new List<string>();
							bool needsRevision = false;

							if (_WID.Version.IsLegacy)
							{
								matchCollection = new Regex(@"(\d+\.?\d*)").Matches(inputString);
								foreach (Match t in matchCollection) {
									if (t.Success) {
										strings.Add(_regexStringVerifier(t, inputString));
									}
									else
									{
										needsRevision = true;
									}
								}
								statistic = new RangedStatistic(needsRevision, strings, _WID, statisticTarget);
								break;
							}

							//statisticTarget will change depending on what case it is, dw
							goto case StatisticOptions.Firerate;
						}
					case StatisticOptions.FireModes:
					case StatisticOptions.Firerate:
						{

							MatchCollection matchCollection;
							List<string> strings = new List<string>();
							bool needsRevision = false;


							matchCollection = regexDictionary[statisticTarget].Matches(inputString);
							foreach (Match t in matchCollection)
							{
								if (t.Success)
								{
									strings.Add(_regexStringVerifier(t, inputString));
								}
								else
								{
									needsRevision = true;
								}
							}
							statistic = new RangedStatistic(needsRevision, strings, _WID, statisticTarget);
							break;
							// \x20?(\d+\x20?[a-zA-Z])\x20?
						}
					default:
						{
							Match match = new Regex(@"\d+\.?\d*").Match(inputString);
							string statisticInputString = _regexStringVerifier(match, inputString);
							statistic = new SingleStatistic(match.Success == false, statisticInputString, _WID, statisticTarget);
							//default
							//\d+\.\d+
							break;
						}
				}
				return statistic;
				//return new Statistic(false, "blah");
			}

			private static string _regexStringVerifier(Match match, string inputString = "")
			{
				string statisticInputString = inputString;
				if (match.Success)
				{
					if (match.Groups.Count > 1)
					{
						statisticInputString = match.Groups[1].Value;
					}
					else
					{
						statisticInputString = match.Groups[0].Value;
					}
				}

				return statisticInputString;
			}
			/*
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
			}*/
		}
	}
}