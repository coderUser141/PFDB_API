using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.Parsing
{
    public interface IFileParse
    {

        public string fileReader(string filepath);
        public IEnumerable<string> findAllStatisticsInFile(WeaponType weaponType, int acceptableSpaces = 3, int acceptableCorruptedWordSpaces = 3, bool consoleWrite = false);
        public IDictionary<SearchTargets, string> findAllStatisticsInFileWithTypes(WeaponType weaponType, int acceptableSpaces = 3, int acceptableCorruptedWordSpaces = 3, bool consoleWrite = false);
    }
}
