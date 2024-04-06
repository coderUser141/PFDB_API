using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.StatisticUtility
{
	public interface ISingleStatistic : IStatistic
	{
		public string StatisticLine { get; }
	}
}
