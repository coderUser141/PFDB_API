using System.Collections.Generic;
using System.Linq;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;

namespace PFDB
{

	namespace Proofreading
	{
		public class RangedStatistic :  IRangedStatistic
		{
			private bool _needsRevision;
			private IEnumerable<string> _statistics;
			private PhantomForcesVersion _version;

			public IEnumerable<string> Statistics { get { return _statistics; } }
			public bool NeedsRevision { get { return _needsRevision; } }

			protected internal RangedStatistic(bool needsRevision, IEnumerable<string> statisticList, PhantomForcesVersion version)
			{
				_version = version;
				_statistics = statisticList;
				_needsRevision = needsRevision;

				if (needsRevision == false && statisticList.ToList<string>().Count <= 0)
					_needsRevision = true;
			}
		}
	}
}