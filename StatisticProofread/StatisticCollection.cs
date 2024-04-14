using PFDB.Logging;
using PFDB.StatisticUtility;
using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.Proofreading
{
	public class StatisticCollection : List<IStatistic>, IStatisticCollection
	{
		public bool CollectionNeedsRevision {  get {
				return this.All(x => x.NeedsRevision); 
		} }

		private PhantomForcesVersion _version;
		public PhantomForcesVersion Version { get { return _version; } }

		public IEnumerable<IStatistic> Statistics => this;

		public new void Add(IStatistic item)
		{
			if(item.Version != _version)
			{
				PFDBLogger.LogError($"Cannot add IStatistic object to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_version.VersionString}, IStatistic version: {item.Version.VersionString}", parameter: item);
				throw new ArgumentException($"Cannot add IStatistic object to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_version.VersionString}, IStatistic version: {item.Version.VersionString}", nameof(item));
			}

			base.Add(item);
		}

		public new void AddRange(IEnumerable<IStatistic> items) { 
			if(items.Any(x => x.Version != this._version))
			{
				PFDBLogger.LogError($"Cannot add IStatistic objects to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_version.VersionString}");
				throw new ArgumentException($"Cannot add IStatistic object to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_version.VersionString}");
			}
			base.AddRange(items);
		}

		public void AddRange(StatisticCollection collection)
		{
			if(collection.Version != this.Version)
			{
				PFDBLogger.LogError($"Cannot add specified StatisticCollection to current StatisticCollection. Version numbers are invalid. Current StatisticCollection version: {_version.VersionString}, Specified StatisticCollection version: {collection.Version.VersionString}");
				throw new ArgumentException($"Cannot add specified StatisticCollection object to current StatisticCollection. Version numbers are invalid. Current StatisticCollection version: {_version.VersionString}, Specified StatisticCollection version: {collection.Version.VersionString}");
			}

			base.AddRange(collection);
		}


		public StatisticCollection(PhantomForcesVersion version) : base() {
			_version = Version;
		}

		public StatisticCollection(PhantomForcesVersion version, int capacity) : base(capacity) {
			_version = version;
		}

		public StatisticCollection(PhantomForcesVersion version, IEnumerable<IStatistic> collection) : base(collection) {
			_version = version;
		}

	}
}
