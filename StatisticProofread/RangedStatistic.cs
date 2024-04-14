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
			private WeaponIdentification _WID;
			private StatisticOptions _option;

			public StatisticOptions Option {  get { return _option; } }
			public WeaponIdentification WeaponID { get { return _WID; } }
			public IEnumerable<string> Statistics { get { return _statistics; } }
			public bool NeedsRevision { get { return _needsRevision; } }

			protected internal RangedStatistic(bool needsRevision, IEnumerable<string> statisticList, WeaponIdentification weaponID, StatisticOptions option)
			{
				_WID = weaponID;
				_statistics = statisticList;
				_needsRevision = needsRevision;
				_option = option;
				if (needsRevision == false && statisticList.ToList<string>().Count <= 0)
					_needsRevision = true;
			}
		}
	}
}