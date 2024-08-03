using PFDB.WeaponUtility;

namespace PFDB.WeaponStructure
{
	public class Class : IClass
	{
		private Classes _classType;
		private ICategoryCollection _categories;

		public bool NeedsRevision { get { return _categories.CollectionNeedsRevision; } }
		public Classes ClassType { get { return _classType; } }
		public ICategoryCollection CategoryCollection { get { return _categories; } }

		public Class(Classes classType, ICategoryCollection categories)
		{
			_classType = classType;
			_categories = categories;
		}
	}
}
