
namespace PFDB.WeaponUtility
{
	public interface ICategoryCollection : IPFDBCollection
	{
		IEnumerable<ICategory> Categories { get; }

		void Add(ICategory category);
		void Add(ICategoryCollection categoryCollection);
		void AddRange(IEnumerable<ICategory> categories);
	}
}
