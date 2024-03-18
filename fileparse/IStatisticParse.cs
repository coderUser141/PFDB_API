using PFDB.WeaponUtility;
using System.Collections.Generic;

namespace PFDB.Parsing
{
    internal interface IStatisticParse
    {
        string Filetext { get; }

        string findStatisticInFile(SearchTargets target, WeaponType weaponType, IEnumerable<char> endings);
    }
}