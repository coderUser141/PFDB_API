using System.Collections.Generic;
using PFDB.StatisticUtility;

namespace PFDB
{

	namespace Proofreading
	{
		public class RangedStatistic : Statistic, IRangedStatistic
		{
			protected internal RangedStatistic(bool needsRevision, string statisticLine) : base(needsRevision, statisticLine)
			{
			}
			private IEnumerable<string> _statistics = new List<string>();
			public IEnumerable<string> Statistics
			{
				get { return _statistics; }
			}
		}
	}
}