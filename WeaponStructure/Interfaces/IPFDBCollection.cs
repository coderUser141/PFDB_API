using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.WeaponUtility
{
	public interface IPFDBCollection
	{
		public bool CollectionNeedsRevision { get; }
	}
}
