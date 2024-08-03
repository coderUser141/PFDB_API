using PFDB.StatisticUtility;
using PFDB.WeaponUtility;
using System.Collections.Generic;
using System.Linq;

namespace PFDB
{
	namespace Proofreading
	{
		public class Statistic : IStatistic
		{
			private bool _needsRevision;
			private IEnumerable<string> _statistics;
			private WeaponIdentification _WID;
			private StatisticOptions _option;

			public IEnumerable<string> Statistics {  get { return _statistics; } }
			public bool NeedsRevision {  get { return _needsRevision; } }
			public WeaponIdentification WeaponID { get { return _WID; } }
			public StatisticOptions Option { get { return _option; } }
			public WeaponType WeaponType { get {  return _WID.WeaponType; ; } }

			protected internal Statistic(bool needsRevision, IEnumerable<string> statistics, WeaponIdentification weaponID, StatisticOptions option)
			{
				_WID = weaponID;
				_option = option;
				_statistics = statistics;
				_needsRevision = needsRevision;

				if (needsRevision == false && statistics.Any() == false)
					_needsRevision = true;
			}

			protected internal Statistic(bool needsRevision, string statistic, WeaponIdentification weaponID, StatisticOptions option)
			{
				_WID = weaponID;
				_option = option;
				_statistics = new List<string>() { statistic };
				_needsRevision = needsRevision;

				if (needsRevision == false && statistic == string.Empty)
					_needsRevision = true;
			}

		}
	}
}