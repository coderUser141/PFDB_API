using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.WeaponUtility
{
	/// <summary>
	/// Defines a collection that adds a check for manual proofreading.
	/// </summary>
	public interface IPFDBCollection
	{
		/// <summary>
		/// Whether a collection requires manual proofreading.
		/// </summary>
		public bool CollectionNeedsRevision { get; }
	}
}
