using PFDB.WeaponUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.WeaponStructure
{
	public class WeaponCollection : List<IWeapon>, IWeaponCollection
	{
		public IEnumerable<IWeapon> Weapons => this;
		public Categories Category => this.First().Category;
		public bool CollectionNeedsRevision
		{
			get
			{
				return this.Any(x => x.NeedsRevision);
			}
		}
		public WeaponCollection()
		{
			
		}

		public WeaponCollection(IEnumerable<IWeapon> weapons)
		{ 
			this.AddRange(weapons);
		}

		public new void Add(IWeapon weapon)
		{

			//todo: add checks
			base.Add(weapon);
		}

		public new void AddRange(IEnumerable<IWeapon> weapons)
		{
			//todo: add checks
			base.AddRange(weapons);
		}

		public void Add(IWeaponCollection weapons)
		{
			//todo: add checks
			base.AddRange(weapons.Weapons);
		}

	}
}
