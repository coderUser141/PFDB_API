
namespace PFDB.WeaponUtility
{
	public interface IClassCollection : IPFDBCollection
	{
		IEnumerable<IClass> Classes { get; }

		void Add(IClass classItem);
		void Add(IClassCollection classes);
		void AddRange(IEnumerable<IClass> classes);
	}
}
