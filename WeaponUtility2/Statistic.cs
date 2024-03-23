using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB
{
	namespace StatisticUtility
	{
		public sealed partial class Statistic
		{
			private bool _needsRevision = false;
			public bool NeedsRevision
			{
				get { return _needsRevision; }
			}
			public Statistic() { 
				
			
			}
		}
	}
}
