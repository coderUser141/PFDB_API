using PFDB.StatisticUtility;

namespace PFDB
{

	namespace Proofreading
	{
		public class Statistic : IStatistic
		{
			private bool _needsRevision;
			private string _statisticLine;
			public string StatisticLine {  get { return _statisticLine; } }
			public bool NeedsRevision {  get { return _needsRevision; } }

			protected internal Statistic(bool needsRevision, string statisticLine)
			{
				_statisticLine = statisticLine;
				_needsRevision = needsRevision;
			}

		}
	}
}