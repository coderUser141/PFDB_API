using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.StatisticUtility
{
	public interface IStatisticCollection
	{
		public bool CollectionNeedsRevision { get; }
		public WeaponIdentification WeaponID { get; }

		public IEnumerable<IStatistic> Statistics { get; }
	}
}
