using PFDB.StatisticUtility;
using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFDB.ConversionUtility;
using PFDB.PythonExecutionUtility;

namespace PFDB.Conversion
{
	/// <summary>
	/// Defines a single default conversion for weapons.
	/// </summary>
	public class DefaultConversion : Conversion, IDefaultConversion
	{
		/// <inheritdoc/>
		public DefaultConversion(IStatisticCollection statisticCollection) : base(statisticCollection)
		{
		}

		/// <inheritdoc/>
		public DefaultConversion(string filename, WeaponIdentification weaponID) : base(filename, weaponID)
		{
			
		}

		/// <inheritdoc/>
		public DefaultConversion(IPythonExecutor pythonExecutor) : base(pythonExecutor) { }
	}
}
