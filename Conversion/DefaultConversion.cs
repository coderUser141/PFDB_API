using PFDB.StatisticUtility;
using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.Weapons
{
	public class DefaultConversion : Conversion
	{



		public DefaultConversion(PhantomForcesVersion version) : base(version)
		{
		}

		public DefaultConversion(IStatisticCollection statisticCollection) : base(statisticCollection)
		{
		}
	}
}
