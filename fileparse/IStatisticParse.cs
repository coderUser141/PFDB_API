using PFDB.ParsingUtility;
using PFDB.WeaponUtility;
using System.Collections.Generic;

namespace PFDB.Parsing
{
	internal interface IStatisticParse
	{
		string Filetext { get; }

		string FindStatisticInFile(SearchTargets target, WeaponType weaponType, IEnumerable<char> endings);
	}
}