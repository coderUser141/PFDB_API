﻿using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.StatisticUtility
{
	public interface IStatistic
	{
		public PhantomForcesVersion Version { get; }
		public StatisticOptions Option { get; }
		public bool NeedsRevision { get; }
	}
}