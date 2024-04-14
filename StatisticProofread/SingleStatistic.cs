using PFDB.StatisticUtility;
using PFDB.WeaponUtility;

namespace PFDB
{
	namespace Proofreading
	{
		public class SingleStatistic : ISingleStatistic
		{
			private bool _needsRevision;
			private string _statisticLine;

			private PhantomForcesVersion _version;
			private StatisticOptions _option;
			public string StatisticLine {  get { return _statisticLine; } }
			public bool NeedsRevision {  get { return _needsRevision; } }
			public PhantomForcesVersion Version { get { return _version; } }
			public StatisticOptions Option { get { return _option; } }

			/*protected internal*/
			public SingleStatistic(bool needsRevision, string statisticLine, PhantomForcesVersion version, StatisticOptions option)
			{
				_version = version;
				_option = option;
				_statisticLine = statisticLine;
				_needsRevision = needsRevision;

				if (needsRevision == false && statisticLine == string.Empty)
					_needsRevision = true;
			}

		}
	}
}