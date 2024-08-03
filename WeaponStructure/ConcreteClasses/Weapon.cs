using PFDB.ConversionUtility;
using PFDB.WeaponUtility;

namespace PFDB.WeaponStructure
{
	public abstract class Weapon : IWeapon
	{

		private protected string _name;
		private protected string? _description;
		private protected IConversionCollection _collection;


		public Categories Category { get; } 
		public virtual IConversionCollection ConversionCollection { get { return _collection; } }

		public bool NeedsRevision => ConversionCollection.CollectionNeedsRevision;

		public Weapon(string name, IConversionCollection collection, Categories category)
		{
			_collection = collection;
			_name = name; _description = null;
			Category = category;
		}
		public Weapon(string name, string? description, IConversionCollection collection, Categories category)
		{
			_collection = collection;
			_name = name;
			_description = description;
		}
	}
}
