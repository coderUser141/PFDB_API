using PFDB.Proofreading;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;
using System.Collections.Generic;

namespace PFDB.Weapons
{
	public class Conversion : IConversion
	{
		protected PhantomForcesVersion _version;
		protected IStatisticCollection _statisticCollection;
		public IStatisticCollection StatisticCollection { get { return _statisticCollection; } }
		public PhantomForcesVersion Version { get { return _version; } }

		public Conversion(PhantomForcesVersion version)
		{
			_version = version;
			_statisticCollection = new StatisticCollection(_version, new List<IStatistic>());

		}

		public Conversion(IStatisticCollection statisticCollection)
		{
			_statisticCollection = statisticCollection;
			_version = statisticCollection.Version;
		}
		
		public Conversion(IPythonExecutor pythonExecutor){
		    _WID = pythonExecutor.Input.WeaponID;
		    if(pythonExecutor.Output is InitOutput init)
		        pythonExecutor.ReturnOutput();
		    
		    
		}
		

	}
}
