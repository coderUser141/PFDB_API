using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFDB.WeaponUtility;

namespace PFDB.WeaponStructure
{
	public class Category : ICategory
	{
		private Categories _categoryType;
		private IWeaponCollection _weaponCollection;

		public string CategoryName { get { return nameof(_categoryType); } }
		public Categories CategoryType { get { return _categoryType; } }
		public bool NeedsRevision { get { return _weaponCollection.CollectionNeedsRevision; } }
		public IWeaponCollection WeaponCollection { get { return _weaponCollection; } }

		public Category(Categories category, IWeaponCollection weaponCollection)
		{
			_weaponCollection = weaponCollection;
			_categoryType = category;

		}
	}
}
