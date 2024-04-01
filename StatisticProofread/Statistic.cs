using PFDB.StatisticUtility;
using PFDB.WeaponUtility;

namespace PFDB
{
	namespace Proofreading
	{
		public class Statistic : IStatistic
		{
			private bool _needsRevision;
			private string _statisticLine;
			private PhantomForcesVersion _version;
			public string StatisticLine {  get { return _statisticLine; } }
			public bool NeedsRevision {  get { return _needsRevision; } }

			protected internal Statistic(bool needsRevision, string statisticLine, PhantomForcesVersion version)
			{
				_version = version;

				_statisticLine = statisticLine;
				_needsRevision = needsRevision;

				if (needsRevision == false && statisticLine == string.Empty)
					_needsRevision = true;
			}

		}
	}
}