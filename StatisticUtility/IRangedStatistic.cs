using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.StatisticUtility
{
	public interface IRangedStatistic : IStatistic
	{
		public IEnumerable<string> Statistics { get; }
	}
}
