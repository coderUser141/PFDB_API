using PFDB.StatisticUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.WeaponUtility
{
	public interface IConversion
	{
		public IStatisticCollection StatisticCollection { get; }
		public PhantomForcesVersion Version { get; }

	}
}
