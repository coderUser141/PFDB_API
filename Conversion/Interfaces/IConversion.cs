using PFDB.StatisticUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFDB.WeaponUtility;

namespace PFDB.ConversionUtility
{
	/// <summary>
	/// Interface that defines a single conversion for weapons.
	/// </summary>
	public interface IConversion
	{
		/// <summary>
		/// A collection of all the statistics of the conversion.
		/// </summary>
		public IStatisticCollection StatisticCollection { get; }

		/// <summary>
		/// The unique weapon identifier of the converion.
		/// </summary>
		public WeaponIdentification WeaponID { get; }

		/// <summary>
		/// Indicates whether the data within <see cref="StatisticCollection"/> is faulty and needs revision.
		/// </summary>
		bool NeedsRevision { get; }

		/// <summary>
		/// Indicates the type of weapon.
		/// </summary>
		WeaponType WeaponType { get; }
	}
}
