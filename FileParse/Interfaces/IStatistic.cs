﻿using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.StatisticUtility
{
	/// <summary>
	/// Interface that defines a statistic.
	/// </summary>
	public interface IStatistic
	{
		/// <summary>
		/// The unique weapon identifier for the statistic.
		/// </summary>
		public WeaponIdentification WeaponID { get; }

		/// <summary>
		/// The type of statistic.
		/// </summary>
		public StatisticOptions Option { get; }

		/// <summary>
		/// Indicates if the statistic is faulty and needs revision.
		/// </summary>
		public bool NeedsRevision { get; }

		/// <summary>
		/// Indicates the weapon type. Derived from <see cref="WeaponID"/>.
		/// </summary>
		public WeaponType WeaponType { get; }

		/// <summary>
		/// The underlying collection of statistics.
		/// </summary>
		IEnumerable<string> Statistics { get; }
	}
}