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

		private WeaponIdentification _WID;
		public WeaponIdentification WeaponID { get { return _WID; } }

		public IEnumerable<IStatistic> Statistics => this;

		public new void Add(IStatistic item)
		{
			if(item.WeaponID != _WID)
			{
				PFDBLogger.LogError($"Cannot add IStatistic object to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_WID.Version.VersionString}, IStatistic version: {item.WeaponID.Version.VersionString}", parameter: item);
				throw new ArgumentException($"Cannot add IStatistic object to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_WID.Version.VersionString}, IStatistic version: {item.WeaponID.Version.VersionString}", nameof(item));
			}

			base.Add(item);
		}

		public new void AddRange(IEnumerable<IStatistic> items) { 
			if(items.Any(x => x.WeaponID != this._WID))
			{
				PFDBLogger.LogError($"Cannot add IStatistic objects to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_WID.Version.VersionString}");
				throw new ArgumentException($"Cannot add IStatistic object to StatisticCollection. Version numbers are invalid. StatisticCollection version: {_WID.Version.VersionString}");
			}
			base.AddRange(items);
		}

		public void AddRange(StatisticCollection collection)
		{
			if(collection.WeaponID.Version != this._WID.Version)
			{
				PFDBLogger.LogError($"Cannot add specified StatisticCollection to current StatisticCollection. Version numbers are invalid. Current StatisticCollection version: {_WID.Version.VersionString}, Specified StatisticCollection version: {collection.WeaponID.Version.VersionString}");
				throw new ArgumentException($"Cannot add specified StatisticCollection object to current StatisticCollection. Version numbers are invalid. Current StatisticCollection version: {_WID.Version.VersionString}, Specified StatisticCollection version: {collection.WeaponID.Version.VersionString}");
			}

			base.AddRange(collection);
		}


		public StatisticCollection(WeaponIdentification weaponID) : base() {
			_WID = weaponID;
		}

		public StatisticCollection(WeaponIdentification weaponID, int capacity) : base(capacity) {
			_WID = weaponID;
		}

		public StatisticCollection(WeaponIdentification weaponID, IEnumerable<IStatistic> collection) : base(collection) {
			_WID = weaponID;
		}

	}
}
